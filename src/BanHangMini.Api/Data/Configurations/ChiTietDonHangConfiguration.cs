using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class ChiTietDonHangConfiguration : IEntityTypeConfiguration<ChiTietDonHang>
{
    public void Configure(EntityTypeBuilder<ChiTietDonHang> e)
    {
        e.ToTable("ChiTietDonHangs");

        e.Property(x => x.DonGia).HasColumnType("decimal(18,2)");
        e.Property(x => x.ThanhTien).HasColumnType("decimal(18,2)");

        e.HasOne(x => x.DonHang).WithMany(d => d.ChiTiets)
            .HasForeignKey(x => x.DonHangId)
            .OnDelete(DeleteBehavior.Cascade);

        e.HasOne(x => x.SanPham).WithMany()
            .HasForeignKey(x => x.SanPhamId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
