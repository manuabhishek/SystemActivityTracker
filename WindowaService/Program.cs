namespace WindowaService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //var builder = Host.CreateApplicationBuilder(args);
            //builder.Services.AddHostedService<Worker>();

            //var host = builder.Build();
            //host.Run();

            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
                 Host.CreateDefaultBuilder(args)
                    .UseWindowsService(options =>
                    {
                        options.ServiceName = "CompanyLicenceVrifier";
                    })
                    .ConfigureServices((hostContext, services) =>
                    {
                        services.AddHostedService<Worker>();
                    });
    }
}