using OnionMaui.ViewModel;

namespace OnionMaui.View;

public partial class ChatGroupsPage : ContentPage
{
	public ChatGroupsPage(ChatGrupsPageViewModel model)
	{
		InitializeComponent();
		BindingContext = model;
	}
}