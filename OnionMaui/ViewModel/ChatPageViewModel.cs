using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.AspNetCore.SignalR.Client;
using OnionMaui.Model;
using System.Collections.ObjectModel;
using System.Diagnostics;
using CommunityToolkit.Mvvm.Input;
using System.Text.Json;
using System.Globalization;

namespace OnionMaui.ViewModel
{


    [QueryProperty(nameof(User),"User")]
    public partial class ChatPageVireModel : BaseViewModel
    {
        public ObservableCollection<Message> Messages { get; set; } = new();

        [ObservableProperty]
        UserForChat user;

        [ObservableProperty]
        public string messageEntry = string.Empty;

        [ObservableProperty]
        public string horizontalOptions = string.Empty;

        [ObservableProperty]
        public byte[] image ;

        [ObservableProperty]
        private string imageLoadNotification = string.Empty;

        public HubConnection? _hubConnection;
        public ChatPageVireModel()
        {
            Messages.Clear();
            InitializeHubConnection();
            Task taskJoinChat = JoinChatGroup();
            image = Array.Empty<byte>();
            user = new UserForChat();
            Task taskGetMessages = GetOldMessages();
        }

        private void InitializeHubConnection()
        {
            var allCookies = GlobalCookieContainer.GetAllCookies();
            var cookie = allCookies.FirstOrDefault(allCookies => allCookies.Name == "tasty-cookie");

            if (cookie != null)
            {
                _hubConnection = new HubConnectionBuilder()
               .WithUrl("https://localhost:7125/chat", options =>
               {
                   options.AccessTokenProvider = () => Task.FromResult(cookie?.Value);
               })
               .WithAutomaticReconnect()
               .Build();

                var messageInput = new Message();

                _hubConnection.On<string, string, string>("ReceiveMessage", (userName, time, message) =>
                {
                    Client.GetAsync("https://localhost:7290/Users/GetUserByIdFromCookie");

                    messageInput.userName = userName;
                    messageInput.time = time;
                    messageInput.messageText = message;
                    messageInput.image = Array.Empty<byte>();

                    if (messageInput.userName == User.UserName)
                    {
                        messageInput.backgroundColor = "#00D930";
                        messageInput.horizontalOptions = LayoutOptions.End;
                    }
                    else
                    {
                        messageInput.backgroundColor = "#3D3D44";
                        messageInput.horizontalOptions = LayoutOptions.Start;
                    }

                    Messages.Add(messageInput);
                    OnPropertyChanged(nameof(Messages));
                });

                var imageInput = new Message();

                _hubConnection.On<string, string, byte[]>("ReceiveImage", (userName, time, image) =>
                {
                    Client.GetAsync("https://localhost:7290/Users/GetUserByIdFromCookie");

                    imageInput.userName = userName;
                    imageInput.time = time;
                    imageInput.messageText = string.Empty;
                    imageInput.image = image;

                    if (imageInput.userName == User.UserName)
                    {
                        imageInput.backgroundColor = "#00D930";
                        imageInput.horizontalOptions = LayoutOptions.End;
                    }
                    else
                    {
                        imageInput.backgroundColor = "#3D3D44";
                        imageInput.horizontalOptions = LayoutOptions.Start;
                    }

                    Messages.Add(imageInput);
                    OnPropertyChanged(nameof(Messages));
                });
            }
            else
            {
                Debug.WriteLine("cookie is null");
                return;
            }

            
        }

        [RelayCommand]
        public async Task JoinChatGroup()
        {
            Messages.Clear();
            try
            {
                if (_hubConnection == null)
                {
                    Debug.WriteLine("_hubConnection is null.");
                    return;
                }

                

                await _hubConnection.StartAsync();

                await _hubConnection.InvokeAsync("JoinChat", User);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                await Shell.Current.DisplayAlert("Error!",
                    $"Что-то пошло не так: {ex.Message}", "OK");
            }
        }

        [RelayCommand]
        public async Task SendMessage()
        {
            await Client.GetAsync("https://localhost:7290/Users/GetUserByIdFromCookie");

            if (_hubConnection == null)
            {
                Debug.WriteLine("_hubConnection is null.");
                return;
            }

            try
            {
                DateTime now = DateTime.Now;
                TimeSpan timeSpan = now.TimeOfDay;

                await _hubConnection.SendAsync("SendMessage", User.ChatRoomId, $"{ timeSpan.Hours}:{ timeSpan.Minutes}",
                MessageEntry, Image);

                MessageEntry = string.Empty;
                Image = Array.Empty<byte>();
                ImageLoadNotification = "";
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                await Shell.Current.DisplayAlert("Error!",
                    $"Что-то пошло не так: {ex.Message}", "OK");
            }
            
        }

        [RelayCommand]
        public async Task PickFile()
        {
            var options = new PickOptions
            {
                PickerTitle = "Выберите файл",
                FileTypes = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
                {
                    { DevicePlatform.iOS, new[] { "public.data" } },
                    { DevicePlatform.Android, new[] { "*/*" } },
                    { DevicePlatform.WinUI, new[] { ".png" } },
                    { DevicePlatform.Tizen, new[] { "*/*" } },
                    { DevicePlatform.macOS, new[] { "public.data" } },
                }
            )
            };

            var result = await FilePicker.Default.PickAsync(options);

            if (result != null)
            {
                var filePath = result.FullPath;

                byte[] imageBytes = File.ReadAllBytes(filePath);

                Image = imageBytes;

                ImageLoadNotification = "Изображение загружено";
            }
        }

        public async Task GetOldMessages()
        {
            await Task.Delay(100);

            var response = await Client.GetAsync($"https://localhost:7125/Messages/GetMessages/{User.ChatRoomId}");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var messages = JsonSerializer.Deserialize<List<Message>>(content);

                if (messages != null && messages.Count > 0)
                    foreach (var message in messages)
                    {
                        if (message.userName == User.UserName) 
                        {
                            message.backgroundColor = "#00D930";
                            message.horizontalOptions = LayoutOptions.End;
                        }
                        else
                        {
                            message.backgroundColor = "#3D3D44";
                            message.horizontalOptions = LayoutOptions.Start;
                        }
                        Messages.Add(message);
                    }
                                                
            }
            else 
            {
                return;
            }
        }

    }
}
