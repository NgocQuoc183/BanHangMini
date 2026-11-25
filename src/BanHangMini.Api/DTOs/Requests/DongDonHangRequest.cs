using System.ComponentModel.DataAnnotations;

public class DongDonHangRequest
{
    [Range(1, int.MaxValue)] public int SanPhamId { get; init; }
    [Range(1, 1000, ErrorMessage = "Số lượng mỗi dòng từ 1 đến 1000")] public int SoLuong { get; init; }
}
