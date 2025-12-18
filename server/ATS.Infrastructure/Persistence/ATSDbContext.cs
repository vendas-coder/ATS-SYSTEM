using ATS.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ATS.Infrastructure.Persistence;

public class ATSDbContext : DbContext
{
    public ATSDbContext(DbContextOptions<ATSDbContext> options)
        : base(options)
    {
    }

    public DbSet<Candidate> Candidates => Set<Candidate>();
    public DbSet<Job> Jobs => Set<Job>();
    public DbSet<JobApplication> Applications => Set<JobApplication>();
    public DbSet<ApplicationNote> ApplicationNotes => Set<ApplicationNote>();
    public DbSet<ApplicationAuditLog> ApplicationAuditLogs => Set<ApplicationAuditLog>();
    public DbSet<ApplicationEvent> ApplicationEvents => Set<ApplicationEvent>();
    public DbSet<Recruiter> Recruiters => Set<Recruiter>();
    public DbSet<JobApplicationAssignment> JobApplicationAssignments => Set<JobApplicationAssignment>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Candidate
        modelBuilder.Entity<Candidate>()
            .HasKey(c => c.Id);

        // Job
        modelBuilder.Entity<Job>()
            .HasKey(j => j.Id);

        // JobApplication
        modelBuilder.Entity<JobApplication>()
            .HasKey(a => a.Id);
        modelBuilder.Entity<JobApplication>()
            .Property(a => a.RowVersion)
            .IsRowVersion();

        // ApplicationNote
        modelBuilder.Entity<ApplicationNote>()
            .HasKey(n => n.Id);

        modelBuilder.Entity<ApplicationNote>()
            .HasOne(n => n.JobApplication)
            .WithMany(a => a.Notes)
            .HasForeignKey(n => n.JobApplicationId)
            .OnDelete(DeleteBehavior.Cascade);

        // ApplicationAuditLog
        modelBuilder.Entity<ApplicationAuditLog>()
            .HasKey(a => a.Id);

        modelBuilder.Entity<ApplicationAuditLog>()
            .HasOne(a => a.JobApplication)
            .WithMany() // audit logs not navigated yet
            .HasForeignKey(a => a.ApplicationId)
            .OnDelete(DeleteBehavior.Cascade);

        // ApplicationEvent
        modelBuilder.Entity<ApplicationEvent>()
            .HasKey(e => e.Id);

        modelBuilder.Entity<ApplicationEvent>()
            .HasOne(e => e.JobApplication)
            .WithMany() // events not navigated yet
            .HasForeignKey(e => e.ApplicationId)
            .OnDelete(DeleteBehavior.Cascade);

        // Recruiter
        modelBuilder.Entity<Recruiter>()
            .HasKey(r => r.Id);

        // JobApplicationAssignment
        modelBuilder.Entity<JobApplicationAssignment>()
            .HasKey(a => a.Id);

        modelBuilder.Entity<JobApplicationAssignment>()
            .HasOne(a => a.JobApplication)
            .WithMany(a => a.Assignments)
            .HasForeignKey(a => a.JobApplicationId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<JobApplicationAssignment>()
            .HasOne(a => a.Recruiter)
            .WithMany() // recruiters don't navigate to assignments
            .HasForeignKey(a => a.RecruiterId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
