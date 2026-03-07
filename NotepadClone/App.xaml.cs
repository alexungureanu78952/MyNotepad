using Microsoft.Extensions.DependencyInjection;
using NotepadClone.Application.Interfaces;
using NotepadClone.Application.Services;
using NotepadClone.Infrastructure.Services;
using NotepadClone.Presentation.ViewModels;
using NotepadClone.Presentation.Views;

namespace NotepadClone;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : System.Windows.Application
{
    private ServiceProvider? _serviceProvider;

    protected override void OnStartup(System.Windows.StartupEventArgs e)
    {
        base.OnStartup(e);

        var services = new ServiceCollection();
        ConfigureServices(services);
        _serviceProvider = services.BuildServiceProvider();

        var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
        mainWindow.Show();
    }

    private void ConfigureServices(IServiceCollection services)
    {
        // Register services
        services.AddSingleton<IFileService, FileService>();
        services.AddSingleton<IDialogService, DialogService>();
        services.AddSingleton<IFolderService, FolderService>();
        services.AddSingleton<IClipboardService, ClipboardService>();
        services.AddSingleton<ITextSearchService, TextSearchService>();

        // Register ViewModels
        services.AddSingleton<MainViewModel>();

        // Register Views
        services.AddSingleton<MainWindow>();
    }

    protected override void OnExit(System.Windows.ExitEventArgs e)
    {
        _serviceProvider?.Dispose();
        base.OnExit(e);
    }
}

