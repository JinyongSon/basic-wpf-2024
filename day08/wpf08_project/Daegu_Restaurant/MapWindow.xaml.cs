using System.Windows;

namespace Daegu_Restaurant
{
    /// <summary>
    /// MapWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MapWindow : Window
    {
        public MapWindow(string location)
        {
            InitializeComponent();
            ShowLocationOnMap(location);
        }
        private void ShowLocationOnMap(string location)
        {
            string mapsUrl = $"https://www.google.com/maps?q={Uri.EscapeDataString(location)}";
            BrsLoc.Address = mapsUrl;
        }
    }
}
