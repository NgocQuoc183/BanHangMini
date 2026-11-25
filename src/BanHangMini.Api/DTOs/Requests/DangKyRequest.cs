using System.ComponentModel.DataAnnotations;

public class DangKyRequest
{
    [Required(ErrorMessage = "Tên đăng nhập là bắt buộc")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "Tên đăng nhập phải từ 3 đến 50 ký tự")]
    [RegularExpression(@"^[a-zA-Z0-9._-]+$", ErrorMessage = "Tên đăng nhập chỉ được chứa chữ cái không dấu, chữ số, dấu chấm, gạch dưới và gạch nối")]
    public string TenDangNhap { get; init; } = string.Empty;

    [Required(ErrorMessage = "Mật khẩu là bắt buộc")]
    [StringLength(128, MinimumLength = 8, ErrorMessage = "Mật khẩu phải từ 8 đến 128 ký tự")]
    public string MatKhau { get; init; } = string.Empty;
}
