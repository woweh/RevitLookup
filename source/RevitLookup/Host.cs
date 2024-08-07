using System.IO;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RevitLookup.Config;
using RevitLookup.Services;
using RevitLookup.Services.Contracts;
using RevitLookup.ViewModels.Contracts;
using RevitLookup.ViewModels.Dialogs.Visualization;
using RevitLookup.ViewModels.Pages;
using RevitLookup.Views;
using RevitLookup.Views.Dialogs.Visualization;
using RevitLookup.Views.Pages;
using Wpf.Ui;

namespace RevitLookup;

/// <summary>
///     Provides a host for the application's services and manages their lifetimes
/// </summary>
public static class Host
{
    private static IHost _host;
    
    /// <summary>
    ///     Starts the host and configures the application's services
    /// </summary>
    public static void Start()
    {
        var builder = Microsoft.Extensions.Hosting.Host.CreateApplicationBuilder(new HostApplicationBuilderSettings
        {
            ContentRootPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly()!.Location),
            DisableDefaults = true
        });
        
        //Logging
        builder.Logging.ClearProviders();
        builder.Logging.AddSerilogConfiguration();
        
        //Configuration
        builder.Services.AddOptions(builder.Configuration);
        
        //Application services
        builder.Services.AddSingleton<ISettingsService, SettingsService>();
        builder.Services.AddSingleton<ISoftwareUpdateService, SoftwareUpdateService>();
        builder.Services.AddHostedService<HostedLifecycleService>();
        
        //UI services
        builder.Services.AddScoped<INavigationService, NavigationService>();
        builder.Services.AddScoped<ISnackbarService, SnackbarService>();
        builder.Services.AddScoped<IContentDialogService, ContentDialogService>();
        builder.Services.AddScoped<NotificationService>();
        
        //Views
        builder.Services.AddScoped<ISnoopVisualService, SnoopVisualService>();
        builder.Services.AddScoped<AboutView>();
        builder.Services.AddScoped<AboutViewModel>();
        builder.Services.AddScoped<DashboardView>();
        builder.Services.AddScoped<IDashboardViewModel, DashboardViewModel>();
        builder.Services.AddScoped<SettingsView>();
        builder.Services.AddScoped<SettingsViewModel>();
        builder.Services.AddScoped<EventsView>();
        builder.Services.AddScoped<IEventsViewModel, EventsViewModel>();
        builder.Services.AddScoped<SnoopView>();
        builder.Services.AddScoped<ISnoopViewModel, SnoopViewModel>();
        builder.Services.AddScoped<IWindow, RevitLookupView>();
        
        //Dialogs
        builder.Services.AddTransient<BoundingBoxVisualizationDialog>();
        builder.Services.AddTransient<BoundingBoxVisualizationViewModel>();
        builder.Services.AddTransient<FaceVisualizationDialog>();
        builder.Services.AddTransient<FaceVisualizationViewModel>();
        builder.Services.AddTransient<MeshVisualizationDialog>();
        builder.Services.AddTransient<MeshVisualizationViewModel>();
        builder.Services.AddTransient<PolylineVisualizationDialog>();
        builder.Services.AddTransient<PolylineVisualizationViewModel>();
        builder.Services.AddTransient<SolidVisualizationDialog>();
        builder.Services.AddTransient<SolidVisualizationViewModel>();
        builder.Services.AddTransient<XyzVisualizationDialog>();
        builder.Services.AddTransient<XyzVisualizationViewModel>();
        
        //Startup view
        builder.Services.AddTransient<ILookupService, LookupService>();
        
        _host = builder.Build();
        _host.Start();
    }
    
    /// <summary>
    ///     Starts the host proxy and configures the application's services
    /// </summary>
    public static void StartProxy(IHost host)
    {
        _host = host;
        host.Start();
    }
    
    /// <summary>
    ///     Stops the host and handle <see cref="IHostedService"/> services
    /// </summary>
    public static void Stop()
    {
        _host.StopAsync().GetAwaiter().GetResult();
    }
    
    /// <summary>
    ///     Get service of type <typeparamref name="T"/>
    /// </summary>
    /// <typeparam name="T">The type of service object to get</typeparam>
    /// <exception cref="System.InvalidOperationException">There is no service of type <typeparamref name="T"/></exception>
    public static T GetService<T>() where T : class
    {
        return _host.Services.GetRequiredService<T>();
    }
}