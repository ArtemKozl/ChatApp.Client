using OnionMaui.ViewModel;

namespace OnionMaui.View;

public partial class LoginPage : ContentPage
{
	public LoginPage(LoginPageViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;

    }

}