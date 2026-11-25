public class ChiTietDonHang
{
    public int Id { get; set; }
    public int DonHangId { get; set; }
    public DonHang DonHang { get; set; } = null!;
    public int SanPhamId { get; set; }
    public SanPham SanPham { get; set; } = null!;
    public int SoLuong { get; set; }
    public decimal DonGia { get; set; }
    public decimal ThanhTien { get; set; }
}
