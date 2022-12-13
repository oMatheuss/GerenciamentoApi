using GerenciamentoAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace GerenciamentoAPI.Repository
{
    public class GerenciamentoDB : DbContext
    {
        public virtual DbSet<User> Users => Set<User>();
        public virtual DbSet<Activity> Activities => Set<Activity>();
        public GerenciamentoDB(DbContextOptions<GerenciamentoDB> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(e =>
            {
                e.HasMany(user => user.Activities)
                .WithMany(activity => activity.Employees);
            });
        }
    }
}
