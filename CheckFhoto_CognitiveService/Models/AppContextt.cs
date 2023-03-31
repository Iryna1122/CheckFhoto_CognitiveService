using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using Microsoft.EntityFrameworkCore;

namespace CheckFhoto_CognitiveService.Models
{
    public class AppContextt:DbContext
    {
        public DbSet<Picture> Pictures { get; set; }

        

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Picture>().ToContainer("Picture");
           
        }

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    optionsBuilder.UseCosmos(
        //      "AccountEndpoint=https://oliinykstore.documents.azure.com:443/;" +
        //      "AccountKey=7PGmhxtUiLnl9fr4h3lvWod3Lk3Jj0CynvfbDWzMfF8YlgL3MmEj4YbMBlrG8wlmCCaDMQPxYBgGACDbcArwHQ==;",
        //      "MyPhoto");
        //}

        public async Task AddImage(IFormFile image)
        {
            await this.Pictures.AddAsync(new Picture()
            {
                Name = image.FileName,
                Path = $"img/{image.FileName}"
            });

            await this.SaveChangesAsync();
        }

        public AppContextt(DbContextOptions<AppContextt> options) : base(options)
        {
            Database.EnsureCreated();

        }
    }
}
