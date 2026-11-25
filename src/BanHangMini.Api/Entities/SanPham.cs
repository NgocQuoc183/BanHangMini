public class SanPham : ICoKiemToan {
    public int Id {get; set;}
    public int DanhMucId {get; set;}
    public DanhMuc DanhMuc {get; set;} = null!;
    public string Ten {get; set;} = string.Empty;
    public decimal DonGia {get; set; }
    public int TonKho {get; set;}
    public string? GhiChu {get; set;}
    public bool DaXoa {get; set;}
    public byte[] PhienBan {get; set;} = Array.Empty<byte>();
    public DateTime NgayTao {get; set;}
    public string? NguoiTao {get; set;}
    public DateTime? NgayCapNhat {get;set;}
    public string? NguoiCapNhat {get;set;}
}