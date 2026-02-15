# LỘ TRÌNH PHÁT TRIỂN FRONTEND (REACTJS)

## Giai đoạn 1: Setup & UI Base

1.  Init: Vite + React + TypeScript + TailwindCSS.
2.  State Management: Zustand (quản lý user session) + TanStack Query (quản lý data server).
3.  HTTP Client: Axios (Cấu hình Interceptor để tự động gắn Bearer Token vào header).

## Giai đoạn 2: Chức năng Core

1.  **File Explorer UI:**
    - Grid View & List View.
    - Component hiển thị Icon theo đuôi file.
2.  **Upload Component:**
    - Sử dụng `Axios onUploadProgress` để vẽ thanh Progress Bar.
    - Hỗ trợ Drag & Drop (Kéo thả file vào trình duyệt).
3.  **Navigation:**
    - Xử lý click vào Folder -> Gọi API lấy nội dung folder đó -> Update URL.
    - Nút "Back" hoặc Breadcrumb để quay lại.

## Giai đoạn 3: Trải nghiệm người dùng (UX)

1.  **Context Menu:** Click chuột phải vào file để hiện menu (Download, Rename, Delete).
2.  **Preview:** Double click vào ảnh/pdf để xem trước mà không cần tải.
