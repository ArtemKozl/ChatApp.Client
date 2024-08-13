using System.Diagnostics;
using System.Text.Json;
using System.Text;
using System.Text.RegularExpressions;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;


namespace OnionMaui.ViewModel
{
    public partial class RegistrationPageViewModel : BaseViewModel
    {
        [ObservableProperty]
        private string userName = string.Empty;

        [ObservableProperty]
        private string email = string.Empty;

        [ObservableProperty]
        private string password = string.Empty;

        [ObservableProperty]
        private string result = string.Empty;



        [RelayCommand]
        public async Task Registration()
        {
            try
            {
                if (Email == string.Empty || Password == string.Empty || UserName == string.Empty)
                {
                    Result = "Остались незаполненные поля";

                    return;

                }

                if (IsValidEmail(Email) != true)
                {
                    Result = "Неправильный формат электронной почты";

                    return;
                }

                Dictionary<string, string> credentialsEmail = new()
                    {
                        {"username", "string.Empty" },
                        {"email", Email },
                        {"password", "string.Empty"},
                };

                var jsonEmail = JsonSerializer.Serialize(credentialsEmail, SerializerOptions);
                var contentEmail = new StringContent(jsonEmail, Encoding.UTF8, "application/json");

                var responseEmail = await Client.PostAsync("https://localhost:7290/Users/UserExistByEmail", contentEmail);

                var responseBody = await responseEmail.Content.ReadAsStringAsync();

                bool exists = JsonSerializer.Deserialize<bool>(responseBody);

                if (exists != true)
                {
                    Dictionary<string, string> credentials = new()
                    {
                        {"username", UserName },
                        {"email", Email },
                        {"password", Password},
                    };

                    var json = JsonSerializer.Serialize(credentials, SerializerOptions);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    var response = await Client.PostAsync("https://localhost:7290/Users/register", content);
                    response.EnsureSuccessStatusCode();

                    Result = "Пользователь заристрирован";
                }
                else
                    Result = "Пользователь с такой почтой уже зарегистрирован";
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                await Shell.Current.DisplayAlert("Ошибка!",
                    $"Не удалось получить информацию о пользователях: {ex.Message}", "OK");
            }
        }
        public bool IsValidEmail(string email)
        {
            Regex regex = new Regex(@"^\S+@\S+(\.\S+)?\.\S+$");
            return regex.IsMatch(email);
        }
    }
}
