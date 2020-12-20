using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using BL;

namespace CLI
{
    public class Worker : BackgroundService
    {
        private IPlateSolverService _plateSolverService;

        public Worker(IPlateSolverService plateSolverService) =>
            _plateSolverService = plateSolverService;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var image = new Uri("http://apod.nasa.gov/apod/image/1206/ldn673s_block1123.jpg");
            var sub = await _plateSolverService.BlindSolveImageUriAsync(image);
                
            while (!stoppingToken.IsCancellationRequested)
            {
                var (success, jobs) = await _plateSolverService.GetJobsForSubmissionAsync(sub.Id);
                if (!success || jobs?.Any() == true)
                {
                    break;
                }
                await Task.Delay(4000, stoppingToken);
            }
        }
    }

    class Program
    {
        static Task Main(string[] args) =>
            CreateHostBuilder(args).Build().RunAsync();

        static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                    services
                        .AddOptions<PlateSolverOptions>()
                            .Bind(hostContext.Configuration.GetSection(nameof(PlateSolverOptions)))
                            .Services
                        .AddNovaAstrometryPlateSolverService()
                        .AddHostedService<Worker>());
    }
}
