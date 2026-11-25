using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class DonHangConfiguration : IEntityTypeConfiguration<DonHang>
{
    public void Configure(EntityTypeBuilder<DonHang> e)
    {
        e.ToTable("DonHangs");

        e.Property(x => x.MaDonHang).HasMaxLength(20).IsUnicode(false).IsRequired();
        e.HasIndex(x => x.MaDonHang).IsUnique();

        e.Property(x => x.TrangThai).HasConversion<string>().HasMaxLength(20);
        e.Property(x => x.TongTien).HasColumnType("decimal(18,2)");

        e.Property(x => x.KhoaChongTrung).HasMaxLength(64).IsUnicode(false);
        e.HasIndex(x => x.KhoaChongTrung)
            .IsUnique()
            .HasFilter("[KhoaChongTrung] IS NOT NULL");

        e.Property(x => x.NguoiTao).HasMaxLength(50);
        e.Property(x => x.NguoiCapNhat).HasMaxLength(50);

        e.HasOne(x => x.NguoiDung).WithMany()
            .HasForeignKey(x => x.NguoiDungId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
