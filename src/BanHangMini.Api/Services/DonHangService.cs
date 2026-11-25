using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

public class DonHangService : IDonHangService
{
    private readonly AppDbContext _context;

    public DonHangService(AppDbContext context)
    {
        _context = context;
    }

    private async Task<int?> TimDonTheoKhoaAsync(int nguoiDungId, string khoaChongTrung, CancellationToken ct)
    {
        return await _context.DonHangs.AsNoTracking()
            .Where(d => d.NguoiDungId == nguoiDungId && d.KhoaChongTrung == khoaChongTrung)
            .Select(d => (int?)d.Id)
            .FirstOrDefaultAsync(ct);
    }

    private async Task<DonHangResponse> LayChiTietNoiBoAsync(int id, CancellationToken ct)
    {
        var don = await _context.DonHangs
            .AsNoTracking()
            .IgnoreQueryFilters()
            .Where(d => d.Id == id)
            .Select(d => new DonHangResponse(
                d.Id, d.MaDonHang, d.TrangThai.ToString(), d.TongTien, d.NgayTao,
                d.ChiTiets.Select(c => new ChiTietDonHangResponse(c.SanPhamId, c.SanPham.Ten, c.SoLuong, c.DonGia, c.ThanhTien)).ToList()))
            .FirstOrDefaultAsync(ct);

        return don ?? throw new KhongTimThayException("Không tìm thấy đơn hàng");
    }

    public async Task<DonHangResponse> TaoAsync(int nguoiDungId, string khoaChongTrung, TaoDonHangRequest yeuCau, CancellationToken ct)
    {
        var idCu = await TimDonTheoKhoaAsync(nguoiDungId, khoaChongTrung, ct);
        if (idCu is int idDaCo)
        {
            return await LayChiTietNoiBoAsync(idDaCo, ct);
        }

        var soSanPhamKhongTrung = yeuCau.Dong.Select(d => d.SanPhamId).Distinct().Count();
        if (soSanPhamKhongTrung != yeuCau.Dong.Count)
            throw new NghiepVuException("Danh sách đặt hàng có sản phẩm bị lặp lại");

        var sanPhamIds = yeuCau.Dong.Select(d => d.SanPhamId).ToList();
        var sanPhams = await _context.SanPhams
            .Where(s => sanPhamIds.Contains(s.Id))
            .ToListAsync(ct);

        if (sanPhams.Count != sanPhamIds.Count)
            throw new NghiepVuException("Có sản phẩm không tồn tại");

        var donHang = new DonHang
        {
            MaDonHang = $"DH{DateTime.UtcNow:yyyyMMddHHmmssfff}",
            NguoiDungId = nguoiDungId,
            TrangThai = TrangThaiDonHang.ChoXuLy,
            KhoaChongTrung = khoaChongTrung
        };

        decimal tongTien = 0;
        foreach (var dong in yeuCau.Dong)
        {
            var sp = sanPhams.First(s => s.Id == dong.SanPhamId);
            if (sp.TonKho < dong.SoLuong)
                throw new NghiepVuException($"Sản phẩm '{sp.Ten}' không đủ tồn kho");

            sp.TonKho -= dong.SoLuong;

            var thanhTien = sp.DonGia * dong.SoLuong;
            tongTien += thanhTien;

            donHang.ChiTiets.Add(new ChiTietDonHang
            {
                SanPhamId = sp.Id,
                SoLuong = dong.SoLuong,
                DonGia = sp.DonGia,
                ThanhTien = thanhTien
            });
        }
        donHang.TongTien = tongTien;

        _context.DonHangs.Add(donHang);

        try
        {
            await _context.SaveChangesAsync(ct);
        }
        catch (DbUpdateConcurrencyException)
        {
            throw new XungDotException("Tồn kho vừa được thay đổi bởi người khác, vui lòng thử lại");
        }
        catch (DbUpdateException ex) when (ex.InnerException is SqlException sql && (sql.Number == 2601 || sql.Number == 2627))
        {
            var idTrung = await TimDonTheoKhoaAsync(nguoiDungId, khoaChongTrung, ct)
                ?? throw new XungDotException("Xung đột khi tạo đơn hàng, vui lòng thử lại");
            return await LayChiTietNoiBoAsync(idTrung, ct);
        }

        return await LayChiTietNoiBoAsync(donHang.Id, ct);
    }

