using ApplicationServer.Models;
using Microsoft.EntityFrameworkCore;

namespace ApplicationServer
{
    //TODO: auto migrations
    public class CoreDbContext: DbContext
    {
        public readonly DbSet<FileDataModel> files;
        public readonly DbSet<SharingDataModel> sharing;
        
        public CoreDbContext(DbContextOptions<CoreDbContext> options):base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<FileDataModel>().HasKey(x => x.URL);
            modelBuilder.Entity<SharingDataModel>().HasKey(x => new { x.URL, x.TaggedUsername });
        }
    }
}