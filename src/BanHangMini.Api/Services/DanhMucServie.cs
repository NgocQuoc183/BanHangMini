using Microsoft.EntityFrameworkCore;

public class DanhMucService : IDanhMucService
{
    private readonly AppDbContext _context;
    public DanhMucService(AppDbContext context) {
        _context = context;
    }

    public async Task<List<DanhMucResponse>> LayTatCaAsync(CancellationToken ct) 
    {
        return await _context.DanhMucs.AsNoTracking()
            .OrderBy(d => d.Ten)
            .Select( d => new DanhMucResponse(d.Id, d.Ten))
            .ToListAsync(ct);
    }
    public async Task<DanhMucResponse> TaoAsync(TaoDanhMucRequest yeuCau, CancellationToken ct) 
    {
        var dm = new DanhMuc { Ten = yeuCau.Ten };
        _context.DanhMucs.Add(dm);
        await _context.SaveChangesAsync(ct);
        return new DanhMucResponse(dm.Id, dm.Ten);
    }
    public async Task<DanhMucResponse> SuaAsync(int id, SuaDanhMucRequest yeuCau, CancellationToken ct) {
        var dm = await _context.DanhMucs.FirstOrDefaultAsync( d => d.Id == id,ct)
            ?? throw new KhongTimThayException("Không tìm thấy danh mục");

            dm.Ten = yeuCau.Ten;
            await _context.SaveChangesAsync(ct);
            return new DanhMucResponse(dm.Id, dm.Ten);
    }

    public async Task XoaAsync(int id, CancellationToken ct) {
        var dm = await _context.DanhMucs.FirstOrDefaultAsync(d => d.Id == id, ct)
        ?? throw new KhongTimThayException("Không tìm thấy danh mục");

        var conSanPham = await _context.SanPhams.IgnoreQueryFilters()
            .AnyAsync( s => s.DanhMucId == id, ct);

        if (conSanPham)
            throw new NghiepVuException("Không thể xoá danh mục còn sản phẩm");

        _context.DanhMucs.Remove(dm);
        await _context.SaveChangesAsync(ct);
    }
}