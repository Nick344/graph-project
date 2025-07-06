using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using ClosedXML.Excel;
using GraphApi.Models;

namespace GraphApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ScheduleController : ControllerBase
{
    private readonly IWebHostEnvironment _env;
    public ScheduleController(IWebHostEnvironment env, ScheduleStorageService storage)
    {
        _env = env;
        _storage = storage;
    }

    private JsonSerializerOptions NameCase()
    { 
       return new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
    }
    
    private readonly ScheduleStorageService _storage;
        

        [HttpGet("imported")]
        public ActionResult<List<ImportSchedule>> GetImported()
        {
            var data = _storage.Load();
            return Ok(data);
        }
    

    [HttpGet]
    public IActionResult Get(string city)
    {
        var path = Path.Combine(_env.ContentRootPath, "data.json");
        var json = System.IO.File.ReadAllText(path);

       
        var allData = JsonSerializer.Deserialize<List<CitySchedule>>(json, NameCase());
        if (allData == null)
        {
            return StatusCode(500, "Помилка");
        }

        var cityData = allData.FirstOrDefault(c =>
            string.Equals(c.City?.Trim(), city?.Trim(), StringComparison.OrdinalIgnoreCase));


        if (cityData == null)
        {
            return NotFound("Місто не знайдено");
        }  
        return Ok(cityData);
    }
    
    [HttpGet("all")]
    public IActionResult GetStatus(string city, string group)
    {
        var path = Path.Combine(_env.ContentRootPath, "data.json");
        var json = System.IO.File.ReadAllText(path);
        var allData = JsonSerializer.Deserialize<List<CitySchedule>>(json, NameCase());

        if (allData == null)
            return StatusCode(500, "Помилка при читанні даних");

        var cityData = allData.FirstOrDefault(c =>
            string.Equals(c.City?.Trim(), city?.Trim(), StringComparison.OrdinalIgnoreCase));

        if (cityData == null)
            return NotFound("Місто не знайдено");

        var groupData = cityData.Data.FirstOrDefault(g =>
            string.Equals(g.Group?.Trim(), group?.Trim(), StringComparison.OrdinalIgnoreCase));

        if (groupData == null)
            return NotFound("Групу не знайдено");

        var now = DateTime.Now.TimeOfDay;

        var isPowerOn = !groupData.Schedule.Any(s =>
            TimeSpan.TryParse(s.TimeFrom, out var from) &&
            TimeSpan.TryParse(s.TimeTo, out var to) &&
            now >= from && now <= to
        );

        return Ok(new
        {
            city = cityData.City,
            group = groupData.Group,
            isPowerOn
        });
    }
    [HttpGet("status")]
    public IActionResult GetLightStatus([FromQuery] string city, [FromQuery] string group)
    {
        var path = Path.Combine(_env.ContentRootPath, "data.json");
        if (!System.IO.File.Exists(path))
            return StatusCode(500, "Файл не знайдено");

        var json = System.IO.File.ReadAllText(path);
        

        var allData = JsonSerializer.Deserialize<List<CitySchedule>>(json, NameCase());

        if (allData == null)
            return StatusCode(500, "Помилка при читанні JSON");

        var cityData = allData.FirstOrDefault(c => string.Equals(c.City?.Trim(), city?.Trim(), StringComparison.OrdinalIgnoreCase));
        if (cityData == null)
            return NotFound("Місто не знайдено");

        var groupData = cityData.Data.FirstOrDefault(g => g.Group == group);
        if (groupData == null)
            return NotFound("Групу не знайдено");

        var now = DateTime.Now;
        string nowStr = now.ToString("HH:mm");

        var currentInterval = groupData.Schedule.FirstOrDefault(s => s.TimeFrom.CompareTo(nowStr) <= 0 && s.TimeTo.CompareTo(nowStr) >= 0);

        if (currentInterval != null)
        {
            return Ok($"Світла немає з {currentInterval.TimeFrom} до {currentInterval.TimeTo}");
        }

        return Ok("Світло є");
    }

    [HttpPost("import")]
    public IActionResult Import([FromBody] List<CitySchedule> scheduleData)
    {
        if (scheduleData == null || !scheduleData.Any())
        {
            return BadRequest("Дані порожні або недійсні.");
        }

        var path = Path.Combine(_env.ContentRootPath, "data.json");

        var options = new JsonSerializerOptions
        {
            WriteIndented = true
        };

        var json = JsonSerializer.Serialize(scheduleData, options);
        System.IO.File.WriteAllText(path, json);

        return Ok("Дані успішно імпортовано.");
    }

    /*[HttpGet("export")]
    public IActionResult Export()
    {
        var path = Path.Combine(_env.ContentRootPath, "data.json");

        if (!System.IO.File.Exists(path))
            return NotFound("Файл не знайдено");

        var json = System.IO.File.ReadAllText(path);

        return File(
            System.Text.Encoding.UTF8.GetBytes(json),
            "application/json",
            "schedule_export.json"
        );
    }*/
    
    [HttpPost("update")]
    public IActionResult UpdateGroupSchedule([FromQuery] string city, [FromQuery] string group, [FromBody] List<ScheduleEntry> newSchedule)
    {
        var path = Path.Combine(_env.ContentRootPath, "data.json");

        if (!System.IO.File.Exists(path))
            return NotFound("Файл не знайдено");

        var json = System.IO.File.ReadAllText(path);
        var allData = JsonSerializer.Deserialize<List<CitySchedule>>(json, NameCase());

        if (allData == null)
            return StatusCode(500, "Помилка при читанні JSON");

        var cityData = allData.FirstOrDefault(c =>
            string.Equals(c.City?.Trim(), city?.Trim(), StringComparison.OrdinalIgnoreCase));

        if (cityData == null)
            return NotFound("Місто не знайдено");

        var groupData = cityData.Data.FirstOrDefault(g =>
            string.Equals(g.Group?.Trim(), group?.Trim(), StringComparison.OrdinalIgnoreCase));

        if (groupData == null)
            return NotFound("Групу не знайдено");

        groupData.Schedule = newSchedule;

        var options = new JsonSerializerOptions { WriteIndented = true };
        System.IO.File.WriteAllText(path, JsonSerializer.Serialize(allData, options));

        return Ok("Графік успішно оновлено.");
    }

    [HttpGet("export")]
    public IActionResult ExportToExcel()
    {
        var path = Path.Combine(_env.ContentRootPath, "data.json");

        if (!System.IO.File.Exists(path))
            return NotFound("Файл не знайдено");

        var json = System.IO.File.ReadAllText(path);
        var allData = JsonSerializer.Deserialize<List<CitySchedule>>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        if (allData == null || allData.Count == 0)
            return BadRequest("Немає даних для експорту");

        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Розклад");

        worksheet.Cell(1, 1).Value = "Місто";
        worksheet.Cell(1, 2).Value = "Група";
        worksheet.Cell(1, 3).Value = "З";
        worksheet.Cell(1, 4).Value = "До";

        int row = 2;

        foreach (var city in allData)
        {
            foreach (var group in city.Data)
            {
                foreach (var entry in group.Schedule)
                {
                    worksheet.Cell(row, 1).Value = city.City;
                    worksheet.Cell(row, 2).Value = group.Group;
                    worksheet.Cell(row, 3).Value = entry.TimeFrom;
                    worksheet.Cell(row, 4).Value = entry.TimeTo;
                    row++;
                }
            }
        }

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        var content = stream.ToArray();

        return File(content,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            "schedule_export.xlsx");
    }


}