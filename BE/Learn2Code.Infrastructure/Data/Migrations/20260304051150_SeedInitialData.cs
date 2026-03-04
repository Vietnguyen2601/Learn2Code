using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Learn2Code.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class SeedInitialData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // ════════════════════════════════════════════════════════════
            //  ROLES
            // ════════════════════════════════════════════════════════════
            migrationBuilder.Sql(@"
                INSERT INTO roles (role_id, role_name, is_active, created_at, updated_at)
                VALUES 
                    ('a1b2c3d4-0001-0000-0000-000000000001', 'Admin',      true, NOW(), NOW()),
                    ('a1b2c3d4-0001-0000-0000-000000000002', 'Instructor', true, NOW(), NOW()),
                    ('a1b2c3d4-0001-0000-0000-000000000003', 'Student',    true, NOW(), NOW())
                ON CONFLICT DO NOTHING;
            ");

            // ════════════════════════════════════════════════════════════
            //  ACCOUNTS  (BCrypt workFactor=11)
            //  Admin@123 / Instructor@123 / Student@123
            // ════════════════════════════════════════════════════════════
            migrationBuilder.Sql(@"
                INSERT INTO accounts (account_id, username, email, password, name, is_active, created_at, updated_at)
                VALUES 
                    ('b1000000-0000-0000-0000-000000000001',
                     'admin', 'admin@learn2code.com',
                     '$2a$11$gDgsl5OYuufvDKjssfALsusLsnDU3Xe8PzZXsNSQAOL8vdIBfN0Su',
                     'System Admin', true, NOW(), NOW()),

                    ('b1000000-0000-0000-0000-000000000002',
                     'instructor01', 'instructor01@learn2code.com',
                     '$2a$11$HPZbGk42LBM9.3opIqcqAuXem3jWejiAyyup2Bl6aZ/i1Bw/pJcAe',
                     'Nguyen Van Giang', true, NOW(), NOW()),

                    ('b1000000-0000-0000-0000-000000000003',
                     'student01', 'student01@learn2code.com',
                     '$2a$11$MrCQ/otdjM9qs3.hNthVWOD0QZ6Cu05UhNJdj6voWRbb4g.kdo4oy',
                     'Tran Thi Mai', true, NOW(), NOW())
                ON CONFLICT DO NOTHING;
            ");

            // ════════════════════════════════════════════════════════════
            //  ACCOUNT ROLES
            // ════════════════════════════════════════════════════════════
            migrationBuilder.Sql(@"
                INSERT INTO account_roles (account_id, role_id, assigned_at)
                VALUES 
                    ('b1000000-0000-0000-0000-000000000001', 'a1b2c3d4-0001-0000-0000-000000000001', NOW()),
                    ('b1000000-0000-0000-0000-000000000002', 'a1b2c3d4-0001-0000-0000-000000000002', NOW()),
                    ('b1000000-0000-0000-0000-000000000003', 'a1b2c3d4-0001-0000-0000-000000000003', NOW())
                ON CONFLICT DO NOTHING;
            ");

            // ════════════════════════════════════════════════════════════
            //  COURSE CATEGORIES
            // ════════════════════════════════════════════════════════════
            migrationBuilder.Sql(@"
                INSERT INTO course_categories (category_id, name, description, is_active, created_at, updated_at)
                VALUES 
                    ('c1000000-0000-0000-0000-000000000001', 'Web Development',       'Frontend, Backend, Full-stack web development', true, NOW(), NOW()),
                    ('c1000000-0000-0000-0000-000000000002', 'Mobile Development',    'iOS, Android, React Native, Flutter',           true, NOW(), NOW()),
                    ('c1000000-0000-0000-0000-000000000003', 'Data Science',          'Python, Machine Learning, AI',                  true, NOW(), NOW()),
                    ('c1000000-0000-0000-0000-000000000004', 'DevOps',                'Docker, Kubernetes, CI/CD',                     true, NOW(), NOW()),
                    ('c1000000-0000-0000-0000-000000000005', 'Programming Languages', 'C#, Java, Python, JavaScript fundamentals',     true, NOW(), NOW()),
                    ('c1000000-0000-0000-0000-000000000006', 'Cybersecurity',         'Network security, Ethical hacking, CTF',        true, NOW(), NOW()),
                    ('c1000000-0000-0000-0000-000000000007', 'Databases',             'SQL, NoSQL, PostgreSQL, MongoDB',               true, NOW(), NOW())
                ON CONFLICT DO NOTHING;
            ");

            // ════════════════════════════════════════════════════════════
            //  SUBSCRIPTION PACKAGES
            // ════════════════════════════════════════════════════════════
            migrationBuilder.Sql(@"
                INSERT INTO subscription_packages (package_id, name, duration_months, price, discount_percent, description, is_active, created_at, updated_at)
                VALUES 
                    ('d1000000-0000-0000-0000-000000000001', '1 Month',  1,  99000,  0,  'Gói 1 tháng — truy cập toàn bộ khoá học',       true, NOW(), NOW()),
                    ('d1000000-0000-0000-0000-000000000002', '6 Months', 6,  499000, 16, 'Gói 6 tháng — tiết kiệm 16% so với gói tháng',  true, NOW(), NOW()),
                    ('d1000000-0000-0000-0000-000000000003', '1 Year',   12, 899000, 25, 'Gói 1 năm — tiết kiệm 25%, phù hợp học dài hạn', true, NOW(), NOW())
                ON CONFLICT DO NOTHING;
            ");

            // ════════════════════════════════════════════════════════════
            //  COURSES
            // ════════════════════════════════════════════════════════════
            migrationBuilder.Sql(@"
                INSERT INTO courses (course_id, title, description, difficulty, is_active, instructor_id, category_id, created_at, updated_at)
                VALUES 
                    ('e1000000-0000-0000-0000-000000000001',
                     'Python Cơ Bản Cho Người Mới Bắt Đầu',
                     'Khoá học Python từ zero — biến, vòng lặp, hàm, OOP và bài tập thực hành có chấm điểm.',
                     'Beginner', true,
                     'b1000000-0000-0000-0000-000000000002',
                     'c1000000-0000-0000-0000-000000000005',
                     NOW(), NOW()),

                    ('e1000000-0000-0000-0000-000000000002',
                     'Web Development với HTML, CSS & JavaScript',
                     'Xây dựng giao diện web hiện đại từ HTML5, CSS3 đến JavaScript ES6+.',
                     'Beginner', true,
                     'b1000000-0000-0000-0000-000000000002',
                     'c1000000-0000-0000-0000-000000000001',
                     NOW(), NOW())
                ON CONFLICT DO NOTHING;
            ");

            // ════════════════════════════════════════════════════════════
            //  COURSE COMPLETION RULES
            // ════════════════════════════════════════════════════════════
            migrationBuilder.Sql(@"
                INSERT INTO course_completion_rules (rule_id, course_id, min_lesson_completion_pct, min_exercise_pass_pct, min_section_quiz_score, require_all_section_quiz, created_at, updated_at)
                VALUES 
                    ('f1000000-0000-0000-0000-000000000001', 'e1000000-0000-0000-0000-000000000001', 100, 70, 60, true, NOW(), NOW()),
                    ('f1000000-0000-0000-0000-000000000002', 'e1000000-0000-0000-0000-000000000002', 100, 60, 60, true, NOW(), NOW())
                ON CONFLICT DO NOTHING;
            ");

            // ════════════════════════════════════════════════════════════
            //  CERTIFICATE TEMPLATES
            // ════════════════════════════════════════════════════════════
            migrationBuilder.Sql(@"
                INSERT INTO certificate_templates (template_id, course_id, title, description, signature_name, created_at, updated_at)
                VALUES 
                    ('f2000000-0000-0000-0000-000000000001', 'e1000000-0000-0000-0000-000000000001',
                     'Chứng Chỉ Python Cơ Bản',
                     'Chứng nhận hoàn thành khoá học Python Cơ Bản tại Learn2Code.',
                     'Nguyen Van Giang', NOW(), NOW()),

                    ('f2000000-0000-0000-0000-000000000002', 'e1000000-0000-0000-0000-000000000002',
                     'Chứng Chỉ Web Development',
                     'Chứng nhận hoàn thành khoá học Web Development tại Learn2Code.',
                     'Nguyen Van Giang', NOW(), NOW())
                ON CONFLICT DO NOTHING;
            ");

            // ════════════════════════════════════════════════════════════
            //  SECTIONS  (Course 1: Python)
            // ════════════════════════════════════════════════════════════
            migrationBuilder.Sql(@"
                INSERT INTO sections (section_id, course_id, title, description, order_number, is_active, created_at, updated_at)
                VALUES 
                    ('a0100000-0000-0000-0000-000000000001', 'e1000000-0000-0000-0000-000000000001',
                     'Làm Quen Với Python', 'Cài đặt, cú pháp cơ bản, biến và kiểu dữ liệu.', 1, true, NOW(), NOW()),

                    ('a0100000-0000-0000-0000-000000000002', 'e1000000-0000-0000-0000-000000000001',
                     'Cấu Trúc Điều Khiển', 'if/else, vòng lặp for/while, break/continue.', 2, true, NOW(), NOW()),

                    ('a0100000-0000-0000-0000-000000000003', 'e1000000-0000-0000-0000-000000000002',
                     'HTML Cơ Bản', 'Cấu trúc trang web, thẻ HTML5 và semantic HTML.', 1, true, NOW(), NOW()),

                    ('a0100000-0000-0000-0000-000000000004', 'e1000000-0000-0000-0000-000000000002',
                     'CSS & Styling', 'Selectors, Box model, Flexbox, Grid.', 2, true, NOW(), NOW())
                ON CONFLICT DO NOTHING;
            ");

            // ════════════════════════════════════════════════════════════
            //  LESSONS
            // ════════════════════════════════════════════════════════════
            migrationBuilder.Sql(@"
                INSERT INTO lessons (lesson_id, section_id, title, order_number, is_free_preview, created_at, updated_at)
                VALUES 
                    ('a0200000-0000-0000-0000-000000000001', 'a0100000-0000-0000-0000-000000000001', 'Giới Thiệu Python & Cài Đặt',  1, true,  NOW(), NOW()),
                    ('a0200000-0000-0000-0000-000000000002', 'a0100000-0000-0000-0000-000000000001', 'Biến & Kiểu Dữ Liệu',          2, false, NOW(), NOW()),
                    ('a0200000-0000-0000-0000-000000000003', 'a0100000-0000-0000-0000-000000000001', 'Nhập/Xuất Dữ Liệu',            3, false, NOW(), NOW()),
                    ('a0200000-0000-0000-0000-000000000004', 'a0100000-0000-0000-0000-000000000002', 'Câu Lệnh if/else',              1, true,  NOW(), NOW()),
                    ('a0200000-0000-0000-0000-000000000005', 'a0100000-0000-0000-0000-000000000002', 'Vòng Lặp for & while',         2, false, NOW(), NOW()),
                    ('a0200000-0000-0000-0000-000000000006', 'a0100000-0000-0000-0000-000000000003', 'HTML là gì?',                   1, true,  NOW(), NOW()),
                    ('a0200000-0000-0000-0000-000000000007', 'a0100000-0000-0000-0000-000000000003', 'Các Thẻ HTML Thông Dụng',      2, false, NOW(), NOW()),
                    ('a0200000-0000-0000-0000-000000000008', 'a0100000-0000-0000-0000-000000000004', 'CSS Selectors & Properties',    1, false, NOW(), NOW()),
                    ('a0200000-0000-0000-0000-000000000009', 'a0100000-0000-0000-0000-000000000004', 'Flexbox Layout',               2, false, NOW(), NOW())
                ON CONFLICT DO NOTHING;
            ");

            // ════════════════════════════════════════════════════════════
            //  EXERCISES
            // ════════════════════════════════════════════════════════════

            // Exercise 1 — Reading (Lesson 1)
            migrationBuilder.Sql(@"
                INSERT INTO exercises (exercise_id, lesson_id, order_number, exercise_type, narrative, language, starter_code, solution_code, instruction, hint, created_at, updated_at)
                VALUES (
                    'a0300000-0000-0000-0000-000000000001',
                    'a0200000-0000-0000-0000-000000000001', 1, 'Reading',
                    $nar$## Python là gì?
Python là ngôn ngữ lập trình bậc cao, đa mục đích, dễ học và có cú pháp rõ ràng.
Được tạo bởi Guido van Rossum năm 1991, Python hiện là ngôn ngữ phổ biến nhất thế giới.

### Ứng dụng của Python
- Web Development: Django, FastAPI
- Data Science & AI: NumPy, Pandas, TensorFlow
- Automation: scripting, xử lý file$nar$,
                    NULL, NULL, NULL, NULL, NULL, NOW(), NOW()
                ) ON CONFLICT DO NOTHING;
            ");

            // Exercise 2 — FreeCode (Lesson 2)
            migrationBuilder.Sql(@"
                INSERT INTO exercises (exercise_id, lesson_id, order_number, exercise_type, narrative, language, starter_code, solution_code, instruction, hint, created_at, updated_at)
                VALUES (
                    'a0300000-0000-0000-0000-000000000002',
                    'a0200000-0000-0000-0000-000000000002', 1, 'FreeCode',
                    $nar$## Biến Trong Python
Biến là nơi lưu trữ giá trị. Python không cần khai báo kiểu dữ liệu trước.

Ví dụ:
    name = 'Alice'   -- string
    age  = 25        -- int
    gpa  = 3.8       -- float$nar$,
                    'python',
                    $code$name = 'Your Name'
age = 0
# In ra: Hello, [name]! You are [age] years old.
print()$code$,
                    $sol$name = 'Alice'
age = 20
print(f'Hello, {name}! You are {age} years old.')$sol$,
                    'Tạo biến name và age, sau đó in ra câu chào sử dụng f-string.',
                    'Dùng f''...'' để nhúng biến vào chuỗi.',
                    NOW(), NOW()
                ) ON CONFLICT DO NOTHING;
            ");

            // Exercise 3 — GradedCode (Lesson 2)
            migrationBuilder.Sql(@"
                INSERT INTO exercises (exercise_id, lesson_id, order_number, exercise_type, narrative, language, starter_code, solution_code, instruction, hint, created_at, updated_at)
                VALUES (
                    'a0300000-0000-0000-0000-000000000003',
                    'a0200000-0000-0000-0000-000000000002', 2, 'GradedCode',
                    $nar$## Bài Tập: Tính Tổng Hai Số
Viết chương trình nhận 2 số nguyên từ input và in ra tổng của chúng.$nar$,
                    'python',
                    $code$a = int(input())
b = int(input())
# Tính tổng và in ra$code$,
                    $sol$a = int(input())
b = int(input())
print(a + b)$sol$,
                    'Đọc 2 số nguyên từ input (mỗi số một dòng) và in ra tổng.',
                    'Dùng int(input()) để đọc số nguyên.',
                    NOW(), NOW()
                ) ON CONFLICT DO NOTHING;
            ");

            // Exercise 4 — GradedCode (Lesson 4)
            migrationBuilder.Sql(@"
                INSERT INTO exercises (exercise_id, lesson_id, order_number, exercise_type, narrative, language, starter_code, solution_code, instruction, hint, created_at, updated_at)
                VALUES (
                    'a0300000-0000-0000-0000-000000000004',
                    'a0200000-0000-0000-0000-000000000004', 1, 'GradedCode',
                    $nar$## Bài Tập: Số Chẵn Hay Lẻ
Viết chương trình kiểm tra một số nguyên là chẵn hay lẻ.
In ra Even nếu chẵn, Odd nếu lẻ.$nar$,
                    'python',
                    $code$n = int(input())
# Kiểm tra chẵn/lẻ$code$,
                    $sol$n = int(input())
if n % 2 == 0:
    print('Even')
else:
    print('Odd')$sol$,
                    'Đọc một số nguyên và in ra Even hoặc Odd.',
                    'Dùng toán tử % (modulo) để kiểm tra số dư.',
                    NOW(), NOW()
                ) ON CONFLICT DO NOTHING;
            ");

            // Exercise 5 — Reading (Lesson 6)
            migrationBuilder.Sql(@"
                INSERT INTO exercises (exercise_id, lesson_id, order_number, exercise_type, narrative, language, starter_code, solution_code, instruction, hint, created_at, updated_at)
                VALUES (
                    'a0300000-0000-0000-0000-000000000005',
                    'a0200000-0000-0000-0000-000000000006', 1, 'Reading',
                    $nar$## HTML là gì?
HTML (HyperText Markup Language) là ngôn ngữ đánh dấu dùng để tạo cấu trúc trang web.

Cấu trúc cơ bản:
    <!DOCTYPE html>
    <html>
      <head><title>Tiêu đề trang</title></head>
      <body>
        <h1>Xin chào!</h1>
        <p>Đây là đoạn văn.</p>
      </body>
    </html>$nar$,
                    NULL, NULL, NULL, NULL, NULL, NOW(), NOW()
                ) ON CONFLICT DO NOTHING;
            ");

            // ════════════════════════════════════════════════════════════
            //  TEST CASES  (chỉ cho GradedCode exercises)
            // ════════════════════════════════════════════════════════════
            migrationBuilder.Sql(@"
                INSERT INTO test_cases (testcase_id, exercise_id, expected_output, is_hidden, weight, created_at, updated_at)
                VALUES 
                    ('a0400000-0000-0000-0000-000000000001', 'a0300000-0000-0000-0000-000000000003', '3',   false, 1, NOW(), NOW()),
                    ('a0400000-0000-0000-0000-000000000002', 'a0300000-0000-0000-0000-000000000003', '0',   false, 1, NOW(), NOW()),
                    ('a0400000-0000-0000-0000-000000000003', 'a0300000-0000-0000-0000-000000000003', '-5',  true,  1, NOW(), NOW()),
                    ('a0400000-0000-0000-0000-000000000004', 'a0300000-0000-0000-0000-000000000003', '100', true,  1, NOW(), NOW()),
                    ('a0400000-0000-0000-0000-000000000005', 'a0300000-0000-0000-0000-000000000004', 'Even', false, 1, NOW(), NOW()),
                    ('a0400000-0000-0000-0000-000000000006', 'a0300000-0000-0000-0000-000000000004', 'Odd',  false, 1, NOW(), NOW()),
                    ('a0400000-0000-0000-0000-000000000007', 'a0300000-0000-0000-0000-000000000004', 'Even', true,  1, NOW(), NOW()),
                    ('a0400000-0000-0000-0000-000000000008', 'a0300000-0000-0000-0000-000000000004', 'Odd',  true,  1, NOW(), NOW())
                ON CONFLICT DO NOTHING;
            ");

            // ════════════════════════════════════════════════════════════
            //  QUIZZES
            // ════════════════════════════════════════════════════════════
            migrationBuilder.Sql(@"
                INSERT INTO quizzes (quiz_id, lesson_id, order_number, question, explanation, created_at, updated_at)
                VALUES 
                    ('a0500000-0000-0000-0000-000000000001', 'a0200000-0000-0000-0000-000000000001', 1,
                     'Python được tạo ra bởi ai?',
                     'Python được tạo bởi Guido van Rossum và phát hành lần đầu năm 1991.',
                     NOW(), NOW()),

                    ('a0500000-0000-0000-0000-000000000002', 'a0200000-0000-0000-0000-000000000001', 2,
                     'Python thuộc loại ngôn ngữ nào?',
                     'Python là ngôn ngữ thông dịch (interpreted), bậc cao (high-level) và đa mục đích.',
                     NOW(), NOW()),

                    ('a0500000-0000-0000-0000-000000000003', 'a0200000-0000-0000-0000-000000000002', 1,
                     'Trong Python, để khai báo biến số nguyên x = 5, ta viết?',
                     'Python không cần từ khoá khai báo kiểu. Chỉ cần viết trực tiếp x = 5.',
                     NOW(), NOW()),

                    ('a0500000-0000-0000-0000-000000000004', 'a0200000-0000-0000-0000-000000000004', 1,
                     'Toán tử nào dùng để kiểm tra số dư trong Python?',
                     'Toán tử % (modulo) trả về số dư của phép chia. Ví dụ: 7 % 2 = 1.',
                     NOW(), NOW()),

                    ('a0500000-0000-0000-0000-000000000005', 'a0200000-0000-0000-0000-000000000006', 1,
                     'HTML là viết tắt của?',
                     'HTML = HyperText Markup Language — ngôn ngữ đánh dấu siêu văn bản.',
                     NOW(), NOW())
                ON CONFLICT DO NOTHING;
            ");

            // ════════════════════════════════════════════════════════════
            //  QUIZ OPTIONS
            // ════════════════════════════════════════════════════════════
            migrationBuilder.Sql(@"
                INSERT INTO quiz_options (option_id, quiz_id, content, is_correct, created_at)
                VALUES 
                    ('a0600000-0000-0000-0000-000000000001', 'a0500000-0000-0000-0000-000000000001', 'Guido van Rossum',  true,  NOW()),
                    ('a0600000-0000-0000-0000-000000000002', 'a0500000-0000-0000-0000-000000000001', 'James Gosling',     false, NOW()),
                    ('a0600000-0000-0000-0000-000000000003', 'a0500000-0000-0000-0000-000000000001', 'Brendan Eich',      false, NOW()),
                    ('a0600000-0000-0000-0000-000000000004', 'a0500000-0000-0000-0000-000000000001', 'Dennis Ritchie',    false, NOW()),
                    ('a0600000-0000-0000-0000-000000000005', 'a0500000-0000-0000-0000-000000000002', 'Thông dịch (Interpreted)', true,  NOW()),
                    ('a0600000-0000-0000-0000-000000000006', 'a0500000-0000-0000-0000-000000000002', 'Biên dịch (Compiled)',     false, NOW()),
                    ('a0600000-0000-0000-0000-000000000007', 'a0500000-0000-0000-0000-000000000002', 'Assembly',                false, NOW()),
                    ('a0600000-0000-0000-0000-000000000008', 'a0500000-0000-0000-0000-000000000002', 'Machine Code',            false, NOW()),
                    ('a0600000-0000-0000-0000-000000000009', 'a0500000-0000-0000-0000-000000000003', 'x = 5',          true,  NOW()),
                    ('a0600000-0000-0000-0000-000000000010', 'a0500000-0000-0000-0000-000000000003', 'int x = 5',      false, NOW()),
                    ('a0600000-0000-0000-0000-000000000011', 'a0500000-0000-0000-0000-000000000003', 'var x = 5',      false, NOW()),
                    ('a0600000-0000-0000-0000-000000000012', 'a0500000-0000-0000-0000-000000000003', 'declare x = 5',  false, NOW()),
                    ('a0600000-0000-0000-0000-000000000013', 'a0500000-0000-0000-0000-000000000004', '%',  true,  NOW()),
                    ('a0600000-0000-0000-0000-000000000014', 'a0500000-0000-0000-0000-000000000004', '/',  false, NOW()),
                    ('a0600000-0000-0000-0000-000000000015', 'a0500000-0000-0000-0000-000000000004', '//', false, NOW()),
                    ('a0600000-0000-0000-0000-000000000016', 'a0500000-0000-0000-0000-000000000004', '**', false, NOW()),
                    ('a0600000-0000-0000-0000-000000000017', 'a0500000-0000-0000-0000-000000000005', 'HyperText Markup Language',    true,  NOW()),
                    ('a0600000-0000-0000-0000-000000000018', 'a0500000-0000-0000-0000-000000000005', 'HighText Machine Language',    false, NOW()),
                    ('a0600000-0000-0000-0000-000000000019', 'a0500000-0000-0000-0000-000000000005', 'HyperText Machine Language',   false, NOW()),
                    ('a0600000-0000-0000-0000-000000000020', 'a0500000-0000-0000-0000-000000000005', 'HyperTransfer Markup Language',false, NOW())
                ON CONFLICT DO NOTHING;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM quiz_options WHERE option_id::text LIKE 'a0600000%';");
            migrationBuilder.Sql("DELETE FROM quizzes WHERE quiz_id::text LIKE 'a0500000%';");
            migrationBuilder.Sql("DELETE FROM test_cases WHERE testcase_id::text LIKE 'a0400000%';");
            migrationBuilder.Sql("DELETE FROM exercises WHERE exercise_id::text LIKE 'a0300000%';");
            migrationBuilder.Sql("DELETE FROM lessons WHERE lesson_id::text LIKE 'a0200000%';");
            migrationBuilder.Sql("DELETE FROM sections WHERE section_id::text LIKE 'a0100000%';");
            migrationBuilder.Sql("DELETE FROM certificate_templates WHERE template_id::text LIKE 'f2000000%';");
            migrationBuilder.Sql("DELETE FROM course_completion_rules WHERE rule_id::text LIKE 'f1000000%';");
            migrationBuilder.Sql("DELETE FROM courses WHERE course_id::text LIKE 'e1000000%';");
            migrationBuilder.Sql("DELETE FROM subscription_packages WHERE package_id::text LIKE 'd1000000%';");
            migrationBuilder.Sql("DELETE FROM course_categories WHERE category_id::text LIKE 'c1000000%';");
            migrationBuilder.Sql("DELETE FROM account_roles WHERE account_id::text LIKE 'b1000000%';");
            migrationBuilder.Sql("DELETE FROM accounts WHERE account_id::text LIKE 'b1000000%';");
            migrationBuilder.Sql("DELETE FROM roles WHERE role_id::text LIKE 'a1b2c3d4%';");
        }
    }
}
