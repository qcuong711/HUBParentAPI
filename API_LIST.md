# Danh sách API dành cho phụ huynh

Base URL khi chạy local:

```text
http://localhost:5183
```

Swagger UI: `http://localhost:5183/swagger`

## Quy ước chung

- `studentCode` là mã số sinh viên, ví dụ `030837210252`.
- `yearStudy` và `termId` là tham số tùy chọn. Khi không lọc, bỏ hẳn query string thay vì truyền giá trị rỗng.
- Các API thời khóa biểu và lịch thi mặc định lấy học kỳ hiện tại theo `psc_Terms.BeginDate` và `EndDate`.
- Khi không có dữ liệu, API trả `200 OK` với danh sách rỗng `[]`, đối tượng tổng hợp rỗng hoặc `null` tùy endpoint.
- API chạy nội bộ trên server và không yêu cầu xác thực hay header `Authorization`.

## Danh sách endpoint

| Nhóm | Method | Endpoint | Mô tả |
|------|--------|----------|-------|
| Hệ thống | GET | `/api/health` | Kiểm tra API có đang hoạt động |
| Điểm trung bình | GET | `/api/averages/year/{studentCode}` | Điểm trung bình theo năm học |
| Điểm trung bình | GET | `/api/averages/terms/{studentCode}` | Điểm trung bình tích lũy theo học kỳ |
| Điểm trung bình | GET | `/api/averages/overall/{studentCode}` | Điểm trung bình tích lũy hiện tại |
| Tốt nghiệp | GET | `/api/averages/graduation/{studentCode}` | Tiến độ tín chỉ theo chương trình đào tạo |
| Điểm | GET | `/api/scores/detailed/{studentCode}` | Điểm tổng kết từng học phần |
| Điểm | GET | `/api/scores/components/{studentCode}` | Điểm thành phần; hỗ trợ lọc năm học/học kỳ |
| Sinh viên | GET | `/api/students/registrations/{studentCode}` | Học phần đăng ký và tổng tải tín chỉ |
| Sinh viên | GET | `/api/students/study-status/{studentCode}` | Trạng thái và lịch sử cảnh báo học vụ |
| Sinh viên | GET | `/api/students/advisor/{studentCode}` | Thông tin liên hệ cố vấn học tập |
| Lịch | GET | `/api/schedules/timetable/{studentCode}` | Thời khóa biểu; mặc định học kỳ hiện tại |
| Lịch | GET | `/api/schedules/exams/{studentCode}` | Lịch thi/đồ án; mặc định học kỳ hiện tại |
| Tài chính | GET | `/api/finance/tuition/terms/{studentCode}` | Tổng học phí, đã đóng, miễn giảm và còn nợ theo học kỳ |
| Tài chính | GET | `/api/finance/tuition/courses/{studentCode}` | Chi tiết học phí theo môn hoặc khoản thu gộp theo kỳ |
| Tài chính | GET | `/api/finance/other-fees/{studentCode}` | Các khoản phí khác và trạng thái thanh toán |
| Tài chính | GET | `/api/finance/payments/{studentCode}` | Lịch sử biên lai và số tiền đã thanh toán |
| Tài chính | GET | `/api/finance/debts/{studentCode}` | Danh sách các khoản học phí/lệ phí còn nợ |
| Tài chính | GET | `/api/finance/deadlines/{studentCode}` | Hạn đóng chung và thời gian gia hạn riêng |
| Schema | GET | `/api/schema/psc-tables` | Danh sách bảng PSC phục vụ kiểm tra schema |

Các endpoint có hỗ trợ lọc nhận query string:

```text
?yearStudy=2024-2025&termId=HK01
```

## Ví dụ 1: Kiểm tra API

```http
GET http://localhost:5183/api/health
```

Kết quả mong đợi: HTTP `200 OK`.

## Ví dụ 2: Lấy điểm thành phần của một học kỳ

```http
GET http://localhost:5183/api/scores/components/030837210252?yearStudy=2024-2025&termId=HK01
```

Response rút gọn:

```json
[
  {
    "studentID": "030837210252",
    "yearStudy": "2024-2025",
    "termID": "HK01",
    "curriculumName": "Tên học phần",
    "assignmentName": "Điểm thi kết thúc học phần",
    "firstMark": 8.5,
    "secondMark": null
  }
]
```

## Ví dụ 3: Xem tải học tập

```http
GET http://localhost:5183/api/students/registrations/030837210252?yearStudy=2024-2025&termId=HK01
```

Response rút gọn:

```json
{
  "studentID": "030837210252",
  "yearStudy": "2024-2025",
  "termID": "HK01",
  "totalCourses": 4,
  "totalCredits": 11,
  "items": [
    {
      "curriculumName": "Tên học phần",
      "credits": 3
    }
  ]
}
```

## Ví dụ 4: Xem tiến độ tốt nghiệp

```http
GET http://localhost:5183/api/averages/graduation/030837210252
```

Response rút gọn:

```json
[
  {
    "studyProgramName": "Tên chương trình đào tạo",
    "requiredCredits": 120,
    "completedCredits": 100,
    "remainingCredits": 20,
    "progressPercent": 83.33
  }
]
```

## Ví dụ 5: Thời khóa biểu và lịch thi lịch sử

Nếu gọi không có query string:

```http
GET http://localhost:5183/api/schedules/timetable/030837210252
GET http://localhost:5183/api/schedules/exams/030837210252
```

API lấy học kỳ hiện tại `2025-2026/HK02`. MSSV mẫu không có dữ liệu trong học kỳ này nên response là `[]`.

Để xem học kỳ gần nhất có dữ liệu:

```http
GET http://localhost:5183/api/schedules/timetable/030837210252?yearStudy=2024-2025&termId=HK01
GET http://localhost:5183/api/schedules/exams/030837210252?yearStudy=2024-2025&termId=HK01
```

Với dữ liệu local hiện tại:

- API thời khóa biểu trả 24 buổi học.
- API lịch thi/đồ án trả 4 bản ghi.
- Một số đồ án hoặc thực tập có thể chưa có ngày, giờ hoặc phòng nên các trường tương ứng trả `null`.


## Ví dụ 6: Tổng hợp học phí theo học kỳ

```http
GET http://localhost:5183/api/finance/tuition/terms/030837210252
```

Có thể lọc một học kỳ cụ thể:

```http
GET http://localhost:5183/api/finance/tuition/terms/030837210252?yearStudy=2024-2025&termId=HK01
```

Response rút gọn:

```json
[
  {
    "studentID": "030837210252",
    "yearStudy": "2024-2025",
    "termID": "HK01",
    "tuitionAmount": 5621000,
    "paidAmount": 5621000,
    "offsetAmount": 0,
    "discountAmount": 0,
    "remainingAmount": 0,
    "isFullyPaid": true,
    "paymentStatus": "PAID"
  }
]
```

## Ví dụ 7: Chi tiết học phí từng môn

```http
GET http://localhost:5183/api/finance/tuition/courses/030837210252?yearStudy=2024-2025&termId=HK01
```

- `isTermLevel=false`: khoản học phí xác định được môn/lớp học phần.
- `isTermLevel=true`: khoản thu gộp theo học kỳ, không thể phân bổ chính xác cho từng môn.
- `paymentStatus` có giá trị `PAID` hoặc `OUTSTANDING`.

## Ví dụ 8: Các khoản phí khác

```http
GET http://localhost:5183/api/finance/other-fees/030837210252
```

Kết quả gồm loại phí, số phải thu, đã đóng, miễn giảm, cấn trừ và còn nợ.

## Ví dụ 9: Lịch sử thanh toán

```http
GET http://localhost:5183/api/finance/payments/030837210252?yearStudy=2024-2025
```

API chỉ trả biên lai hợp lệ, tự loại biên lai đã hủy, đã rút hoặc thuộc nhóm nghiệp vụ không phải khoản thu thông thường.

## Ví dụ 10: Các khoản còn nợ

```http
GET http://localhost:5183/api/finance/debts/010121160003
```

- `isTuition=true`: học phí.
- `isTuition=false`: lệ phí hoặc khoản phí khác.
- Nếu sinh viên không còn nợ, API trả `[]`.

## Ví dụ 11: Hạn đóng và gia hạn

```http
GET http://localhost:5183/api/finance/deadlines/030126100558?yearStudy=2016-2017&termId=HK01
```

- `originalDeadline`: hạn đóng ban đầu.
- `extensionEndDate`: hạn gia hạn riêng của sinh viên, nếu có.
- `effectiveDeadline`: hạn cuối thực tế sau khi áp dụng gia hạn.
- `isOverdue`: đã quá hạn tại thời điểm gọi API hay chưa.

## Điều kiện kết nối dữ liệu tài chính

Các API tài chính dùng connection `DefaultConnection` hiện có và truy vấn chéo sang database `[AccountsFee]` trên cùng SQL Server. Không cần thêm hoặc đổi connection string production, nhưng tài khoản SQL của API phải có quyền `SELECT` trên cả `CoreUis` và `AccountsFee`.