using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OnionMaui.Model;
using OnionMaui.View;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;
using System.Text.Json;

namespace OnionMaui.ViewModel
{
    [QueryProperty(nameof(User), "User")]
    public partial class ChatGrupsPageViewModel : BaseViewModel
    {
        public ObservableCollection<Group> Groups { get; set; } = new();
        public ObservableCollection<LastMessage> LastMessages {get; set;} = new();
        [ObservableProperty]
        User user;

        public ChatGrupsPageViewModel()
        {
            Task task = GetGroups();
            User = new User();
        }

        [RelayCommand]        
        public async Task GetGroups()
        {

            await Client.GetAsync("https://localhost:7290/Users/GetUserByIdFromCookie");

            try
            {
                Groups.Clear();

                var response = await Client.GetAsync("https://localhost:7125/ChatGroups/GetGroups");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var groups = JsonSerializer.Deserialize<List<Group>>(content);

                    if (groups != null && groups.Count > 0)
                    {
                        foreach (var group in groups)
                        {
                            var lastMessages = await GetLastMessage(group.id);

                            group.userName = lastMessages.userName;
                            group.message = lastMessages.message;
                            group.time = lastMessages.time;

                            Groups.Add(group);
                        }
                    }          
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                await Shell.Current.DisplayAlert("Error!",
                    $"Что-то пошло не так: {ex.Message}", "OK");
            }

        }
        [RelayCommand]
        public async Task GoToChatPage(Group group)
        {

            await Client.GetAsync("https://localhost:7290/Users/GetUserByIdFromCookie");

            try
            {
                if (group != null && User.UserName != null)
                {
                    var user = new UserForChat()
                    {
                        UserName = User.UserName,
                        ChatRoom = group.name,
                        ChatRoomId = group.id,
                    };

                    var navigationParameters = new Dictionary<string, object>
                    {
                        { "User", user}
                    };

                    await Shell.Current.GoToAsync(nameof(ChatPage), navigationParameters);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                await Shell.Current.DisplayAlert("Error!",
                    $"Что-то пошло не так: {ex.Message}", "OK");
            }
              
        }
        [RelayCommand]
        public async Task GoToAddChatGroupPage()
        {
            var navigationParameters = new Dictionary<string, object>
            {
                { "User", User}
            };

            await Shell.Current.GoToAsync(nameof(AddChatGroupPage), navigationParameters);
        }

        [RelayCommand]
        public void SetTrashImageVisibility()
        {
            foreach (var item in Groups)
            {
                item.CheckBoxVisibility = !item.CheckBoxVisibility;
            }
        }

        [RelayCommand]
        public async Task DeleteGroup(Group group)
        {
            await Client.GetAsync("https://localhost:7290/Users/GetUserByIdFromCookie");

            Dictionary<string, string> credentials1 = new()
                    {
                        {"name", "123"},
                        {"id", Convert.ToString(group.id) }
                    };

            var jsonAddGroup = JsonSerializer.Serialize(credentials1);
            var contentAddGroup = new StringContent(jsonAddGroup, Encoding.UTF8, "application/json");

            var response = await Client.PostAsync("https://localhost:7125/ChatGroups/QuitGroup", contentAddGroup);

            Groups.Remove(group);
        }

        public async Task<LastMessage> GetLastMessage(int groupId)
        {
            var responseLastMessage = await Client.GetAsync($"https://localhost:7125/Messages/GetLastMessage/{groupId}");

            if (responseLastMessage.IsSuccessStatusCode)
            {
                var content = await responseLastMessage.Content.ReadAsStringAsync();
                var message = JsonSerializer.Deserialize<LastMessage>(content);

                return message ?? new LastMessage();
            }
            else
            {
                return new LastMessage();
            }
        }
    }
}
