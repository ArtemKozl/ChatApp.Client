using OnionMaui.ViewModel;

namespace OnionMaui.View;

public partial class ChatPage : ContentPage
{
	public ChatPage(ChatPageVireModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}

}