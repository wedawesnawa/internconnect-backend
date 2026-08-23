using InternconnectBackend.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace InternconnectBackend.Data
{
    public class InternconnectDbContext : DbContext
    {
        public InternconnectDbContext(DbContextOptions dbContextOptions) : base(dbContextOptions)
        {
            
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Logbook> Logbooks { get; set; }
        public DbSet<UserDetail> UserDetails { get; set; }
        public DbSet<LogbookDetail> LogbookDetails { get; set; }
        public DbSet<LogbookShared> LogbookShareds { get; set; }
        public DbSet<Monev> Monevs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasMany<Logbook>()
                .WithOne(l => l.User)
                .HasForeignKey(l => l.Username);

            base.OnModelCreating(modelBuilder);
        }
        public void UpdateLogbookProgress(Guid kodeLogbook)
        {
            var logbook = Logbooks.FirstOrDefault(l => l.KodeLogbook == kodeLogbook);
            if (logbook != null)
            {
                logbook.TotalDateRange = (logbook.DateEnd - logbook.DateStart).Days;
                logbook.TotalLogbookDetails = LogbookDetails.Count(d => d.KodeLogbook == kodeLogbook);
                SaveChanges();
            }
        }
    }
}
