using Microsoft.EntityFrameworkCore;

namespace sample_app.Models {
    public class ToDoContext : DbContext {
        public ToDoContext(DbContextOptions<ToDoContext> options) : base(options) {}

        public DbSet<User> users {get; set;}
        public DbSet<ToDoList> lists { get; set; }
        public DbSet<Item> items {get; set;}
    }
}