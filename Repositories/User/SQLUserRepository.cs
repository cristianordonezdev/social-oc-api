
using Microsoft.EntityFrameworkCore;
using social_oc_api.Data;
using social_oc_api.Models.Domain;
using social_oc_api.Models.Domain.Images;
using social_oc_api.Utils;
using static System.Net.Mime.MediaTypeNames;

namespace social_oc_api.Repositories.User
{
    public class SQLUserRepository : IUserRepository
    {
        private readonly SocialOCDBContext _dbContext;
        private readonly IImageRepository _imageRepository;
        private readonly IUtils _utils;
        public SQLUserRepository(SocialOCDBContext dbContext, IImageRepository imageRepository, IUtils utils)
        {
            _dbContext = dbContext;
            _imageRepository = imageRepository;
            _utils = utils;
        }
        public async Task<bool> DeleteUser(Guid userId)
        {
            var user = await _dbContext.Users.FindAsync(userId);
            if (user == null)
            {
                return false;
            }

            _dbContext.Users.Remove(user);
            await _dbContext.SaveChangesAsync();

            return true;
        }

        public async Task<ApplicationUser> UploadImageProfile(UserImage userImage)
        {

            var existsImage = await _dbContext.UserImages.FirstOrDefaultAsync(i => i.UserId == userImage.UserId);
            if (existsImage == null)
            {
                var imageDomain = await _imageRepository.UploadImage(userImage, "UserImages");
            } else
            {
                _utils.DeleteImageFromFolder(existsImage.FilePath);
                _dbContext.UserImages.Remove(existsImage);
                await _dbContext.SaveChangesAsync();
                var imageDomain = await _imageRepository.UploadImage(userImage, "UserImages");
            }
            var userDomain = await _dbContext.Users.Include(i => i.ImageProfile).FirstOrDefaultAsync(u => u.Id == userImage.UserId);

            return userDomain;
        }
    }
}
