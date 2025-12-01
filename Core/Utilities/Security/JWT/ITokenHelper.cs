using System.Collections.Generic;
using Core.Entities.Concrete;

namespace Core.Utilities.Security.JWT
{
    public interface ITokenHelper
    {
        /// <summary>
        /// Verilen kullanıcı ve yetki listesi için JWT access token oluşturur.
        /// </summary>
        AccessToken CreateToken(User user, List<OperationClaim> operationClaims);
    }
}
