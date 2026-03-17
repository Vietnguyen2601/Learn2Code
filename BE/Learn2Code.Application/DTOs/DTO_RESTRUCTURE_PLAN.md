# DTO Restructure Plan

## 1) Muc tieu
- Tach moi DTO class thanh 1 file rieng.
- Gom DTO theo domain folder (AccountDTOs, AuthDTOs, ...).
- Giam xung dot file lon, de tim kiem, review va maintain.
- Dam bao code chay nhu cu (khong doi behavior).

## 2) Cau truc dich
```text
Learn2Code.Application/
  DTOs/
    AccountDTOs/
      AccountRequests/
        CreateAccountRequest.cs
        UpdateAccountRequest.cs
      AccountResponses/
        AccountDto.cs
    AuthDTOs/
      AuthRequests/
        RegisterRequest.cs
        VerifyOtpRequest.cs
        LoginRequest.cs
        ForgotPasswordRequest.cs
        ResetPasswordRequest.cs
        RefreshTokenRequest.cs
        UpdateProfileRequest.cs
      AuthResponses/
        RegisterResponse.cs
        VerifyOtpResponse.cs
        LoginResponse.cs
        ResetPasswordResponse.cs
        RefreshTokenResponse.cs
        MeResponse.cs
        UpdateProfileResponse.cs
    CategoryDTOs/
      CategoryRequests/
      CategoryResponses/
    CourseDTOs/
      CourseRequests/
      CourseResponses/
    EnrollmentDTOs/
      EnrollmentRequests/
      EnrollmentResponses/
    ExerciseDTOs/
      ExerciseRequests/
      ExerciseResponses/
    LessonDTOs/
      LessonRequests/
      LessonResponses/
    PaymentDTOs/
      PaymentRequests/
      PaymentResponses/
    ProgressDTOs/
      ProgressRequests/
      ProgressResponses/
    QuizDTOs/
      QuizRequests/
      QuizResponses/
    SectionDTOs/
      SectionRequests/
      SectionResponses/
    SectionQuizDTOs/
      SectionQuizRequests/
      SectionQuizResponses/
    SubscriptionDTOs/
      SubscriptionRequests/
      SubscriptionResponses/
    TestCaseDTOs/
      TestCaseRequests/
      TestCaseResponses/
    CertificationDTOs/
      CertificationRequests/
      CertificationResponses/
```

## 3) Rules bat buoc (khong duoc vi pham)
- Giu nguyen ten class hien tai.
- Giu nguyen ten property hien tai.
- Giu nguyen attributes (`JsonPropertyName`, `Required`, `MinLength`, ...).
- Giu nguyen access modifier, nullable, default value, inheritance.
- Khong doi contract API JSON (field name trong `JsonPropertyName` phai giu nguyen).
- Moi file DTO chi chua 1 class.
- Ten file theo quy uoc: `<ClassName>Dto.cs` neu can thong nhat naming file; class ben trong van giu ten cu.
- Class Request phai nam trong thu muc `*Requests`, class Response/Dto phai nam trong thu muc `*Responses`.
- Sau khi tach folder/namespace, phai cap nhat `using` cho tat ca noi dang dung DTO.

## 4) Namespace strategy
Chon 1 trong 2 cach va giu nhat quan toan bo project:

### Cach A (an toan, it doi using)
- Namespace tat ca file DTO van la: `Learn2Code.Application.DTOs`
- Folder chi de to chuc file.
- Loi ich: han che sua nhieu file, rui ro thap.

### Cach B (ro domain, doi using nhieu hon)
- Namespace theo folder, vi du:
  - `Learn2Code.Application.DTOs.AccountDTOs.AccountRequests`
  - `Learn2Code.Application.DTOs.AccountDTOs.AccountResponses`
  - `Learn2Code.Application.DTOs.AuthDTOs.AuthRequests`
  - `Learn2Code.Application.DTOs.AuthDTOs.AuthResponses`
- Bat buoc sua `using` o Controllers/Services/Interfaces/Mappers.
- Loi ich: namespace ro domain, de quan ly phu thuoc.

