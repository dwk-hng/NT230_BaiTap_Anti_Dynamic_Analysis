# NT230_BaiTap_Anti_Dynamic_Analysis

**Lớp:** NT230.Q21.ANTT

**Nhóm:** G04

**Danh sách thành viên:**
| Họ và tên | MSSV |
| --- | --- |
| Nguyễn Đức Hùng | 23520565 |
| Nguyễn Lê Hưng | 23520567 |
| Nguyễn Viết Khang | 23520700 |
| Nguyễn Việt Dũng | 23520337 |

## Hướng dẫn chạy demo

Clone code của nhóm về bằng câu lệnh:
```
git clone https://github.com/dwk-hng/NT230_BaiTap_Anti_Dynamic_Analysis.git
```

Tiếp theo, build project này và đi tới thư mục `\NT230_BaiTap_Anti_Dynamic_Analysis\BaiTap_G04\bin\Debug` để lấy file `.exe`.

### Chạy trên môi trường sạch (không phải môi trường phân tích)
Chạy file `.exe` trên máy tính bình thường (không phải môi trường phân tích) sẽ hiện một MessageBox với thông báo "NT230.Q21.ANTT | G04 Group | Payload Executed".

### Chạy trên môi trường debug
- Sử dụng x32dbg để kiểm tra. 
- Chọn lần lượt File -> Open rồi load file `.exe` này vào môi trường debug.
- Sau đó chọn Run liên tiếp. Khi đó sẽ không hiện MessageBox mà chỉ thấy thông báo debugging stopped. 
- Ở tab Log sẽ thấy các dòng như sau:
```
Thread 12904 exit
Thread 24556 exit
Thread 14884 exit
Thread 16016 exit
Thread 9444 exit
Process stopped with exit code 0x0 (0)
Saving database to C:\Users\Lenovo\Downloads\File download\snapshot_2026-04-20_19-04\release\x32\db\BaiTap_G04.exe.dd32 0ms
Debugging stopped!
```

Từ đó có thể thấy trên môi trường debug, chương trình sẽ không hiện MessageBox mà sẽ âm thầm thoát để tránh bị phát hiện.

### Chạy trên môi trường VM
_Chưa cập nhật mô tả._

### Chạy trên môi trường sandbox

Sau khi build mã nguồn thành file `.exe`, thực hiện 2 kịch bản sau trên VMWare:

### Kịch bản 1: Không có tương tác

1. Chạy file `.exe` và buông tay khỏi chuột.
2. Đợi khoảng 2 đến 3 giây (hàm sleep được đặt là 2 giây).

**Kết quả:**  
Chương trình sẽ kết thúc trong im lặng để che giấu hành vi.  
(Nếu dùng Procmon để xem, sẽ thấy sự kiện `Thread Exit` với trạng thái `SUCCESS`).

### Kịch bản 2: Có tương tác

1. Chạy file `.exe` và liên tục rê chuột trên màn hình.

**Kết quả:**  
Sau 2 giây, mã độc nhận diện có thao tác người dùng và tiến hành bung payload  
(hiển thị hộp thoại `MessageBox` với nội dung `"Payload Executed"`).
