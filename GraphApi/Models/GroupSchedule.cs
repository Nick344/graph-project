namespace GraphApi.Models;

public class GroupSchedule
{
    public string Group {get; set;}
    public List<ScheduleEntry> Schedule {get; set;}
}