// ================================================================
//  CODING LEARNING PLATFORM — Final Schema
// ================================================================
//
//  CONTENT HIERARCHY:
//
//    Course
//      └── Section
//            ├── Lesson
//            │     ├── Exercises  (Reading | FreeCode | GradedCode)
//            │     │     └── TestCases  (chỉ có khi GradedCode)
//            │     └── Quizzes    (mỗi quiz = 1 câu hỏi trắc nghiệm)
//            │           └── QuizOptions
//            │
//            └── SectionQuiz     (mở khóa khi hoàn thành tất cả lesson)
//                  └── gom Quiz từ tất cả lesson trong section
//
//  3 DẠNG EXERCISE:
//    Reading   → trái: text,             phải: ảnh/video   → luôn next được
//    FreeCode  → trái: text,             phải: editor tự do      → chạy được là next
//    GradedCode→ trái: text,             phải: editor có chấm    → đúng expected mới next
//
//  QUIZ LOGIC:
//    - Mỗi Quiz gắn vào 1 Lesson (1 câu hỏi / quiz)
//    - SectionQuiz gom tất cả Quiz của các Lesson trong Section
//    - SectionQuiz chỉ mở khi tất cả Lesson trong Section = Completed
//
//  FREE PREVIEW:
//    - Lesson.is_free_preview = true → học không cần subscription
//
//  SUBSCRIPTION LOCK:
//    - Khi hết hạn → lock tại lesson InProgress cuối (dựa vào LessonProgress)
//
// ================================================================


// ─────────────────────────────────────────
//  ACCOUNTS & ROLES
// ─────────────────────────────────────────

Table Accounts {
  account_id   uuid      [pk]
  username     varchar   [unique, not null]
  password     varchar   [not null]
  email        varchar   [unique, not null]
  name         varchar
  phone_number varchar
  is_active    boolean   [default: true]
  created_at   timestamp [not null, default: `CURRENT_TIMESTAMP`]
  updated_at   timestamp [not null, default: `CURRENT_TIMESTAMP`]
}

Table Roles {
  role_id    uuid      [pk]
  role_name  varchar   [unique, not null]  // Admin  | Student
  is_active  boolean   [default: true]
  created_at timestamp [not null, default: `CURRENT_TIMESTAMP`]
  updated_at timestamp [not null, default: `CURRENT_TIMESTAMP`]
}

Table AccountRoles {
  account_id  uuid      [not null, ref: > Accounts.account_id]
  role_id     uuid      [not null, ref: > Roles.role_id]
  assigned_at timestamp [not null, default: `CURRENT_TIMESTAMP`]

  Indexes {
    (account_id, role_id) [pk]
    role_id
  }
}


// ─────────────────────────────────────────
//  SUBSCRIPTION
// ─────────────────────────────────────────

Table SubscriptionPackages {
  package_id       uuid      [pk]
  name             varchar   [unique, not null]  // "1 Month" | "6 Months" | "1 Year"
  duration_months  int       [not null]
  price            decimal   [not null]
  discount_percent decimal   [default: 0]
  description      text
  is_active        boolean   [default: true]
  created_at       timestamp [not null, default: `CURRENT_TIMESTAMP`]
  updated_at       timestamp [not null, default: `CURRENT_TIMESTAMP`]
}

Table UserSubscriptions {
  subscription_id uuid      [pk]
  user_id         uuid      [not null, ref: > Accounts.account_id]
  package_id      uuid      [not null, ref: > SubscriptionPackages.package_id]
  start_date      timestamp [not null]
  end_date        timestamp [not null]
  status          enum('Pending','Active','Expired','Cancelled') [default: 'Pending']
  renewed_from_id uuid      [ref: > UserSubscriptions.subscription_id]
  created_at      timestamp [not null, default: `CURRENT_TIMESTAMP`]
  updated_at      timestamp [not null, default: `CURRENT_TIMESTAMP`]

  Indexes {
    user_id
    (user_id, status)
    end_date          // dùng cho scheduled expiry job
  }
}

