using Microsoft.Extensions.DependencyInjection;
using OrderingViewModel;
using OrderingViewModel.Services;
using OrderingViewModel.Services.Interfaces;
using System.Windows;

namespace OrderingWPFUI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private ServiceProvider? _serviceProvider;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var services = new ServiceCollection();

            services.AddHttpClient<IItemService, ItemService>(client =>
            {
                client.BaseAddress = new Uri("https://localhost:7272/");
            });

            services.AddSingleton<IOrderService, OrderService>();
            services.AddSingleton<MainWindowViewModel>();
            services.AddSingleton<MainWindow>();

            _serviceProvider = services.BuildServiceProvider();

            var window = _serviceProvider.GetRequiredService<MainWindow>();
            window.Show();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            _serviceProvider?.Dispose();
            base.OnExit(e);
        }
    }

}
