RESTful API Design — Coding Learning Platform
Conventions
Base URL: https://api.domain.com/v1
Auth: Authorization: Bearer <access_token> (JWT)
Response envelope:
json
{ "success": true, "data": {}, "message": "" }
{ "success": false, "error": { "code": "ERR_CODE", "message": "" } }
Roles: Admin | Student (Instructor = Admin trong scope này)
[A] = Admin only · [S] = Student only · [*] = cả hai · [-] = public

1. AUTH ( đã xong toàn bộ )
   Method
   Endpoint
   Access
   Mô tả
   POST
   /auth/register
   [-]
   Đăng ký tài khoản
   POST
   /auth/login
   [-]
   Đăng nhập → trả về access_token + refresh_token
   POST
   /auth/logout
   [*]
   Đăng xuất (revoke refresh_token)
   POST
   /auth/refresh
   [-]
   Lấy access_token mới từ refresh_token
   POST
   /auth/forgot-password
   [-]
   Gửi email reset password
   POST
   /auth/reset-password
   [-]
   Đặt lại password bằng token từ email
   GET
   /auth/me
   [*]
   Lấy thông tin tài khoản hiện tại
   PATCH
   /auth/me
   [*]
   Cập nhật profile (name, phone)
   PATCH
   /auth/me/password
   [*]
   Đổi password

Flow: Login
POST /auth/login
→ validate credentials
→ trả về { access_token, refresh_token, expires_in }
→ frontend lưu vào memory (access) + httpOnly cookie (refresh)

POST /auth/refresh (gọi tự động khi access_token hết hạn)
→ trả về access_token mới