Table Payments {
  payment_id      uuid      [pk]
  subscription_id uuid      [not null, ref: > UserSubscriptions.subscription_id]
  amount          decimal   [not null]
  payment_method  enum('VNPay','BankTransfer') [not null]
  transaction_id  varchar   [unique]
  status          enum('Pending','Success','Failed') [default: 'Pending']
  paid_at         timestamp
  created_at      timestamp [not null, default: `CURRENT_TIMESTAMP`]

  Indexes {
    subscription_id
    transaction_id
    status
  }
}


// ─────────────────────────────────────────
//  COURSE CATALOGUE
// ─────────────────────────────────────────

Table CourseCategories {
  category_id uuid      [pk]
  name        varchar   [unique, not null]
  description text
  is_active   boolean   [default: true]
  created_at  timestamp [not null, default: `CURRENT_TIMESTAMP`]
  updated_at  timestamp [not null, default: `CURRENT_TIMESTAMP`]
}

Table Courses {
  course_id     uuid      [pk]
  title         varchar   [not null]
  description   text
  difficulty    enum('Beginner','Intermediate','Advanced')
  is_active     boolean   [default: true]
  instructor_id uuid      [not null, ref: > Accounts.account_id]
  category_id   uuid      [ref: > CourseCategories.category_id]
  created_at    timestamp [not null, default: `CURRENT_TIMESTAMP`]
  updated_at    timestamp [not null, default: `CURRENT_TIMESTAMP`]

  Indexes {
    instructor_id
    category_id
  }
}

Table Sections {
  section_id   uuid      [pk]
  course_id    uuid      [not null, ref: > Courses.course_id]
  title        varchar   [not null]
  description  text
  order_number int       [not null]
  is_active    boolean   [default: true]
  created_at   timestamp [not null, default: `CURRENT_TIMESTAMP`]
  updated_at   timestamp [not null, default: `CURRENT_TIMESTAMP`]

  Indexes {
    (course_id, order_number) [unique]
    course_id
  }
}

Table Lessons {
  lesson_id       uuid      [pk]
  section_id      uuid      [not null, ref: > Sections.section_id]
  title           varchar   [not null]
  order_number    int       [not null]
  is_free_preview boolean   [default: false]  // true = học không cần subscription
  created_at      timestamp [not null, default: `CURRENT_TIMESTAMP`]
  updated_at      timestamp [not null, default: `CURRENT_TIMESTAMP`]

  Indexes {
    (section_id, order_number) [unique]
    section_id
    is_free_preview
  }
}


// ─────────────────────────────────────────
//  LESSON CONTENT — EXERCISES
//
//  exercise_type:
//    Reading   → narrative + media (ảnh/video),  không có editor
//    FreeCode  → narrative,  editor tự do         (chạy được là next)
//    GradedCode→ narrative,  editor có chấm       (phải đúng expected mới next)
// ─────────────────────────────────────────

Table Exercises {
  exercise_id    uuid      [pk]
  lesson_id      uuid      [not null, ref: > Lessons.lesson_id]
  order_number   int       [not null]
  exercise_type  enum('Reading','FreeCode','GradedCode') [not null]

  // phần trái màn hình — luôn có
  narrative      text      [not null]   // nội dung lý thuyết (markdown)

  // phần phải màn hình — tuỳ exercise_type
  language       varchar               // python|javascript|... (null nếu Reading)
  starter_code   text                  // code mẫu điền sẵn (null nếu Reading)
  solution_code  text                  // đáp án chuẩn, ẩn với student (null nếu Reading)
  instruction    text                  // yêu cầu cụ thể (dùng cho FreeCode & GradedCode (null nếu reading))
  hint           text                  // gợi ý khi bấm "Get Hint" (null nếu Reading)

  /// Hàm main mặc định dùng khi student bấm Run (gắn vào sau starter_code).
  /// Null = bài output-only (print hello, ...) → chạy student code thẳng.
  default_main_code text               // null nếu Reading hoặc output-only

  created_at     timestamp [not null, default: `CURRENT_TIMESTAMP`]
  updated_at     timestamp [not null, default: `CURRENT_TIMESTAMP`]

  Indexes {
    (lesson_id, order_number) [unique]
    lesson_id
    exercise_type
  }
}

