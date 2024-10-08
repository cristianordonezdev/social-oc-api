using social_oc_api.Models.Domain;

namespace social_oc_api.Repositories
{
    public interface IImageRepository
    {
        Task<Image?> DeleteImage(Guid Id);
        Task<Image> UploadImage(PostImage image, string type);
    }
}
