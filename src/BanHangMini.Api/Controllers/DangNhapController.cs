using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/auth")]
public class DangNhapController : ControllerBase
{
    private readonly IDangNhapService _dangNhapService;

    public DangNhapController(IDangNhapService dangNhapService)
    {
        _dangNhapService = dangNhapService;
    }
    
    [AllowAnonymous]
    [HttpPost("dang-ky")]
    public async Task<IActionResult> DangKy(
        [FromBody] DangKyRequest yeuCau, CancellationToken ct)
    {
        var ketQua = await _dangNhapService.DangKyAsync(yeuCau, ct);
        return StatusCode(
            StatusCodes.Status201Created,
            ApiResponse<DangKyResponse>.Ok(ketQua));
    }

    [AllowAnonymous]
    [HttpPost("dang-nhap")]
    public async Task<IActionResult> DangNhap(
        [FromBody] DangNhapRequest yeuCau, CancellationToken ct) {
            var ketQua = await _dangNhapService.DangNhapAsync(yeuCau, ct);
            if(ketQua is null) {
                return Unauthorized(
                    ApiResponse<object>.Fail("Tên đăng nhập hoặc mật khẩu không đúng"));
            }
             return Ok(ApiResponse<DangNhapResponse>.Ok(ketQua));
    }
}