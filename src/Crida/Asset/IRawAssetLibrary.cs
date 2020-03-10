using System;
using System.Collections.Generic;

namespace Crida.Asset
{
    public interface IRawAssetLibrary
    {
        public object Load(Type type, string id);
        public IEnumerable<(string id, string extension)> List();
    }
}
