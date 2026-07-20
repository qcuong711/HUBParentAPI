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

