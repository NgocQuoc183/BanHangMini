using System.ComponentModel.DataAnnotations;

public class SuaDanhMucRequest
{
    [Required, MaxLength(100)]
    public string Ten { get; init; } = string.Empty;
}
