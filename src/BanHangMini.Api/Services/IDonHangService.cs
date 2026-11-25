public interface IDonHangService 
{
    Task<DonHangResponse> TaoAsync(int nguoiDungID, String khoaChongTrung, TaoDonHangRequest yeuCau, CancellationToken ct);
    Task<PhanTrang<DonHangResponse>> LayDanhSachAsync(int nguoiDungId, bool laQuanTri, int Trang, int soDong, CancellationToken ct);
    Task<DonHangResponse> LayChiTietAsync(int id, int nguoiDungId, bool laQuanTri, CancellationToken ct);
    Task<DonHangResponse> HuyAsync(int id, int nguoiDungId, bool laQuanTri, CancellationToken ct);
    Task<DonHangResponse> XacNhanAsync(int id, CancellationToken ct);
}