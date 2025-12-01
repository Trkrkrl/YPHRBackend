using Autofac;
using Autofac.Extras.DynamicProxy;
using Castle.DynamicProxy;
using Business.Abstract;
using Business.Concrete;
using Core.Utilities.Interceptors;
using Core.Utilities.Security.JWT;
using Core.Utilities.Helpers.FileHelpers;
using DataAccess.Abstract;
using DataAccess.Concrete.EntityFramework;

namespace Business.DependencyResolver.Autofac
{
    public class AutofacBusinessModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<DepartmentManager>().As<IDepartmentService>().SingleInstance();
            builder.RegisterType<EfDepartmentDal>().As<IDepartmentDal>().SingleInstance();

            builder.RegisterType<UserManager>().As<IUserService>().SingleInstance();
            builder.RegisterType<EfUserDal>().As<IUserDal>().SingleInstance();

            builder.RegisterType<EmployeeManager>().As<IEmployeeService>().SingleInstance();
            builder.RegisterType<EfEmployeeDal>().As<IEmployeeDal>().SingleInstance();

            builder.RegisterType<TitleManager>().As<ITitleService>().SingleInstance();
            builder.RegisterType<EfTitleDal>().As<ITitleDal>().SingleInstance();

            // Ortak servis kayıtları (isteğe göre uyarlayın)
            builder.RegisterType<FileHelperManager>().As<IFileHelper>().SingleInstance();

            builder.RegisterType<AuthManager>().As<IAuthService>();
            builder.RegisterType<JWTHelper>().As<ITokenHelper>();

            var assembly = System.Reflection.Assembly.GetExecutingAssembly();

            builder.RegisterAssemblyTypes(assembly).AsImplementedInterfaces()
                .EnableInterfaceInterceptors(new ProxyGenerationOptions
                {
                    Selector = new AspectInterceptorSelector()
                }).SingleInstance();
        }
    }
}
