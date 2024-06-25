using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace RinneganCore
{
    public static class Extensions
    {
        private static bool IsSubclassOfRinnegan(this Type toCheck, IEnumerable<System.Type> validTypes)
        {
            if (toCheck == typeof(object))
                return false;

            toCheck = toCheck.BaseType;
            if (!toCheck.IsGenericType)
                return false;

            if (!validTypes.Any(t =>t.Namespace == toCheck.Namespace && t.Name == toCheck.Name))
                return false;

            return true;
        }
        private static IServiceCollection AddRinnegan(this IServiceCollection services, Action<RinneganOptions> rinneganOptions)
        {
            var rinneganOptionModel = new RinneganOptions();
            rinneganOptions(rinneganOptionModel);


            var typesToRegister = rinneganOptionModel.Assemblies.SelectMany(a => a.GetTypes())
                .Where(t => t.IsClass && t.IsSubclassOfRinnegan(rinneganOptionModel.ValidParentTypes) && !rinneganOptionModel.IgnoreNameSpaces.Any(ins => ins == t.Namespace)).ToList();


            foreach (var type in typesToRegister)
            {
                services.AddTransient(type);
            }

            return services;
        }

        private static bool IsRinnegan(this Type @this,Type rinnegan)
        {
            if (@this == null || !@this.IsClass)
                return false;

            if (@this.Namespace == rinnegan.Namespace && @this.Name == rinnegan.Name)
                return true;

            return @this.BaseType.IsRinnegan(rinnegan);
        }
        public static IServiceCollection AddRinnegan(this IServiceCollection services, Assembly? assembly)
        {
            if (assembly == null)
                throw new ArgumentNullException(nameof(assembly));
            var types = assembly
                .GetTypes()
                .Where(t => t.BaseType.IsRinnegan(typeof(Rinnegan<,>)));

            foreach (var type in types)
                services.AddTransient(type);

            return services;
        }
        public class RinneganOptions
        {

            public IEnumerable<Assembly> Assemblies { get; set; }
            public IEnumerable<System.Type> ValidParentTypes { get; set; }
            public IEnumerable<string> IgnoreNameSpaces { get; set; }
        }
    }
}
