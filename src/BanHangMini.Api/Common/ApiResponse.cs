public class ApiResponse<T>
{
    public bool ThanhCong { get; init ;}
    public string? Loi { get; init ;}
    public T? DuLieu { get; init ;}

    public static ApiResponse<T> Ok(T? duLieu)
    {
        return new ApiResponse<T>
        {
            ThanhCong = true,
            DuLieu = duLieu
        };
    }

    public static ApiResponse<T> Fail(string loi)
    {
        return new ApiResponse<T>
        {
            ThanhCong = false,
            Loi = loi
        };
    }
}