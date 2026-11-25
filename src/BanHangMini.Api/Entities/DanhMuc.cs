public class DanhMuc {
    public int Id {get; set;}
    public string Ten {get;set;} = string.Empty;
    public List<SanPham> SanPhams {get;set;} = new();
}