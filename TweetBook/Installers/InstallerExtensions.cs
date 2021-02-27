using System;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace TweetBook.Installers
{
    public static class InstallerExtensions
    {
        public static void InstallServicesFromAssembly(this IServiceCollection services, IConfiguration configuration, Type type)
        {
            type.Assembly.ExportedTypes
                .Where(x =>
                    typeof(IInstaller).IsAssignableFrom(x) &&
                    !x.IsInterface &&
                    !x.IsAbstract)
                .Select(Activator.CreateInstance)
                .Cast<IInstaller>()
                .ToList()
                .ForEach(installer => installer.InstallServices(services, configuration));
        }
    }
}