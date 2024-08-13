using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnionMaui.Model
{
    public partial class Group : ObservableObject
    {
        public int id { get; set; }
        public string name { get; set; } = string.Empty;
        public string userName { get; set; } = string.Empty;
        public string time {  get; set; } = string.Empty;
        public string message { get; set; } = string.Empty;

        [ObservableProperty]
        public bool checkBoxVisibility  = false;
    }
}
