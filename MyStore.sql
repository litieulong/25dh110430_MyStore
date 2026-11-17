-- Bang User: quan ly ca tai khoan cua Khach hang va Admin
CREATE TABLE [dbo].[User] (
    [Username]     NVARCHAR (255) NOT NULL,
    [Password] NCHAR (50)     NOT NULL,
   [UserRole]     NCHAR (1) NOT NULL, -- A: admin, C: customer
    PRIMARY KEY CLUSTERED ([Username] ASC)
);

SELECT Username, UserRole
FROM dbo.[User];



--Bang Customer: luu thong tin khach hang
CREATE TABLE [dbo].[Customer] (
    [CustomerID]          INT IDENTITY (1, 1) NOT NULL,
    [CustomerName]     NVARCHAR (MAX) NOT NULL,
    [CustomerPhone]    NVARCHAR (15)  NOT NULL,
    [CustomerEmail]      NVARCHAR (MAX) NULL,
    [CustomerAddress]  NVARCHAR (MAX) NULL,
    [Username]              NVARCHAR (255) NOT NULL,
    PRIMARY KEY CLUSTERED ([CustomerID] ASC),
    CONSTRAINT [FK_User_Customer] FOREIGN KEY ([Username]) REFERENCES [dbo].[User] ([Username])
);

--Bang Category: luu thong tin danh muc san pham
CREATE TABLE [dbo].[Category] (
    [CategoryID]            INT            IDENTITY (1, 1) NOT NULL,
    [CategoryName]      NVARCHAR (MAX) NOT NULL,
    PRIMARY KEY CLUSTERED ([CategoryID] ASC)
);

SELECT *
FROM dbo.Category;

SELECT 
    p.ProductID,
    p.ProductName,
    p.CategoryID,
    c.CategoryName
FROM dbo.Product p
INNER JOIN dbo.Category c
    ON p.CategoryID = c.CategoryID;

    SELECT ProductID, ProductName, CategoryID
FROM dbo.Product
ORDER BY CategoryID;

--Bang Product: luu thong tin san pham
CREATE TABLE [dbo].[Product] (
    [ProductID]              INT             IDENTITY (1, 1) NOT NULL,
    [CategoryID]            INT            NOT NULL,
    [ProductName]        NVARCHAR (MAX)  NOT NULL,
    [ProductDecription]  NVARCHAR (MAX)  NOT NULL,
    [ProductPrice]          DECIMAL (18, 2) NOT NULL,
    [ProductImage]        NVARCHAR (MAX)  NULL,
    PRIMARY KEY CLUSTERED ([ProductID] ASC),
    CONSTRAINT [FK_Pro_Category] FOREIGN KEY ([CategoryID]) REFERENCES [dbo].[Category] ([CategoryID])
);

--Product thêm tồn kho
ALTER TABLE [dbo].[Product]
ADD [StockQuantity] INT NOT NULL DEFAULT 0;


SELECT DB_NAME() AS CurrentDatabase;

SELECT COUNT(*) AS SoLuongSanPham
FROM dbo.Product;

SELECT *
FROM dbo.Product;

--Product có thể có nhiều màu
CREATE TABLE [dbo].[ProductColor] (
    [ProductColorID] INT IDENTITY (1,1) NOT NULL,
    [ProductID] INT NOT NULL,
    [ColorName] NVARCHAR(100) NOT NULL,

    PRIMARY KEY CLUSTERED ([ProductColorID] ASC),

    CONSTRAINT [FK_ProductColor_Product]
        FOREIGN KEY ([ProductID])
        REFERENCES [dbo].[Product] ([ProductID])
);

SELECT * FROM ProductColor;

ALTER TABLE OrderDetail
ADD ProductColorID INT NULL;

ALTER TABLE OrderDetail
ADD CONSTRAINT FK_OrderDetail_ProductColor
FOREIGN KEY (ProductColorID)
REFERENCES ProductColor(ProductColorID);

