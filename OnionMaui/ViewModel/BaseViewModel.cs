using CommunityToolkit.Mvvm.ComponentModel;
using System.Net;
using System.Text.Json;

namespace OnionMaui.ViewModel
{
    public partial class BaseViewModel : ObservableObject
    {
        protected static CookieContainer GlobalCookieContainer = new CookieContainer();

        protected readonly HttpClient Client;
        protected readonly JsonSerializerOptions SerializerOptions;

        public BaseViewModel()
        {
            var handler = new HttpClientHandler
            {
                UseCookies = true,
                CookieContainer = GlobalCookieContainer 
            };
            handler.UseCookies = true;
            Client = new HttpClient(handler);

            SerializerOptions = new JsonSerializerOptions();

        }



    }

}

