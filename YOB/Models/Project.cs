using System.Text.Json.Serialization;

namespace YOB.Models
{
    public class Project
    {
        public int ID { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;

        [JsonIgnore]
        public int CategoryID { get; set; }

        public Category? Category { get; set; }

        public List<ProjectTask> Tasks { get; set; } = [];

        public List<Tag> Tags { get; set; } = [];

        public override string ToString() => $"{Name}";
    }

    public class DaysJson
    {
        public List<DayJson> Days { get; set; } = [];
    }

    public class DayJson
    {
        public string Date { get; set; } = null!;

        public string Passages { get; set; } = null!;
    }
}