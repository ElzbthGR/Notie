using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Notie.Models;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Forms.Maps;
using Xamarin.Essentials;
using Plugin.AudioRecorder;
using Plugin.Media;
using Plugin.Media.Abstractions;

namespace Notie
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NotePage : ContentPage
    {
        Note note;
        string audiofile = "";
        Xamarin.Forms.Maps.Map map;
        AudioRecorderService recorder;
        AudioPlayer player;
        public NotePage(Note nt)
        {
            InitializeComponent();
            note = nt;
            BindingContext = note;
            recorder = new AudioRecorderService();
            player = new AudioPlayer();

            if (note.Audio != null)
            {
                audiofile = note.Audio;
                listen.IsVisible = true;
                play.IsVisible = false;
                stop.IsVisible = false;
                deleteAudio.IsVisible = true;
            }

            if (note.Image != null)
            {
                takePhoto.IsVisible = false;
                galleryPhoto.IsVisible = false;
                deleteImage.IsVisible = true;
            }

            foreach (Folder fol in App.Database.GetFolders())
            {
                foldersList.Items.Add(fol.Name);
            }

            foldersList.Items.Add("None");

            if (note.Folder_Id != 0 && App.Database.GetFolder(note.Folder_Id) != null)
            {
                foldersList.SelectedItem = App.Database.GetFolder(note.Folder_Id).Name;
                DisplayAlert("", foldersList.SelectedIndex.ToString(), "OK");
            }
            else
            {
                foldersList.SelectedItem = "None";
            }

            if (note.Latitude != null && note.Longitude != null)
            {
                var position = new Position(Convert.ToDouble(note.Latitude), Convert.ToDouble(note.Longitude));
                map = new Xamarin.Forms.Maps.Map(MapSpan.FromCenterAndRadius(position, Distance.FromMiles(0.3)))
                {
                    IsShowingUser = true,
                    HeightRequest = 150,
                    WidthRequest = 960,
                    VerticalOptions = LayoutOptions.FillAndExpand
                };

                var pin = new Pin
                {
                    Type = PinType.Place,
                    Position = position,
                    Label = "Вы здесь"
                };
                map.Pins.Add(pin);
                mapGeo.Children.Add(map);
                mapGeo.IsVisible = true;
                addGeo.IsVisible = false;
                deleteGeo.IsVisible = true;
            }
        }

        private void SaveNote(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(note.Title))
            {
                if (audiofile != "")
                {
                    note.Audio = audiofile;
                }

                if (foldersList.SelectedItem.ToString() != "None")
                {
                    note.Folder_Id = App.Database.GetFolderByName(foldersList.SelectedItem.ToString()).Id;
                }
                else
                {
                    note.Folder_Id = 0;
                }

                App.Database.SaveItem(note);
            }
            this.Navigation.PopModalAsync();
        }

        async void DeleteNote(object sender, EventArgs e)
        {
            if (await DisplayAlert("Удалить заметку?", "Заметка удалится безвозвратно", "Да", "Нет"))
            {
                App.Database.DeleteItem(note.Id);
                await this.Navigation.PopModalAsync();
            }
        }

        private void Cancel(object sender, EventArgs e)
        {
            this.Navigation.PopModalAsync();
        }

        async void RecordAudio_Button(object sender, EventArgs e)
        {
            play.IsVisible = false;
            stop.IsVisible = true;
            listen.IsVisible = false;
            deleteAudio.IsVisible = false;
            await recorder.StartRecording();
        }

        async void StopAudio_Button(object sender, EventArgs e)
        {
            await recorder.StopRecording();
            audiofile = recorder.FilePath;
            listen.IsVisible = true;
            stop.IsVisible = false;
            deleteAudio.IsVisible = true;
        }

        private void ListenAudio_Button(object sender, EventArgs e)
        {
            player.Play(audiofile);
        }

        async void AddPhoto_Button(object sender, EventArgs e)
        {
            if (CrossMedia.Current.IsPickPhotoSupported)
            {
                MediaFile photo = await CrossMedia.Current.PickPhotoAsync();
                image.Source = ImageSource.FromFile(photo.Path);
                note.Image = photo.Path;
                takePhoto.IsVisible = false;
                galleryPhoto.IsVisible = false;
                deleteImage.IsVisible = true;
            }
        }

        async void TakePhoto_Button(object sender, EventArgs e)
        {
            if (CrossMedia.Current.IsCameraAvailable && CrossMedia.Current.IsTakePhotoSupported)
            {
                MediaFile file = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
                {
                    SaveToAlbum = true,
                    Directory = "Photos",
                    Name = $"{DateTime.Now.ToString("dd.MM.yyyy_hh.mm.ss")}.jpg"
                });

                if (file == null)
                    return;
                image.Source = ImageSource.FromFile(file.Path);
                note.Image = file.Path;
                takePhoto.IsVisible = false;
                galleryPhoto.IsVisible = false;
                deleteImage.IsVisible = true;
            }
            else
            {
                await DisplayAlert("Error", "Camera is not available", "OK");
            }
        }

        async void DeleteAudio_Button(object sender, EventArgs e)
        {
            if (await DisplayAlert("Удалить аудио?", "Оно нигде не сохранится", "Да", "Нет"))
            {
                File.Delete(audiofile);
                audiofile = "";
                note.Audio = null;
                deleteAudio.IsVisible = false;
                play.IsVisible = true;
                listen.IsVisible = false;
            }
        }

        async void DeleteImage_Button(object sender, EventArgs e)
        {
            if (await DisplayAlert("Удалить фото?", "Придётся добавлять заново", "Да", "Нет"))
            {
                image.IsVisible = false;
                image.Source = "";
                note.Image = null;
                takePhoto.IsVisible = true;
                galleryPhoto.IsVisible = true;
                deleteImage.IsVisible = false;
            }
        }

        async void AddGeolocation(object sender, EventArgs e)
        {
            try
            {
                var request = new GeolocationRequest(GeolocationAccuracy.Medium);
                var location = await Geolocation.GetLocationAsync(request);

                if (location != null)
                {
                    note.Latitude = location.Latitude.ToString();
                    note.Longitude = location.Longitude.ToString();
                    addGeo.IsVisible = false;
                    deleteGeo.IsVisible = true;
                }

                var position = new Position(location.Latitude, location.Longitude); 
                map = new Xamarin.Forms.Maps.Map(MapSpan.FromCenterAndRadius(position, Distance.FromMiles(0.3)))
                {
                    IsShowingUser = true,
                    HeightRequest = 150,
                    WidthRequest = 960,
                    VerticalOptions = LayoutOptions.FillAndExpand
                };

                var pin = new Pin
                {
                    Type = PinType.Place,
                    Position = position,
                    Label = "Вы здесь"
                };
                map.Pins.Add(pin);
                mapGeo.Children.Add(map);
                mapGeo.IsVisible = true;
            }
            catch
            {
                geoloc.IsVisible = true;
            }
        }

        async void DeleteGeolocation(object sender, EventArgs e)
        {
            if (await DisplayAlert("Удалить геометку?", "Вы уверены, что этого хотите?", "Да", "Нет"))
            {
                mapGeo.Children.Remove(map);
                map = null;
                note.Latitude = null;
                note.Longitude = null;
                geoloc.IsVisible = false;
                addGeo.IsVisible = true;
                deleteGeo.IsVisible = false;
            }
        }
    }
}
