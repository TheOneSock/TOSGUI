using System;

namespace TOSGUI.Examples.ResourceManagement
{
    public interface IResource : IDisposable
    {
        string Name { get; }
    }
}