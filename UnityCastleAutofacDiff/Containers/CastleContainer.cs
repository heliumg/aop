using System.Collections;
using Castle.Core;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using UnityCastleAutofacDiff.Interceptors;

namespace UnityCastleAutofacDiff.Containers
{
    public class CastleContainer : IIOCContainer
    {
        readonly WindsorContainer _container = new WindsorContainer();
        public string Name => "Castle Windsor";

        public T Resolve<T>(object args) where T : class
        {
            //args is anonymous object
            return _container.Resolve<T>(args);
        }

        public void SetupForTransientTest()
        {
            _container.Register(Component.For<IController>().ImplementedBy<UserController>().LifestyleTransient().Interceptors(InterceptorReference.ForType<LoggingInterceptorCastle>()).Anywhere);
            _container.Register(Component.For<LoggingInterceptorCastle>().LifestyleSingleton());
        }

        public void SetupForSingletonTest()
        {
            _container.Register(Component.For<IController>().ImplementedBy<UserController>().LifestyleSingleton().Interceptors(InterceptorReference.ForType<LoggingInterceptorCastle>()).Anywhere);
            _container.Register(Component.For<LoggingInterceptorCastle>().LifestyleSingleton());

        }
    }
}
