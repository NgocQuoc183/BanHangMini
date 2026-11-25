using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

public class XuLyNgoaiLeMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<XuLyNgoaiLeMiddleware> _nhatKy;
    private readonly JsonSerializerSettings _caiDatJson;

    public XuLyNgoaiLeMiddleware(RequestDelegate next,
        ILogger<XuLyNgoaiLeMiddleware> nhatKy,
        IOptions<MvcNewtonsoftJsonOptions> tuyChonJson)
    {
        _next = next;
        _nhatKy = nhatKy;
        _caiDatJson = tuyChonJson.Value.SerializerSettings;
    }

    public async Task InvokeAsync(HttpContext ngucanh)
    {
        try
        {
            await _next(ngucanh);
        }
        catch (Exception ex)
        {
            if (ngucanh.Response.HasStarted)
                throw;

            var (maTrangThai, loi) = ex switch
            {
                KhongTimThayException e => (StatusCodes.Status404NotFound, e.Message),
                NghiepVuException e => (StatusCodes.Status400BadRequest, e.Message),
                XungDotException e => (StatusCodes.Status409Conflict, e.Message),
                _ => (StatusCodes.Status500InternalServerError, "Lỗi hệ thống")
            };

            if (maTrangThai == StatusCodes.Status500InternalServerError)
            {
                _nhatKy.LogError(ex, "Lỗi chưa xử lý tại {DuongDan}", ngucanh.Request.Path);
            }
            else
            {
                _nhatKy.LogWarning(
                    "{LoaiLoi} tại {DuongDan}: {ThongDiep}",
                    ex.GetType().Name,
                    ngucanh.Request.Path,
                    ex.Message);
            }

            ngucanh.Response.StatusCode = maTrangThai;
            ngucanh.Response.ContentType = "application/json; charset=utf-8";
            await ngucanh.Response.WriteAsync(
                JsonConvert.SerializeObject(ApiResponse<object>.Fail(loi), _caiDatJson));
        }
    }
}