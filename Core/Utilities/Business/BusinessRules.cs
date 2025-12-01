using System.Collections.Generic;
using Core.Utilities.Results;

namespace Core.Utilities.Business
{
    public static class BusinessRules
    {
        public static IResult? Run(params IResult[] logics)
        {
            foreach (var logic in logics)
            {
                if (!logic.Success)
                {
                    // Parametre ile gönderilen iş kurallarında başarısız olanı geri döndür.
                    return logic;
                }
            }

            // Tüm kurallar başarılı ise null dön (hata yok).
            return null;
        }
    }
}
