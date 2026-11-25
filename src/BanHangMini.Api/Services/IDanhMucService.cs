public interface IDanhMucService {
    Task<List<DanhMucResponse>> LayTatCaAsync(CancellationToken ct);
    Task<DanhMucResponse> TaoAsync(TaoDanhMucRequest yeuCau, CancellationToken ct);
    Task<DanhMucResponse> SuaAsync(int id, SuaDanhMucRequest yeuCau, CancellationToken ct);
    Task XoaAsync(int id,CancellationToken ct);
}