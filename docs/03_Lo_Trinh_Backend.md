# LỘ TRÌNH PHÁT TRIỂN BACKEND (ASP.NET CORE)

## Giai đoạn 1: Khởi tạo & Auth

1.  Init Project: Web API, Entity Framework Core (PostgreSQL).
2.  Setup Identity: Viết API Login/Register trả về JWT Token.
3.  Cấu hình Swagger: Để test API dễ dàng.

## Giai đoạn 2: Cơ chế Upload/Download (Quan trọng nhất)

1.  **Cấu hình Kestrel:** Tăng giới hạn `MaxRequestBodySize` (mặc định chỉ 30MB) lên giới hạn mong muốn (vd: 5GB).
2.  **Service Layer:** Viết `FileStorageService`.
    - Input: `IFormFile` (cho file nhỏ) hoặc `MultipartReader` (cho file lớn).
    - Logic: Tạo tên file unique -> Write to Disk -> Return path.
3.  **API Upload:** - Endpoint: `POST /api/files/upload`
    - Nhận file, lưu vào đĩa, lưu metadata vào DB `Files`.
4.  **API Download:**
    - Endpoint: `GET /api/files/{id}/download`
    - Tìm path trong DB -> Return `FileStreamResult` (hỗ trợ Resume download).

## Giai đoạn 3: Quản lý thư mục (File System Logic)

1.  **API Create Folder:** `POST /api/folders` (Name, ParentId).
2.  **API Get Content:** `GET /api/folders/{id}/content`.
    - Trả về danh sách folder con và file trong folder đó.
    - Xử lý đệ quy hoặc query đơn giản theo `ParentId`.
3.  **API Breadcrumb:** Trả về đường dẫn từ Root đến folder hiện tại (VD: Home > Work > Project A).

## Giai đoạn 4: Tối ưu & Bảo mật

1.  **Validation:** Chỉ cho phép user truy cập file do mình sở hữu (`OwnerId == CurrentUserId`).
2.  **Background Job:** Dùng `Hangfire` hoặc `Quartz.net` để dọn dẹp file rác (file vật lý tồn tại nhưng không có trong DB).
