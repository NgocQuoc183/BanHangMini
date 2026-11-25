using System.ComponentModel.DataAnnotations;

public class LocSanPhamRequest
{
    public int? DanhMucId { get; init; }
    [MaxLength(100)] public string? TuKhoa { get; init; }
    [Range(0, double.MaxValue)] public decimal? GiaTu { get; init; }
    [Range(0, double.MaxValue)] public decimal? GiaDen { get; init; }
    [Range(1, int.MaxValue)] public int Trang { get; init; } = 1;
    [Range(1, 200, ErrorMessage = "Số dòng mỗi trang phải từ 1 đến 200")]
    public int SoDong { get; init; } = 20;
}
