/*
Normalize legacy POS product categories into a single service category.

Reason:
- UI now uses one unified category: N'Dịch vụ'.
- Older data may still store variants of N'Thuê Dụng cụ'.
*/

UPDATE dbo.Products
SET Category = N'Dịch vụ'
WHERE LTRIM(RTRIM(ISNULL(Category, N''))) IN (
    N'Thuê Dụng cụ',
    N'Thuê dụng cụ',
    N'Thue Dung cu',
    N'Thue dung cu'
);
GO
