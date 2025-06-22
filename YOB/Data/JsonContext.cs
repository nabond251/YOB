using System.Text.Json.Serialization;
using YOB.Models;

[JsonSerializable(typeof(Project))]
[JsonSerializable(typeof(ProjectTask))]
[JsonSerializable(typeof(DaysJson))]
[JsonSerializable(typeof(DayJson))]
[JsonSerializable(typeof(Category))]
[JsonSerializable(typeof(Tag))]
public partial class JsonContext : JsonSerializerContext
{
}