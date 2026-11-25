using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    private readonly INguoiDungHienTai _nguoiDung;

    public AppDbContext(DbContextOptions<AppDbContext> options, INguoiDungHienTai nguoiDung)
        : base(options) => _nguoiDung = nguoiDung;

    public DbSet<DanhMuc> DanhMucs => Set<DanhMuc>();
    public DbSet<SanPham> SanPhams => Set<SanPham>();
    public DbSet<NguoiDung> NguoiDungs => Set<NguoiDung>();
    public DbSet<DonHang> DonHangs => Set<DonHang>();
    public DbSet<ChiTietDonHang> ChiTietDonHangs => Set<ChiTietDonHang>();
    public DbSet<HopThuDi> HopThuDis => Set<HopThuDi>();

    protected override void OnModelCreating(ModelBuilder mb)
        => mb.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

    public override Task<int> SaveChangesAsync(CancellationToken ct = default)
    {
        var bayGio = DateTime.UtcNow;
        foreach (var muc in ChangeTracker.Entries<ICoKiemToan>())
        {
            if (muc.State == EntityState.Added)
            {
                muc.Entity.NgayTao = bayGio;
                muc.Entity.NguoiTao = _nguoiDung.TenDangNhap;
            }
            else if (muc.State == EntityState.Modified)
            {
                muc.Entity.NgayCapNhat = bayGio;
                muc.Entity.NguoiCapNhat = _nguoiDung.TenDangNhap;
            }
        }
        return base.SaveChangesAsync(ct);
    }
}
