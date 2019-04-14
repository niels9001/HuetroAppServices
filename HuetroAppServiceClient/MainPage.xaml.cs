using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.ApplicationModel.AppService;
using Q42.HueApi;
using Q42.HueApi.ColorConverters;
using Q42.HueApi.ColorConverters.Original;
using Newtonsoft.Json;

namespace HuetroAppServiceClient
{
    public sealed partial class MainPage : Page
    {
        private AppServiceConnection huetroService;
        public MainPage()
        {
            this.InitializeComponent();

        }

        private void GetLights()
        {

        }

        private async void ConnectBtn_Click(object sender, RoutedEventArgs e)
        {
            LightsBtn.IsEnabled = false;
            // Add the connection.
            if (this.huetroService == null)
            {
                this.huetroService = new AppServiceConnection();
            }

            // Required, do not change
            this.huetroService.AppServiceName = "com.huetro.appservice";
            this.huetroService.PackageFamilyName = "27078NielsLaute.HuetroforHue_91se88q2mhfz2";


            var status = await this.huetroService.OpenAsync();



            if (status != AppServiceConnectionStatus.Success)
            {
                System.Diagnostics.Debug.WriteLine("Failed to connect");
                this.huetroService = null;
            }
            else
            {
                LightsBtn.IsEnabled = true;
            }
        }

        // Get lights and bind them to a ListView
        private async void LightsBtn_Click(object sender, RoutedEventArgs e)
        {
            ValueSet Message = new ValueSet();
            Message.Add("Command", "GetLights");

            AppServiceResponse response = await this.huetroService.SendMessageAsync(Message);

            if (response.Status == AppServiceResponseStatus.Success)
            {
                LightsView.SelectionChanged -= LightsView_SelectionChanged;
                LightsView.ItemsSource = response.Message;
                LightsView.SelectionChanged += LightsView_SelectionChanged;
            }
             
        }

        private void LightsView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (LightsView.SelectedItems.Count > 0)
            {
                LightControlPanel.Visibility = Visibility.Visible;
            }
            else
            {
                LightControlPanel.Visibility = Visibility.Collapsed;
            }
        }

        private void DimBtn_Click(object sender, RoutedEventArgs e)
        {
            SendLightCommand(new LightCommand() { BrightnessIncrement = -100 });
        }

        private void TurnBlueBtn_Click(object sender, RoutedEventArgs e)
        {
            SendLightCommand(new LightCommand().SetColor(new RGBColor(0, 0, 255)));
        }

        private void TurnRedBtn_Click(object sender, RoutedEventArgs e)
        {
            SendLightCommand(new LightCommand().SetColor(new RGBColor("ff0000")));
        }

        private void TurnOffBtn_Click(object sender, RoutedEventArgs e)
        {
            SendLightCommand(new LightCommand().TurnOff());
        }

        private void TurnOnBtn_Click(object sender, RoutedEventArgs e)
        {
            SendLightCommand(new LightCommand().TurnOn());
        }


        private async void SendLightCommand(LightCommand Command)
        {
            ValueSet Message = new ValueSet();
            Message.Add("Command", "SendLightCommand");
            Message.Add("LightCommand", ObjectToValue(Command));

            List<string> LightIDs = new List<string>();
            foreach (KeyValuePair<string, object> x in LightsView.SelectedItems)
            {
                LightIDs.Add(x.Key);
            }
            Message.Add("LightIDs", ObjectToValue(LightIDs));
            AppServiceResponse response = await this.huetroService.SendMessageAsync(Message);
        }

        private string ObjectToValue(object Obj)
        {
            return JsonConvert.SerializeObject(Obj, Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
        }
    }
}
