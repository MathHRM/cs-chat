using backend;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Diagnostics;

namespace backend.Http.Controllers;

[ApiController]
[Route("ping")]
public sealed class PingController : ControllerBase
{
    private readonly AppDbContext _db;

    public PingController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> Get(CancellationToken cancellationToken)
    {
        var process = Process.GetCurrentProcess();
        var startedAtUtc = new DateTimeOffset(process.StartTime.ToUniversalTime(), TimeSpan.Zero);

        var system = new
        {
            status = "ok",
            timeUtc = DateTimeOffset.UtcNow,
            processUptimeSeconds = (long)Math.Floor((DateTimeOffset.UtcNow - startedAtUtc).TotalSeconds),
        };

        try
        {
            await _db.Database.OpenConnectionAsync(cancellationToken);

            await using var cmd = _db.Database.GetDbConnection().CreateCommand();
            cmd.CommandText = "SELECT 1";
            cmd.CommandType = CommandType.Text;

            var result = await cmd.ExecuteScalarAsync(cancellationToken);

            return Ok(new
            {
                status = "ok",
                system,
                database = new { healthy = true, result },
            });
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status503ServiceUnavailable, new
            {
                status = "degraded",
                system,
                database = new { healthy = false, error = ex.Message },
            });
        }
        finally
        {
            try
            {
                await _db.Database.CloseConnectionAsync();
            }
            catch
            {
                // best-effort
            }
        }
    }
}
