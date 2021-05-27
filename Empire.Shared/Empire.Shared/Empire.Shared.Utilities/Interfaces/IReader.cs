using System;

namespace Empire.Shared.Utilities
{
    interface IReader
    {
        string Read(string sKey);
        string[] Read(string sKey, params string[] aDelimiters);
    }
}