// Media đính kèm cho Exercise dạng Reading (ảnh, video)
Table ExerciseMedia {
  media_id      uuid      [pk]
  exercise_id   uuid      [not null, ref: > Exercises.exercise_id]
  media_type    enum('Image','Video') [not null]
  url           varchar   [not null]   // CDN URL
  caption       varchar                // mô tả ảnh/video
  order_number  int       [not null]   // thứ tự hiển thị
  created_at    timestamp [not null, default: `CURRENT_TIMESTAMP`]
  updated_at    timestamp [not null, default: `CURRENT_TIMESTAMP`]

  Indexes {
    (exercise_id, order_number) [unique]
    exercise_id
  }
}

// TestCases chỉ dùng cho exercise_type = GradedCode
Table TestCases {
  testcase_id     uuid      [pk]
  exercise_id     uuid      [not null, ref: > Exercises.exercise_id]
  expected_output text      [not null]
  is_hidden       boolean   [default: false]  // ẩn với student (chống hardcode)
  weight          decimal   [default: 1]

  /// Hàm main riêng của testcase.
  /// Khi Submit: student_code + "\n\n" + main_code được chạy như 1 file.
  /// Null = chạy student_code trực tiếp (stdin = text_input).
  main_code       text                  // null = output-based exercise
  created_at      timestamp [not null, default: `CURRENT_TIMESTAMP`]
  updated_at      timestamp [not null, default: `CURRENT_TIMESTAMP`]

  Indexes {
    exercise_id
    is_hidden
  }
}


// ─────────────────────────────────────────
//  LESSON CONTENT — QUIZ
//
//  Mỗi Quiz = 1 câu hỏi, gắn vào 1 Lesson.
//  SectionQuiz gom tất cả Quiz thuộc các Lesson trong Section
//  → mở khóa khi student hoàn thành tất cả Lesson trong Section.
// ─────────────────────────────────────────

Table Quizzes {
  quiz_id       uuid      [pk]
  lesson_id     uuid      [not null, ref: > Lessons.lesson_id]
  order_number  int       [not null]   // thứ tự trong lesson
  question      text      [not null]
  explanation   text                   // giải thích đáp án đúng (hiện sau khi trả lời)
  created_at    timestamp [not null, default: `CURRENT_TIMESTAMP`]
  updated_at    timestamp [not null, default: `CURRENT_TIMESTAMP`]

  Indexes {
    (lesson_id, order_number) [unique]
    lesson_id
  }
}

Table QuizOptions {
  option_id   uuid      [pk]
  quiz_id     uuid      [not null, ref: > Quizzes.quiz_id]
  content     text      [not null]
  is_correct  boolean   [default: false]
  created_at  timestamp [not null, default: `CURRENT_TIMESTAMP`]

  Indexes {
    quiz_id
  }
}

// SectionQuiz = bài thi tổng kết Section
// Không lưu lại danh sách quiz vì đã query được qua:
//   Section → Lessons → Quizzes
// Chỉ cần track trạng thái mở khóa và kết quả của student.
Table SectionQuizAttempts {
  attempt_id  uuid      [pk]
  section_id  uuid      [not null, ref: > Sections.section_id]
  student_id  uuid      [not null, ref: > Accounts.account_id]
  score       decimal   [not null]   // điểm % (0–100)
  is_passed   boolean   [not null]
  attempted_at timestamp [not null, default: `CURRENT_TIMESTAMP`]

  Indexes {
    (section_id, student_id, attempted_at)
    student_id
    section_id
  }

  
}

// Câu trả lời của student trong mỗi lần thi SectionQuiz
Table SectionQuizAnswers {
  answer_id   uuid      [pk]
  attempt_id  uuid      [not null, ref: > SectionQuizAttempts.attempt_id]
  quiz_id     uuid      [not null, ref: > Quizzes.quiz_id]
  option_id   uuid      [not null, ref: > QuizOptions.option_id]
  is_correct  boolean   [not null]

  Indexes {
    (attempt_id, quiz_id) [unique]
    attempt_id
  }
}


// ─────────────────────────────────────────
//  STUDENT ACTIVITY
// ─────────────────────────────────────────

