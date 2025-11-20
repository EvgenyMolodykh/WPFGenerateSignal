using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using WPFGenerateSignal.Interfaces;
using WPFGenerateSignal.Services;
using WPFGenerateSignal.ViewModels;
using WPFGenerateSignal.Date.Context;
using WPFGenerateSignal.Repositoriess;

namespace WPFGenerateSignal
{
    public partial class App : Application
    {
        public static ServiceProvider ServiceProvider { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var services = new ServiceCollection();
            services.AddDbContext<ApplicationDbContext>();
            services.AddScoped<ISignalRepository, SignalRepository>();
            services.AddSingleton<ISignalService, SignalService>();
            services.AddSingleton<ISignalStorageService, SignalStorageService>();
            services.AddSingleton<SignalViewModel>();
            services.AddTransient<MainWindow>();

            ServiceProvider = services.BuildServiceProvider();

            using var scope = ServiceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            context.Database.EnsureCreated();

            var mainWindow = ServiceProvider.GetService<MainWindow>();
            mainWindow.DataContext = ServiceProvider.GetService<SignalViewModel>();
            mainWindow.Show();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            ServiceProvider?.Dispose();
            base.OnExit(e);
        }
    }
}