namespace GraphApi.Models;

public class ImportSchedule
{
    public string Group { get; set; }
    public TimeSpan TimeFrom { get; set; }
    public TimeSpan TimeTo { get; set; }
}