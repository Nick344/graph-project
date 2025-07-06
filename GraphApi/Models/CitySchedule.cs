namespace GraphApi.Models;

public class CitySchedule
{
    public string City {get; set;} = string.Empty;
    public List<GroupSchedule> Data { get; set; } = new();
}