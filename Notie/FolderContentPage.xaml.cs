using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Notie.Models;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Notie
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FolderContentPage : ContentPage
    {
        Folder folder;
        public FolderContentPage(Folder f)
        {
            InitializeComponent();
            folder = f;
        }

        protected override void OnAppearing()
        {
            notes.ItemsSource = App.Database.GetItemsInFolder(folder);
            base.OnAppearing();
        }

        private void Cancel(object sender, EventArgs e)
        {
            this.Navigation.PopModalAsync();
        }

        private void NoteSelected(object sender, SelectedItemChangedEventArgs e)
        {
            this.Navigation.PushModalAsync(new NotePage((Note)e.SelectedItem));
        }

        private void EditFolder(object sender, EventArgs e)
        {
            this.Navigation.PushModalAsync(new FolderPage(this.folder));
        }
    }
}
