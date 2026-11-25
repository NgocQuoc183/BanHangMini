# Quy trình nghiệp vụ - BanHangMini

## 1. Mục đích và phạm vi

Tài liệu mô tả nghiệp vụ hiện được triển khai trong API BanHangMini, làm căn cứ để tiếp tục phát triển và kiểm thử. Mô tả được tổng hợp từ controller, service, entity và cấu hình Entity Framework Core; khi đặc tả này khác với mã nguồn, cần xác nhận yêu cầu trước khi thay đổi hành vi.

Phạm vi hiện tại:

- Đăng ký, đăng nhập và phân quyền khách hàng/quản trị.
- Tra cứu, quản lý danh mục và sản phẩm.
- Tạo, tra cứu, hủy và xác nhận đơn hàng; quản lý tồn kho liên quan.
- Các quy tắc dữ liệu, lỗi và giới hạn cần giữ khi mở rộng.

Thanh toán, giao hàng, giỏ hàng, đổi trả, khuyến mãi và báo cáo doanh số chưa có quy trình API hoàn chỉnh trong mã nguồn hiện tại.

## 2. Vai trò

| Vai trò | Quyền nghiệp vụ hiện có |
|---|---|
| Khách hàng (`KhachHang`) | Đăng ký/đăng nhập; xem danh mục và sản phẩm; tạo đơn; xem/hủy đơn của chính mình. |
| Quản trị (`QuanTri`) | Các quyền khách hàng có; tạo/sửa/xóa danh mục và sản phẩm; xem đơn của mọi người; xác nhận đơn; hủy đơn đang chờ xử lý. |
| Hệ thống | Phát hành JWT, kiểm tra dữ liệu, ghi nhận đơn và cập nhật tồn kho; ghi nhật ký tạo/cập nhật ở các entity có kiểm toán. |

Đăng ký công khai luôn tạo tài khoản vai trò `KhachHang`. Việc tạo tài khoản quản trị là thao tác vận hành riêng, không phải quy trình đăng ký thông thường.

## 3. Quy trình nghiệp vụ

### 3.1. Tạo tài khoản và đăng nhập

1. Người dùng gửi tên đăng nhập và mật khẩu đến `POST /api/auth/dang-ky`.
2. Hệ thống kiểm tra dữ liệu:
   - Tên đăng nhập bắt buộc, dài 3–50 ký tự, chỉ gồm chữ cái không dấu, chữ số, `.`, `_`, `-`.
   - Mật khẩu bắt buộc, dài 8–128 ký tự.
   - Tên đăng nhập phải duy nhất.
3. Hệ thống băm mật khẩu, gán vai trò `KhachHang`, lưu người dùng và trả kết quả tạo mới.
4. Người dùng gửi thông tin đến `POST /api/auth/dang-nhap`.
5. Nếu tài khoản và mật khẩu hợp lệ, hệ thống phát hành JWT có mã người dùng, tên đăng nhập, vai trò và thời hạn hết hạn. Nếu không hợp lệ, trả `401`.
6. Các API cần xác thực nhận token qua `Authorization: Bearer <token>`. API đơn hàng yêu cầu đăng nhập; API quản trị yêu cầu vai trò `QuanTri`.

### 3.2. Tra cứu danh mục và sản phẩm
   
1. Người dùng xem danh mục qua `GET /api/danh-muc`.
2. Người dùng xem sản phẩm qua `GET /api/san-pham`, có thể lọc theo danh mục, từ khóa tên và khoảng giá; kết quả được sắp xếp theo tên rồi mã sản phẩm và phân trang.
3. Mặc định phân trang sản phẩm là trang 1, 20 dòng; mỗi trang tối đa 200 dòng. Từ khóa tối đa 100 ký tự; giá không âm.
4. Người dùng xem một sản phẩm qua `GET /api/san-pham/{id}`. Sản phẩm đã xóa mềm không xuất hiện trong tra cứu thông thường.
5. Nếu ID không tồn tại hoặc sản phẩm đã bị xóa mềm, hệ thống trả lỗi không tìm thấy.

### 3.3. Quản trị danh mục và sản phẩm

