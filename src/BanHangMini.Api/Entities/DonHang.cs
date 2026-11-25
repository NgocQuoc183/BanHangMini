public class DonHang : ICoKiemToan
{
    public int Id { get; set; }
    public string MaDonHang { get; set; } = string.Empty;
    public int NguoiDungId { get; set; }
    public NguoiDung NguoiDung { get; set; } = null!;
    public TrangThaiDonHang TrangThai { get; set; }
    public decimal TongTien { get; set; }
    public string? KhoaChongTrung { get; set; }
    public List<ChiTietDonHang> ChiTiets { get; set; } = new();

    public DateTime NgayTao { get; set; }
    public string? NguoiTao { get; set; }
    public DateTime? NgayCapNhat { get; set; }
    public string? NguoiCapNhat { get; set; }
}