using System.Collections.ObjectModel;
using System.Diagnostics;
using Plugin.BLE.Abstractions;
using Plugin.BLE.Abstractions.Contracts;
using Plugin.BLE.Abstractions.Exceptions;
using Xamarin.Forms;

namespace PluginBleTest
{
    public partial class PeripheralsPage : ContentPage
    {
        IBluetoothLE _BluetoothLe;
        IAdapter _Adapter;
        private ObservableCollection<IDevice> _deviceList = new ObservableCollection<IDevice>();

        public PeripheralsPage()
        {
            InitializeComponent();

            _BluetoothLe = Plugin.BLE.CrossBluetoothLE.Current;
            _Adapter = _BluetoothLe.Adapter;
            _Adapter.ScanTimeout = 5000;
            _Adapter.DeviceDiscovered += (s, e) =>
            {
                _deviceList.Add(e.Device);
            };
            _Adapter.ScanTimeoutElapsed += (s, e) =>
            {
                foreach (var device in _Adapter.ConnectedDevices)
                {
                    _deviceList.Add(device);
                }
            };
            ListView1.ItemsSource = _deviceList;
            AddSearchButton();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            var devices = _Adapter.ConnectedDevices;
            foreach (var device in devices)
            {
                await _Adapter.DisconnectDeviceAsync(device);
            }
        }

        private void AddSearchButton()
        {
            var Toolbarbutton = new ToolbarItem { Text = "Search" };
            Toolbarbutton.Clicked += async (s, e) =>
            {
                if (_BluetoothLe.State == BluetoothState.Off)
                {
                    _deviceList.Clear();
                    await DisplayAlert("Bluetooth LE", "BLE states is off.", "OK");
                    return;
                }
                if (_Adapter.IsScanning)
                {
                    return;
                }
                _deviceList.Clear();
                await _Adapter.StartScanningForDevicesAsync();
            };
            ToolbarItems.Add(Toolbarbutton);
        }

        async void ListView1_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            var device = e.SelectedItem as IDevice;
            if (device == null)
            {
                return;
            }

            try
            {
                await _Adapter.ConnectToDeviceAsync(device);
                if (device.State == DeviceState.Disconnected)
                {
                    await DisplayAlert("PluginBleTest", "Do Not Connect", "OK");
                    return;
                }
                if (device.State == DeviceState.Connected || device.State == DeviceState.Limited)
                {
                    await Navigation.PushAsync(new ServicesPage(device));
                }
            }
            catch (DeviceConnectionException ex)
            {
                Debug.WriteLine(ex.StackTrace);
                await DisplayAlert("PluginBleTest", "Do Not Connect", "OK");
            }
            finally
            {
                ListView1.SelectedItem = null;
            }
        }
    }
}