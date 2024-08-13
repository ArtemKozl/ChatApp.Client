using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OnionMaui.Model;
using System.Diagnostics;
using System.Text.Json;
using System.Text;
using OnionMaui.View;
using System.Net;


namespace OnionMaui.ViewModel
{
    public partial class LoginPageViewModel : BaseViewModel
    {
        [ObservableProperty]
        private string email = string.Empty;

        [ObservableProperty]
        private string password = string.Empty;

        [ObservableProperty]
        private string username = string.Empty;

        [ObservableProperty]
        private string result = string.Empty;

        public LoginPageViewModel()
        {

        }

        [RelayCommand]
        public async Task Login()
        {
            try
            {
                if (string.IsNullOrEmpty(Email) || string.IsNullOrEmpty(Password))
                {
                    Result = "Остались незаполненные поля";
                    return;
                }

                Dictionary<string, string> credentials = new()
                {
                    {"email", Email },
                    {"password", Password},
                };

                var json = JsonSerializer.Serialize(credentials);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await Client.PostAsync("https://localhost:7290/Users/login", content);


                if (response.IsSuccessStatusCode)
                {
                    var responseBody = await response.Content.ReadAsStringAsync();
                    var userInfo = JsonSerializer.Deserialize<User>(responseBody);

                    if (userInfo != null) 
                    {

                        var user = new User()
                        {
                            Id = userInfo.Id,
                            UserName = userInfo.UserName
                        };

                        var navigationParameters = new Dictionary<string, object>
                        {
                            { "User", user}
                        };

                        await Shell.Current.GoToAsync(nameof(ChatGroupsPage), navigationParameters);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                await Shell.Current.DisplayAlert("Ошибка!",
                    $"Не удалось получить информацию о пользователях: {ex.Message}", "OK");
            }
        }

        [RelayCommand]
        public async Task ToRegistrationPage()
        {
            await Shell.Current.GoToAsync(nameof(RegistrationPage));
        }

        public async Task CheckTokens()
        {
            var response = await Client.GetAsync("https://localhost:7290/Users/GetUserByIdFromCookie");


            if (response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                Username = responseBody;

                var navigationParameters = new Dictionary<string, object>
                    {
                        { "UserName", Username }
                    };

                await SaveCookiesAsync();

                await Shell.Current.GoToAsync(nameof(ChatGroupsPage), navigationParameters);
            }
            else
            {
                Debug.WriteLine("Куки не найдены");
            }
        }
        public async Task SaveCookiesAsync()
        {
            var allCookies = GlobalCookieContainer.GetAllCookies();
            var tastyCookie = allCookies.FirstOrDefault(allCookies => allCookies.Name == "tasty-cookie");
            var tastiestCookie = allCookies.FirstOrDefault(allCookies => allCookies.Name == "tastiest-cookie");

            await SecureStorage.Default.SetAsync("tasty-cookie", JsonSerializer.Serialize(tastyCookie));
            await SecureStorage.Default.SetAsync("tastiest-cookie", JsonSerializer.Serialize(tastiestCookie));
        }

        public async Task LoadCookiesAsync()
        {
            var tastyCookieJson = await SecureStorage.Default.GetAsync("tasty-cookie");
            var tastiestCookieJson = await SecureStorage.Default.GetAsync("tastiest-cookie");

            if (!string.IsNullOrEmpty(tastyCookieJson))
            {
                var deserializedTastyCookie = JsonSerializer.Deserialize<Cookie>(tastyCookieJson);
                if (deserializedTastyCookie != null)
                    GlobalCookieContainer.Add(deserializedTastyCookie);
            }

            if (!string.IsNullOrEmpty(tastiestCookieJson))
            {
                var deserializedTastiestCookie = JsonSerializer.Deserialize<Cookie>(tastiestCookieJson);
                if(deserializedTastiestCookie != null)
                    GlobalCookieContainer.Add(deserializedTastiestCookie);
            }
        }
    }
}