Các thao tác ghi dữ liệu trong quy trình này chỉ dành cho quản trị và phải có JWT đúng vai trò.

**Danh mục**

- Tạo danh mục: tên bắt buộc, tối đa 100 ký tự; tên danh mục có ràng buộc duy nhất trong cơ sở dữ liệu.
- Sửa danh mục: cập nhật tên theo ID.
- Xóa danh mục: chỉ được xóa khi không còn sản phẩm liên kết. Kiểm tra hiện tại tính cả sản phẩm đã xóa mềm; nếu còn liên kết, hệ thống từ chối.

**Sản phẩm**

- Tạo sản phẩm: cần danh mục tồn tại; tên bắt buộc, tối đa 200 ký tự; đơn giá và tồn kho không âm; ghi chú tối đa 500 ký tự.
- Sửa sản phẩm: cập nhật tên, giá, tồn kho và ghi chú; yêu cầu gửi kèm phiên bản hiện tại (`PhienBan`) để phát hiện cập nhật đồng thời.
- Nếu phiên bản đã thay đổi do người khác cập nhật trước, hệ thống trả xung đột và yêu cầu tải lại dữ liệu rồi thử lại.
- Xóa sản phẩm là xóa mềm (`DaXoa = true`), không xóa vật lý khỏi cơ sở dữ liệu.
- Sản phẩm gắn với một danh mục; xóa vật lý bị ràng buộc bởi quan hệ dữ liệu.

### 3.4. Tạo đơn hàng và trừ tồn kho

1. Khách hàng đăng nhập và gửi `POST /api/don-hang`, kèm:
   - `Idempotency-Key` trong header (bắt buộc; cơ sở dữ liệu giới hạn tối đa 64 ký tự).
   - Ít nhất một dòng hàng, tối đa 50 dòng.
   - Mỗi dòng có mã sản phẩm hợp lệ và số lượng từ 1 đến 1.000.
2. Hệ thống kiểm tra khóa chống gửi lặp theo khách hàng. Nếu khách đó đã tạo đơn bằng khóa này, hệ thống trả lại đơn đã tạo, không tạo đơn mới.
3. Hệ thống từ chối nếu một sản phẩm xuất hiện lặp trong danh sách, hoặc có mã sản phẩm không tồn tại/đã bị xóa mềm.
4. Với mỗi dòng, hệ thống kiểm tra tồn kho đủ đáp ứng. Nếu thiếu, toàn bộ yêu cầu tạo đơn thất bại.
5. Giá bán được lấy từ giá sản phẩm tại thời điểm đặt; hệ thống lưu đơn giá, số lượng và thành tiền ở chi tiết đơn. Tổng tiền là tổng thành tiền các dòng.
6. Hệ thống tạo mã đơn, gán chủ sở hữu là người đang đăng nhập, đặt trạng thái `ChoXuLy`, giảm tồn kho và lưu đơn cùng chi tiết.
7. Nếu tồn kho bị cập nhật đồng thời, hệ thống báo xung đột để khách thử lại. Nếu hai yêu cầu dùng cùng khóa chống lặp đồng thời, hệ thống trả đơn đã tồn tại khi tìm thấy đơn đó.

Việc lưu đơn, chi tiết và tồn kho được thực hiện trong cùng thao tác lưu của EF Core; không được tách thành các bước lưu độc lập nếu chưa có đảm bảo giao dịch tương đương.

### 3.5. Tra cứu đơn hàng

- `GET /api/don-hang`: khách hàng chỉ xem đơn của mình; quản trị xem được đơn của tất cả người dùng. Kết quả phân trang, sắp xếp mới nhất trước.
- `GET /api/don-hang/{id}`: khách hàng chỉ xem đơn của mình; quản trị có thể xem mọi đơn.
- Nếu đơn không tồn tại hoặc thuộc về người dùng khác, phản hồi là không tìm thấy; API không tiết lộ đơn của người khác.
- Chi tiết trả về trạng thái, tổng tiền và các dòng hàng gồm sản phẩm, số lượng, đơn giá, thành tiền.

### 3.6. Hủy và xác nhận đơn

