# THIẾT KẾ CƠ SỞ DỮ LIỆU (DATABASE SCHEMA)

## 1. Bảng Users (Người dùng)

| Column       | Type      | Description                 |
| :----------- | :-------- | :-------------------------- |
| Id           | UUID (PK) | Khóa chính                  |
| Username     | Varchar   | Tên đăng nhập               |
| PasswordHash | Varchar   | Mật khẩu đã mã hóa          |
| StorageQuota | BigInt    | Giới hạn dung lượng (bytes) |
| UsedStorage  | BigInt    | Dung lượng đã dùng          |

## 2. Bảng Folders (Thư mục)

_Mô hình: Adjacency List (Danh sách kề) cho cấu trúc cây._

| Column    | Type                | Description                                          |
| :-------- | :------------------ | :--------------------------------------------------- |
| Id        | UUID (PK)           | Khóa chính                                           |
| Name      | Varchar             | Tên thư mục                                          |
| ParentId  | UUID (FK, Nullable) | Trỏ về chính bảng Folders. Null = Thư mục gốc (Root) |
| OwnerId   | UUID (FK)           | Trỏ về bảng Users                                    |
| Path      | Ltree/Varchar       | (Optional) Đường dẫn phân cấp để truy vấn nhanh      |
| CreatedAt | DateTime            | Ngày tạo                                             |

## 3. Bảng Files (Tệp tin)

| Column       | Type                | Description                                                |
| :----------- | :------------------ | :--------------------------------------------------------- |
| Id           | UUID (PK)           | Khóa chính                                                 |
| FileName     | Varchar             | Tên hiển thị (vd: baocao.pdf)                              |
| PhysicalPath | Varchar             | Đường dẫn thực trên ổ cứng (vd: /data/u1/2026/02/guid.pdf) |
| ContentType  | Varchar             | MIME Type (application/pdf, image/png...)                  |
| Size         | BigInt              | Kích thước file (bytes)                                    |
| FolderId     | UUID (FK, Nullable) | Thư mục chứa file. Null = Root                             |
| OwnerId      | UUID (FK)           | Người sở hữu                                               |
| IsDeleted    | Boolean             | Soft delete (thùng rác)                                    |

## 4. Lưu ý quan trọng

- **Indexing:** Cần đánh Index cho `ParentId` và `OwnerId` để tốc độ duyệt thư mục nhanh.
- **Foreign Keys:** Thiết lập `ON DELETE CASCADE` cẩn thận (Xóa folder cha -> xóa folder con và file).
