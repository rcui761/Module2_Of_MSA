using System;
using Microsoft.WindowsAzure.MobileServices;
using Newtonsoft.Json;
public class kiwifruitmodel
{
    [JsonProperty(PropertyName = "Id")]
    public string ID { get; set; }

    [JsonProperty(PropertyName = "Longitude")]
    public float Longitude { get; set; }

    [JsonProperty(PropertyName = "Latitude")]
    public float Latitude { get; set; }

    [JsonProperty(PropertyName = "City")]
    public string City { get; set; }
}