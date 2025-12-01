using System;
using Castle.DynamicProxy;

namespace Core.Utilities.Interceptors
{
    /// <summary>
    /// Before / After / OnException / OnSuccess hook'larını sağlayan
    /// ve Intercept içinde hepsini yöneten temel aspect sınıfı.
    /// CacheAspect, ValidationAspect, LogAspect vb. hepsi buradan türetilir.
    /// </summary>
    public abstract class MethodInterception : MethodInterceptionBaseAttribute
    {
        /// <summary>
        /// Metot çalışmadan hemen önce çalışacak hook.
        /// </summary>
        protected virtual void OnBefore(IInvocation invocation) { }

        /// <summary>
        /// Metot çalıştıktan sonra (başarılı/başarısız) her durumda çalışacak hook.
        /// </summary>
        protected virtual void OnAfter(IInvocation invocation) { }

        /// <summary>
        /// Metot içinde exception fırlatıldığında çalışacak hook.
        /// </summary>
        protected virtual void OnException(IInvocation invocation, System.Exception e) { }

        /// <summary>
        /// Metot hatasız tamamlandığında çalışacak hook.
        /// </summary>
        protected virtual void OnSuccess(IInvocation invocation) { }

        /// <summary>
        /// Castle DynamicProxy tarafından çağrılan ana interception akışı.
        /// Sırasıyla OnBefore -> Proceed -> OnSuccess/OnException -> OnAfter çalışır.
        /// </summary>
        public override void Intercept(IInvocation invocation)
        {
            var isSuccess = true;
            OnBefore(invocation);
            try
            {
                invocation.Proceed();
            }
            catch (System.Exception e)
            {
                isSuccess = false;
                OnException(invocation, e);
                throw;
            }
            finally
            {
                if (isSuccess)
                {
                    OnSuccess(invocation);
                }
            }
            OnAfter(invocation);
        }
    }
}
