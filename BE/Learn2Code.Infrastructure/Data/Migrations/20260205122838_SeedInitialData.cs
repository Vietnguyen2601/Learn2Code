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
            // Seed Roles with random UUIDs
            migrationBuilder.Sql(@"
                INSERT INTO roles (role_id, role_name, description, is_active, created_at, updated_at)
                VALUES 
                    (gen_random_uuid(), 'Admin', 'System Administrator with full access', true, NOW(), NOW()),
                    (gen_random_uuid(), 'Student', 'Learner who can enroll in courses', true, NOW(), NOW()),
                    (gen_random_uuid(), 'Instructor', 'Teacher who can create and manage courses', true, NOW(), NOW())
                ON CONFLICT DO NOTHING;
            ");

            // Seed Admin Account with random UUID
            // Password: Admin@123 (BCrypt hash - you should regenerate with your actual hasher)
            migrationBuilder.Sql(@"
                INSERT INTO accounts (account_id, username, email, password, name, is_active, created_at, updated_at)
                VALUES (
                    gen_random_uuid(),
                    'admin',
                    'admin@learn2code.com',
                    '$2a$11$K6Vj8GZXK.0dQzXZxzxzxuO8mWXpK8QVHqJGZ1Zy8N6L0h5qJvKfC',
                    'System Admin',
                    true,
                    NOW(),
                    NOW()
                )
                ON CONFLICT DO NOTHING;
            ");

            // Assign Admin Role to Admin Account
            migrationBuilder.Sql(@"
                INSERT INTO account_roles (account_id, role_id, assigned_at)
                SELECT a.account_id, r.role_id, NOW()
                FROM accounts a
                CROSS JOIN roles r
                WHERE a.username = 'admin' AND r.role_name = 'Admin'
                ON CONFLICT DO NOTHING;
            ");

            // Seed Course Categories
            migrationBuilder.Sql(@"
                INSERT INTO course_categories (name, description, created_at)
                VALUES 
                    ('Web Development', 'Frontend, Backend, Full-stack web development', NOW()),
                    ('Mobile Development', 'iOS, Android, React Native, Flutter', NOW()),
                    ('Data Science', 'Python, Machine Learning, AI', NOW()),
                    ('DevOps', 'Docker, Kubernetes, CI/CD', NOW()),
                    ('Programming Languages', 'C#, Java, Python, JavaScript fundamentals', NOW())
                ON CONFLICT DO NOTHING;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"DELETE FROM account_roles WHERE account_id IN (SELECT account_id FROM accounts WHERE username = 'admin');");
            migrationBuilder.Sql(@"DELETE FROM accounts WHERE username = 'admin';");
            migrationBuilder.Sql(@"DELETE FROM roles WHERE role_name IN ('Admin', 'Student', 'Instructor');");
            migrationBuilder.Sql(@"DELETE FROM course_categories WHERE name IN ('Web Development', 'Mobile Development', 'Data Science', 'DevOps', 'Programming Languages');");
        }
    }
}
