using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Reflection;

namespace RinneganCore
{
    public static class Extensions
    {
        private static bool Include(this Type @this, Type rinnegan)
        {
            if (@this == null || !@this.IsClass)
                return false;

            if (@this.Namespace == rinnegan.Namespace && @this.Name == rinnegan.Name)
                return true;

            return @this.BaseType.Include(rinnegan);
        }
        public static IServiceCollection AddRinnegan(this IServiceCollection services, Assembly? assembly)
        {
            if (assembly == null)
                throw new ArgumentNullException(nameof(assembly));
            var types = assembly
                .GetTypes()
                .Where(t => t.BaseType.Include(typeof(Rinnegan<,>)));

            foreach (var type in types)
                services.TryAddTransient(type);

            return services;
        }
    }
}
