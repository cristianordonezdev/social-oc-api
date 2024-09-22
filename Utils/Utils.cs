using Microsoft.AspNetCore.Mvc.ModelBinding;
using social_oc_api.Models.Errors;

namespace social_oc_api.Utils
{
    public class Utils : IUtils

    {
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
    }

}
