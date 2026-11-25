using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

public static class DuLieuMau
{
    public static async Task NapAsync(IServiceProvider sp)
    {
        var context = sp.GetRequiredService<AppDbContext>();
        var cauHinh = sp.GetRequiredService<IConfiguration>();

        if (!await context.NguoiDungs.AnyAsync(x => x.VaiTro == VaiTro.QuanTri))
        {
            var matKhau = cauHinh["QuanTri:MatKhauKhoiTao"] ?? "123";
            var admin = new NguoiDung
            {
                TenDangNhap = "admin",
                VaiTro = VaiTro.QuanTri,
                NgayTao = DateTime.UtcNow
            };
            admin.MatKhauBam = new PasswordHasher<NguoiDung>().HashPassword(admin, matKhau);

            context.NguoiDungs.Add(admin);
            await context.SaveChangesAsync();
        }

        if (!await context.DanhMucs.AnyAsync())
        {
            var dienTu = new DanhMuc { Ten = "Điện tử" };
            var giaDung = new DanhMuc { Ten = "Gia dụng" };
            var vanPhongPham = new DanhMuc { Ten = "Văn phòng phẩm" };

            context.DanhMucs.AddRange(dienTu, giaDung, vanPhongPham);

            var sanPhams = new List<SanPham>
            {
                new() { DanhMuc = dienTu, Ten = "Tai nghe Bluetooth", DonGia = 350_000, TonKho = 50 },
                new() { DanhMuc = dienTu, Ten = "Chuột không dây", DonGia = 180_000, TonKho = 80 },
                new() { DanhMuc = dienTu, Ten = "Bàn phím cơ", DonGia = 750_000, TonKho = 30 },
                new() { DanhMuc = dienTu, Ten = "Sạc dự phòng 10000mAh", DonGia = 290_000, TonKho = 60 },
                new() { DanhMuc = giaDung, Ten = "Nồi cơm điện 1.8L", DonGia = 620_000, TonKho = 20 },
                new() { DanhMuc = giaDung, Ten = "Bình giữ nhiệt 500ml", DonGia = 150_000, TonKho = 100 },
                new() { DanhMuc = giaDung, Ten = "Quạt để bàn", DonGia = 320_000, TonKho = 40 },
                new() { DanhMuc = vanPhongPham, Ten = "Sổ tay bìa da A5", DonGia = 65_000, TonKho = 200 },
                new() { DanhMuc = vanPhongPham, Ten = "Bút ký cao cấp", DonGia = 95_000, TonKho = 150 },
                new() { DanhMuc = vanPhongPham, Ten = "Bộ bút chì màu 24 cây", DonGia = 120_000, TonKho = 70 }
            };

            context.SanPhams.AddRange(sanPhams);
            await context.SaveChangesAsync();
        }
    }
}
