using Microsoft.EntityFrameworkCore;
using HelloQuery.Models;

namespace HelloQuery.Data
{
    public class HelloQueryContext : DbContext
    {
        public HelloQueryContext(DbContextOptions<HelloQueryContext> options)
            : base(options)
        {
        }

        public DbSet<HelloQuery.Models.Lesson> Lesson { get; set; } = default!;
        public DbSet<HelloQuery.Models.User> User { get; set; } = default!;
        public DbSet<HelloQuery.Models.UserLesson> UserLesson { get; set; } = default!;
        public DbSet<HelloQuery.Models.Book> Book { get; set; } = default!;
    }
}
