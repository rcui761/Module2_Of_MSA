using Plugin.Media;
using Plugin.Media.Abstractions;
using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Xamarin.Forms;
using Newtonsoft.Json.Linq;
using System.Linq;
using Newtonsoft.Json;
using Plugin.Geolocator;
using System.Diagnostics;

namespace Tabs
{
    public partial class CustomVision : ContentPage
    {
        public CustomVision()
        {
            InitializeComponent();
        }

        private async void loadCamera(object sender, EventArgs e)
        {
            await CrossMedia.Current.Initialize();

            if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
            {
                await DisplayAlert("No Camera", ":( No camera available.", "OK");
                return;
            }

            MediaFile file = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
            {
                PhotoSize = PhotoSize.Medium,
                Directory = "Sample",
                Name = $"{DateTime.UtcNow}.jpg"
            });

            if (file == null)
                return;

            image.Source = ImageSource.FromStream(() =>
            {
                return file.GetStream();
            });
            await postLocationAsync();


            await MakePredictionRequest(file);

        }




        private async void ChoosePicture(object sender, EventArgs e)
        {

            await CrossMedia.Current.Initialize();

            if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsPickPhotoSupported)
            {
                await DisplayAlert("No Photo Could Choose", ":( No Photo available.", "OK");
                return;
            }


            MediaFile file = await CrossMedia.Current.PickPhotoAsync(null);

            if (file == null)
                return;

            // Assembly a = null;
            // image.Source = ImageSource.FromResource(file.Path, a);
            //await postLocationAsync();
            image.Source = ImageSource.FromStream(() =>
            {
                return file.GetStream();
            });

            await postLocationAsync();
            await MakePredictionRequest(file);

        }






        static byte[] GetImageAsByteArray(MediaFile file)
        {
            var stream = file.GetStream();
            BinaryReader binaryReader = new BinaryReader(stream);
            return binaryReader.ReadBytes((int)stream.Length);


        }








        async Task MakePredictionRequest(MediaFile file)
        {
            var client = new HttpClient();

            client.DefaultRequestHeaders.Add("Prediction-Key", "2b785ad623f047a0a511d9e14d9e7f2c");

            string url = "https://southcentralus.api.cognitive.microsoft.com/customvision/v1.0/Prediction/226891aa-f1db-4015-a7a4-c9c383f3e158/image?iterationId=91ae0a9d-0b1d-447a-883a-76f8baf6de2e";

            HttpResponseMessage response;

            byte[] byteData = GetImageAsByteArray(file);


            using (var content = new ByteArrayContent(byteData))
            {

                content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                response = await client.PostAsync(url, content);


                if (response.IsSuccessStatusCode)
                {
                    TagLabel.Text = "";
                    PredictionLabel.Text = "";

                    var responseString = await response.Content.ReadAsStringAsync();
                    EvaluationModel responseModel = JsonConvert.DeserializeObject<EvaluationModel>(responseString);
                    double max = responseModel.Predictions.Max(m => m.Probability);
                    JObject rss = JObject.Parse(responseString);
                    
                    //Querying with LINQ
                    //Get all Prediction Values
                    var Probability = from p in rss["Predictions"] select (string)p["Probability"];
                    var Tag = from p in rss["Predictions"] select (string)p["Tag"];

                    //Truncate values to labels in XAML
                    foreach (var item in Tag)
                    {
                        TagLabel.Text += item + ": \n";
                    }

                    foreach (var item in Probability)
                    {
                        PredictionLabel.Text += item + "\n";
                    }
                }

                //Get rid of file once we have finished using it
                file.Dispose();
            }
        }




        async Task postLocationAsync()
        {

            try
            {
                var locator = CrossGeolocator.Current;
                locator.DesiredAccuracy = 50;

              
                var position = await locator.GetPositionAsync(TimeSpan.FromSeconds(100));

                kiwifruitmodel model = new kiwifruitmodel()
                {
                    Longitude = (float)position.Longitude,
                    Latitude = (float)position.Latitude

                };

                await AzureManager.AzureManagerInstance.PostkiwifruitInformation(model);
            }
            catch(Exception exception)
            {
                Debug.WriteLine("XXXXXXX"+ exception);
            }
        }
    }
}