SELECT
    COLUMN_NAME,
    DATA_TYPE,
    IS_NULLABLE
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'OrderDetail';

SELECT
    fk.name AS ForeignKeyName,
    COL_NAME(fkc.parent_object_id, fkc.parent_column_id) AS ColumnName,
    OBJECT_NAME(fk.referenced_object_id) AS ReferencedTable
FROM sys.foreign_keys fk
JOIN sys.foreign_key_columns fkc
    ON fk.object_id = fkc.constraint_object_id
WHERE fk.name = 'FK_OrderDetail_ProductColor';


--Product có thể có nhiều hình
CREATE TABLE [dbo].[ProductImage] (
    [ProductImageID] INT IDENTITY (1,1) NOT NULL,
    [ProductColorID] INT NOT NULL,
    [ImagePath] NVARCHAR(MAX) NOT NULL,

    PRIMARY KEY CLUSTERED ([ProductImageID] ASC),

    CONSTRAINT [FK_ProductImage_ProductColor]
        FOREIGN KEY ([ProductColorID])
        REFERENCES [dbo].[ProductColor] ([ProductColorID])
);

SELECT *
FROM ProductImage;

--Bang Order: luu thong tin don hang cua khach hang
CREATE TABLE [dbo].[Order] (
    [OrderID]                 INT            IDENTITY (1, 1) NOT NULL,
    [CustomerID]          INT            NOT NULL,
    [OrderDate]             DATE           NOT NULL,
    [TotalAmount]         DECIMAL (18, 2) NOT NULL,
    [PaymentStatus]     NVARCHAR (MAX) NULL,
    [AddressDelivery]   NVARCHAR (MAX) NOT NULL,
    PRIMARY KEY CLUSTERED ([OrderID] ASC),
    FOREIGN KEY ([CustomerID]) REFERENCES [dbo].[Customer] ([CustomerID])
);

SELECT ProductID, ProductName, ProductPrice, StockQuantity
FROM Product
ORDER BY ProductID DESC;

--Bang OrderDetail: luu thong tin chi tiet cac san pham co trong don hang
CREATE TABLE [dbo].[OrderDetail] (
    [ID]        INT        IDENTITY (1, 1) NOT NULL,
    [ProductID] INT        NOT NULL,
    [OrderID]   INT        NOT NULL,
    [Quantity]  INT        NOT NULL,
    [UnitPrice] DECIMAL (18, 2) NOT NULL,
    PRIMARY KEY CLUSTERED ([ID] ASC),
    FOREIGN KEY ([ProductID]) REFERENCES [dbo].[Product] ([ProductID]),
    FOREIGN KEY ([OrderID]) REFERENCES [dbo].[Order] ([OrderID])
);

SELECT DB_NAME() AS CurrentDatabase;

SELECT COLUMN_NAME, DATA_TYPE
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'Order'
  AND COLUMN_NAME = 'CancelledAt';

ALTER TABLE [Order]
ADD CancelledAt DATETIME NULL;

SELECT TOP 10 OrderID, OrderStatus, CancelledAt
FROM [Order];

BEGIN TRANSACTION;

-- Xóa chi tiết của đơn hàng đã hủy quá 6 ngày
DELETE FROM OrderDetail
WHERE OrderID IN
(
    SELECT OrderID
    FROM [Order]
    WHERE OrderStatus = N'Đã hủy'
      AND CancelledAt IS NOT NULL
      AND CancelledAt <= DATEADD(DAY, -6, GETDATE())
);

-- Xóa đơn hàng đã hủy quá 6 ngày
DELETE FROM [Order]
WHERE OrderStatus = N'Đã hủy'
  AND CancelledAt IS NOT NULL
  AND CancelledAt <= DATEADD(DAY, -6, GETDATE());

COMMIT TRANSACTION;

  

ALTER TABLE [dbo].[Order]
ADD [OrderStatus] NVARCHAR(50)
NOT NULL
DEFAULT N'Chờ xác nhận';



ALTER AUTHORIZATION ON DATABASE::MyStore TO sa;

