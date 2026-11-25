using System.ComponentModel.DataAnnotations;

public class SuaSanPhamRequest {
    [Required, MaxLength(200)]
    public string Ten {get; init;} = string.Empty;

    [Range(0, double.MaxValue)]
    public decimal DonGia {get; init;}

    [Range(0, int.MaxValue)]
    public int TonKho {get;init;}

    [MaxLength(500)]
    public string? GhiChu {get; init;}

    [Required]
    public byte[] PhienBan {get; init;} = Array.Empty<byte>();
}