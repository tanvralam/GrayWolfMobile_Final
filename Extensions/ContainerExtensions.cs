using GalaSoft.MvvmLight.Ioc;
using System;

namespace GrayWolf.Extensions
{
    /// <summary>
    /// Contains container extensions that help android not to crash when recreating activity
    /// </summary>
    public static class ContainerExtensions
    {
        public static void TryRegister<TInterface, TImplementation>(this SimpleIoc container)
            where TInterface : class
            where TImplementation : class, TInterface
        {
            if (!container.IsRegistered<TInterface>())
            {
                container.Register<TInterface, TImplementation>();
            }
        }

        public static void TryRegister<TClass>(this SimpleIoc container, Func<TClass> factory)
            where TClass : class
        {
            if (!container.IsRegistered<TClass>())
            {
                container.Register(factory);
            }
        }

        public static void TryRegister<TClass>(this SimpleIoc container, Func<TClass> factory, string key)
            where TClass : class
        {
            if (!container.IsRegistered<TClass>(key))
            {
                container.Register(factory, key);
            }
        }
    }
}
