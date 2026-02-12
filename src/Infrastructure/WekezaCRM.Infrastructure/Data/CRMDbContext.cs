using Microsoft.EntityFrameworkCore;
using WekezaCRM.Domain.Entities;

namespace WekezaCRM.Infrastructure.Data;

public class CRMDbContext : DbContext
{
    public CRMDbContext(DbContextOptions<CRMDbContext> options) : base(options)
    {
    }

    public DbSet<Customer> Customers { get; set; }
    public DbSet<Account> Accounts { get; set; }
    public DbSet<Transaction> Transactions { get; set; }
    public DbSet<Case> Cases { get; set; }
    public DbSet<CaseNote> CaseNotes { get; set; }
    public DbSet<Interaction> Interactions { get; set; }
    public DbSet<Campaign> Campaigns { get; set; }
    
    // Phase 2 entities
    public DbSet<NextBestAction> NextBestActions { get; set; }
    public DbSet<SentimentAnalysis> SentimentAnalyses { get; set; }
    public DbSet<WorkflowDefinition> WorkflowDefinitions { get; set; }
    public DbSet<WorkflowInstance> WorkflowInstances { get; set; }
    public DbSet<Notification> Notifications { get; set; }
    public DbSet<AnalyticsReport> AnalyticsReports { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Customer configuration
        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Email).IsUnique();
            entity.HasIndex(e => e.PhoneNumber);
            entity.HasIndex(e => e.CustomerReference).IsUnique();
            entity.Property(e => e.FirstName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.LastName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(255);
            entity.Property(e => e.PhoneNumber).IsRequired().HasMaxLength(20);
            entity.Property(e => e.CreditScore).HasPrecision(18, 2);
            entity.Property(e => e.LifetimeValue).HasPrecision(18, 2);
        });

        // Account configuration
        modelBuilder.Entity<Account>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.AccountNumber).IsUnique();
            entity.Property(e => e.AccountNumber).IsRequired().HasMaxLength(20);
            entity.Property(e => e.Balance).HasPrecision(18, 2);
            entity.HasOne(e => e.Customer)
                  .WithMany(c => c.Accounts)
                  .HasForeignKey(e => e.CustomerId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // Transaction configuration
        modelBuilder.Entity<Transaction>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.TransactionReference).IsUnique();
            entity.Property(e => e.Amount).HasPrecision(18, 2);
            entity.Property(e => e.BalanceAfter).HasPrecision(18, 2);
            entity.HasOne(e => e.Account)
                  .WithMany(a => a.Transactions)
                  .HasForeignKey(e => e.AccountId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // Case configuration
        modelBuilder.Entity<Case>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.CaseNumber).IsUnique();
            entity.Property(e => e.CaseNumber).IsRequired().HasMaxLength(20);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(255);
            entity.HasOne(e => e.Customer)
                  .WithMany(c => c.Cases)
                  .HasForeignKey(e => e.CustomerId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // CaseNote configuration
        modelBuilder.Entity<CaseNote>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.Case)
                  .WithMany(c => c.CaseNotes)
                  .HasForeignKey(e => e.CaseId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // Interaction configuration
        modelBuilder.Entity<Interaction>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Subject).IsRequired().HasMaxLength(255);
            entity.HasOne(e => e.Customer)
                  .WithMany(c => c.Interactions)
                  .HasForeignKey(e => e.CustomerId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // Campaign configuration
        modelBuilder.Entity<Campaign>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(255);
        });

        // NextBestAction configuration
        modelBuilder.Entity<NextBestAction>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(255);
            entity.Property(e => e.ConfidenceScore).HasPrecision(5, 2);
            entity.HasOne(e => e.Customer)
                  .WithMany()
                  .HasForeignKey(e => e.CustomerId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // SentimentAnalysis configuration
        modelBuilder.Entity<SentimentAnalysis>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.SentimentScore).HasPrecision(5, 2);
            entity.HasOne(e => e.Customer)
                  .WithMany()
                  .HasForeignKey(e => e.CustomerId)
                  .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.Interaction)
                  .WithMany()
                  .HasForeignKey(e => e.InteractionId)
                  .OnDelete(DeleteBehavior.NoAction);
            entity.HasOne(e => e.Case)
                  .WithMany()
                  .HasForeignKey(e => e.CaseId)
                  .OnDelete(DeleteBehavior.NoAction);
        });

        // WorkflowDefinition configuration
        modelBuilder.Entity<WorkflowDefinition>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(255);
            entity.Property(e => e.TriggerType).IsRequired().HasMaxLength(100);
        });

        // WorkflowInstance configuration
        modelBuilder.Entity<WorkflowInstance>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.WorkflowDefinition)
                  .WithMany(w => w.WorkflowInstances)
                  .HasForeignKey(e => e.WorkflowDefinitionId)
                  .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.Customer)
                  .WithMany()
                  .HasForeignKey(e => e.CustomerId)
                  .OnDelete(DeleteBehavior.NoAction);
            entity.HasOne(e => e.Case)
                  .WithMany()
                  .HasForeignKey(e => e.CaseId)
                  .OnDelete(DeleteBehavior.NoAction);
        });

        // Notification configuration
        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(255);
            entity.HasOne(e => e.Customer)
                  .WithMany()
                  .HasForeignKey(e => e.CustomerId)
                  .OnDelete(DeleteBehavior.SetNull);
        });

        // AnalyticsReport configuration
        modelBuilder.Entity<AnalyticsReport>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ReportName).IsRequired().HasMaxLength(255);
            entity.Property(e => e.ReportType).IsRequired().HasMaxLength(100);
        });
    }
}
