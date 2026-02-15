# TRIỂN KHAI LÊN RASPBERRY PI 5 (DEPLOYMENT)

## 1. Chuẩn bị môi trường

- OS: Raspberry Pi OS Lite (64-bit).
- Docker & Docker Compose: Cài đặt bản mới nhất.
- Mount ổ cứng NVMe:
  - Format ext4.
  - Mount vào đường dẫn cố định, ví dụ: `/mnt/nvme_data`.

## 2. Docker Compose File (`docker-compose.yml`)

Cấu trúc service cần thiết:

1.  **Postgres:** Map volume data ra thẻ nhớ hoặc NVMe.
2.  **Backend (ASP.NET):**
    - Build image từ `Dockerfile`.
    - Map volume: `/mnt/nvme_data:/app/data` (Để container ghi được vào ổ cứng thật).
    - Environment: ConnectionString, JWT_Secret.
3.  **Frontend (Nginx/Node):** Serve static files.

## 3. Public ra Internet (Không mở port)

- Sử dụng **Cloudflare Tunnel (`cloudflared`)**.
- Setup:
  - Cài `cloudflared` lên RPi.
  - Authenticate với Cloudflare.
  - Config tunnel trỏ domain `drive.cuatoi.com` -> `localhost:Frontend_Port`.

## 4. Maintenance (Bảo trì)

- Backup Database định kỳ (Cronjob dump SQL).
- Monitor nhiệt độ RPi.
