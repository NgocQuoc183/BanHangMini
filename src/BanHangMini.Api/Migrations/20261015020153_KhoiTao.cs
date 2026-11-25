using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BanHangMini.Api.Migrations
{
    /// <inheritdoc />
    public partial class KhoiTao : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DanhMucs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Ten = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DanhMucs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HopThuDis",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Loai = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    NoiDung = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NgayTao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NgayGui = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SoLanThu = table.Column<int>(type: "int", nullable: false),
                    LoiCuoi = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HopThuDis", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "NguoiDungs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenDangNhap = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MatKhauBam = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    VaiTro = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    NgayTao = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NguoiDungs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SanPhams",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DanhMucId = table.Column<int>(type: "int", nullable: false),
                    Ten = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    DonGia = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TonKho = table.Column<int>(type: "int", nullable: false),
                    GhiChu = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    DaXoa = table.Column<bool>(type: "bit", nullable: false),
                    PhienBan = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false),
                    NgayTao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NguoiTao = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    NgayCapNhat = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NguoiCapNhat = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SanPhams", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SanPhams_DanhMucs_DanhMucId",
                        column: x => x.DanhMucId,
                        principalTable: "DanhMucs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DonHangs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaDonHang = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: false),
                    NguoiDungId = table.Column<int>(type: "int", nullable: false),
                    TrangThai = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    TongTien = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    KhoaChongTrung = table.Column<string>(type: "varchar(64)", unicode: false, maxLength: 64, nullable: true),
                    NgayTao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NguoiTao = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    NgayCapNhat = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NguoiCapNhat = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DonHangs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DonHangs_NguoiDungs_NguoiDungId",
                        column: x => x.NguoiDungId,
                        principalTable: "NguoiDungs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ChiTietDonHangs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DonHangId = table.Column<int>(type: "int", nullable: false),
                    SanPhamId = table.Column<int>(type: "int", nullable: false),
                    SoLuong = table.Column<int>(type: "int", nullable: false),
                    DonGia = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ThanhTien = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChiTietDonHangs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChiTietDonHangs_DonHangs_DonHangId",
                        column: x => x.DonHangId,
                        principalTable: "DonHangs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChiTietDonHangs_SanPhams_SanPhamId",
                        column: x => x.SanPhamId,
                        principalTable: "SanPhams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietDonHangs_DonHangId",
                table: "ChiTietDonHangs",
                column: "DonHangId");

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietDonHangs_SanPhamId",
                table: "ChiTietDonHangs",
                column: "SanPhamId");

            migrationBuilder.CreateIndex(
                name: "IX_DanhMucs_Ten",
                table: "DanhMucs",
                column: "Ten",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DonHangs_KhoaChongTrung",
                table: "DonHangs",
                column: "KhoaChongTrung",
                unique: true,
                filter: "[KhoaChongTrung] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_DonHangs_MaDonHang",
                table: "DonHangs",
                column: "MaDonHang",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DonHangs_NguoiDungId",
                table: "DonHangs",
                column: "NguoiDungId");

            migrationBuilder.CreateIndex(
                name: "IX_NguoiDungs_TenDangNhap",
                table: "NguoiDungs",
                column: "TenDangNhap",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SanPhams_DanhMucId_DonGia",
                table: "SanPhams",
                columns: new[] { "DanhMucId", "DonGia" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChiTietDonHangs");

            migrationBuilder.DropTable(
                name: "HopThuDis");

            migrationBuilder.DropTable(
                name: "DonHangs");

            migrationBuilder.DropTable(
                name: "SanPhams");

            migrationBuilder.DropTable(
                name: "NguoiDungs");

            migrationBuilder.DropTable(
                name: "DanhMucs");
        }
    }
}