Table Enrollments {
  enrollment_id   uuid      [pk]
  student_id      uuid      [not null, ref: > Accounts.account_id]
  course_id       uuid      [not null, ref: > Courses.course_id]
  status          enum('Enrolled','InProgress','Completed') [default: 'Enrolled']
  progress_pct    decimal   [default: 0]    // cached, tính từ LessonProgress
  enrolled_at     timestamp [not null, default: `CURRENT_TIMESTAMP`]
  activated_at    timestamp                  // lần đầu tiên bắt đầu học
  completed_at    timestamp
  subscription_id uuid      [ref: > UserSubscriptions.subscription_id]
  // null = học free preview hoặc không cần subscription

  Indexes {
    (student_id, course_id) [unique]
    student_id
    course_id
    subscription_id
    status
  }
}

// Track tiến độ mỗi Lesson
Table LessonProgress {
  progress_id      uuid      [pk]
  student_id       uuid      [not null, ref: > Accounts.account_id]
  lesson_id        uuid      [not null, ref: > Lessons.lesson_id]
  status           enum('NotStarted','InProgress','Completed') [default: 'NotStarted']
  last_accessed_at timestamp
  completed_at     timestamp
  created_at       timestamp [not null, default: `CURRENT_TIMESTAMP`]
  updated_at       timestamp [not null, default: `CURRENT_TIMESTAMP`]

  Indexes {
    (student_id, lesson_id) [unique]
    student_id
    lesson_id
    status
  }
}

// Track tiến độ mỗi Exercise
Table ExerciseProgress {
  exprogress_id uuid      [pk]
  student_id    uuid      [not null, ref: > Accounts.account_id]
  exercise_id   uuid      [not null, ref: > Exercises.exercise_id]

  is_completed  boolean   [default: false]

  // chỉ có giá trị khi exercise_type = GradedCode
  is_passed     boolean   [default: false]

  // auto-save code mới nhất của student (FreeCode & GradedCode)
  last_code     text

  completed_at  timestamp
  created_at    timestamp [not null, default: `CURRENT_TIMESTAMP`]
  updated_at    timestamp [not null, default: `CURRENT_TIMESTAMP`]

  Indexes {
    (student_id, exercise_id) [unique]
    student_id
    exercise_id
  }
}


// ─────────────────────────────────────────
//  COURSE COMPLETION & CERTIFICATION
// ─────────────────────────────────────────

Table CourseCompletionRules {
  rule_id                   uuid      [pk]
  course_id                 uuid      [unique, not null, ref: > Courses.course_id]
  min_weight_score    decimal   [default: 0]    // điểm tối thiểu SectionQuiz
  require_all_section_quiz  boolean   [default: true]   // phải thi hết tất cả SectionQuiz
  created_at                timestamp [not null, default: `CURRENT_TIMESTAMP`]
  updated_at                timestamp [not null, default: `CURRENT_TIMESTAMP`]
}

Table CertificateTemplates {
  template_id          uuid      [pk]
  course_id            uuid      [unique, not null, ref: > Courses.course_id]
  title                varchar   [not null]
  description          text
  background_image_url varchar
  signature_name       varchar
  signature_image_url  varchar
  created_at           timestamp [not null, default: `CURRENT_TIMESTAMP`]
  updated_at           timestamp [not null, default: `CURRENT_TIMESTAMP`]
}

Table Certifications {
  certification_id uuid      [pk]
  student_id       uuid      [not null, ref: > Accounts.account_id]
  course_id        uuid      [not null, ref: > Courses.course_id]
  certificate_code varchar   [unique, not null]
  certificate_url  varchar
  issued_at        timestamp [not null, default: `CURRENT_TIMESTAMP`]

  Indexes {
    (student_id, course_id) [unique]
    course_id
  }
}


// ─────────────────────────────────────────
//  SOCIAL / GAMIFICATION
// ─────────────────────────────────────────

Table Feedbacks {
  feedback_id uuid      [pk]
  course_id   uuid      [not null, ref: > Courses.course_id]
  student_id  uuid      [not null, ref: > Accounts.account_id]
  rating      int       [not null]  // 1–5
  comment     text
  created_at  timestamp [not null, default: `CURRENT_TIMESTAMP`]
  updated_at  timestamp [not null, default: `CURRENT_TIMESTAMP`]

  Indexes {
    (course_id, student_id) [unique]
    course_id
  }
}