Trạng thái hiện có:

| Trạng thái | Ý nghĩa |
|---|---|
| `ChoXuLy` | Đơn mới tạo, còn có thể hủy hoặc được quản trị xác nhận. |
| `DaXacNhan` | Quản trị đã xác nhận; không thể hủy qua quy trình hiện tại. |
| `DaHuy` | Đơn đã hủy; không thể xác nhận hoặc hủy lần nữa. |

Quy tắc chuyển trạng thái:

```text
Tạo đơn ──> ChoXuLy ──(khách sở hữu hoặc quản trị hủy)──> DaHuy
                       └──(quản trị xác nhận)──────────> DaXacNhan
```

- Hủy qua `POST /api/don-hang/{id}/huy` chỉ hợp lệ khi đơn đang `ChoXuLy` và người gọi là chủ đơn hoặc quản trị.
- Khi hủy thành công, hệ thống cộng trả tồn kho theo số lượng từng dòng và chuyển trạng thái sang `DaHuy`.
- Xác nhận qua `POST /api/don-hang/{id}/xac-nhan` chỉ dành cho quản trị và chỉ hợp lệ khi đơn đang `ChoXuLy`.
- Không có API chuyển đơn đã xác nhận sang trạng thái giao hàng/hoàn tất trong phiên bản hiện tại.

## 4. Quy ước phản hồi và xử lý lỗi

- Phản hồi thành công thường theo cấu trúc `ApiResponse<T>`; danh sách lớn sử dụng thêm thông tin phân trang.
- Dữ liệu gửi lên sai validation trả `400`.
- Nghiệp vụ không hợp lệ (ví dụ thiếu tồn kho, trạng thái không cho phép hủy) trả `400`.
- Không tìm thấy tài nguyên trả `404`.
- Xung đột dữ liệu hoặc khóa chống lặp trả `409`.
- Thiếu/sai token trả `401`; không đủ vai trò trả `403`.
- Lỗi không dự kiến trả `500` với thông báo chung; chi tiết được ghi log phía máy chủ.
- Thời gian được xử lý và serialize theo UTC.

## 5. Dữ liệu nghiệp vụ chính

| Đối tượng | Vai trò trong quy trình |
|---|---|
| `NguoiDung` | Tài khoản, mật khẩu đã băm, vai trò. Tên đăng nhập duy nhất. |
| `DanhMuc` | Phân loại sản phẩm; tên duy nhất; không xóa được khi còn sản phẩm liên kết. |
| `SanPham` | Thông tin bán hàng, giá, tồn kho, danh mục; có xóa mềm và phiên bản chống cập nhật đồng thời. |
| `DonHang` | Mã đơn, chủ sở hữu, trạng thái, tổng tiền, khóa chống lặp và thông tin kiểm toán. |
| `ChiTietDonHang` | Sản phẩm, số lượng, đơn giá và thành tiền được chốt khi tạo đơn. |
| `HopThuDi` | Có entity/cấu hình dữ liệu cho gửi thông điệp, nhưng chưa thấy luồng nghiệp vụ/API gửi tương ứng. |

## 6. API theo luồng

| API | Đăng nhập/phân quyền | Mục đích |
|---|---|---|
| `POST /api/auth/dang-ky` | Công khai | Tạo tài khoản khách hàng |
| `POST /api/auth/dang-nhap` | Công khai | Đăng nhập và nhận JWT |
| `GET /api/danh-muc` | Công khai | Liệt kê danh mục |
| `POST /api/danh-muc` | Quản trị | Tạo danh mục |
| `PUT /api/danh-muc/{id}` | Quản trị | Sửa danh mục |
| `DELETE /api/danh-muc/{id}` | Quản trị | Xóa danh mục nếu không còn sản phẩm |
| `GET /api/san-pham` | Công khai | Lọc và phân trang sản phẩm |
| `GET /api/san-pham/{id}` | Công khai | Xem chi tiết sản phẩm |
| `POST /api/san-pham` | Quản trị | Tạo sản phẩm |
| `PUT /api/san-pham/{id}` | Quản trị | Sửa sản phẩm có kiểm tra phiên bản |
| `DELETE /api/san-pham/{id}` | Quản trị | Xóa mềm sản phẩm |
| `GET /api/don-hang` | Đăng nhập | Danh sách đơn theo phạm vi quyền |
| `GET /api/don-hang/{id}` | Đăng nhập | Chi tiết đơn theo phạm vi quyền |
| `POST /api/don-hang` | Đăng nhập | Tạo đơn; bắt buộc `Idempotency-Key` |
| `POST /api/don-hang/{id}/huy` | Đăng nhập | Chủ đơn hoặc quản trị hủy đơn chờ xử lý |
| `POST /api/don-hang/{id}/xac-nhan` | Quản trị | Xác nhận đơn chờ xử lý |
| `GET /api/he-thong/ping` | Công khai | Kiểm tra API đang hoạt động |

