using ESPRESSO.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

public class AppContext : DbContext
    {
    public DbSet<UrlData> PageCounters { get; set; }
    public AppContext(DbContextOptions<AppContext> options) : base(options)
    {
    }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<UrlData>().ToTable("pagecounter").HasKey(u => u.ID);
            modelBuilder.Entity<UrlData>().Property(u => u.Url).IsRequired();
            modelBuilder.Entity<UrlData>().Property(u => u.COUNT).IsRequired();
            modelBuilder.Entity<UrlData>().Property(u => u.STREAM_NAME);
            modelBuilder.Entity<UrlData>().Property(u => u.EMAIL);
            modelBuilder.Entity<UrlData>().Property(u => u.LAST_UPDATED_DATE_AND_TIME) ;

    }
    // DbSet properties representing your database tables go here
}

