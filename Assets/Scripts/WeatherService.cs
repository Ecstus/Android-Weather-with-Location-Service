using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using Newtonsoft.Json;
using TMPro;
using System.Linq.Expressions;

public class WeatherService : MonoBehaviour
{
    [SerializeField] private TMP_Text tempText;
    [SerializeField] private TMP_Text conditionText;
    [SerializeField] private TMP_Text cityText;

    public LocationService locationService; // Reference to LocationService script
    private float lat;
    private float lon;

    private static readonly string apiKey = "6575158b2ad9b37c0ae5c151df577b11";
    private static readonly string weatherUrl = "https://api.openweathermap.org/data/2.5/weather?appid=" + apiKey + "&units=metric";
    void Start()
    {
        // Find the LocationService instance in the scene
        locationService = LocationService.Instance;

        // Start fetching weather data using the current location
        StartCoroutine(GetWeatherData());
    }

    IEnumerator GetWeatherData()
    {
        conditionText.text = "Getting weather data...";
        yield return new WaitForSeconds(3);

        lat = LocationService.Instance.latitude;
        lon = LocationService.Instance.longitude;

        string url = weatherUrl + "&lat=" + lat.ToString() + "&lon=" + lon.ToString();

        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Error: " + webRequest.error);
                conditionText.text = "Error: " + webRequest.error;
            }
            else
            {
                try
                {
                    WeatherData weatherData = JsonConvert.DeserializeObject<WeatherData>(webRequest.downloadHandler.text);
                    UpdateWeatherUI(weatherData);
                }
                catch (System.Exception e)
                {
                    Debug.LogError("JSON Parse Error: " + e.Message);
                    conditionText.text = "JSON Parse Error: " + e.Message;
                }
            }
        }
    }

    private void UpdateWeatherUI(WeatherData data)
    {
        if (data != null)
        {
            tempText.text = data.main.temp + "°C"; // Temperature now directly in Celsius
            conditionText.text = data.weather[0].description;
            cityText.text = data.name;
        }
    }
}

    [System.Serializable]
public class WeatherData
{
    public MainWeather main;
    public Weather[] weather;
    public string name; // Areaname

    [System.Serializable]
    public class MainWeather
    {
        public string temp;
    }

    [System.Serializable]
    public class Weather
    {
        public string main; // General condition like Rain, Sunny, etc.
        public string description; // More detailed condition description
    }
}
