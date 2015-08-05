using System;
using Mono.Cecil;
using System.Collections.Generic;

namespace ILRepacking
{
    public class RepackAssemblyResolver : BaseAssemblyResolver
    {

        readonly IDictionary<string, AssemblyDefinition> cache;

        public RepackAssemblyResolver()
        {
            cache = new Dictionary<string, AssemblyDefinition>(StringComparer.Ordinal);
        }

        public override AssemblyDefinition Resolve(AssemblyNameReference name)
        {
            if (name == null)
                throw new ArgumentNullException("name");

            AssemblyDefinition assembly;
            if (cache.TryGetValue(name.FullName, out assembly))
                return assembly;

            // Try to resolve again ignoring the version
            foreach (var cachedAssembly in cache)
            {
                if (cachedAssembly.Value.Name.Name == name.Name)
                {
                    cache[name.FullName] = cachedAssembly.Value;
                    return cachedAssembly.Value;
                }
            }

            assembly = base.Resolve(name);
            cache[name.FullName] = assembly;

            return assembly;
        }

        protected void RegisterAssembly(AssemblyDefinition assembly)
        {
            if (assembly == null)
                throw new ArgumentNullException("assembly");

            var name = assembly.Name.FullName;
            if (cache.ContainsKey(name))
                return;

            cache[name] = assembly;
        }

        public void RegisterAssemblies(List<AssemblyDefinition> mergedAssemblies)
        {
            foreach (var assemblyDefinition in mergedAssemblies)
            {
                RegisterAssembly(assemblyDefinition);
            }
        }
    }
}
