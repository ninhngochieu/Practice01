- Chọn kiến trúc: Clean Architecture với các layer Presentation, Application, Domain, Infrastructure
+ Các kiến trúc tương đương: Vertical Slice, Controller Service Repository
+ Lý do chọn: 
  + Phân tách các nhiệm vụ rõ ràng giữa các layer. Presentation Layer trả dữ liệu, Application xử lý logic, Domain thể hiện nghiệp vụ, Infrastructure tích hợp giao tiếp các hệ thống bên ngoài
+ Bất lợi: 
  + Phải tìm hiểu kĩ trước khi triển khai
  + Đặt các thành phần phải phù hợp với các layer để không bị phình to
  + Ban đầu sẽ phức tạp hơn với kiến trúc cổ điển như Controller Service Repository

- Sử dụng MediatR
+ Công nghệ tương đường: Service
+ Lý do: 
  + Che dấu việc triển khai cụ thể, bớt khai báo interface nhiều
  + Phân chia Command / Query. Code gọn hơn vì tách riêng logic đọc và ghi. Sau này có thể triển khai mô hình DB read write
  + Hỗ trợ publish nếu sau này cần triển khai mô hình event
  + Service sẽ bị phình to logic quá phức tạp. Có thể giải quyết tách logic bằng Partial class
  + Có Behaviour Pipeline, bóc tách 1 số logic như fluent validation, logging rất tiện
+ Bất lợi :
  + MediatR không tái sử dụng được logic của nhau (Thực tế ít logic được sử dụng chung)
  + Thư viện triển khai mô hình commercial license. Community Key phải gia hạn 1 năm. Hiện tại không sử dụng community key chỉ đang cảnh báo lỗi vì chưa siết chặt. Nếu siết chặt bản quyền cần phải tự viết
  + Phải viết cấu trúc đơn giản, rõ ràng để biết code chạy ở đâu

- Log: Sử dụng Serilog
+ Các công nghệ tương đường: Ms Log, NLog, Log4net
+ Lý do: 
  + Serilog tương đối hiện đại, cấu hình tinh chỉnh được nhiều
  + Hỗ trợ sink qua nhiều nguồn: Server, File, Console, Database, ElasticSearch, etc
  + MS log chỉ hỗ trợ cơ bản, không có cơ chế sink
  + NLog, Log4net hơi cũ
+ Bất lợi:
  + Phải biết cấu hình
  + Bắt log nhiều tốn tài nguyên server

- Database quan hệ: Postgres
+ Các công nghệ tương đường: MySQL, SQL Server, Oracle
+ Lý do:
  + Postgres tương đối hiện đại, hiện tại đang là xu thế
  + Postgres đọc ghi tốt
  + Query plan chuẩn
  + Hỗ trợ NoSQL (Có thể không cần thiết)
  + MSSQL tốn chi phí bản quyền, chi phí bản quyền cần tính toán kĩ lưỡng, query plan không chuẩn, cùng 1 câu query, chạy SP nhanh hơn query raw (5-10 lần), engine chọn index không tốt
  + Oracle chạy tốt trong hệ thống lớn, độ tin cậy cao, được dùng nhiều trong hệ thống ngân hàng, nhưng chi phí không hề rẻ
  + MySQL phù hợp với hệ thống nhỏ, hạn chế 1 số tính năng tùy phiên bản
+ Bất lợi:
  + Phải biết cấu hình, hiểu cơ chế
  + Sử dụng kèm với các framework như entity framework phải xem xét kĩ, nếu không ef sinh câu query không tối ưu, chạy rất chậm
  + Setup connection pool để chịu lỗi

- NoSQL: Mongo
+ Các công nghệ tương đường: Azure Cosmos DB
+ Lý do:
  + Đọc ghi nhanh
  + Triết lý hạn chế join, nên lấy kết quả rất tốt
  + Index chạy chuẩn, đánh nhiều index vẫn ok
  + MongoDriver hỗ trợ tốt
  + Có triển khai ACID nếu cần
  + Lưu trữ được nhiều dữ liệu rất lớn. 1 tỷ record đánh index chuẩn chạy vẫn nhanh (Phải đánh index)
  + Hỗ trợ 1 số tính năng mà NoSQL không có như: Inc, Dsc, TTL Index
  + Khả năng chịu tải tốt hơn nhiều so với DB quan hệ
+ Bất lợi
  + Làm report không tốt vì mongo dùng aggregate để join, khá phức tạp và chậm. Giải quyết bằng cách tiền xử lý dữ liệu
  + Setup connection pool để chịu lỗi

- Cache: Redis
+ Các công nghệ tương đường: Memcached
+ Lý do:
  + Khả năng chịu tải cực tốt
  + Có nhiều tính năng rất mạnh: Lock, inc, desc, pub sub, triển khai ratelimit
  + Quản lý, thao tác cache được
  + Tốc độ đọc ghi rất nhanh vì lưu trên ram
  + Các instance truy cập vào redis cache dễ dàng
  + ImmemoryCache chỉ lưu trong ram của instance, mất dữ liệu
+ Bất lợi
  + Cần cơ chế validate cache chính xác
  + Cần cơ chế sync data giữa cache và datasource để không sai lệch dữ liệu
  + Không cache giá trị quá lớn (< 5mb)
  + Chỉ cache các giá trị thường xuyên lặp đi lặp lại để hạn chế query xuống DB


