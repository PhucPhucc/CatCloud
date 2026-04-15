# DỰ ÁN: PERSONAL CLOUD (SELF-HOSTED GOOGLE DRIVE)

## 1. Giới thiệu

Xây dựng hệ thống lưu trữ đám mây cá nhân (tương tự Google Drive) và mở rộng thành Git Server (tương tự GitHub). Hệ thống chạy trên Raspberry Pi 5 tại nhà, truy cập từ internet an toàn.

## 2. Tech Stack (Công nghệ)

- **Backend:** ASP.NET Core Web API (.NET 8/9).
- **Frontend:** ReactJS (Vite) + Tailwind CSS + TanStack Query.
- **Database:** PostgreSQL (Lưu metadata).
- **Cache:** Redis (Tùy chọn, dùng cho session hoặc cache folder tree).
- **Infrastructure:** Raspberry Pi 5 (OS Linux), Docker, Docker Compose.
- **Storage:** NVMe SSD (Mount vào Docker Container).
- **Networking:** Cloudflare Tunnel (Không mở port router).

## 3. Kiến trúc hệ thống (System Architecture)

### Nguyên tắc cốt lõi (Core Principles)

1.  **Zero Binary in DB:** Tuyệt đối không lưu nội dung file (byte[]) vào Database. Database chỉ lưu Metadata (tên, đường dẫn, size...).
2.  **Streaming Upload:** Backend không buffer toàn bộ file vào RAM. Sử dụng `MultipartReader` để stream dữ liệu trực tiếp từ Network -> Disk.
3.  **Physical Storage:** File vật lý được lưu trên ổ cứng theo cấu trúc: `/data/{UserId}/{Year}/{Month}/{FileName}` để tránh quá tải một folder.

### Luồng dữ liệu (Data Flow)

1.  **Upload:** Client -> Nginx -> ASP.NET Core (Stream) -> NVMe Disk.
2.  **Metadata:** Sau khi lưu file thành công -> Ghi thông tin vào PostgreSQL.
3.  **Download:** Client -> Request ID -> API check quyền -> Trả về FileStream.

## 4. Mở rộng tương lai

- **Private Git Server:** Tích hợp Gitea hoặc LibGit2Sharp.
- **Media Streaming:** Xem video trực tiếp không cần tải về.



cloud-stogare
-shared
-temp
-users
