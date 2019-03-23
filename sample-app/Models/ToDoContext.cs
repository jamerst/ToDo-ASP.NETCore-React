using System.Data;
using Microsoft.EntityFrameworkCore;

namespace sample_app.Models {
    public class ToDoContext : DbContext {
        public ToDoContext(DbContextOptions<ToDoContext> options) : base(options) {}

        public DbSet<User> users {get; set;}
        public DbSet<ToDoList> lists { get; set; }
        public DbSet<Item> items {get; set;}

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            // make lists and items cascade on delete
            modelBuilder.Entity<User>()
                .HasMany(u => u.lists)
                .WithOne(l => l.user)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ToDoList>()
                .HasMany(l => l.items)
                .WithOne(i => i.list)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}