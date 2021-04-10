using Microsoft.EntityFrameworkCore;
using Model.WuxiaWorldModel;

namespace Persistence.Context
{
    internal sealed class WuxiaWorldContext : DbContext
    {
        internal DbSet<Novel> Novels { get; set; }
        internal DbSet<Volume> Volumes { get; set; }
        internal DbSet<Chapter> Chapters { get; set; }

        internal static WuxiaWorldContext GetWuxiaWorldContext()
        {
            var connectionString = @"Server=localhost\SQLEXPRESS;Database=WuxiaWorldDb;Trusted_Connection=True;";
            DbContextOptionsBuilder<WuxiaWorldContext> options = new DbContextOptionsBuilder<WuxiaWorldContext>();
            options.UseSqlServer(connectionString);
            return new WuxiaWorldContext(options.Options);
        }

        private WuxiaWorldContext(DbContextOptions<WuxiaWorldContext> options) : base(options)
        {
            Database.Migrate();
        }
    }
}
