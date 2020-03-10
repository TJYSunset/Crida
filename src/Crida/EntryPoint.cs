using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Crida.Asset;
using Crida.Shared.Serialization;
using Crida.States;
using MessagePack;
using MessagePack.ImmutableCollection;
using MessagePack.Resolvers;
using Ninject;
using NLog;

namespace Crida
{
    public static class EntryPoint
    {
        private static readonly ILogger Log = LogManager.GetCurrentClassLogger();

        public static async Task Main(string[] args)
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                Log.Fatal("OSes other than Windows aren't supported yet. Sorry!");
                return;
            }

            if (!BitConverter.IsLittleEndian)
            {
                Log.Fatal("Big endian systems aren't supported yet. Sorry!");
                return;
            }

            // configure messagepack
            var resolver = CompositeResolver.Create(
                ReallyBadMathResolver.Instance,
                ImmutableCollectionResolver.Instance,
                StandardResolver.Instance
            );
            var options = MessagePackSerializerOptions.Standard.WithResolver(resolver);
            MessagePackSerializer.DefaultOptions = options;

            // configure ninject
            var ninject = new StandardKernel(new FluentNinjectModule(_ => _
                .BindSingleton(typeof(MainThread).Assembly.GetTypes()
                    .Where(x => x.IsClass)
                    .Where(x => x.GetInterfaces().Contains(typeof(IState))).ToArray())
                .BindExisting<IRawAssetLibrary>(new DirectoryRawAssetLibrary(
                    Path.Combine(Path.GetDirectoryName(typeof(MainThread).Assembly.Location)!, "assets")))
            ));

            await new MainThread().Run(ninject);
            Log.Info("Main thread quit normally");
        }
    }
}
