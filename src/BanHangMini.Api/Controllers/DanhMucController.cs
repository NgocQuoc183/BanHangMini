using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/danh-muc")]

public class DanhMucController : ControllerBase {
    private readonly IDanhMucService _danhMucService;

    public DanhMucController(IDanhMucService danhMucService){
        _danhMucService = danhMucService;
    }
    [HttpGet]
    public async Task<IActionResult> DanhSach(CancellationToken ct)
    {
        var ketQua = await _danhMucService.LayTatCaAsync(ct);
        return Ok(ApiResponse<List<DanhMucResponse>>.Ok(ketQua));
    }
    [Authorize(Roles = nameof(VaiTro.QuanTri))]
    [HttpPost] 
    public async Task<IActionResult> Tao([FromBody] TaoDanhMucRequest yeuCau, CancellationToken ct)
    {
        var dm = await _danhMucService.TaoAsync(yeuCau, ct);
        return CreatedAtAction(nameof(DanhSach), ApiResponse<DanhMucResponse>.Ok(dm));
    }
    [Authorize(Roles = nameof(VaiTro.QuanTri))]
    [HttpPut("{id}")]
    public async Task<IActionResult > Sua(int id,[FromBody] SuaDanhMucRequest yeuCau, CancellationToken ct)
    {
        var dm = await _danhMucService.SuaAsync(id, yeuCau, ct);
        return Ok(ApiResponse<DanhMucResponse>.Ok(dm));
    }
    [Authorize(Roles = nameof(VaiTro.QuanTri))]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Xoa(int id, CancellationToken ct)
    {
        await _danhMucService.XoaAsync(id, ct);
        return NoContent();
    }

}
