using BL;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class NovaNetPlateSolverServiceEx
    {
        public static IServiceCollection AddNovaAstrometryPlateSolverService(this IServiceCollection services)
        {
            services.AddHttpClient<IPlateSolverService, NovaNetPlateSolverService>(
                client => client.BaseAddress = new Uri("http://nova.astrometry.net")
            );

            return services;
        }
    }
}