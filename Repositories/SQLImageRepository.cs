using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using social_oc_api.Data;
using social_oc_api.Utils;
using social_oc_api.Models.Domain;
using Microsoft.EntityFrameworkCore;
namespace social_oc_api.Repositories
{
    public class SQLImageRepository : IImageRepository
    {

        private readonly SocialOCDBContext _dbContext;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUtils _utils;

        public SQLImageRepository(SocialOCDBContext dbContext, IWebHostEnvironment webHostEnvironment,
             IHttpContextAccessor httpContextAccessor, IUtils utils)
        {
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
            _utils = utils;
            _webHostEnvironment = webHostEnvironment;
        }
        public async Task<Image?> DeleteImage(Guid Id)
        {
            var imageDomain = await _dbContext.PostImages.FirstOrDefaultAsync(item => item.Id == Id);
            if (imageDomain == null)
            {
                return null;
            }
            _utils.DeleteImageFromFolder(imageDomain.FilePath);

            _dbContext.PostImages.Remove(imageDomain);
            await _dbContext.SaveChangesAsync();

            return imageDomain;
        }

        public async Task<Image> UploadImage(PostImage image, string type)
        {
            string unique_name = GenerateUniqueFileName();
            string extensionImage = Path.GetExtension(image.File.FileName);
            // store files in our app... images folder
            var localFilePath = Path.Combine(_webHostEnvironment.ContentRootPath, "Images", $"{unique_name}{extensionImage}");

            //Upload Image to local Path
            using var stream = new FileStream(localFilePath, FileMode.Create);
            await image.File.CopyToAsync(stream);


            // https://localhost:1234/images/image.jpg
            var urlFilePath = $"{_httpContextAccessor.HttpContext.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host}{_httpContextAccessor.HttpContext.Request.PathBase}/Images/{unique_name}{extensionImage}";
            image.FilePath = urlFilePath;
            
            // Add Image to the images table
            if (type == "PostImages")
            {
                await _dbContext.PostImages.AddAsync(image);

            }
            await _dbContext.SaveChangesAsync();

            return image;
        }
        private string GenerateUniqueFileName()
        {
            string timestamp = DateTime.Now.ToString("yyyyMMddHHmmssfff");
            return timestamp;
        }
    }
}
