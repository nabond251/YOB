using System.Text.Json;
using Microsoft.Extensions.Logging;
using YOB.Models;

namespace YOB.Data
{
    public class SeedDataService
    {
        private readonly ProjectRepository _projectRepository;
        private readonly TaskRepository _taskRepository;
        private readonly TagRepository _tagRepository;
        private readonly CategoryRepository _categoryRepository;
        private readonly string _seedDataFilePath = "SeedData.json";
        private readonly ILogger<SeedDataService> _logger;

        public SeedDataService(ProjectRepository projectRepository, TaskRepository taskRepository, TagRepository tagRepository, CategoryRepository categoryRepository, ILogger<SeedDataService> logger)
        {
            _projectRepository = projectRepository;
            _taskRepository = taskRepository;
            _tagRepository = tagRepository;
            _categoryRepository = categoryRepository;
            _logger = logger;
        }

        public async Task LoadSeedDataAsync()
        {
            ClearTables();

            await using Stream templateStream = await FileSystem.OpenAppPackageFileAsync(_seedDataFilePath);

            DaysJson? payload = null;
            try
            {
                payload = JsonSerializer.Deserialize(templateStream, JsonContext.Default.DaysJson);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error deserializing seed data");
            }

            try
            {
                if (payload is not null)
                {
                    foreach (var day in payload.Days)
                    {
                        if (day is null)
                        {
                            continue;
                        }

                        var project = new Project()
                        {
                            Name = day.Date,
                            Description = day.Passages,
                        };
                        await _projectRepository.SaveItemAsync(project);

                        if (day?.Passages is not null)
                        {
                            foreach (var passage in ParsePassages(day.Passages))
                            {
                                var task = new ProjectTask
                                {
                                    ProjectID = project.ID,
                                    Title = passage,
                                    IsCompleted = false,
                                };
                                await _taskRepository.SaveItemAsync(task);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error saving seed data");
                throw;
            }
        }

        private static IEnumerable<string> ParsePassages(string passages) =>
            passages
            .Split(';')
            .Select(x => x.Trim())
            .Select(SplitBookAndChapters)
            .Select(bc => (CompleteBookName(bc.Item1), bc.Item2))
            .SelectMany(bc => ParsePassageRange(bc.Item2)
            .Select(c => $"{bc.Item1} {c}"));

        private static (string, string) SplitBookAndChapters(string passages)
        {
            // can't use split since some books have spaces in names
            var bookIndex = passages.LastIndexOf(' ');
            var book = passages[..bookIndex];
            var chapters = passages[(bookIndex + 1)..];

            return (book, chapters);
        }

        private static string CompleteBookName(string book) =>
            book switch
            {
                "Gen" => "Genesis",
                "Ex" => "Exodus",
                "Lev" => "Leviticus",
                "Num" => "Numbers",
                "Deut" => "Deuteronomy",
                "Josh" => "Joshua",
                "Jdgs" => "Judges",
                "Ru" => "Ruth",
                "1 Sa" => "1 Samuel",
                "2 Sa" => "2 Samuel",
                "1 Kgs" => "1 Kings",
                "2 Kgs" => "2 Kings",
                "1 Chr" => "1 Chronicles",
                "2 Chr" => "2 Chronicles",
                "Ezr" => "Ezra",
                "Neh" => "Nehemiah",
                "Est" => "Esther",
                "Job" => "Job",
                "Ps" => "Psalms",
                "Pro" => "Proverbs",
                "Ecc" => "Ecclesiastes",
                "SoS" => "Song of Solomon",
                "Isa" => "Isaiah",
                "Jer" => "Jeremiah",
                "Lam" => "Lamentations",
                "Eze" => "Ezekiel",
                "Dan" => "Daniel",
                "Hos" => "Hosea",
                "Joe" => "Joel",
                "Amo" => "Amos",
                "Oba" => "Obadiah",
                "Jon" => "Jonah",
                "Mic" => "Micah",
                "Nah" => "Nahum",
                "Hab" => "Habakkuk",
                "Zep" => "Zephaniah",
                "Hag" => "Haggai",
                "Zec" => "Zechariah",
                "Mal" => "Malachi",
                "Matt" => "Matthew",
                "Mark" => "Mark",
                "Luke" => "Luke",
                "John" => "John",
                "Acts" => "Acts",
                "Rom" => "Romans",
                "1 Co" => "1 Corinthians",
                "2 Co" => "2 Corinthians",
                "Gal" => "Galatians",
                "Eph" => "Ephesians",
                "Phi" => "Philippians",
                "Col" => "Colossians",
                "1 Th" => "1 Thessalonians",
                "2 Th" => "2 Thessalonians",
                "1 Tim" => "1 Timothy",
                "2 Tim" => "2 Timothy",
                "Ti" => "Titus",
                "Phlm" => "Philemon",
                "Heb" => "Hebrews",
                "Jam" => "James",
                "1 Pe" => "1 Peter",
                "2 Pe" => "2 Peter",
                "1 Jn" => "1 John",
                "2 Jn" => "2 John",
                "3 Jn" => "3 John",
                "Jude" => "Jude",
                "Rev" => "Revelation",
                _ => throw new InvalidOperationException(),
            };

        private static IEnumerable<int> ParsePassageRange(string chapters)
        {
            IEnumerable<int> retVal;

            if (chapters.Contains('-'))
            {
                var tokens = chapters.Split('-');
                var begin = int.Parse(tokens[0]);
                var end = int.Parse(tokens[1]);

                retVal = Enumerable.Range(begin, end - begin + 1);
            }
            else
            {
                retVal = [int.Parse(chapters)];
            }

            return retVal;
        }

        private async void ClearTables()
        {
            try
            {
                await Task.WhenAll(
                    _projectRepository.DropTableAsync(),
                    _taskRepository.DropTableAsync(),
                    _tagRepository.DropTableAsync(),
                    _categoryRepository.DropTableAsync());
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}