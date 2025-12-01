using System.IO;
using Core.Utilities.Results;
using Microsoft.AspNetCore.Http;
using Core.Utilities.Messages;


namespace Core.Utilities.Helpers.FileHelpers
{
    public class FileHelperManager : IFileHelper
    {

        public async Task<IResult> DeleteAsync(string filePath)
        {
           
                if (string.IsNullOrEmpty(filePath))
                {
                    return new SuccessResult();
                }

                if (File.Exists(filePath))
                {
                    try
                    {
                        File.Delete(filePath);
                        return new SuccessResult(AspectMessages.FileDeleted);
                    }
                    catch (Exception ex)
                    {

                        return new ErrorResult($"{AspectMessages.FileDeleteFailed}: {ex.Message}");
                    }
                }

                return new ErrorResult(AspectMessages.FileNotFound);
           
        }

        public async Task<IDataResult<string>> UploadAsync(IFormFile file, string root)
        {
            if (file == null || file.Length == 0)
            {
                return new ErrorDataResult<string>(AspectMessages.FileEmpty);
            }

            try
            {
                if (!Directory.Exists(root))
                {
                    Directory.CreateDirectory(root);
                }

                string extension = Path.GetExtension(file.FileName);
                string guid = GuidHelper.CreateGuid(); 
                string fileName = guid + extension;
                string filePath = Path.Combine(root, fileName);

                using (FileStream fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None, 4096, useAsync: true))
                {
                    await file.CopyToAsync(fileStream);

                    return new SuccessDataResult<string>(filePath, AspectMessages.FileUploaded);
                }
            }
            catch (Exception ex)
            {
                return new ErrorDataResult<string>(null, $"{AspectMessages.FileUploadFailed}: {ex.Message}");
            }
        }

        public async Task<IDataResult<string>> UpdateAsync(IFormFile file, string oldFilePath, string root)
        {
            if (!string.IsNullOrEmpty(oldFilePath))
            {
                var deleteResult = await DeleteAsync(oldFilePath);
                if (!deleteResult.Success && oldFilePath != null) 
                {
                    return new ErrorDataResult<string>(null, $"{AspectMessages.FileUpdateFailed} - {deleteResult.Message}");
                }
            }

            return await UploadAsync(file, root);
        }
    }
}
