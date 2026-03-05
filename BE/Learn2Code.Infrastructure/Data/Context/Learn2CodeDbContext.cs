using Learn2Code.Domain.Entities;
using Learn2Code.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Learn2Code.Infrastructure.Data.Context;

public class Learn2CodeDbContext : DbContext
{
    public Learn2CodeDbContext(DbContextOptions<Learn2CodeDbContext> options) : base(options)
    {
    }

    public Learn2CodeDbContext()
    {
    }

    public static string GetConnectionString(string connectionStringName)
    {
        string? envConnectionString = Environment.GetEnvironmentVariable($"ConnectionStrings__{connectionStringName}");
        if (!string.IsNullOrEmpty(envConnectionString))
        {
            return envConnectionString;
        }

        var basePath = AppDomain.CurrentDomain.BaseDirectory;

        var currentDir = new DirectoryInfo(basePath);
        while (currentDir != null && !File.Exists(Path.Combine(currentDir.FullName, "Learn2Code.API", "appsettings.json")))
        {
            currentDir = currentDir.Parent;
        }

        var configPath = currentDir != null
            ? Path.Combine(currentDir.FullName, "Learn2Code.API")
            : basePath;

        var config = new ConfigurationBuilder()
            .SetBasePath(configPath)
            .AddJsonFile("appsettings.json", optional: true)
            .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development"}.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        return config.GetConnectionString(connectionStringName) ?? string.Empty;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseNpgsql(GetConnectionString("DefaultConnection"))
                .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
        }
    }

    public DbSet<Account> Accounts { get; set; } = null!;
    public DbSet<Role> Roles { get; set; } = null!;
    public DbSet<AccountRole> AccountRoles { get; set; } = null!;
    public DbSet<SubscriptionPackage> SubscriptionPackages { get; set; } = null!;
    public DbSet<UserSubscription> UserSubscriptions { get; set; } = null!;
    public DbSet<CourseCategory> CourseCategories { get; set; } = null!;
    public DbSet<Course> Courses { get; set; } = null!;
    public DbSet<Enrollment> Enrollments { get; set; } = null!;
    public DbSet<Payment> Payments { get; set; } = null!;
    public DbSet<Section> Sections { get; set; } = null!;
    public DbSet<Lesson> Lessons { get; set; } = null!;
    public DbSet<Exercise> Exercises { get; set; } = null!;
    public DbSet<ExerciseMedia> ExerciseMedias { get; set; } = null!;
    public DbSet<TestCase> TestCases { get; set; } = null!;
    public DbSet<Quiz> Quizzes { get; set; } = null!;
    public DbSet<QuizOption> QuizOptions { get; set; } = null!;
    public DbSet<SectionQuizAttempt> SectionQuizAttempts { get; set; } = null!;
    public DbSet<SectionQuizAnswer> SectionQuizAnswers { get; set; } = null!;
    public DbSet<LessonProgress> LessonProgresses { get; set; } = null!;
    public DbSet<ExerciseProgress> ExerciseProgresses { get; set; } = null!;
    public DbSet<CertificateTemplate> CertificateTemplates { get; set; } = null!;
    public DbSet<Certification> Certifications { get; set; } = null!;
    public DbSet<CourseCompletionRule> CourseCompletionRules { get; set; } = null!;
    public DbSet<Feedback> Feedbacks { get; set; } = null!;
    public DbSet<Leaderboard> Leaderboards { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ── Composite keys ──────────────────────────────────────────────────────
        modelBuilder.Entity<AccountRole>()
            .HasKey(ar => new { ar.AccountId, ar.RoleId });

        // ── Unique constraints ──────────────────────────────────────────────────
        modelBuilder.Entity<Account>()
            .HasIndex(a => a.Username).IsUnique();

        modelBuilder.Entity<Account>()
            .HasIndex(a => a.Email).IsUnique();

        modelBuilder.Entity<CourseCategory>()
            .HasIndex(c => c.Name).IsUnique();

        modelBuilder.Entity<Certification>()
            .HasIndex(c => c.CertificateCode).IsUnique();

        modelBuilder.Entity<SubscriptionPackage>()
            .HasIndex(p => p.Name).IsUnique();

        modelBuilder.Entity<Payment>()
            .HasIndex(p => p.TransactionId).IsUnique();

        modelBuilder.Entity<Leaderboard>()
            .HasIndex(l => new { l.CourseId, l.StudentId }).IsUnique();

        modelBuilder.Entity<Leaderboard>()
            .HasIndex(l => new { l.CourseId, l.Rank }).IsUnique();

        modelBuilder.Entity<Feedback>()
            .HasIndex(f => new { f.CourseId, f.StudentId }).IsUnique();

        modelBuilder.Entity<Enrollment>()
            .HasIndex(e => new { e.StudentId, e.CourseId }).IsUnique();

        modelBuilder.Entity<LessonProgress>()
            .HasIndex(lp => new { lp.StudentId, lp.LessonId }).IsUnique();

        modelBuilder.Entity<ExerciseProgress>()
            .HasIndex(ep => new { ep.StudentId, ep.ExerciseId }).IsUnique();

        modelBuilder.Entity<SectionQuizAnswer>()
            .HasIndex(a => new { a.AttemptId, a.QuizId }).IsUnique();

        modelBuilder.Entity<Section>()
            .HasIndex(s => new { s.CourseId, s.OrderNumber }).IsUnique();

        modelBuilder.Entity<Lesson>()
            .HasIndex(l => new { l.SectionId, l.OrderNumber }).IsUnique();

        modelBuilder.Entity<Exercise>()
            .HasIndex(e => new { e.LessonId, e.OrderNumber }).IsUnique();

        modelBuilder.Entity<Quiz>()
            .HasIndex(q => new { q.LessonId, q.OrderNumber }).IsUnique();

        modelBuilder.Entity<ExerciseMedia>()
            .HasIndex(m => new { m.ExerciseId, m.OrderNumber }).IsUnique();

        modelBuilder.Entity<Certification>()
            .HasIndex(c => new { c.StudentId, c.CourseId }).IsUnique();

        // ── One-to-one relationships ────────────────────────────────────────────
        modelBuilder.Entity<Course>()
            .HasOne(c => c.CertificateTemplate)
            .WithOne(ct => ct.Course)
            .HasForeignKey<CertificateTemplate>(ct => ct.CourseId);

        modelBuilder.Entity<Course>()
            .HasOne(c => c.CompletionRule)
            .WithOne(cr => cr.Course)
            .HasForeignKey<CourseCompletionRule>(cr => cr.CourseId);

        // ── Self-referencing UserSubscription ──────────────────────────────────
        modelBuilder.Entity<UserSubscription>()
            .HasOne(u => u.RenewedFrom)
            .WithMany(u => u.RenewedSubscriptions)
            .HasForeignKey(u => u.RenewedFromId)
            .OnDelete(DeleteBehavior.SetNull);

        // ── Enum → string conversions ───────────────────────────────────────────
        modelBuilder.Entity<Course>()
            .Property(c => c.Difficulty)
            .HasConversion<string>();

        modelBuilder.Entity<Enrollment>()
            .Property(e => e.Status)
            .HasConversion<string>();

        modelBuilder.Entity<Payment>()
            .Property(p => p.PaymentMethod)
            .HasConversion<string>();

        modelBuilder.Entity<Payment>()
            .Property(p => p.Status)
            .HasConversion<string>();

        modelBuilder.Entity<Exercise>()
            .Property(e => e.ExerciseType)
            .HasConversion<string>();

        modelBuilder.Entity<UserSubscription>()
            .Property(u => u.Status)
            .HasConversion<string>();

        modelBuilder.Entity<ExerciseMedia>()
            .Property(m => m.MediaType)
            .HasConversion<string>();

        modelBuilder.Entity<LessonProgress>()
            .Property(lp => lp.Status)
            .HasConversion<string>();

        // ── Decimal precision ───────────────────────────────────────────────────
        modelBuilder.Entity<SubscriptionPackage>()
            .Property(p => p.Price)
            .HasPrecision(18, 2);

        modelBuilder.Entity<SubscriptionPackage>()
            .Property(p => p.DiscountPercent)
            .HasPrecision(5, 2);

        modelBuilder.Entity<Payment>()
            .Property(p => p.Amount)
            .HasPrecision(18, 2);

        modelBuilder.Entity<Enrollment>()
            .Property(e => e.ProgressPct)
            .HasPrecision(5, 2);

        modelBuilder.Entity<TestCase>()
            .Property(t => t.Weight)
            .HasPrecision(5, 2);

        modelBuilder.Entity<SectionQuizAttempt>()
            .Property(a => a.Score)
            .HasPrecision(5, 2);

        modelBuilder.Entity<CourseCompletionRule>()
            .Property(c => c.MinLessonCompletionPct)
            .HasPrecision(5, 2);

        modelBuilder.Entity<CourseCompletionRule>()
            .Property(c => c.MinExercisePassPct)
            .HasPrecision(5, 2);

        modelBuilder.Entity<CourseCompletionRule>()
            .Property(c => c.MinSectionQuizScore)
            .HasPrecision(5, 2);

        modelBuilder.Entity<Leaderboard>()
            .Property(l => l.TotalScore)
            .HasPrecision(10, 2);
    }
}
