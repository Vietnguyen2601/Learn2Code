# H??ng d?n Database Migration cho Discussion/Comments

## Các b??c ?ã hoàn thành:

? **1. T?o các Entities:**
   - `Discussion.cs` - L?u tr? topic th?o lu?n
   - `DiscussionComment.cs` - L?u tr? bình lu?n/tr? l?i
   - `CommentLike.cs` - L?u tr? l??t like comment

? **2. C?p nh?t DbContext:**
   - Thêm 3 DbSet: Discussions, DiscussionComments, CommentLikes
   - C?u hình relationships và constraints

? **3. T?o Migration File:**
   - `20260401000000_AddDiscussionAndComments.cs` - Up/Down migration
   - C?p nh?t ModelSnapshot v?i entities m?i

? **4. Build thành công**

## ?? áp d?ng migration, ch?y m?t trong các l?nh sau:

### Option 1: S? d?ng Package Manager Console (trong Visual Studio)
```
Update-Database
```

### Option 2: S? d?ng .NET CLI (trong terminal)
```bash
cd BE
dotnet ef database update --project Learn2Code.Infrastructure --startup-project Learn2Code.API
```

### Option 3: N?u dùng PowerShell
```powershell
cd "D:\dai hoc FPT\TERM 8\PRN232\Learn2Code\BE"
dotnet ef database update --project Learn2Code.Infrastructure --startup-project Learn2Code.API
```

## Database Schema ???c t?o:

### Table: discussions
```
- discussion_id (UUID) - Primary Key
- lesson_id (UUID) - Foreign Key to lessons
- creator_id (UUID) - Foreign Key to accounts
- title (TEXT) - Required
- content (TEXT) - Required
- is_pinned (BOOLEAN)
- is_resolved (BOOLEAN)
- view_count (INTEGER)
- created_at (TIMESTAMP)
- updated_at (TIMESTAMP)
```

### Table: discussion_comments
```
- comment_id (UUID) - Primary Key
- discussion_id (UUID) - Foreign Key to discussions
- author_id (UUID) - Foreign Key to accounts
- parent_comment_id (UUID) - Foreign Key to discussion_comments (nullable, for nested replies)
- content (TEXT) - Required
- like_count (INTEGER)
- is_answer (BOOLEAN)
- created_at (TIMESTAMP)
- updated_at (TIMESTAMP)
```

### Table: comment_likes
```
- comment_id (UUID) - Part of Composite Primary Key
- user_id (UUID) - Part of Composite Primary Key
- like_id (UUID)
- created_at (TIMESTAMP)
Primary Key: (comment_id, user_id)
```

## Tính n?ng h? tr?:

? T?o discussion cho m?i lesson
? Bình lu?n trên discussion
? Reply comment (nested comments)
? Like comment
? Pin/Resolve discussion
? Theo dõi view count
? Cascade delete khi xóa lesson/discussion
? Restrict delete khi xóa creator/author

## B??c ti?p theo:

Sau khi database update thành công, b?n có th? t?o:
1. DTOs (Data Transfer Objects) cho Discussion và Comment
2. Repositories và Services
3. Controllers API
