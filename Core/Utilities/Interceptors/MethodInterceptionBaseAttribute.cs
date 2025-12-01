using System;
using Castle.DynamicProxy;

namespace Core.Utilities.Interceptors
{
    /// <summary>
    /// Tüm method aspect'lerinin miras aldığı temel attribute.
    /// Priority ile çalışma sırası belirlenir, Intercept metodu override edilir.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public abstract class MethodInterceptionBaseAttribute : Attribute, IInterceptor
    {
        /// <summary>
        /// Aspect'in çalışma sırası. Küçük değer önce çalışır.
        /// </summary>
        public int Priority { get; set; }

        /// <summary>
        /// Aspect'in gövdesini tutan metot. Alt sınıflar tarafından override edilir.
        /// </summary>
        public virtual void Intercept(IInvocation invocation)
        {
            // Varsayılan davranış: hiçbir şey yapma, direkt metodu çalıştır (alt sınıfta implemente edilir).
        }
    }
}
