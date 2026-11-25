using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

public static class DatLaiMatKhauQuanTri
{
    public static async Task TaoTaiKhoanAsync(
        IServiceProvider services, string tenDangNhap)
    {
        tenDangNhap = tenDangNhap.Trim();
        if (string.IsNullOrWhiteSpace(tenDangNhap) || tenDangNhap.Length > 50)
        {
            throw new InvalidOperationException(
                "Tên đăng nhập phải có từ 1 đến 50 ký tự.");
        }

        using var phamVi = services.CreateScope();
        var context = phamVi.ServiceProvider.GetRequiredService<AppDbContext>();
        if (await context.NguoiDungs.AnyAsync(x => x.TenDangNhap == tenDangNhap))
        {
            throw new InvalidOperationException(
                $"Tài khoản '{tenDangNhap}' đã tồn tại.");
        }

        var matKhau = NhapMatKhauMoi();
        var admin = new NguoiDung
        {
            TenDangNhap = tenDangNhap,
            VaiTro = VaiTro.QuanTri,
            NgayTao = DateTime.UtcNow
        };
        admin.MatKhauBam = new PasswordHasher<NguoiDung>()
            .HashPassword(admin, matKhau);

        context.NguoiDungs.Add(admin);
        try
        {
            await context.SaveChangesAsync();
        }
        catch (DbUpdateException ex) when (
            ex.InnerException is SqlException sqlException
            && (sqlException.Number == 2601 || sqlException.Number == 2627))
        {
            throw new InvalidOperationException(
                $"Tài khoản '{tenDangNhap}' đã tồn tại.", ex);
        }

        Console.WriteLine($"Đã tạo tài khoản quản trị '{tenDangNhap}'.");
    }

    public static async Task ThucHienAsync(IServiceProvider services)
    {
        var matKhau = NhapMatKhauMoi();

        using var phamVi = services.CreateScope();
        var context = phamVi.ServiceProvider.GetRequiredService<AppDbContext>();
        var admin = await context.NguoiDungs.SingleOrDefaultAsync(
            x => x.VaiTro == VaiTro.QuanTri);
        if (admin is null)
        {
            throw new InvalidOperationException(
                "Không tìm thấy tài khoản quản trị trong database đang kết nối.");
        }

        admin.MatKhauBam = new PasswordHasher<NguoiDung>()
            .HashPassword(admin, matKhau);
        await context.SaveChangesAsync();

        Console.WriteLine("Đã đặt lại mật khẩu admin thành công.");
    }

    private static string NhapMatKhauMoi()
    {
        if (Console.IsInputRedirected || Console.IsOutputRedirected)
        {
            throw new InvalidOperationException(
                "Thao tác cần được chạy trong terminal tương tác.");
        }

        var matKhau = NhapMatKhau("Nhập mật khẩu mới: ");
        if (matKhau.Length < 8 || matKhau.Length > 128)
        {
            throw new InvalidOperationException(
                "Mật khẩu phải có từ 8 đến 128 ký tự.");
        }

        var xacNhan = NhapMatKhau("Nhập lại mật khẩu mới: ");
        if (!string.Equals(matKhau, xacNhan, StringComparison.Ordinal))
        {
            throw new InvalidOperationException("Hai mật khẩu không khớp.");
        }

        return matKhau;
    }

    private static string NhapMatKhau(string loiNhac)
    {
        Console.Write(loiNhac);
        var matKhau = new StringBuilder();

        while (true)
        {
            var phim = Console.ReadKey(intercept: true);
            if (phim.Key == ConsoleKey.Enter)
            {
                Console.WriteLine();
                return matKhau.ToString();
            }

            if (phim.Key == ConsoleKey.Backspace)
            {
                if (matKhau.Length > 0)
                {
                    matKhau.Length--;
                    Console.Write("\b \b");
                }

                continue;
            }

            if (!char.IsControl(phim.KeyChar))
            {
                matKhau.Append(phim.KeyChar);
                Console.Write('*');
            }
        }
    }
}
