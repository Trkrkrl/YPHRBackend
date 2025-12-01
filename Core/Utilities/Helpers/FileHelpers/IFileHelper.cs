using Core.Utilities.Results;
using Microsoft.AspNetCore.Http;

namespace Core.Utilities.Helpers.FileHelpers
{
    public interface IFileHelper
    {
        Task<IDataResult<string>> UploadAsync(IFormFile file, string root);
        Task<IResult> DeleteAsync(string filePath);
        Task<IDataResult<string>> UpdateAsync(IFormFile file, string filePath, string root);
    }
}
