using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class HopThuDiConfiguration : IEntityTypeConfiguration<HopThuDi>
{
    public void Configure(EntityTypeBuilder<HopThuDi> e)
    {
        e.ToTable("HopThuDis");

        e.Property(x => x.Loai).HasMaxLength(50).IsUnicode(false).IsRequired();
        e.Property(x => x.NoiDung).IsRequired(); // mặc định nvarchar(MAX), không lọc theo cột này
        e.Property(x => x.LoiCuoi).HasMaxLength(500);
    }
}
