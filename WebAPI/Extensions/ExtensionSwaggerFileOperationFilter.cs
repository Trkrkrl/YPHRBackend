using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace WebAPI.Extensions
{
    
        public class ExtensionSwaggerFileOperationFilter : IOperationFilter
        {
            public void Apply(OpenApiOperation operation, OperationFilterContext context)
            {
                var fileUploadMime = "multipart/form-data";

                // Eğer RequestBody boşsa veya multipart/form-data içermiyorsa işlemi durdur.
                if (operation.RequestBody == null || !operation.RequestBody.Content.Any(x => x.Key.Equals(fileUploadMime, StringComparison.InvariantCultureIgnoreCase)))
                    return;

                // IFormFile tipindeki parametreleri bul
                var fileParams = context.MethodInfo.GetParameters().Where(p => p.ParameterType == typeof(IFormFile));

                // RequestBody şemasını IFormFile'lar için "type: string, format: binary" olarak yeniden tanımla.
                // Bu, Swagger UI'da dosya yükleme düğmesini gösterir.
                operation.RequestBody.Content[fileUploadMime].Schema.Properties =
                    fileParams.ToDictionary(k => k.Name, v => new OpenApiSchema()
                    {
                        Type = "string",
                        Format = "binary"
                    });
            }
        }
    }