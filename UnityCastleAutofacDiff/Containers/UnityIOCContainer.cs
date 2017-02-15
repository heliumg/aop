using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;
using Castle.MicroKernel.Lifestyle;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.InterceptionExtension;
using UnityCastleAutofacDiff.Interceptors;

namespace UnityCastleAutofacDiff.Containers
{
    public class UnityIOCContainer : IIOCContainer
    {
        private readonly IUnityContainer _container = new UnityContainer();

        public UnityIOCContainer()
        {
            _container.AddNewExtension<Interception>();
        }

        public string Name => "Unity";
        public T Resolve<T>(object args) where T : class
        {
            //new ResolverOverride[]
            //                       {
            //                           new ParameterOverride("userd", "data"), 
            //                             new ParameterOverride("number", 21)
            //  
            IDictionary<string, object> expando = new ExpandoObject();
            foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(args.GetType()))
                expando.Add(property.Name, property.GetValue(args));

            var newArgs = expando.Keys.Select(k => new ParameterOverride(k, expando[k])).ToArray();
            return _container.Resolve<T>(newArgs);
        }

        public void SetupForTransientTest()
        {
            _container.RegisterType<IController, UserController>(
                 new TransientLifetimeManager(), 
                 new Interceptor<InterfaceInterceptor>(),
                 new InterceptionBehavior<LoggingInterceptorUnity>());
        }

        public void SetupForSingletonTest()
        {
            _container.RegisterType<IController, UserController>(
                 new ContainerControlledLifetimeManager(),
                 new Interceptor<InterfaceInterceptor>(),
                 new InterceptionBehavior<LoggingInterceptorUnity>());
        }
    }
}
