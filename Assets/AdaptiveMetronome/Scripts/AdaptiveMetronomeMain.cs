using UnityEngine;

public class AdaptiveMetronomeMain : MonoBehaviour
{
    // This will be populated from the AdaptiveMetronomeSettingsWindow
    public AdaptiveMetronomePlayer[] playerReferences;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (playerReferences != null && playerReferences.Length > 0)
        {
            Debug.Log($"There are {playerReferences.Length} player references assigned.");
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Logic to update metronome functionality or player actions can be placed here
    }
}
