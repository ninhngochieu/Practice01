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

