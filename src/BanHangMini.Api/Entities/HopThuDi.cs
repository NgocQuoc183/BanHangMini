public class HopThuDi
{
    public long Id { get; set; }
    public string Loai { get; set; } = string.Empty;
    public string NoiDung { get; set; } = string.Empty;
    public DateTime NgayTao { get; set; }
    public DateTime? NgayGui { get; set; }
    public int SoLanThu { get; set; }
    public string? LoiCuoi { get; set; }
}
