using System;
using System.Collections.Concurrent;
using System.Linq;
using Crida.Asset;
using Crida.Shared.RawAsset;
using NLog;

namespace Crida.States
{
    public class AssetFactoryState : IState
    {
        public AssetFactoryState(GraphicsState graphics, IRawAssetLibrary rawAssetLibrary)
        {
            Graphics = graphics;
            RawAssetLibrary = rawAssetLibrary;

            var rawTypeLut = typeof(IRawAsset).Assembly.ExportedTypes
                .Where(x => x.IsClass && x.GetInterfaces().Contains(typeof(IRawAsset)))
                .ToDictionary(x => x.Name.ToLowerInvariant());
            var typeLut = typeof(IManagedAsset).Assembly.ExportedTypes.Where(x =>
                    x.IsClass && x.GetInterfaces().Contains(typeof(IManagedAsset)))
                .ToDictionary(x => "." + x.Name.ToLowerInvariant(),
                    x => ((Type managed, Type raw)) (x, rawTypeLut[$"raw{x.Name.ToLowerInvariant()}"]));
            foreach (var (id, extension) in rawAssetLibrary.List())
            {
                if (!typeLut.TryGetValue(extension, out var outVar)) continue;
                var (managedType, rawType) = outVar;
                LoadTasks[(managedType, id)] = new Lazy<IManagedAsset>(() => (IManagedAsset)
                    Activator.CreateInstance(managedType, this, RawAssetLibrary.Load(rawType, id))!);
            }
        }

        public GraphicsState Graphics { get; }

        private static ILogger Log { get; } = LogManager.GetCurrentClassLogger();

        private IRawAssetLibrary RawAssetLibrary { get; }

        public ConcurrentDictionary<(Type type, string id), Lazy<IManagedAsset>> LoadTasks { get; } =
            new ConcurrentDictionary<(Type type, string id), Lazy<IManagedAsset>>();

        public void Dispose()
        {
            foreach (var task in LoadTasks.Values)
                if (task.IsValueCreated)
                    try
                    {
                        task.Value.Dispose();
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex, "error disposing asset of type {type}", task.Value.GetType());
                    }
        }

        public T Get<T>(string id) where T : IManagedAsset
        {
            return (T) LoadTasks[(typeof(T), id)].Value;
        }
    }
}
