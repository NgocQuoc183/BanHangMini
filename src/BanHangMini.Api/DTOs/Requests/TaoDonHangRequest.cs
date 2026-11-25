using System.ComponentModel.DataAnnotations;

public class TaoDonHangRequest
{
    [Required, MinLength(1, ErrorMessage = "Đơn hàng phải có ít nhất một dòng"), MaxLength(50)]
    public List<DongDonHangRequest> Dong { get; init; } = new();
}
