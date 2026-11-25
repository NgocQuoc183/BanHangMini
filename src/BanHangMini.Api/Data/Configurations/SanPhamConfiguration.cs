using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class SanPhamConfiguration : IEntityTypeConfiguration<SanPham>
{
    public void Configure(EntityTypeBuilder<SanPham> e)
    {
        e.ToTable("SanPhams");
        e.Property(x => x.Ten).HasMaxLength(200).IsRequired();
        e.Property(x => x.DonGia).HasColumnType("decimal(18,2)");
        e.Property(x => x.GhiChu).HasMaxLength(500);
        e.Property(x => x.NguoiTao).HasMaxLength(50);
        e.Property(x => x.NguoiCapNhat).HasMaxLength(50);
        e.Property(x => x.PhienBan).IsRowVersion();
        e.HasQueryFilter(x => !x.DaXoa);

        // Lọc theo danh mục (so sánh bằng) rồi theo khoảng giá: cột bằng đặt trước (9.3)
        e.HasIndex(x => new { x.DanhMucId, x.DonGia });

        e.HasOne(x => x.DanhMuc).WithMany(d => d.SanPhams)
            .HasForeignKey(x => x.DanhMucId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
