using System.ComponentModel.DataAnnotations;

public class TaoDanhMucRequest
{
    [Required, MaxLength(100)]
    public string Ten { get; init; } = string.Empty;
}
