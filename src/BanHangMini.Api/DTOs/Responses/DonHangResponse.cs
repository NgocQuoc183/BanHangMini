public record DonHangResponse(int Id, string MaDonHang, string TrangThai, decimal TongTien, DateTime NgayTao, List<ChiTietDonHangResponse> Dong);
