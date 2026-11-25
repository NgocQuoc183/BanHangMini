using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

const string lenhDatLaiMatKhauQuanTri = "--reset-admin-password";
const string lenhTaoTaiKhoanQuanTri = "--create-admin";
var datLaiMatKhauQuanTri = args.Contains(
    lenhDatLaiMatKhauQuanTri, StringComparer.Ordinal);
var thamSoChay = args.ToList();
var viTriTaoQuanTri = thamSoChay.IndexOf(lenhTaoTaiKhoanQuanTri);
string? tenDangNhapQuanTriMoi = null;
if (viTriTaoQuanTri >= 0)
{
    if (datLaiMatKhauQuanTri)
    {
        throw new ArgumentException(
            "Chỉ được chạy một trong hai lệnh quản trị mỗi lần.");
    }

    if (viTriTaoQuanTri + 1 >= thamSoChay.Count
        || thamSoChay[viTriTaoQuanTri + 1].StartsWith(
            "--", StringComparison.Ordinal))
    {
        throw new ArgumentException(
            "Cách dùng: --create-admin <tên-đăng-nhập>");
    }

    tenDangNhapQuanTriMoi = thamSoChay[viTriTaoQuanTri + 1];
    thamSoChay.RemoveRange(viTriTaoQuanTri, 2);
}
if (datLaiMatKhauQuanTri)
{
    thamSoChay.RemoveAll(arg => string.Equals(
        arg, lenhDatLaiMatKhauQuanTri, StringComparison.Ordinal));
}
var builder = WebApplication.CreateBuilder(
    thamSoChay.ToArray());

builder.Services.AddControllers().AddNewtonsoftJson(o =>
{
    var s = o.SerializerSettings;
    s.ContractResolver = new CamelCasePropertyNamesContractResolver();
    s.NullValueHandling = NullValueHandling.Ignore;
    s.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
    s.DateTimeZoneHandling = DateTimeZoneHandling.Utc;
    s.DateFormatString = "yyyy-MM-dd'T'HH:mm:ss'Z'";
});
var jwtKey = builder.Configuration["Jwt:Key"]
    ?? throw new InvalidOperationException("Thiếu Jwt:Key");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(o =>
    {
        o.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtKey)),

            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],

            ValidateAudience = true,
            ValidAudience = builder.Configuration["Jwt:Audience"],

            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });
builder.Services.AddAuthorization();
// Giữ cơ chế tự trả 400 của [ApiController], chỉ đổi khuôn lời nhắn cho khớp ApiResponse
builder.Services.Configure<ApiBehaviorOptions>(o =>
    o.InvalidModelStateResponseFactory = ngucanh =>
    {
        var loi = string.Join("; ", ngucanh.ModelState.Values
            .SelectMany(v => v.Errors)
            .Select(e => string.IsNullOrEmpty(e.ErrorMessage) ? "Dữ liệu gửi lên không hợp lệ" : e.ErrorMessage));
        return new BadRequestObjectResult(ApiResponse<object>.Fail(loi));
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(o =>
{
    var luocDo = new OpenApiSecurityScheme
    {
        Name = "Authorization", In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http, Scheme = "bearer", BearerFormat = "JWT",
        Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
    };
    o.AddSecurityDefinition("Bearer", luocDo);
    o.AddSecurityRequirement(new OpenApiSecurityRequirement { [luocDo] = Array.Empty<string>() });
});

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<INguoiDungHienTai, NguoiDungHienTai>();
builder.Services.AddScoped<ISanPhamService, SanPhamService>();
builder.Services.AddScoped<IDanhMucService, DanhMucService>();
builder.Services.AddScoped<IDonHangService, DonHangService>();
builder.Services.AddScoped<IDangNhapService, DangNhapService>();
builder.Services.AddDbContext<AppDbContext>(o =>
{
    o.UseSqlServer(builder.Configuration.GetConnectionString("Default"));
    if (builder.Environment.IsDevelopment())
        o.LogTo(Console.WriteLine, LogLevel.Information).EnableSensitiveDataLogging();
});

var app = builder.Build();

if (datLaiMatKhauQuanTri)
{
    await DatLaiMatKhauQuanTri.ThucHienAsync(app.Services);
    await app.DisposeAsync();
    return;
}

if (tenDangNhapQuanTriMoi is not null)
{
    await DatLaiMatKhauQuanTri.TaoTaiKhoanAsync(
        app.Services, tenDangNhapQuanTriMoi);
    await app.DisposeAsync();
    return;
}

if (app.Environment.IsDevelopment())
{
    using var phamVi = app.Services.CreateScope();
    await DuLieuMau.NapAsync(phamVi.ServiceProvider);
}

app.UseMiddleware<XuLyNgoaiLeMiddleware>();   // 1. ngoài cùng
if (app.Environment.IsDevelopment()) { app.UseSwagger(); app.UseSwaggerUI(); }
// GĐ6: app.UseStaticFiles(); app.UseCors(...);   2. gọi chéo nguồn
// GĐ7: app.UseResponseCompression();             3. nén
// GĐ7: app.UseRateLimiter();                     4. giới hạn tần suất
app.UseAuthentication();                  
app.UseAuthorization();                         // 6. phân quyền
app.MapControllers();                           // 7. định tuyến
app.Run();

public partial class Program { }   // để dự án kiểm thử dùng được WebApplicationFactory<Program>