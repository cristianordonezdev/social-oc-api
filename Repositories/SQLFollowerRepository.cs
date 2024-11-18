
using Microsoft.EntityFrameworkCore;
using social_oc_api.Data;
using social_oc_api.Models.Domain;

namespace social_oc_api.Repositories
{
    public class SQLFollowerRepository : IFollowerRepository
    {
        private readonly SocialOCDBContext _dbContext;
        public SQLFollowerRepository(SocialOCDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Boolean> GetVisibility(string userId, string ownUserId)
        {
            var areYouFollowing = await _dbContext.Followers.AnyAsync(f => f.FollowerId == ownUserId && f.FollowingId == userId);
            return areYouFollowing || (ownUserId == userId);
        }

        public async Task<Follower?> ToggleFollowAction(Follower follower)
        {
            var followAction = await _dbContext.Followers.FirstOrDefaultAsync(i => i.FollowingId.Equals(follower.FollowingId) && i.FollowerId.Equals(follower.FollowerId));

            // Start follow
            if (followAction == null)
            {
                await _dbContext.Followers.AddAsync(follower);
                await _dbContext.SaveChangesAsync();
                return follower;
            } else
            // Unfollow action
            {
                _dbContext.Followers.Remove(followAction);
                await _dbContext.SaveChangesAsync();
                return null;
            }
        }
    }
}
