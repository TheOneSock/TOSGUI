using System;

namespace TOSGUI.Examples.ResourceManagement
{
    public interface IResourceManager : IDisposable
    {
        IResourceManager Load<TResource>(string name) where TResource : class, IResource;
        TResource Get<TResource>(string name) where TResource : class, IResource;
    }
}