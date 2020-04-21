using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Notie.Models;
using Xamarin.Forms;

namespace Notie
{
    public partial class MainPage : ContentPage
    {
        WeatherViewModel mvm;
        public MainPage()
        {
            InitializeComponent();
            mvm = new WeatherViewModel();
            BindingContext = mvm;
            mvm.LoadDataCommand.Execute(null);
        }

        protected override void OnAppearing()
        {
            notes.ItemsSource = App.Database.GetItems();
            folders.ItemsSource = App.Database.GetFolders();
            base.OnAppearing();
        }

        private void NoteSelected(object sender, SelectedItemChangedEventArgs e)
        {
            Navigation.PushModalAsync(new NotePage((Note)e.SelectedItem));
        }

        private void FolderSelected(object sender, SelectedItemChangedEventArgs e)
        {
            Navigation.PushModalAsync(new FolderContentPage((Folder)e.SelectedItem));
        }

        private void AddNote_Clicked(object sender, EventArgs e)
        {
            Navigation.PushModalAsync(new NotePage(new Note()));
        }

        private void AddFolder_Clicked(object sender, EventArgs e)
        {
            Navigation.PushModalAsync(new FolderPage(new Folder()));
        }
    }
}
