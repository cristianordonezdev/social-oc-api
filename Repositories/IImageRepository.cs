using social_oc_api.Models.Domain.Images;

namespace social_oc_api.Repositories
{
    public interface IImageRepository
    {
        Task<Image?> DeleteImage(Guid Id);
        Task<Image> UploadImage<T>(T image, string type) where T : Image;
    }
}
