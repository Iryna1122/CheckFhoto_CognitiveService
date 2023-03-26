using Microsoft.EntityFrameworkCore;

namespace CheckFhoto_CognitiveService.Models
{
    public class AppContextt:DbContext
    {
        public DbSet<Picture> Pictures { get; set; }

        public async Task AddImage(IFormFile image)
        {
            await this.Pictures.AddAsync(new Picture()
            {
                Name = image.FileName,
                Path = $"img/{image.FileName}"
            });

            await this.SaveChangesAsync();
        }

        public AppContextt(DbContextOptions<AppContextt> options)
            : base(options)
        { 
        Database.EnsureCreated();
        
        }
    }
}
