
using Microsoft.EntityFrameworkCore;
using social_oc_api.Data;
using social_oc_api.Models.Domain;
using social_oc_api.Models.Domain.Images;

namespace social_oc_api.Repositories.User
{
    public class SQLUserRepository : IUserRepository
    {
        private readonly SocialOCDBContext _dbContext;
        private readonly IImageRepository _imageRepository;

        public SQLUserRepository(SocialOCDBContext dbContext, IImageRepository imageRepository)
        {
            _dbContext = dbContext;
            _imageRepository = imageRepository;
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
            var imageDomain = await _imageRepository.UploadImage(userImage, "UserImages");
            var userDomain = await _dbContext.Users.Include(i => i.ImageProfile).FirstOrDefaultAsync(u => u.Id == userImage.UserId);

            return userDomain;
        }
    }
}
