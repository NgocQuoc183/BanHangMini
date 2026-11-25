using Microsoft.EntityFrameworkCore;

public class SanPhamService : ISanPhamService {
    private readonly AppDbContext _context;

    public SanPhamService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<PhanTrang<SanPhamResponse>> LayDanhSachAsync(LocSanPhamRequest loc, CancellationToken ct) {
        var truyVan = _context.SanPhams.AsNoTracking();

        if (loc.DanhMucId is int danhMucId ) truyVan = truyVan.Where(s => s.DanhMucId == danhMucId);
        if (!string.IsNullOrWhiteSpace(loc.TuKhoa)) truyVan = truyVan.Where(s => s.Ten.Contains(loc.TuKhoa));
        if (loc.GiaTu is decimal giaTu) truyVan = truyVan.Where(s => s.DonGia >= giaTu);
        if (loc.GiaDen is decimal giaDen) truyVan = truyVan.Where(s => s.DonGia <= giaDen);

        var tongSo = await truyVan.CountAsync(ct);
        var danhSach = await truyVan
            .OrderBy(s => s.Ten).ThenBy(s => s.Id)
            .Skip((loc.Trang - 1) * loc.SoDong)
            .Take(loc.SoDong)
            .Select(s => new SanPhamResponse(s.Id, s.Ten, s.DanhMuc.Ten, s.DonGia, s.TonKho, s.GhiChu, s.PhienBan))
            .ToListAsync(ct);

        return new PhanTrang<SanPhamResponse>(danhSach, tongSo, loc.Trang, loc.SoDong);
    }

    public async Task<SanPhamResponse> LayChiTietAsync(int id, CancellationToken ct)
    {
        var sp = await _context.SanPhams.AsNoTracking()
            .Where(s => s.Id == id)
            .Select(s => new SanPhamResponse(s.Id, s.Ten, s.DanhMuc.Ten, s.DonGia, s.TonKho, s.GhiChu, s.PhienBan))
            .FirstOrDefaultAsync(ct);

        return sp ?? throw new KhongTimThayException("Không tìm thấy sản phẩm");
    }
    public async Task<SanPhamResponse> TaoAsync(TaoSanPhamRequest yeuCau, CancellationToken ct) {
        var danhMuc = await _context.DanhMucs.FindAsync(new object[] {yeuCau.DanhMucId}, ct)
        ?? throw new NghiepVuException("Danh mục không tồn tại");

        var sp = new SanPham{
            DanhMucId = danhMuc.Id,
            Ten = yeuCau.Ten,
            DonGia = yeuCau.DonGia,
            TonKho = yeuCau.TonKho,
            GhiChu = yeuCau.GhiChu
        };
        _context.SanPhams.Add(sp);
        await _context.SaveChangesAsync(ct);

        return new SanPhamResponse(sp.Id, sp.Ten, danhMuc.Ten,sp.DonGia,sp.TonKho,sp.GhiChu, sp.PhienBan);
    }

    public async Task<SanPhamResponse> SuaAsync(int id, SuaSanPhamRequest yeuCau, CancellationToken ct) {
        var sp = await _context.SanPhams.Include(s => s.DanhMuc).FirstOrDefaultAsync(s => s.Id == id, ct)
        ?? throw new KhongTimThayException("Không tìm thấy sản phẩm");

        _context.Entry(sp).Property(s => s.PhienBan).OriginalValue = yeuCau.PhienBan;
        sp.Ten = yeuCau.Ten;
        sp.DonGia = yeuCau.DonGia;
        sp.TonKho = yeuCau.TonKho;
        sp.GhiChu = yeuCau.GhiChu;

        try {
            await _context.SaveChangesAsync(ct);
        } catch (DbUpdateConcurrencyException) {
            throw new XungDotException("Sản phẩm vừa được người khác sửa, vui lòng thử lại");
        }
        return new SanPhamResponse(sp.Id, sp.Ten, sp.DanhMuc.Ten, sp.DonGia, sp.TonKho, sp.GhiChu, sp.PhienBan);
    }
    public async Task XoaAsync(int id, CancellationToken ct){
        var sp = await _context.SanPhams.FirstOrDefaultAsync (s => s.Id == id,ct)
            ?? throw new KhongTimThayException("Không tìm thấy sản phẩm");

            sp.DaXoa = true;
            await _context.SaveChangesAsync(ct);
    }
}