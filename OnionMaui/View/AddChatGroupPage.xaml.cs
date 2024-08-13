using OnionMaui.ViewModel;

namespace OnionMaui.View;

public partial class AddChatGroupPage : ContentPage
{
	public AddChatGroupPage(AddChatGroupPageViewModel viewModel)
	{
		InitializeComponent();
        BindingContext = viewModel;
	}
}