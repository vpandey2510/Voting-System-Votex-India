using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using VotingSystem.Models;

public class ApplicationDbContext : IdentityDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Area> Areas { get; set; }
    public DbSet<Candidate> Candidates { get; set; }
    public DbSet<Election> Elections { get; set; }
    public DbSet<Party> Parties { get; set; }
    public DbSet<Voter> Voters { get; set; }
    public DbSet<Vote> Votes { get; set; }
    public DbSet<Feedback> Feedbacks { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Unique constraint on Voter Username
        modelBuilder.Entity<Voter>()
            .HasIndex(v => v.Username)
            .IsUnique();

        // Candidate to Election: Required relationship
        modelBuilder.Entity<Candidate>()
            .HasOne(c => c.Election)
            .WithMany()
            .HasForeignKey(c => c.ElectionID)
            .OnDelete(DeleteBehavior.Restrict);

        // Party to Candidates: One-to-many relationship
        modelBuilder.Entity<Candidate>()
            .HasOne(c => c.Party)
            .WithMany(p => p.Candidates)
            .HasForeignKey(c => c.PartyID)
            .OnDelete(DeleteBehavior.Restrict);

        // Candidate to Area: One-to-many relationship
        modelBuilder.Entity<Candidate>()
            .HasOne(c => c.Area)
            .WithMany(a => a.Candidates)
            .HasForeignKey(c => c.AreaID)
            .OnDelete(DeleteBehavior.Restrict);

        // Vote to Candidate: Foreign key relationship with CandidateID
        modelBuilder.Entity<Vote>()
            .HasOne(v => v.Candidate)
            .WithMany()
            .HasForeignKey(v => v.CandidateID)
            .OnDelete(DeleteBehavior.Restrict);

        // Vote to Election: Foreign key relationship with ElectionID
        modelBuilder.Entity<Vote>()
            .HasOne(v => v.Election)
            .WithMany()
            .HasForeignKey(v => v.ElectionID)
            .OnDelete(DeleteBehavior.Restrict);

        // Composite unique constraint to ensure only one vote per VoterHash per ElectionID
        modelBuilder.Entity<Vote>()
            .HasIndex(v => new { v.VoterHashID, v.ElectionID })
            .IsUnique();
    }
}
