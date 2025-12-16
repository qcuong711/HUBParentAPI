using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Api.Infrastructure.Persistence;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SchemaController : ControllerBase
{
    private readonly AppDbContext _db;
    public SchemaController(AppDbContext db) { _db = db; }

    [HttpGet("psc-tables")]
    public async Task<IActionResult> GetPscTables()
    {
        await using var conn = _db.Database.GetDbConnection();
        await conn.OpenAsync();
        await using var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT name FROM sys.tables WHERE name LIKE 'psc_%' ORDER BY name";
        var result = new List<string>();
        await using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            result.Add(reader.GetString(0));
        }
        return Ok(result);
    }
}

