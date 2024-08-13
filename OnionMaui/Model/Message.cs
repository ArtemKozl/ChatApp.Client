namespace OnionMaui.Model
{
    public class Message
    {
        public string userName { get; set; } = string.Empty;
        public string time {  get; set; } = string.Empty ;
        public string messageText { get; set; } = string.Empty;
        public byte[]? image {  get; set; }
        public LayoutOptions horizontalOptions { get; set; } 
        public string backgroundColor {  get; set; } = string.Empty;

    }
}
