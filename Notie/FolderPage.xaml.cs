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
    public partial class FolderPage : ContentPage
    {
        Folder folder;
        public FolderPage(Folder f)
        {
            InitializeComponent();
            folder = f;
            BindingContext = f;
        }

        private void SaveFolder(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(folder.Name))
            {
                if (App.Database.GetFolderByName(folder.Name) == null)
                {
                    App.Database.SaveFolder(folder);
                    this.Navigation.PopModalAsync();
                }
                else
                {
                    DisplayAlert("Ошибка!", "Папка с таким именем уже существует. Пожалуйста, введите другое имя.", "ОК");
                }
            }
            else
            {
                DisplayAlert("Ошибка!", "Чтобы сохранить, пожалуйста, введите имя.", "ОК");
            }
        }

        async void DeleteFolder(object sender, EventArgs e)
        {
            var answer = await DisplayAlert("Удалить папку?", "Папка удалится безвозвратно", "Да", "Нет");

            if (answer == true)
            {
                App.Database.DeleteFolder(folder.Id);
                this.Navigation.PopModalAsync();
                this.Navigation.PopModalAsync();
            }

        }

        private void Cancel(object sender, EventArgs e)
        {
            this.Navigation.PopModalAsync();
        }
    }
}