2. SUBSCRIPTION & PAYMENT
   Method
   Endpoint
   Access
   Mô tả
   GET
   /subscription-packages
   [-]
   Danh sách gói (public — hiển thị trang pricing) (
   GET
   /subscription-packages/:id
   [-]
   Chi tiết 1 gói
   POST

/subscription-packages
[A]
Tạo gói mới (

1. trường discount_percent phải được tự động tính dựa trên gói 1 tháng, chứ không được nhập, chỉ được nhập price và hệ thống sẽ dựa vào price hiện tại so với gói 1 tháng và coi thử nó giảm bao nhiêu,
2. price =0 vẫn được tạo gói là sai

PATCH
/subscription-packages/:id
[A]
Cập nhật gói (price =0 vẫn đc cập nhật)
DELETE
/subscription-packages/:id
[A]
Vô hiệu hóa gói
GET
/subscriptions/me
[S]
Subscription hiện tại của student (
POST
/subscriptions
[S]
Đăng ký gói → tạo UserSubscription + khởi tạo Payment
POST
/subscriptions/:id/renew
[S]
Gia hạn subscription
POST
/subscriptions/:id/cancel
[S]
Hủy subscription
GET
/subscriptions
[A]
Danh sách tất cả subscription (admin)
POST
/payments/callback/vnpay
[-]
Webhook callback từ VNPay

GET
/payments/me
[S]
Lịch sử thanh toán của student
GET
/payments
[A]
Tất cả lịch sử thanh toán

Flow: Đăng ký gói

1. GET /subscription-packages → student chọn gói
2. POST /subscriptions { package_id } → tạo UserSubscription (Pending)
   → tạo Payment (Pending)
   → trả về { payment_url }
3. Student redirect đến payment_url (VNPay)
4. POST /payments/callback/vnpay → VNPay gọi callback
   → cập nhật Payment → Success
   → cập nhật UserSubscription → Active
   → mở khóa tất cả khóa học cho student

5. COURSE & CONTENT
   3.1 Courses (DONE)
   Method
   Endpoint
   Access
   Mô tả
   GET
   /courses
   [-]
   Danh sách khóa học (filter: category, difficulty, search) (DONE)
   GET
   /courses/:courseId
   [-]
   Chi tiết khóa học + danh sách section (không có content) (DONE)
   POST
   /courses
   [A]
   Tạo khóa học (DONE)
   PATCH
   /courses/:courseId
   [A]
   Cập nhật khóa học (DONE)
   DELETE
   /courses/:courseId
   [A]
   Vô hiệu hóa khóa học (DONE)
   GET
   /categories
   [-]
   Danh sách category (DONE)
   POST
   /categories
   [A]
   Tạo category (DONE)
   PATCH
   /categories/:id
   [A]
   Cập nhật category (DONE)

3.2 Sections
Method
Endpoint
Access
Mô tả
GET
/courses/:courseId/sections
[-]
Danh sách section của khóa học (DONE)
POST
/courses/:courseId/sections
[A]
Tạo section (DONE)
PATCH
/courses/:courseId/sections/:sectionId
[A]
Cập nhật section (DONE)
DELETE
/courses/:courseId/sections/:sectionId
[A]
Xóa section (DONE)
PATCH
/courses/:courseId/sections/reorder
[A]
Sắp xếp lại thứ tự section (DONE)

3.3 Lessons
Method
Endpoint
Access
Mô tả
GET
/sections/:sectionId/lessons
[*]
Danh sách lesson trong section (DONE)
GET
/lessons/:lessonId
[*]
Chi tiết lesson (check quyền truy cập)  
POST
/sections/:sectionId/lessons
[A]
Tạo lesson (DONE)
PATCH
/lessons/:lessonId
[A]
Cập nhật lesson (DONE)
DELETE
/lessons/:lessonId
[A]
Xóa lesson (DONE)
PATCH
/sections/:sectionId/lessons/reorder
[A]
Sắp xếp lại thứ tự lesson (DONE)

3.4 Exercises
Method
Endpoint
Access
Mô tả
GET
/lessons/:lessonId/exercises
[*]
Danh sách exercise trong lesson (DONE) a
GET
/exercises/:exerciseId
[*]
Chi tiết exercise (check quyền) (DONE)
POST
/lessons/:lessonId/exercises
[A]
Tạo exercise (DONE)
PATCH
/exercises/:exerciseId
[A]
Cập nhật exercise (DONE)
DELETE
/exercises/:exerciseId
[A]
Xóa exercise (DONE)
POST
/exercises/:exerciseId/media
[A]
Upload media (ảnh/video) cho Reading exercise
DELETE
/exercises/:exerciseId/media/:mediaId
[A]
Xóa media
POST
/exercises/:exerciseId/testcases
[A]
Thêm testcase (GradedCode) (DONE)
PATCH
/exercises/:exerciseId/testcases/:testcaseId
[A]
Cập nhật testcase (DONE)
DELETE
/exercises/:exerciseId/testcases/:testcaseId
[A]
Xóa testcase (DONE)

Method
Endpoint
Access
Mô tả
GET
/lessons/:lessonId/quizzes
[*]
Danh sách quiz trong lesson (DONE)
POST
/lessons/:lessonId/quizzes
[A]
Tạo quiz (1 câu hỏi) (DONE)
PATCH
/quizzes/:quizId
[A]
Cập nhật quiz (DONE)
DELETE
/quizzes/:quizId
[A]
Xóa quiz (DONE)
POST
/quizzes/:quizId/options
[A]
Thêm option cho quiz (DONE)
PATCH
/quizzes/:quizId/options/:optionId
[A]
Cập nhật option (DONE)
DELETE
/quizzes/:quizId/options/:optionId
[A]
Xóa option (DONE)

3.5 Quizzes (trong lesson) 4. STUDENT LEARNING FLOW
4.1 Enrollment(done)
Method
Endpoint
Access
Mô tả
GET
/enrollments/me
[S]
Danh sách khóa đang học
POST
/enrollments
[S]
Enroll vào khóa học (check subscription)
GET
/enrollments/me/:courseId
[S]
Chi tiết enrollment + progress tổng
GET
/enrollments
[A]
Tất cả enrollment (admin)

Flow: Truy cập nội dung bài học
GET /lessons/:lessonId
→ check Enrollments (student đã enroll chưa?)
→ check Lesson.is_free_preview (nếu true → cho qua)
→ check UserSubscriptions (có subscription Active không?)
→ check LessonProgress (bài này có bị lock không?)
→ trả về nội dung lesson + danh sách exercises
4.2 Exercise — Chạy code(done, mock result do chưa gắn code execute service)
Method
Endpoint
Access
Mô tả
POST
/exercises/:exerciseId/run
[S]
Chạy code (FreeCode — không chấm)
POST
/exercises/:exerciseId/submit
[S]
Nộp code (GradedCode — chấm testcases)
PATCH
/exercises/:exerciseId/progress
[S]
Auto-save last_code + đánh dấu completed

Flow: FreeCode Exercise

1. Student viết code
2. POST /exercises/:id/run { code, language }
   → gửi đến code execution service
   → trả về { output, runtime_ms }
   → student thấy output, tự đánh giá
3. Student bấm "Next"
4. PATCH /exercises/:id/progress { is_completed: true, last_code }
   → cập nhật ExerciseProgress
   → kiểm tra nếu tất cả exercise trong lesson completed
   → tự động cập nhật LessonProgress = Completed
   Flow: GradedCode Exercise
5. Student viết code
6. POST /exercises/:id/submit { code, language }
   → chạy code qua tất cả TestCases
   → trả về { is_passed, results: [{ testcase_id, is_passed, actual_output (nếu không hidden) }] }
7. Nếu is_passed = true
   → PATCH /exercises/:id/progress { is_completed: true, is_passed: true, last_code }
   → kiểm tra nếu tất cả exercise completed → LessonProgress = Completed
8. Nếu is_passed = false → student sửa và submit lại
   Flow: Reading Exercise
9. Student đọc nội dung + xem ảnh/video
10. Bấm "Next"
11. PATCH /exercises/:id/progress { is_completed: true }
    → cập nhật ExerciseProgress
    → kiểm tra → LessonProgress

12. QUIZ & SECTION QUIZ
    5.1 Quiz trong Lesson (done chưa test)
    Method
    Endpoint
    Access
    Mô tả
    POST
    /quizzes/:quizId/answer
    [S]
    Student trả lời 1 quiz → trả về is_correct + explanation

5.2 Section Quiz (thi tổng kết)(done chưa test)
Method
Endpoint
Access
Mô tả
GET
/sections/:sectionId/section-quiz
[S]
Lấy toàn bộ quiz của section (check unlock)
POST
/sections/:sectionId/section-quiz/attempt
[S]
Nộp bài thi section quiz
GET
/sections/:sectionId/section-quiz/attempts/me
[S]
Lịch sử các lần thi của student

Flow: Section Quiz(done chưa test)

1. GET /sections/:sectionId/section-quiz
   → check tất cả LessonProgress trong section = Completed
   → nếu chưa đủ → 403 { error: "SECTION_NOT_COMPLETED" }
   → nếu đủ → trả về danh sách tất cả Quiz (gom từ các Lesson)

2. Student làm bài (toàn bộ trên frontend, không gọi API từng câu)

3. POST /sections/:sectionId/section-quiz/attempt
   { answers: [{ quiz_id, option_id }] }
   → chấm điểm
   → lưu SectionQuizAttempts + SectionQuizAnswers
   → trả về { score, is_passed, answers: [{ quiz_id, is_correct, explanation }] }
   → kiểm tra CourseCompletionRules → nếu đủ điều kiện → trigger cấp Certification

4. PROGRESS & COMPLETION(done chưa test)
   Method
   Endpoint
   Access
   Mô tả
   GET
   /courses/:courseId/progress/me
   [S]
   Tổng quan progress toàn khóa (% từng section, lesson)
   GET
   /lessons/:lessonId/progress/me
   [S]
   Progress chi tiết từng exercise trong lesson
   GET
   /courses/:courseId/progress
   [A]
   Progress của tất cả student trong khóa (admin)

Response mẫu GET /courses/:courseId/progress/me
json
{
"enrollment_status": "InProgress",
"progress_pct": 45.5,
"sections": [
{
"section_id": "...",
"title": "Chương 1",
"lessons_total": 5,
"lessons_completed": 5,
"section_quiz_unlocked": true,
"section_quiz_passed": true,
"section_quiz_score": 80
},
{
"section_id": "...",
"title": "Chương 2",
"lessons_total": 4,
"lessons_completed": 2,
"section_quiz_unlocked": false,
"section_quiz_passed": false
}
]
}

7. CERTIFICATION (DONE CHƯA TEST)
   Method
   Endpoint
   Access
   Mô tả
   GET
   /certifications/me
   [S]
   Danh sách chứng chỉ của student
   GET
   /certifications/:code
   [-]
   Xem / verify chứng chỉ bằng certificate_code (public)
   GET
   /certifications
   [A]
   Tất cả chứng chỉ đã cấp
   POST
   /courses/:courseId/certifications/check
   [S]
   Kiểm tra đủ điều kiện nhận chứng chỉ chưa

Flow: Cấp chứng chỉ
Trigger tự động sau POST /section-quiz/attempt (lần thi cuối)
→ kiểm tra CourseCompletionRules:
min_lesson_completion_pct → từ LessonProgress
min_exercise_pass_pct → từ ExerciseProgress
min_section_quiz_score → từ SectionQuizAttempts
require_all_section_quiz → tất cả section đã thi chưa
→ nếu đủ điều kiện:
→ tạo Certifications { certificate_code, certificate_url }
→ cập nhật Enrollments.status = Completed
→ trả về { certified: true, certificate_code }
→ nếu chưa đủ:
→ trả về { certified: false, missing: [...] }

8. LEADERBOARD & FEEDBACK
   Method
   Endpoint
   Access
   Mô tả
   GET
   /courses/:courseId/leaderboard
   [*]
   Bảng xếp hạng của khóa học (top 50)
   GET
   /courses/:courseId/feedbacks
   [-]
   Danh sách đánh giá của khóa học
   POST
   /courses/:courseId/feedbacks
   [S]
   Gửi đánh giá (chỉ student đã enroll)
   PATCH
   /courses/:courseId/feedbacks/me
   [S]
   Sửa đánh giá của mình
   DELETE
   /courses/:courseId/feedbacks/me
   [S]
   Xóa đánh giá của mình
   DELETE
   /courses/:courseId/feedbacks/:feedbackId
   [A]
   Admin xóa đánh giá vi phạm

9. ADMIN
   Method
   Endpoint
   Access
   Mô tả
   GET
   /admin/users
   [A]
   Danh sách users (filter, search, pagination)
   GET
   /admin/users/:id
   [A]
   Chi tiết user
   PATCH
   /admin/users/:id
   [A]
   Cập nhật user (role, is_active)
   GET
   /admin/dashboard
   [A]
   Thống kê tổng: users, enrollments, revenue
   GET
   /admin/dashboard/revenue
   [A]
   Doanh thu theo tháng
   GET
   /admin/dashboard/enrollments
   [A]
   Số lượng enrollment theo khóa học

Error Codes
Code
HTTP
Mô tả
UNAUTHORIZED
401
Chưa đăng nhập
FORBIDDEN
403
Không có quyền
NOT_FOUND
404
Không tìm thấy resource
SUBSCRIPTION_REQUIRED
403
Cần subscription để truy cập
SUBSCRIPTION_EXPIRED
403
Subscription đã hết hạn
LESSON_LOCKED
403
Bài học bị lock (subscription hết hạn giữa chừng)
SECTION_NOT_COMPLETED
403
Chưa hoàn thành tất cả lesson trong section
ALREADY_ENROLLED
409
Đã enroll khóa học này rồi
ALREADY_CERTIFIED
409
Đã có chứng chỉ rồi
VALIDATION_ERROR
422
Dữ liệu đầu vào không hợp lệ
