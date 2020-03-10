using System;
using Ninject.Modules;

namespace Crida
{
    public class FluentNinjectModule : NinjectModule
    {
        public FluentNinjectModule(Action<FluentNinjectModule> loadAction)
        {
            LoadAction = loadAction;
        }

        private Action<FluentNinjectModule> LoadAction { get; }

        public override void Load()
        {
            LoadAction(this);
        }

        public FluentNinjectModule BindSingleton(params Type[] types)
        {
            foreach (var type in types) Bind(type).ToSelf().InSingletonScope();
            return this;
        }

        public FluentNinjectModule BindSingleton<T>()
        {
            return BindSingleton(typeof(T));
        }

        public FluentNinjectModule BindExisting<T>(T value)
        {
            Bind<T>().ToConstant(value);
            return this;
        }
    }
}
