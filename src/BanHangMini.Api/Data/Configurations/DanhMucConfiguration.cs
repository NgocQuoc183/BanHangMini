using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class DanhMucConfiguration : IEntityTypeConfiguration<DanhMuc>
{
    public void Configure(EntityTypeBuilder<DanhMuc> e)
    {
        e.ToTable("DanhMucs");
        e.Property(x => x.Ten).HasMaxLength(100).IsRequired();
        e.HasIndex(x => x.Ten).IsUnique();
    }
}
