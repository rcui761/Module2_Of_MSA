using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Microsoft.WindowsAzure.MobileServices;

namespace Tabs
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AzureTables : ContentPage
    {
        MobileServiceClient client = AzureManager.AzureManagerInstance.AzureClient;
        public AzureTables()
        {
            InitializeComponent();
        }
        async void Handle_ClickedAsync(object sender, System.EventArgs e)
        {
            System.Diagnostics.Debug.Write("This is kiwi fruid info:");
            List<kiwifruitmodel> kiwifruitInformation = await AzureManager.AzureManagerInstance.GetkiwifruitInformation();
           
            HotDogList.ItemsSource = kiwifruitInformation;
        }
    }
}