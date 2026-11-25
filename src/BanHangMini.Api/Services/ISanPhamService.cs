public interface ISanPhamService {
    Task<PhanTrang<SanPhamResponse>> LayDanhSachAsync(LocSanPhamRequest loc, CancellationToken ct);
    Task<SanPhamResponse> LayChiTietAsync(int id, CancellationToken ct);
    Task<SanPhamResponse> TaoAsync(TaoSanPhamRequest yeuCau, CancellationToken ct);
    Task<SanPhamResponse> SuaAsync(int id, SuaSanPhamRequest yeuCau,CancellationToken ct );
    Task XoaAsync(int id, CancellationToken ct);
}