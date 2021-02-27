using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace TweetBook.Installers
{
    public class EndpointInstaller : IInstaller
    {
        public void InstallServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddControllersWithViews();

            services.AddSwaggerGen(x => x.SwaggerDoc("v1", new OpenApiInfo {Title = "Tweetbook API", Version = "v1"}));
        }
    }
}