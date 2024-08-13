using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OnionMaui.Model;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace OnionMaui.ViewModel
{
    [QueryProperty(nameof(User), "User")]
    public partial class AddChatGroupPageViewModel : BaseViewModel
    {
        public ObservableCollection<User> Users { get; set; } = new();

        [ObservableProperty]
        User user;

        [ObservableProperty]
        private string id = string.Empty;

        [ObservableProperty]
        private string groupName = string.Empty;

        public AddChatGroupPageViewModel() 
        {
            User = new User();
        }

        [RelayCommand]
        public async Task AddUserById()
        {
            await Client.GetAsync("https://localhost:7290/Users/GetUserByIdFromCookie");

            try
            {

                foreach (var user in Users)
                    if (user.Id == Convert.ToInt32(Id))
                    {
                        Id = string.Empty;
                        return;
                    }
                        

                if (User.Id != Convert.ToInt32(Id))
                {
                    var response = await Client.GetAsync($"https://localhost:7290/Users/{Id}");

                    if (response.IsSuccessStatusCode)
                    {
                        var responseBody = await response.Content.ReadAsStringAsync();

                        User user = new User()
                        {
                            Id = Convert.ToInt32(Id),
                            UserName = responseBody
                        };

                        Users.Add(user);

                        Id = string.Empty;
                    }
                    else
                    {
                        await Shell.Current.DisplayAlert("Error!",
                        $"Пользователся с таким id не существует", "OK");
                    }
                }
                else
                {
                    await Shell.Current.DisplayAlert("Error!",
                    $"Нельзя добавить в группу самого себя", "OK");
                }

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                await Shell.Current.DisplayAlert("Error!",
                    $"Произошла ошибка: {ex.Message}", "OK");
            }
        }

        [RelayCommand]
        public async Task CreateGroup()
        {

            await Client.GetAsync("https://localhost:7290/Users/GetUserByIdFromCookie");

            try
            {
                if (GroupName != null && GroupName != string.Empty)
                {
                    Dictionary<string, string> credentials1 = new()
                    {
                        {"name", GroupName},
                        {"id", "0" },
                    };

                    var jsonAddGroup = JsonSerializer.Serialize(credentials1);
                    var contentAddGroup = new StringContent(jsonAddGroup, Encoding.UTF8, "application/json");

                    var response = await Client.PostAsync("https://localhost:7125/ChatGroups/AddGroup", contentAddGroup);

                    if (response.IsSuccessStatusCode)
                    {
                        var groupId = await response.Content.ReadAsStringAsync();
                        if (Users.Count != 0)
                        {
                            foreach (var user in Users)
                            {
                                Dictionary<string, string> credentials = new()
                                {
                                    {"userId", Convert.ToString(user.Id)},
                                    {"groupId", groupId },
                                };

                                var json = JsonSerializer.Serialize(credentials);
                                var content = new StringContent(json, Encoding.UTF8, "application/json");

                                await Client.PostAsync("https://localhost:7125/ChatGroups/addUserToGroup", content);
                            }
                            Users.Clear();

                            await Shell.Current.GoToAsync("..");
                        }
                    }
                    else
                    {
                        await Shell.Current.DisplayAlert("Error!",
                        $"Произошла ошибка", "OK");
                    }
                }
                else
                {
                    await Shell.Current.DisplayAlert("Error!",
                        $"Имя группы не может быть пустым!", "OK");
                }
            }
            catch (Exception ex) { Debug.WriteLine(ex); await Shell.Current.DisplayAlert("Error!",
                        $"Произошла ошибка: {ex.Message}", "OK");
            }
        }

        [RelayCommand]
        public void DeleteUser(User user) 
        {
            Users.Remove(user);
        }
       
    }
}
