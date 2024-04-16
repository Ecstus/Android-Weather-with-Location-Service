using UnityEngine;
using System.Collections;
using UnityEngine.Android;
using TMPro;


public class LocationService : MonoBehaviour
{
    public static LocationService Instance { get; private set; }
    [SerializeField] private TMP_Text latText;
    [SerializeField] private TMP_Text lonText;
    [SerializeField] private TMP_Text statusText;

    [SerializeField] public float latitude;
    [SerializeField] public float longitude;
    private bool isGPSActive = false;
    private static readonly object lockObject = new object();

    // Public getter for isGPSActive
    public bool IsGPSActive
    {
        get { return isGPSActive; }
    }

    void Awake()
    {
        lock (lockObject)
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }

    // Start is called before the first frame update
    IEnumerator Start()
    {
        // Check if the user has location service enabled.
        if (!Input.location.isEnabledByUser)
        {
            Debug.Log("Location services are not enabled by the user.");
            statusText.text = "Location services are not enabled by the user.";
            yield return new WaitForSeconds(2);
            yield return RequestLocationPermissions();
        }

        // Inform the user about the need for location permissions.
        statusText.text = "Requesting location permissions...";

        // Start the location service.
        Input.location.Start();

        // Wait until service initializes
        int maxWait = 20; // Wait for 20 seconds.
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            yield return new WaitForSeconds(1);
            maxWait--;
        }

        // Service didn't initialize in 20 seconds.
        if (maxWait < 1)
        {
            Debug.Log("Timed out while waiting for location services to initialize.");
            statusText.text = "Timed out while waiting for location services to initialize.";
            yield break;
        }

        // Connection has failed
        if (Input.location.status == LocationServiceStatus.Failed)
        {
            Debug.Log("Unable to determine device location. Failed.");
            statusText.text = "Unable to determine device location. Failed.";
        }
        else
        {
            Debug.Log("Location service is working.");
            statusText.text = "Location service is working.";
            // Continuously update location
            while (true)
            {
                latitude = Input.location.lastData.latitude;
                longitude = Input.location.lastData.longitude;
                isGPSActive = true;
                yield return new WaitForSeconds(1); // Update every second or as needed
            }
        }
    }

    void Update()
    {
        // Update the text objects with latitude and longitude values
        latText.text = "Latitude: " + latitude.ToString();
        lonText.text = "Longitude: " + longitude.ToString();
    }

    private IEnumerator RequestLocationPermissions()
    {
        if (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
        {
            Permission.RequestUserPermission(Permission.FineLocation);
            // Wait until the user responds to the permission request
            while (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
            {
                yield return null;
            }
        }

        if (!Permission.HasUserAuthorizedPermission(Permission.CoarseLocation))
        {
            Permission.RequestUserPermission(Permission.CoarseLocation);
            // Wait until the user responds to the permission request
            while (!Permission.HasUserAuthorizedPermission(Permission.CoarseLocation))
            {
                yield return null;
            }
        }
    }
}
