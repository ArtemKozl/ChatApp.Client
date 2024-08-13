using OnionMaui.View;

namespace OnionMaui
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(RegistrationPage), typeof(RegistrationPage));
            Routing.RegisterRoute(nameof(ChatGroupsPage), typeof(ChatGroupsPage));
            Routing.RegisterRoute(nameof(ChatPage), typeof(ChatPage));
            Routing.RegisterRoute(nameof(AddChatGroupPage), typeof(AddChatGroupPage));
        }
    }
}
