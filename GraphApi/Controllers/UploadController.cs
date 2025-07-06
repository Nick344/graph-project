namespace GraphApi.Controllers;

using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc;
using GraphApi.Models;

[ApiController]
[Route("api/[controller]")]
public class UploadController : ControllerBase
{
    private readonly ScheduleStorageService _storage;

    public UploadController(ScheduleStorageService storage)
    {
        _storage = storage;
    }

    [HttpPost("excel")]
    public async Task<IActionResult> UploadExcel(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("Файл не выбран");

        var schedules = new List<ImportSchedule>();

        using (var stream = file.OpenReadStream())
        using (var workbook = new XLWorkbook(stream))
        {
            var worksheet = workbook.Worksheet(1);
            var rows = worksheet.RangeUsed().RowsUsed().Skip(1);

            foreach (var row in rows)
            {
                try
                {
                    var schedule = new ImportSchedule
                    {
                        Group = row.Cell(1).GetString(),
                        TimeFrom = TimeSpan.Parse(row.Cell(2).GetString()),
                        TimeTo = TimeSpan.Parse(row.Cell(3).GetString())
                    };

                    schedules.Add(schedule);
                }
                catch (Exception ex)
                {
                    return BadRequest($"Ошибка в строке {row.RowNumber()}: {ex.Message}");
                }
            }
        }
        _storage.Save(schedules);

        return Ok(new { Count = schedules.Count });
    }

}
