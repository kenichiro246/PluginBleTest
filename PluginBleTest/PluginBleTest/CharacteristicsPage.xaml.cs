using System.Collections.ObjectModel;
using Plugin.BLE.Abstractions.Contracts;
using Xamarin.Forms;

namespace PluginBleTest
{
    public partial class CharacteristicsPage : ContentPage
    {
        private ObservableCollection<ICharacteristic> _CharacteristicList = new ObservableCollection<ICharacteristic>();
        private IService _service = null;

        public CharacteristicsPage(IService service)
        {
            InitializeComponent();

            _service = service;
            ListView1.ItemsSource = _CharacteristicList;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            var characteristics = await _service.GetCharacteristicsAsync();
            foreach (var characteristic in characteristics)
            {
                if(characteristic.CanRead)
                {
                    await characteristic.ReadAsync();
                }
                _CharacteristicList.Add(characteristic);
            }
        }
    }
}
