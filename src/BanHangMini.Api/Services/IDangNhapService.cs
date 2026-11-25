public interface IDangNhapService
{
    Task<DangNhapResponse> DangNhapAsync(DangNhapRequest yeuCau, CancellationToken ct);
    Task<DangKyResponse> DangKyAsync(DangKyRequest yeuCau, CancellationToken ct);
}