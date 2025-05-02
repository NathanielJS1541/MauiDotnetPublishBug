using MauiDotnetPublishBug.Common.Resources;
using MauiDotnetPublishBug.Common.Settings;
using MauiDotnetPublishBug.Core.Settings;
using MauiDotnetPublishBug.Resources;
using Microsoft.Extensions.Logging;

namespace MauiDotnetPublishBug
{
    public static class MauiProgram
    {
        #region Methods

        #region Public Static

        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            // Register services for dependency injection.
            builder.Services
                .AddSingleton<IResourceManager, ResourceManager>()
                .AddSingleton<ISettingsManager, SettingsManager>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }

        #endregion

        #endregion
    }
}
