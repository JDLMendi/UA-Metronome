using UnityEngine;

public class AdaptiveMetronomeMain : MonoBehaviour
{
    // This will be populated from the AdaptiveMetronomeSettingsWindow
    public AdaptiveMetronomePlayer[] playerReferences;
    public int numIntroTones;
    public float volume;
    public string midiFile;

    // Audio clip for the intro tone (you can assign it in the Unity Inspector)
    public AudioClip introToneClip;
    private AudioSource audioSource;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (playerReferences != null && playerReferences.Length > 0)
        {
            Debug.Log($"There are {playerReferences.Length} player references assigned.");
        }

        // Set up the AudioSource component
        audioSource = GetComponent<AudioSource>();

        if (audioSource == null)
        {
            // If there is no AudioSource component, add one
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // Set the volume of the AudioSource based on the 'volume' variable
        audioSource.volume = volume / 100f; // Assuming volume is between 0 and 100 in the editor
    }

    // Update is called once per frame
    void Update()
    {
        // Check if the space bar is pressed and call the PlayIntroTones function
        if (Input.GetKeyDown(KeyCode.Space))
        {
            PlayIntroTones();
        }
    }

    // Function to play the intro tones
    private void PlayIntroTones()
    {
        if (introToneClip != null && numIntroTones > 0)
        {
            StartCoroutine(PlayIntroToneSequence());
        }
        else
        {
            Debug.LogWarning("Intro tone clip is not assigned or numIntroTones is zero.");
        }
    }

    // Coroutine to play the intro tones one by one with a delay between each
    private System.Collections.IEnumerator PlayIntroToneSequence()
    {
        for (int i = 0; i < numIntroTones; i++)
        {
            audioSource.PlayOneShot(introToneClip); // Play the intro tone once
            yield return new WaitForSeconds(1f); // Wait for the duration of the sound clip before playing the next tone
        }
    }
}
