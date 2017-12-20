using System.Collections.ObjectModel;
using Plugin.BLE.Abstractions.Contracts;
using Xamarin.Forms;

namespace PluginBleTest
{
    public partial class ServicesPage : ContentPage
    {
        private ObservableCollection<IService> _ServiceList = new ObservableCollection<IService>();
        private IDevice _device = null;

        public ServicesPage(IDevice device)
        {
            InitializeComponent();

            _device = device;
            ListView1.ItemsSource = _ServiceList;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            _ServiceList.Clear();
            var services = await _device.GetServicesAsync();
            foreach (var service in services)
            {
                _ServiceList.Add(service);
            }
        }

        async void ListView1_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            var service = e.SelectedItem as IService;
            if (service == null)
            {
                return;
            }
            await Navigation.PushAsync(new CharacteristicsPage(service));
            ListView1.SelectedItem = null;
        }
    }
}