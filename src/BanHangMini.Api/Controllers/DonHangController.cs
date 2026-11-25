using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Authorize]
[Route("api/don-hang")]
public class DonHangController : ControllerBase
{
    private readonly IDonHangService _donHangService;
    private readonly INguoiDungHienTai _nguoiDungHienTai;

    public DonHangController(
        IDonHangService donHangService,
        INguoiDungHienTai nguoiDungHienTai)
    {
        _donHangService = donHangService;
        _nguoiDungHienTai = nguoiDungHienTai;
    }

    [HttpGet]
    public async Task<IActionResult> DanhSach(
        [FromQuery] int trang = 1,
        [FromQuery] int soDong = 20,
        CancellationToken ct = default)
    {
        var nguoiDungId = _nguoiDungHienTai.Id!.Value;
        var laQuanTri = User.IsInRole(nameof(VaiTro.QuanTri));
        var ketQua = await _donHangService.LayDanhSachAsync(
            nguoiDungId, laQuanTri, trang, soDong, ct);

        return Ok(ApiResponse<PhanTrang<DonHangResponse>>.Ok(ketQua));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> ChiTiet(int id, CancellationToken ct)
    {
        var nguoiDungId = _nguoiDungHienTai.Id!.Value;
        var laQuanTri = User.IsInRole(nameof(VaiTro.QuanTri));
        var don = await _donHangService.LayChiTietAsync(
            id, nguoiDungId, laQuanTri, ct);

        return Ok(ApiResponse<DonHangResponse>.Ok(don));
    }

    [HttpPost]
    public async Task<IActionResult> Tao(
        [FromHeader(Name = "Idempotency-Key")] string? khoaChongTrung,
        [FromBody] TaoDonHangRequest yeuCau,
        CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(khoaChongTrung))
        {
            return BadRequest(ApiResponse<object>.Fail(
                "Header Idempotency-Key là khoá bắt buộc"));
        }

        var nguoiDungId = _nguoiDungHienTai.Id!.Value;
        var don = await _donHangService.TaoAsync(
            nguoiDungId, khoaChongTrung, yeuCau, ct);

        return CreatedAtAction(
            nameof(ChiTiet),
            new { id = don.Id },
            ApiResponse<DonHangResponse>.Ok(don));
    }

    [HttpPost("{id}/huy")]
    public async Task<IActionResult> Huy(int id, CancellationToken ct)
    {
        var nguoiDungId = _nguoiDungHienTai.Id!.Value;
        var laQuanTri = User.IsInRole(nameof(VaiTro.QuanTri));
        var don = await _donHangService.HuyAsync(
            id, nguoiDungId, laQuanTri, ct);

        return Ok(ApiResponse<DonHangResponse>.Ok(don));
    }
        
    [Authorize(Roles = nameof(VaiTro.QuanTri))]
    [HttpPost("{id}/xac-nhan")]
    public async Task<IActionResult> XacNhan(int id, CancellationToken ct)
    {
        var don = await _donHangService.XacNhanAsync(id, ct);
        return Ok(ApiResponse<DonHangResponse>.Ok(don));
    }
}
     