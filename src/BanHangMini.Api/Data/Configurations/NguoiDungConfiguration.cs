using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class NguoiDungConfiguration : IEntityTypeConfiguration<NguoiDung>
{
    public void Configure(EntityTypeBuilder<NguoiDung> e)
    {
        e.ToTable("NguoiDungs");
        e.Property(x => x.TenDangNhap).HasMaxLength(50).IsUnicode(false).IsRequired();
        e.HasIndex(x => x.TenDangNhap).IsUnique();
        e.Property(x => x.MatKhauBam).HasMaxLength(200).IsRequired();
        e.Property(x => x.VaiTro).HasConversion<string>().HasMaxLength(20);
    }
}