    public async Task<PhanTrang<DonHangResponse>> LayDanhSachAsync(int nguoiDungId, bool laQuanTri, int trang, int soDong, CancellationToken ct)
    {
        var truyVan = _context.DonHangs.AsNoTracking().AsQueryable();
        if (!laQuanTri)
            truyVan = truyVan.Where(d => d.NguoiDungId == nguoiDungId);

        var tongSo = await truyVan.CountAsync(ct);
        var danhSach = await truyVan
            .OrderByDescending(d => d.NgayTao)
            .Skip((trang - 1) * soDong)
            .Take(soDong)
            .Select(d => new DonHangResponse(
                d.Id, d.MaDonHang, d.TrangThai.ToString(), d.TongTien, d.NgayTao,
                d.ChiTiets.Select(c => new ChiTietDonHangResponse(c.SanPhamId, c.SanPham.Ten, c.SoLuong, c.DonGia, c.ThanhTien)).ToList()))
            .ToListAsync(ct);

        return new PhanTrang<DonHangResponse>(danhSach, tongSo, trang, soDong);
    }

    public async Task<DonHangResponse> LayChiTietAsync(int id, int nguoiDungId, bool laQuanTri, CancellationToken ct)
    {
        var donHang = await _context.DonHangs.AsNoTracking()
            .FirstOrDefaultAsync(d => d.Id == id, ct)
            ?? throw new KhongTimThayException("Không tìm thấy đơn hàng");

        if (!laQuanTri && donHang.NguoiDungId != nguoiDungId)
            throw new KhongTimThayException("Không tìm thấy đơn hàng");

        return await LayChiTietNoiBoAsync(id, ct);
    }

    public async Task<DonHangResponse> HuyAsync(int id, int nguoiDungId, bool laQuanTri, CancellationToken ct)
    {
        var donHang = await _context.DonHangs
            .Include(d => d.ChiTiets).ThenInclude(c => c.SanPham)
            .FirstOrDefaultAsync(d => d.Id == id, ct)
            ?? throw new KhongTimThayException("Không tìm thấy đơn hàng");

        if (!laQuanTri && donHang.NguoiDungId != nguoiDungId)
            throw new KhongTimThayException("Không tìm thấy đơn hàng");

        if (donHang.TrangThai != TrangThaiDonHang.ChoXuLy)
            throw new NghiepVuException("Chỉ có thể huỷ đơn hàng đang chờ xử lý");

        foreach (var dong in donHang.ChiTiets)
            dong.SanPham.TonKho += dong.SoLuong;

        donHang.TrangThai = TrangThaiDonHang.DaHuy;
        await _context.SaveChangesAsync(ct);

        return await LayChiTietNoiBoAsync(id, ct);
    }

    public async Task<DonHangResponse> XacNhanAsync(int id, CancellationToken ct)
    {
        var donHang = await _context.DonHangs.FirstOrDefaultAsync(d => d.Id == id, ct)
            ?? throw new KhongTimThayException("Không tìm thấy đơn hàng");

        if (donHang.TrangThai != TrangThaiDonHang.ChoXuLy)
            throw new NghiepVuException("Chỉ có thể xác nhận đơn hàng đang chờ xử lý");

        donHang.TrangThai = TrangThaiDonHang.DaXacNhan;
        await _context.SaveChangesAsync(ct);

        return await LayChiTietNoiBoAsync(id, ct);
    }
}
