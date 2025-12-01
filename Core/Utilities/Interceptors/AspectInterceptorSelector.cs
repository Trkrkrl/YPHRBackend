using System;
using System.Linq;
using System.Reflection;
using Castle.DynamicProxy;

namespace Core.Utilities.Interceptors
{
    /// <summary>
    /// Bir sınıf/metot üzerindeki tüm aspect attribute'lerini toplar,
    /// öncelik (Priority) değerine göre sıralar ve Castle DynamicProxy'e verir.
    /// </summary>
    public class AspectInterceptorSelector : IInterceptorSelector
    {
        /// <summary>
        /// Seçilen type ve method için uygulanacak interceptor listesini döner.
        /// </summary>
        public IInterceptor[] SelectInterceptors(Type type, MethodInfo method, IInterceptor[] interceptors)
        {
            var classAttributes = type
                .GetCustomAttributes<MethodInterceptionBaseAttribute>(true)
                .ToList();

            var methodAttributes = type
                .GetMethod(method.Name)!
                .GetCustomAttributes<MethodInterceptionBaseAttribute>(true);

            classAttributes.AddRange(methodAttributes);

            // Priority değeri küçük olan aspect önce çalışır.
            return classAttributes
                .OrderBy(x => x.Priority)
                .ToArray();
        }
    }
}
