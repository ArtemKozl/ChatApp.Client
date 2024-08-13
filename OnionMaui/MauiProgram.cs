using Microsoft.Extensions.Logging;
using OnionMaui.View;
using OnionMaui.ViewModel;
using CommunityToolkit.Maui;

namespace OnionMaui
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder.UseMauiApp<App>().ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                fonts.AddFont("Inter-V.ttf", "Inter");
            }).UseMauiCommunityToolkit();
#if DEBUG
            builder.Logging.AddDebug();
#endif
            builder.Services.AddTransient<ChatGroupsPage>();
            builder.Services.AddSingleton<LoginPage>();
            builder.Services.AddTransient<RegistrationPage>();
            builder.Services.AddTransient<ChatPage>();
            builder.Services.AddTransient<AddChatGroupPage>();
            builder.Services.AddTransient<ChatGrupsPageViewModel>();
            builder.Services.AddSingleton<LoginPageViewModel>();
            builder.Services.AddTransient<RegistrationPageViewModel>();
            builder.Services.AddTransient<ChatPageVireModel>();
            builder.Services.AddTransient<AddChatGroupPageViewModel>();
            builder.Services.AddSingleton<IFilePicker>(FilePicker.Default);
            return builder.Build();
        }
    }
}