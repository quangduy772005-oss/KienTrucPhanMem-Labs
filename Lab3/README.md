# Lab 3 - Three-Tier Architecture (Todo)

## Cách chạy (VS Code)
Mở thư mục ThreeTierArchitecture bằng VS Code, nhấn F5 (Run Todo.API). Database nằm file riêng TodoDB.sql.
1. Mở SSMS, chạy `Database/TodoDB.sql`.
2. Sửa `Server=` trong `Todo.API/appsettings.json` cho đúng tên SQL Server của bạn.
3. Tạo solution & chạy API:
   ```
   cd ThreeTierArchitecture
   dotnet new sln -n ThreeTierArchitecture
   dotnet sln add Todo.Domain Todo.Infrastructure Todo.Application Todo.API
   dotnet run --project Todo.API --launch-profile https
   ```
   (hoặc mở Visual Studio, Add Existing Project cho 4 project rồi F5)
4. Mở Swagger kiểm tra API, xem port HTTPS rồi sửa `API_BASE` trong `Todo.UI/index.html`.
5. Mở `Todo.UI/index.html` bằng trình duyệt.

## API
| Method | URL | Mô tả |
|---|---|---|
| GET | /api/v1/todos | Lấy tất cả |
| GET | /api/v1/todos/{id} | Lấy 1 todo |
| POST | /api/v1/todos | Tạo mới `{ "title": "...", "isCompleted": false }` |
| PUT | /api/v1/todos/{id} | Cập nhật |
| DELETE | /api/v1/todos/{id} | Xóa |
