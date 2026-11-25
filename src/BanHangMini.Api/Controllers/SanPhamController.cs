using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/san-pham")]

public class SanPhamController : ControllerBase  {
    private readonly ISanPhamService _sanPhamService;

    public SanPhamController(ISanPhamService sanPhamService)
    {
        _sanPhamService = sanPhamService;
    }
    [HttpGet] 
    public async Task<IActionResult> DanhSach([FromQuery] LocSanPhamRequest loc, CancellationToken ct)
    {
        var ketQua = await _sanPhamService.LayDanhSachAsync(loc, ct);
        return Ok(ApiResponse<PhanTrang<SanPhamResponse>>.Ok(ketQua));
    }
    [HttpGet("{id}")]
    public async Task<IActionResult> ChiTiet(int id, CancellationToken ct) {
        var sp = await _sanPhamService.LayChiTietAsync(id, ct);
        return Ok(ApiResponse<SanPhamResponse>.Ok(sp));
    }
    [Authorize(Roles = nameof(VaiTro.QuanTri))]
    [HttpPost]
    public async Task<IActionResult> Tao([FromBody] TaoSanPhamRequest yeuCau, CancellationToken ct) {
        var sp = await _sanPhamService.TaoAsync(yeuCau, ct);
        return CreatedAtAction(nameof(ChiTiet), new {id = sp.Id}, ApiResponse<SanPhamResponse>.Ok(sp));
    }
    [Authorize(Roles = nameof(VaiTro.QuanTri))]
    [HttpPut("{id}")]
    public async Task<IActionResult> Sua(int id, [FromBody] SuaSanPhamRequest yeuCau, CancellationToken ct) {
        var sp = await _sanPhamService.SuaAsync(id, yeuCau, ct);
        return Ok(ApiResponse<SanPhamResponse>.Ok(sp));
    }
    [Authorize(Roles = nameof(VaiTro.QuanTri))]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Xoa(int id, CancellationToken ct)
    {
        await _sanPhamService.XoaAsync(id, ct);
        return NoContent();
    }
}
