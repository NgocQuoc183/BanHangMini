
using System.Security.Claims;

public interface INguoiDungHienTai
{
    int? Id { get; }
    string TenDangNhap { get; }
}

public class NguoiDungHienTai : INguoiDungHienTai
{
    private readonly IHttpContextAccessor _truyCap;
    public NguoiDungHienTai(IHttpContextAccessor truyCap) => _truyCap = truyCap;

    private ClaimsPrincipal? Nguoi => _truyCap.HttpContext?.User;
    public int? Id =>
        int.TryParse(Nguoi?.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : null;
    public string TenDangNhap => Nguoi?.Identity?.Name ?? "he-thong";
}
