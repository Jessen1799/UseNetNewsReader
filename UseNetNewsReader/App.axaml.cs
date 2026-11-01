using System;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using UseNetNewsReader.Services;
using UseNetNewsReader.ViewModels;
using UseNetNewsReader.Views;

namespace UseNetNewsReader;

public partial class App : Application
{
    public IServiceProvider? Services { get; set; }
    
    public new static App Current => (App)Application.Current;
    public override void Initialize()
    {
        Services = ConfigureServices();
        
        AvaloniaXamlLoader.Load(this);
    }

    private static IServiceProvider ConfigureServices()
    {
        var services = new ServiceCollection();

        services.AddSingleton<IConnection, NntpService>();
        services.AddSingleton<MainViewModel>();
        services.AddSingleton<LoginViewModel>();
        services.AddSingleton<HomeViewModel>();

        return services.BuildServiceProvider();
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindowView
            {
                DataContext = Services.GetService<MainViewModel>()
            };
        }

        base.OnFrameworkInitializationCompleted();
    }
}