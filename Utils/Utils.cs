using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using social_oc_api.Models.Errors;

namespace social_oc_api.Utils
{
    public class Utils : IUtils

    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        public Utils(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        public ErrorResponse BuildErrorResponse(ModelStateDictionary modelState)
        {
            var errorResponse = new ErrorResponse
            {
                Status = 400,
                Title = "One or more validation errors occurred.",
                Detail = "Please refer to the errors property for additional details.",
                Errors = modelState
                    .Where(x => x.Value.Errors.Count > 0)
                    .ToDictionary(
                        kvp => kvp.Key,
                        kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                    )
            };
            return errorResponse;
        }

        public void DeleteImageFromFolder(string imageName)
        {
            string[] segments = imageName.Split('/');
            string folderPath = Path.Combine(_webHostEnvironment.ContentRootPath, "Images");
            string filePath = Path.Combine(folderPath, segments[^1]);

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }
        public void ValidateFileUpload(List<IFormFile> files, ModelStateDictionary modelState)
        {
            var allowedExtension = new string[] { ".jpg", ".jpeg", ".png", ".mp4" };

            foreach (var file in files)
            {
                if (!allowedExtension.Contains(Path.GetExtension(file.FileName)))
                {
                    modelState.AddModelError("file", "Unsupported file extension");
                }

                if (file.Length > 10485760)
                {
                    modelState.AddModelError("File", "File size more than 10MB, please upload a smaller file");
                }
            }
        }
    }

}
