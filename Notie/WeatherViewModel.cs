using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.ComponentModel;
using System.Net.Http;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Essentials;
using Notie.Models;

namespace Notie
{
    public class WeatherViewModel : INotifyPropertyChanged
    {

        public string temp;
        public string temp_min;
        public string temp_max;

        public string Temp
        {
            get { return temp; }
            private set
            {
                temp = value;
                OnPropertyChanged("Temp");
            }
        }

        public string TempMin
        {
            get { return temp_min; }
            private set
            {
                temp_min = value;
                OnPropertyChanged("TempMin");
            }
        }

        public string TempMax
        {
            get { return temp_max; }
            private set
            {
                temp_max = value;
                OnPropertyChanged("TempMax");
            }
        }

        public ICommand LoadDataCommand { protected set; get; }
        public WeatherViewModel()
        {
            this.LoadDataCommand = new Command(LoadData);

            this.Temp = "Temp is loading...";
            this.TempMin = "temp min";
            this.TempMax = "temp max";
        }

        private async void LoadData()
        {
            try
            {
                var request = new GeolocationRequest(GeolocationAccuracy.Medium);
                var location = await Geolocation.GetLocationAsync(request);
                string url = $"https://api.openweathermap.org/data/2.5/weather?lat={location.Latitude}&lon={location.Longitude}&units=metric&appid=1e9f56c831e1d39c0290200c800bf85a";
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri(url);
                var response = await client.GetAsync(client.BaseAddress);
                response.EnsureSuccessStatusCode(); // выброс исключения, если произошла ошибка

                // десериализация ответа в формате json
                var content = await response.Content.ReadAsStringAsync();
                JObject o = JObject.Parse(content);

                var str = o.SelectToken(@"$.main");
                this.temp = str.ToString();
                var weatherInfo = JsonConvert.DeserializeObject<WeatherInfo>(str.ToString());

                this.Temp = "Current temp is " + weatherInfo.temp.ToString() + "C";
                this.TempMin = "Min temp is " + weatherInfo.temp_min.ToString() + "C";
                this.TempMax = "Max temp is " + weatherInfo.temp_max.ToString() + "C";

            }
            catch (Exception e)
            {
                this.temp = e.Message;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