## 5) Pham vi anh huong can cap nhat using
- BE/Learn2Code.API/Controllers/*.cs
- BE/Learn2Code.Application/Services/*.cs
- BE/Learn2Code.Application/Interfaces/*.cs
- BE/Learn2Code.Application/Mapper/*.cs

Ghi chu:
- Hien tai nhieu file dang `using Learn2Code.Application.DTOs;`.
- Neu chon Cach B thi can thay bang cac using domain tuong ung.

## 6) Ke hoach thuc hien theo phase

### Phase 1 - Chuan bi
- Tao folder domain trong `DTOs/`.
- Chot namespace strategy (A hoac B).
- Chot quy uoc ten file.

### Phase 2 - Tach file DTO
- Moi file `*Dtos.cs` se duoc tach thanh nhieu file, moi class 1 file.
- Moi domain sau khi tach file se tiep tuc phan loai vao 2 folder con: `*Requests` va `*Responses`.
- Uu tien tach theo domain lon truoc: Auth, Course, Exercise, Subscription.
- Xoa file tong cu sau khi xac nhan class da duoc move het.

### Phase 3 - Cap nhat reference
- Cap nhat `using` cho Controllers/Services/Interfaces/Mappers.
- Xu ly ambiguous type neu trung ten class giua cac domain.
- Dam bao khong con import namespace cu sai.

### Phase 4 - Validate
- Build solution: `dotnet build BE/Learn2Code.BE.sln`
- Chay test (neu co): `dotnet test`
- Smoke test API auth/course/exercise de xac nhan JSON contract khong doi.

### Phase 5 - Hoan tat va tai lieu
- Tao commit theo tung nhom domain de de rollback.
- Cap nhat API specification ngay sau khi hoan thanh dot refactor nay.

## 7) Checklist migration theo file hien tai
- [x] AccountDtos.cs
- [x] AuthDtos.cs
- [x] CategoryDtos.cs
- [x] CertificationDtos.cs
- [x] CourseDtos.cs
- [x] EnrollmentDtos.cs
- [x] ExerciseDtos.cs
- [x] LessonDtos.cs
- [x] PaymentDtos.cs
- [x] ProgressDtos.cs
- [x] QuizDtos.cs
- [x] SectionDtos.cs
- [x] SectionQuizDtos.cs
- [x] SubscriptionDtos.cs
- [x] TestCaseDtos.cs

## 8) Checklist review truoc merge
- [ ] Khong doi ten class/property.
- [ ] Khong doi `JsonPropertyName`.
- [ ] Khong doi request/response contract.
- [ ] Khong con compile error do using/namespace.
- [ ] `dotnet build` pass.
- [ ] API spec duoc cap nhat.

## 9) Rollback plan
- Neu phat sinh loi namespace lon:
  - Revert theo commit tung domain (khong revert tat ca).
  - Quay ve Cach A (giu namespace cu) de giam blast radius.

## 10) Goi y thu tu thuc hien de an toan
1. AuthDTOs
2. AccountDTOs
3. CourseDTOs + SectionDTOs + LessonDTOs
4. ExerciseDTOs + TestCaseDTOs
5. QuizDTOs + SectionQuizDTOs
6. SubscriptionDTOs + PaymentDTOs
7. CategoryDTOs + EnrollmentDTOs + ProgressDTOs + CertificationDTOs

## 11) Tien do da lam
- [x] Da chon namespace strategy B cho 2 domain dau voi subfolders:
  - `Learn2Code.Application.DTOs.AccountDTOs.AccountRequests`
  - `Learn2Code.Application.DTOs.AccountDTOs.AccountResponses`
  - `Learn2Code.Application.DTOs.AuthDTOs.AuthRequests`
  - `Learn2Code.Application.DTOs.AuthDTOs.AuthResponses`
- [x] Da tach tat ca class trong `AccountDtos.cs` thanh file rieng trong `DTOs/AccountDTOs/`.
- [x] Da tach tat ca class trong `AuthDtos.cs` thanh file rieng trong `DTOs/AuthDTOs/`.
- [x] Da xoa 2 file tong hop cu: `AccountDtos.cs`, `AuthDtos.cs`.
- [x] Da cap nhat `using` cho Controllers/Interfaces/Services/Mapper lien quan den Auth + Account theo model Requests/Responses.
- [x] Da build thanh cong solution sau khi refactor Auth + Account theo model Requests/Responses (0 errors).
- [x] Da tach xong `CourseDtos.cs` theo `CourseRequests/` + `CourseResponses/`.
- [x] Da tach xong `SectionDtos.cs` theo `SectionRequests/` + `SectionResponses/`.
- [x] Da tach xong `LessonDtos.cs` theo `LessonRequests/` + `LessonResponses/`.
- [x] Da cap nhat namespace + using cho Controllers/Interfaces/Services/Mapper lien quan den Course + Section + Lesson.
- [x] Da xoa 3 file tong hop cu: `CourseDtos.cs`, `SectionDtos.cs`, `LessonDtos.cs`.
- [x] Da build thanh cong solution sau khi refactor Course + Section + Lesson (0 errors).
- [x] Da tach xong `ExerciseDtos.cs` theo `ExerciseRequests/` + `ExerciseResponses/`.
- [x] Da tach xong `TestCaseDtos.cs` theo `TestCaseRequests/` + `TestCaseResponses/`.
- [x] Da cap nhat namespace + using cho Controllers/Interfaces/Services/Mapper lien quan den Exercise + TestCase.
- [x] Da xoa 2 file tong hop cu: `ExerciseDtos.cs`, `TestCaseDtos.cs`.
- [x] Da build thanh cong solution sau khi refactor Exercise + TestCase (0 errors).

## 12) Dieu chinh ke hoach moi (Request/Response subfolders)
- [x] Refactor lai `AccountDTOs` thanh `AccountRequests/` va `AccountResponses/`.
- [x] Refactor lai `AuthDTOs` thanh `AuthRequests/` va `AuthResponses/`.
- [x] Cap nhat namespace + using theo cau truc subfolder moi cho Account/Auth.
- [x] Build lai sau khi doi cau truc subfolder.
