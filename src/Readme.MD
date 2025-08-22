- Chia kiến trúc làm 5 layer
+ Startup layer: Nơi khởi chạy appp
+ Presentation layer: trả về UI, Api Response
+ Application layer: Xử lý logic business chính
+ Domain layer: Các domain nghiệp vụ
+ Infrastructure layer: Xử lý data, hệ thống kết nối bên ngoài
- Lý do: Mỗi layer đảm nhận 1 công việc riêng như đảm nhận xử lý business, kết nối bên ngoài, trả về format dữ liệu....

- Tại sao chọn mô hình Mediator, không chọn mô hình Controller Service Repository
+ Che dấu logic thực sự của yêu cầu, chỉ cần quan tâm yêu cầu command, trả kết quả
+ Đối với service, phải khai báo cụ thể hàm nào
+ Mở rộng trong tương lai nếu
  + Tách việc đọc ghi riêng
  + Publish event
+ Bất lợi của MediatR
  + Thư viện trở thành commercial và community, phải mua license nếu doanh thu > 5tr đô
  + Key Community có thời hạn 1 năm, mỗi năm phải sinh key mới. 
    + Hiện tại mediatR kiểm soát không chặt chẽ, chỉ báo log khi sử dụng, chưa có vấn đề gì trong hiện tại
    + Cần phải viết MediatR thủ công nếu bản quyền bị siết chặt
+ Bất lợi của Service
  + Business phình to, source code phình to, khó kiểm soát nếu tổ chức không tốt

Log sử dụng Serilog
- Cấu hình dễ sử dụng, hiện đại
- Sink được nhiều bên: File, Console, hệ thống ghi log
- Nhược điểm: Cấu hình bằng code chủ yếu, cũng có file ...
- Chỉ log những thông tin quan trọng, không log thông tin dư thừa
So sánh:
- NLog. Log4Net hơi cũ
- MS Logger ghi log đơn giản, ít tính năng
- OpenTelemetry: Cần cấu hình


Database Quan hệ
- Postgres vì read ổn, query plan chuẩn
  + Hiệu suất ok
  + Postgres đã tính hợp sẵn cấu hình pool trong connection string, max pool size mặc định = 100
- Không chọn SQL Server
  + Tốn chi phí license
  + Tốc độ truy vấn raw và truy vấn store quá khác biệt
  + Engine chạy index không tối ưu
  + Chỉ phù hợp với hệ thống trung bình trở xuống

NoSQL 
- MongoDB
  + NoSQL phổ biến, tài liệu đẩy đủ, cộng đồng lớn
  + Read Write tốt, chịu tải cao hơn DB quan hệ
  + Triết lý hạn chế join, nên tốc độ rất tốt
  + Nhược điểm: Không hỗ trợ join tốt, phải dùng aggregate. Không phù hợp với báo cáo

- Kết hợp NoSql và DB quan hệ: Mongo cho khả năng đọc tốt, Postgres DB quan hệ đảm bảo độ tin cậy và transaction tốt
+ Cần cơ chế Streaming data để sync 2 NoSQL và DB

Cache: Redis
- Không sử dụng ImmemoryCache, vì ram của ứng dụng hạn chế, cache sẽ bị xóa nếu restart server. Không thể quản lý trực tiếp như thêm xóa, không thể chia sẻ data cho các service khác
- Redis không chỉ là cache. Có 1 số cơ chế rất hay: lock, inc, pub sub
- Tốc độ nhanh, Khả năng chịu tải của nó rất tốt
- Cần cơ chế validate cache chính xác để data đảm bảo đúng
- Không sử dụng DB làm cache, vì tăng tải lên DB
- ConnectionMultiplexer singleton lifecycle đã tái sử dụng, threadsafe