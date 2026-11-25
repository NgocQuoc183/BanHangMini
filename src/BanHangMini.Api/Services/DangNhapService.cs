using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

public class DangNhapService : IDangNhapService 
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _cauHinh;
    private readonly PasswordHasher<NguoiDung> _bamMatKhau = new();

    public DangNhapService(AppDbContext context, IConfiguration cauHinh)
        {
            _context = context;
            _cauHinh = cauHinh;
        }

public async Task<DangKyResponse> DangKyAsync(
    DangKyRequest yeuCau, CancellationToken ct)
{
        if (await _context.NguoiDungs.AnyAsync(
            x => x.TenDangNhap == yeuCau.TenDangNhap, ct))
        {
            throw new XungDotException("Tên đăng nhập đã tồn tại");
        }

        var nguoiDung = new NguoiDung
        {
            TenDangNhap = yeuCau.TenDangNhap,
            VaiTro = VaiTro.KhachHang,
            NgayTao = DateTime.UtcNow
        };
        nguoiDung.MatKhauBam = _bamMatKhau.HashPassword(
            nguoiDung, yeuCau.MatKhau);

        _context.NguoiDungs.Add(nguoiDung);
        try
        {
            await _context.SaveChangesAsync(ct);
        }
        catch (DbUpdateException ex) when (
            ex.InnerException is SqlException sqlException
            && (sqlException.Number == 2601 || sqlException.Number == 2627))
        {
            throw new XungDotException("Tên đăng nhập đã tồn tại");
        }

        return new DangKyResponse(nguoiDung.TenDangNhap);
}

public async Task<DangNhapResponse> DangNhapAsync(
    DangNhapRequest yeuCau, CancellationToken ct) {
        var nguoiDung = await _context.NguoiDungs.SingleOrDefaultAsync(
            x => x.TenDangNhap == yeuCau.TenDangNhap,ct);
        if (nguoiDung is null)
        return null;
        var ketQuaKiemTra = _bamMatKhau.VerifyHashedPassword(
            nguoiDung,
            nguoiDung.MatKhauBam,
            yeuCau.MatKhau 
        );
        if (ketQuaKiemTra == PasswordVerificationResult.Failed)
         return null;
        var token = TaoJwt(nguoiDung);

        return new DangNhapResponse(token);
    }

    private string TaoJwt(NguoiDung nguoiDung){
        var issuer = _cauHinh["Jwt:Issuer"]
            ?? throw new InvalidOperationException("Thiếu Jwt:Issuer");

        var audience = _cauHinh["Jwt:Audience"]
            ?? throw new InvalidOperationException("Thiếu Jwt:Audience");

        var key = _cauHinh["Jwt:Key"]
            ?? throw new InvalidOperationException("Thiếu Jwt:Key");

        var expireMinutes = _cauHinh.GetValue<int>(
            "Jwt:expireMinutes",60
        );
    
  var claims = new List<Claim>
{
    new(ClaimTypes.NameIdentifier, nguoiDung.Id.ToString()),
    new(ClaimTypes.Name, nguoiDung.TenDangNhap),
    new(ClaimTypes.Role, nguoiDung.VaiTro.ToString())
};

var khoaBaoMat = new SymmetricSecurityKey(
    Encoding.UTF8.GetBytes(key));

var thongTinKy = new SigningCredentials(
    khoaBaoMat,
    SecurityAlgorithms.HmacSha256);

var token = new JwtSecurityToken(
    issuer: issuer,
    audience: audience,
    claims: claims,
    expires: DateTime.UtcNow.AddMinutes(expireMinutes),
    signingCredentials: thongTinKy);    

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

}