using Microsoft.AspNetCore.Mvc.ModelBinding;
using social_oc_api.Models.Errors;

namespace social_oc_api.Utils
{
    public interface IUtils
    {
        public ErrorResponse BuildErrorResponse(ModelStateDictionary modelState);

        public void ValidateFileUpload(List<IFormFile> files, ModelStateDictionary modelState);

        public void DeleteImageFromFolder(string imageName);
    }
}
