using System;
using System.Collections.Generic;

namespace TOSGUI.Examples.ResourceManagement
{
    public class ResourceManager : IResourceManager
    {
        private readonly Dictionary<Type, Dictionary<string, IResource>> _resourceMap = new Dictionary<Type, Dictionary<string, IResource>>();

        public void Init()
        {
        }

        public IResourceManager Load<TResource>(string name) where TResource : class, IResource
        {
            if (ResourceLoaded(typeof(TResource), name))
                return this;

            IResource resource;

            if (typeof(TResource) == typeof(ShaderResource))
                resource = ShaderResource.LoadResource(name);
            else 
                return this;

            if (!_resourceMap.ContainsKey(typeof(TResource))) _resourceMap[typeof(TResource)] = new Dictionary<string, IResource>();

            _resourceMap[typeof(TResource)][name] = resource;

            return this;
        }

        public TResource Get<TResource>(string name) where TResource : class, IResource
        {
            Dictionary<string, IResource> resourceDict;
            IResource resource;
            if (!_resourceMap.TryGetValue(typeof(TResource), out resourceDict) || !resourceDict.TryGetValue(name, out resource))
                return null;

            return resource as TResource;
        }

        public void Dispose()
        {
            foreach (var resourceDictionary in _resourceMap.Values)
            foreach (var resource in resourceDictionary.Values)
                resource.Dispose();
        }

        private bool ResourceLoaded(Type type, string name)
        {
            Dictionary<string, IResource> resourceDict;
            IResource resource;
            return _resourceMap.TryGetValue(type, out resourceDict) && resourceDict.TryGetValue(name, out resource);
        }
    }
}