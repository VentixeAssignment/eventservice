using Microsoft.EntityFrameworkCore;
using WebApi.Entities;

namespace WebApi.Data
{
    public class DataContext(DbContextOptions<DataContext> options) : DbContext(options)
    {
        public DbSet<CategoryEntity> Categories { get; set; }
        public DbSet<EventEntity> Events { get; set; }
        public DbSet<EventsCategoriesEntity> EventsCategories { get; set; }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<EventsCategoriesEntity>()
                .HasKey(ec => new {ec.EventId, ec.CategoryId});

            modelBuilder.Entity<EventsCategoriesEntity>()
                .HasOne(ec => ec.Event)
                .WithMany(e => e.EventsCategories)
                .HasForeignKey(ec => ec.EventId);

            modelBuilder.Entity<EventsCategoriesEntity>()
                .HasOne(ec => ec.Category)
                .WithMany(c => c.EventsCategories)
                .HasForeignKey(ec => ec.CategoryId);

            base.OnModelCreating(modelBuilder);
        }
    }
}
