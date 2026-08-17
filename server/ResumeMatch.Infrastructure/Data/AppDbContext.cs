using Microsoft.EntityFrameworkCore;
using ResumeMatch.Infrastructure.Entities;

namespace ResumeMatch.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Scan> Scans => Set<Scan>();
    public DbSet<ScanRedFlag> ScanRedFlags => Set<ScanRedFlag>();
    public DbSet<ScanSuggestion> ScanSuggestions => Set<ScanSuggestion>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("users");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.Id).HasColumnName("id");
            entity.Property(x => x.Email).HasColumnName("email").HasMaxLength(255).IsRequired();
            entity.Property(x => x.PasswordHash).HasColumnName("password_hash").HasMaxLength(255).IsRequired();
            entity.Property(x => x.CreatedAt).HasColumnName("created_at");

            entity.HasIndex(x => x.Email).IsUnique();
        });

        modelBuilder.Entity<Scan>(entity =>
        {
            entity.ToTable("scans");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.Id).HasColumnName("id");
            entity.Property(x => x.UserId).HasColumnName("user_id");
            entity.Property(x => x.ResumeFilename).HasColumnName("resume_filename").HasMaxLength(255);
            entity.Property(x => x.JobTitle).HasColumnName("job_title").HasMaxLength(255);
            entity.Property(x => x.MatchScore).HasColumnName("match_score").HasPrecision(5, 2);
            entity.Property(x => x.CreatedAt).HasColumnName("created_at");

            entity.HasIndex(x => x.UserId);
        });

        modelBuilder.Entity<ScanRedFlag>(entity =>
        {
            entity.ToTable("scan_red_flags");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.Id).HasColumnName("id");
            entity.Property(x => x.ScanId).HasColumnName("scan_id");
            entity.Property(x => x.FlagType).HasColumnName("flag_type").HasMaxLength(100).IsRequired();
            entity.Property(x => x.Description).HasColumnName("description").IsRequired();
        });

        modelBuilder.Entity<ScanSuggestion>(entity =>
        {
            entity.ToTable("scan_suggestions");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.Id).HasColumnName("id");
            entity.Property(x => x.ScanId).HasColumnName("scan_id");
            entity.Property(x => x.Suggestion).HasColumnName("suggestion").IsRequired();
        });
    }
}