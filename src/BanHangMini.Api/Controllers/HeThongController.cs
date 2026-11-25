using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/he-thong")]
public class HeThongController : ControllerBase
{
    [HttpGet("ping")]
    public IActionResult Ping()
    {
        return Ok(ApiResponse<object>.Ok(new { gioHienTai = DateTime.UtcNow }));
    }

    [HttpGet("loi-thu")]
    public IActionResult LoiThu()
    {
        throw new InvalidOperationException("thử");
    }
}
