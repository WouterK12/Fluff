using Microsoft.Extensions.DependencyModel;
using System.Reflection;

namespace Fluff
{
    internal class TypeFinder : ITypeFinder
    {
        public IEnumerable<Type> FindAllTypes()
        {
            IEnumerable<Assembly> assemblies = GetRefencingAssemblies();

            return assemblies.SelectMany(a => a.GetTypes());
        }

        private IEnumerable<Assembly> GetRefencingAssemblies()
        {
            string? thisAssemblyName = GetType().Assembly.FullName;

            IEnumerable<Assembly> result = from library in DependencyContext.Default?.RuntimeLibraries
                                           where library.Dependencies.Any(d => thisAssemblyName.StartsWith(d.Name))
                                           select Assembly.Load(new AssemblyName(library.Name));
            return result;
        }
    }
}
