using DotnetPublishBug.Models;
using DotnetPublishBug.PageModels;

namespace DotnetPublishBug.Pages
{
    public partial class MainPage : ContentPage
    {
        public MainPage(MainPageModel model)
        {
            InitializeComponent();
            BindingContext = model;
        }
    }
}