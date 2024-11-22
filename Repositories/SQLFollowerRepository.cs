
using Microsoft.EntityFrameworkCore;
using social_oc_api.Data;
using social_oc_api.Models.Domain;
using social_oc_api.Models.DTO.Followers;
using social_oc_api.Models.DTO.Posts;

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
            }
            else
            {
                _dbContext.Followers.Remove(followAction);
                await _dbContext.SaveChangesAsync();
                return null;
            }
        }

        public async Task<bool> HandleGetRequest(Follower follower)
        {

            var followAction = await _dbContext.RequestFollowers.FirstOrDefaultAsync(i => i.FollowingId.Equals(follower.FollowingId) && i.FollowerId.Equals(follower.FollowerId));

            if (followAction == null)
            {
                RequestFollower requestFollower = new RequestFollower
                {
                    Id = follower.Id,
                    FollowerId = follower.FollowerId,
                    FollowingId = follower.FollowingId,
                    CreatedAt = follower.CreatedAt
                };
                await _dbContext.RequestFollowers.AddAsync(requestFollower);
                await _dbContext.SaveChangesAsync();
                return true;
            } else
            {
                _dbContext.RequestFollowers.Remove(followAction);
                await _dbContext.SaveChangesAsync();
                return false;
            }
        }

        public async Task<List<RequestFollowerDto>> GetListOfRequests(string userId, int page, int pageSize)
        {
            int skip = (page - 1) * pageSize;

            var requests = await _dbContext.RequestFollowers
                .Where(i => i.FollowingId == userId)
                .Skip(skip)
                .Take(pageSize)
                .Include(i => i.FollowerUser)
                .Select(req => new RequestFollowerDto
                {
                    Id = req.Id,
                    FollowerId = req.FollowerId,
                    FollowingId = req.FollowingId,
                    CreatedAt = req.CreatedAt,
                    User = new PostHomeUser
                    {
                        Username = req.FollowerUser.UserName,
                        ImageProfile = req.FollowerUser.ImageProfile.FilePath
                    }
                })
                .ToListAsync();
            return requests;
        }

        public async Task<bool?> deleteRequest(Guid requestId, string userId)
        {
            var requestDomain = await _dbContext.RequestFollowers.FirstOrDefaultAsync(f => f.Id == requestId && f.FollowingId == userId);
            if (requestDomain == null) { return null; }

            _dbContext.Remove(requestDomain);
            await _dbContext.SaveChangesAsync();

            return true;
        }

        public async Task<Follower?> acceptRequest(Guid requestId, string userId)
        {
            var requestDomain = await _dbContext.RequestFollowers.FirstOrDefaultAsync(f => f.Id == requestId && f.FollowingId == userId);
            if (requestDomain == null) { return null; }

            var follower = new Follower
            {
                FollowerId = requestDomain.FollowerId,
                FollowingId = userId,
            };

            var followAction = await ToggleFollowAction(follower);

            _dbContext.Remove(requestDomain);
            await _dbContext.SaveChangesAsync();

            return followAction;
        }
    }
}