## 7. Hạng mục cần xác nhận trước khi mở rộng

Các mục dưới đây là giới hạn quan sát được từ mã nguồn, không phải quy trình đã cam kết:

- DTO `BaoCaoDoanhSoRequest` tồn tại nhưng chưa thấy endpoint/service báo cáo doanh số.
- Entity `HopThuDi` tồn tại nhưng chưa thấy quy trình gửi thông báo, retry hoặc API quản trị hàng đợi.
- Chưa có quy trình thanh toán, vận chuyển, hoàn tiền, đổi trả hoặc trạng thái hoàn tất đơn.
- Chưa có lịch sử nhập kho/điều chỉnh kho; hiện tại tồn kho thay đổi khi tạo và hủy đơn, cũng có thể được quản trị sửa qua cập nhật sản phẩm.
- Khóa idempotency được tìm theo người dùng và khóa, nhưng chỉ mục duy nhất ở cơ sở dữ liệu hiện đặt trên khóa không-null toàn bảng. Nếu yêu cầu cùng một khóa được dùng độc lập bởi nhiều người dùng, cần sửa chỉ mục thành khóa ghép `(NguoiDungId, KhoaChongTrung)` và kiểm thử đồng thời.
- Trước khi phát triển luồng đơn mới, cần xác định rõ quyền thao tác, trạng thái hợp lệ, tác động tồn kho, khả năng thử lại và quy tắc giữ dữ liệu lịch sử.

## 8. Danh sách kiểm thử hồi quy gợi ý

1. Đăng ký hợp lệ, tên đăng nhập trùng, dữ liệu sai định dạng và độ dài mật khẩu.
2. Đăng nhập đúng/sai; token hết hạn; khách hàng gọi API quản trị.
3. Lọc sản phẩm theo từng điều kiện, phân trang, sản phẩm xóa mềm và ID không tồn tại.
4. Tạo/sửa/xóa danh mục; không xóa danh mục còn sản phẩm, kể cả sản phẩm đã xóa mềm.
5. Tạo đơn thành công: đúng tổng tiền, lưu giá tại thời điểm mua và trừ tồn kho đúng số lượng.
6. Đơn có dòng rỗng, quá số dòng, sản phẩm trùng, sản phẩm không tồn tại, số lượng sai hoặc thiếu tồn kho.
7. Gửi lại cùng khóa idempotency trả cùng đơn; kiểm tra hai yêu cầu đồng thời không tạo đơn/trừ kho hai lần.
8. Khách hàng không đọc/hủy được đơn người khác; quản trị xem/hủy được đơn theo quyền.
9. Hủy đơn chờ xử lý trả đủ tồn kho; không cho hủy hoặc xác nhận lại đơn đã chuyển trạng thái.
10. Sửa sản phẩm với phiên bản cũ trả xung đột và không ghi đè thay đổi mới.

## 9. Mã nguồn tham chiếu

- [Controllers](../src/BanHangMini.Api/Controllers/)
- [Services](../src/BanHangMini.Api/Services/)
- [Entities](../src/BanHangMini.Api/Entities/)
- [DTOs](../src/BanHangMini.Api/DTOs/)
- [EF Core configurations](../src/BanHangMini.Api/Data/Configurations/)
- [Program.cs](../src/BanHangMini.Api/Program.cs)
