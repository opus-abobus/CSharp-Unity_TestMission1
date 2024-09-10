using Microsoft.Extensions.DependencyInjection;
using ProjectManagement.Services;
using ProjectManagement.Services.UserServices;

namespace ProjectManagement
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            new Application(ConfigureServices()).Run();
        }

        private static ServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection()
                .AddSingleton<IRegisterService, RegisterService>()
                .AddSingleton<IAuthenticateService, AuthenticateService>()
                .AddSingleton<ITaskManagementService, TaskManagementService>()
                .AddSingleton<ITaskLogService, TaskLogService>();

            return services.BuildServiceProvider();
        }
    }
}