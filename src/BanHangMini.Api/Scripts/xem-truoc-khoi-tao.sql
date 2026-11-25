IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
GO

CREATE TABLE [DanhMucs] (
    [Id] int NOT NULL IDENTITY,
    [Ten] nvarchar(100) NOT NULL,
    CONSTRAINT [PK_DanhMucs] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [HopThuDis] (
    [Id] bigint NOT NULL IDENTITY,
    [Loai] varchar(50) NOT NULL,
    [NoiDung] nvarchar(max) NOT NULL,
    [NgayTao] datetime2 NOT NULL,
    [NgayGui] datetime2 NULL,
    [SoLanThu] int NOT NULL,
    [LoiCuoi] nvarchar(500) NULL,
    CONSTRAINT [PK_HopThuDis] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [NguoiDungs] (
    [Id] int NOT NULL IDENTITY,
    [TenDangNhap] varchar(50) NOT NULL,
    [MatKhauBam] nvarchar(200) NOT NULL,
    [VaiTro] nvarchar(20) NOT NULL,
    [NgayTao] datetime2 NOT NULL,
    CONSTRAINT [PK_NguoiDungs] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [SanPhams] (
    [Id] int NOT NULL IDENTITY,
    [DanhMucId] int NOT NULL,
    [Ten] nvarchar(200) NOT NULL,
    [DonGia] decimal(18,2) NOT NULL,
    [TonKho] int NOT NULL,
    [GhiChu] nvarchar(500) NULL,
    [DaXoa] bit NOT NULL,
    [PhienBan] rowversion NOT NULL,
    [NgayTao] datetime2 NOT NULL,
    [NguoiTao] nvarchar(50) NULL,
    [NgayCapNhat] datetime2 NULL,
    [NguoiCapNhat] nvarchar(50) NULL,
    CONSTRAINT [PK_SanPhams] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_SanPhams_DanhMucs_DanhMucId] FOREIGN KEY ([DanhMucId]) REFERENCES [DanhMucs] ([Id]) ON DELETE NO ACTION
);
GO

CREATE TABLE [DonHangs] (
    [Id] int NOT NULL IDENTITY,
    [MaDonHang] varchar(20) NOT NULL,
    [NguoiDungId] int NOT NULL,
    [TrangThai] nvarchar(20) NOT NULL,
    [TongTien] decimal(18,2) NOT NULL,
    [KhoaChongTrung] varchar(64) NULL,
    [NgayTao] datetime2 NOT NULL,
    [NguoiTao] nvarchar(50) NULL,
    [NgayCapNhat] datetime2 NULL,
    [NguoiCapNhat] nvarchar(50) NULL,
    CONSTRAINT [PK_DonHangs] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_DonHangs_NguoiDungs_NguoiDungId] FOREIGN KEY ([NguoiDungId]) REFERENCES [NguoiDungs] ([Id]) ON DELETE NO ACTION
);
GO

CREATE TABLE [ChiTietDonHangs] (
    [Id] int NOT NULL IDENTITY,
    [DonHangId] int NOT NULL,
    [SanPhamId] int NOT NULL,
    [SoLuong] int NOT NULL,
    [DonGia] decimal(18,2) NOT NULL,
    [ThanhTien] decimal(18,2) NOT NULL,
    CONSTRAINT [PK_ChiTietDonHangs] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_ChiTietDonHangs_DonHangs_DonHangId] FOREIGN KEY ([DonHangId]) REFERENCES [DonHangs] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_ChiTietDonHangs_SanPhams_SanPhamId] FOREIGN KEY ([SanPhamId]) REFERENCES [SanPhams] ([Id]) ON DELETE NO ACTION
);
GO

CREATE INDEX [IX_ChiTietDonHangs_DonHangId] ON [ChiTietDonHangs] ([DonHangId]);
GO

CREATE INDEX [IX_ChiTietDonHangs_SanPhamId] ON [ChiTietDonHangs] ([SanPhamId]);
GO

CREATE UNIQUE INDEX [IX_DanhMucs_Ten] ON [DanhMucs] ([Ten]);
GO

CREATE UNIQUE INDEX [IX_DonHangs_KhoaChongTrung] ON [DonHangs] ([KhoaChongTrung]) WHERE [KhoaChongTrung] IS NOT NULL;
GO

CREATE UNIQUE INDEX [IX_DonHangs_MaDonHang] ON [DonHangs] ([MaDonHang]);
GO

CREATE INDEX [IX_DonHangs_NguoiDungId] ON [DonHangs] ([NguoiDungId]);
GO

CREATE UNIQUE INDEX [IX_NguoiDungs_TenDangNhap] ON [NguoiDungs] ([TenDangNhap]);
GO

CREATE INDEX [IX_SanPhams_DanhMucId_DonGia] ON [SanPhams] ([DanhMucId], [DonGia]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20261015020153_KhoiTao', N'8.0.10');
GO

COMMIT;
GO

