using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MessagePack;

namespace Crida.Asset
{
    public class DirectoryRawAssetLibrary : IRawAssetLibrary
    {
        public DirectoryRawAssetLibrary(string libraryDirectory)
        {
            LibraryDirectory = libraryDirectory;
        }

        private string LibraryDirectory { get; }

        public object Load(Type type, string id)
        {
            // todo unit test: ensure invocation only uses IRawAsset
            var path = Path.Combine(LibraryDirectory, id + "." + type.Name.Substring(3));
            return MessagePackSerializer.Deserialize(type, File.ReadAllBytes(path));
        }

        public IEnumerable<(string id, string extension)> List()
        {
            return Directory.GetFiles(LibraryDirectory).Select(file =>
                (Path.GetFileNameWithoutExtension(file), Path.GetExtension(file)));
        }
    }
}
