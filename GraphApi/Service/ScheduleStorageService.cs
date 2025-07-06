using System.Text.Json;
using GraphApi.Models;

public class ScheduleStorageService
{
    private readonly string _filePath = "Data/imported-schedules.json";

    public void Save(List<ImportSchedule> schedules)
    {
        var json = JsonSerializer.Serialize(schedules, new JsonSerializerOptions
        {
            WriteIndented = true
        });

        Directory.CreateDirectory(Path.GetDirectoryName(_filePath)!);
        File.WriteAllText(_filePath, json);
    }

    public List<ImportSchedule> Load()
    {
        if (!File.Exists(_filePath))
            return new List<ImportSchedule>();

        var json = File.ReadAllText(_filePath);
        return JsonSerializer.Deserialize<List<ImportSchedule>>(json)!;
    }
}