using UnityEngine;
using TMPro;  // For using Text Mesh Pro
using System;
using System.Collections;

public class DigitalClock : MonoBehaviour
{
    public TMP_Text clockText;  // Assign this in the inspector
    private bool showColon = true;  // This will determine whether to show or hide the colon

    void Start()
    {
        if (clockText == null)
        {
            Debug.LogError("Clock Text component not assigned!");
            this.enabled = false;  // Disable script if no text component is assigned
            return;
        }

        // Start the clock update coroutine
        StartCoroutine(UpdateClock());
    }

    IEnumerator UpdateClock()
    {
        while (true)
        {
            // Get the current time
            DateTime now = DateTime.Now;
            // Format the time based on whether `showColon` is true or false
            clockText.text = now.ToString(showColon ? "HH:mm":"HH mm");
            // Toggle the colon display
            showColon = !showColon;

            // Wait for one second before updating the clock again
            yield return new WaitForSeconds(0.5f);
        }
    }
}
