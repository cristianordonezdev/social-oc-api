
using Microsoft.EntityFrameworkCore;
using social_oc_api.Data;
using social_oc_api.Models.Domain;
using social_oc_api.Models.Domain.Images;
using social_oc_api.Models.DTO.User;
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

        public async Task<ProfileUser?> GetProfile(string username)
        {
            var userDomain = await _dbContext.Users
                .Include(u => u.Posts)
                    .ThenInclude(i => i.PostImages)
                .Include(u => u.ImageProfile)
                .FirstOrDefaultAsync(u => u.UserName == username);

                

            if (userDomain == null)
            {
                return null;
            }

            string userId = userDomain.Id;

            var following = await _dbContext.Followers.CountAsync(i => i.FollowerId == userId);
            var followers = await _dbContext.Followers.CountAsync(i => i.FollowingId == userId);
            var postCount = userDomain.Posts.Count;

            var profile = new ProfileUser
            {
                User = userDomain,
                MetricsProfile = new MetricsProfile
                {
                    PostCount = postCount,
                    Followers = followers,
                    Following = following
                }
            };

            return profile;
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
            var userDomain = await _dbContext.Users.Include(i => i.ImageProfile).FirstOrDefaultAsync(u => u.Id.Equals(userImage.UserId));

            return userDomain;
        }
    }
}
