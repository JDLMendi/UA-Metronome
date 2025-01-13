using UnityEngine;
using UnityEditor;
using System.Linq;

public class AdaptiveMetronomeSettingsWindow : EditorWindow
{
    private const int MaxPlayers = 4;
    private Vector2 scrollPosition;

    // Metronome Settings 
    private float masterVolume = 100f;
    private string midiFilePath = "";
    private AdaptiveMetronomeMain adaptiveMetronomeScript;
    private int numIntroTones = 4;
    private AdaptiveMetronomePlayer[] playerReferences = new AdaptiveMetronomePlayer[MaxPlayers];
    private bool playersCreated = false;


    [MenuItem("Window/Adaptive Metronome")]
    public static void ShowWindow()
    {
        GetWindow<AdaptiveMetronomeSettingsWindow>("Adaptive Metronome Settings");
    }

    private void OnEnable()
    {
        Debug.Log("Adaptive Metronome has been loaded.");
    }

    private void OnGUI()
    {
        // Main GUI drawing function to display all settings for the Adaptive Metronome.
        DisplayGlobalSettings();
        DisplayPlayerSettings();
    }

    private void DisplayGlobalSettings()
    {
        // Displays global settings for the Adaptive Metronome.
        if (UnityEngine.Object.FindFirstObjectByType<AdaptiveMetronomeMain>() == null)
        {
            // If no AdaptiveMetronomeMain exists in the scene, show the "Create" button.
            if (GUILayout.Button("Create Adaptive Metronome"))
            {
                CreateAdaptiveMetronome();
            }
        }
        else
        {
            // Display Global Settings if the AdaptiveMetronomeMain is already in the scene.
            GUILayout.BeginVertical("box");
            GUILayout.Label("Metronome Settings", EditorStyles.boldLabel);

            DisplayAdaptiveMetronomeScriptField();
            DisplayIntroToneSettings();
            DisplayMasterVolumeSettings();

            GUILayout.Space(10);
            DisplayMidiFileSettings();

            GUILayout.Space(20);

            // // Button to apply the changes made in the global settings.
            // if (GUILayout.Button("Update Global Settings"))
            // {
            //     UpdateSettings();
            // }

            GUILayout.EndVertical();
            GUILayout.Space(20);
        }
    }

    private void DisplayAdaptiveMetronomeScriptField()
    {
        // Displays a field to assign the AdaptiveMetronomeMain script object.
        adaptiveMetronomeScript = (AdaptiveMetronomeMain)EditorGUILayout.ObjectField("Script", adaptiveMetronomeScript, typeof(AdaptiveMetronomeMain), true);
    }

    private void DisplayIntroToneSettings()
    {
        // Displays a field for setting the number of intro tones.
        numIntroTones = EditorGUILayout.IntField("Number of Intro Tones", numIntroTones);
        adaptiveMetronomeScript.numIntroTones = numIntroTones;
    }

    private void DisplayMasterVolumeSettings()
    {
        // Displays a slider for adjusting the master volume.
        masterVolume = EditorGUILayout.Slider("Master Volume", masterVolume, 0f, 100f);
        adaptiveMetronomeScript.volume = masterVolume;
    }

    private void DisplayMidiFileSettings()
    {
        // Displays buttons to load a MIDI file and shows the selected MIDI file path.
        GUILayout.Label("MIDI File");
        if (GUILayout.Button("Load MIDI File"))
        {
            midiFilePath = EditorUtility.OpenFilePanel("Load MIDI File", "", "mid,midi");
            if (!string.IsNullOrEmpty(midiFilePath))
            {
                Debug.Log("Filepath has been loaded to metronome");
                adaptiveMetronomeScript.midiFile = midiFilePath;
            }
        }

        GUILayout.Label("MIDI File Path: " + midiFilePath);
    }

    private void DisplayPlayerSettings()
    {
        // Displays settings related to players and their configuration.
        bool allPlayersExist = playerReferences.Length == MaxPlayers && playerReferences.All(player => player != null);

        if (!allPlayersExist && GUILayout.Button("Create Players"))
        {
            // If not all players exist, display the button to create new players.
            CreatePlayers();
        }

        if (playersCreated)
        {
            GUILayout.Label("Player Parameters", EditorStyles.boldLabel);
            scrollPosition = GUILayout.BeginScrollView(scrollPosition, false, true);

            for (int i = 0; i < MaxPlayers; i++)
            {
                GUILayout.BeginVertical("box");
                GUILayout.Label($"Player {i + 1}", EditorStyles.boldLabel);

                playerReferences[i] = (AdaptiveMetronomePlayer)EditorGUILayout.ObjectField("Player Object", playerReferences[i], typeof(AdaptiveMetronomePlayer), true);

                if (playerReferences[i] != null)
                {
                    // If the player object is not null, display and edit their settings.
                    DisplayPlayerData(playerReferences[i]);
                }

                GUILayout.EndVertical();
                GUILayout.Space(10);
            }

            GUILayout.EndScrollView();
        }
    }

    private void DisplayPlayerData(AdaptiveMetronomePlayer player)
    {
        // Displays settings for each player, including volume, noise levels, and alpha-beta pairs.
        player.playerData.volume = EditorGUILayout.Slider("Volume", player.playerData.volume, 0f, 1f);
        player.playerData.motorNoiseSTD = EditorGUILayout.FloatField("Motor Noise STD", player.playerData.motorNoiseSTD);
        player.playerData.timeKeeperNoiseSTD = EditorGUILayout.FloatField("Time Keeper Noise STD", player.playerData.timeKeeperNoiseSTD);

        GUILayout.Label("Alpha and Beta Pairs");
        for (int j = 0; j < 4; j++)
        {
            GUILayout.BeginHorizontal();
            player.playerData.alphaBetaPairs[j].x = EditorGUILayout.FloatField($"Alpha {j + 1}", player.playerData.alphaBetaPairs[j].x);
            player.playerData.alphaBetaPairs[j].y = EditorGUILayout.FloatField($"Beta {j + 1}", player.playerData.alphaBetaPairs[j].y);
            GUILayout.EndHorizontal();
        }
    }

    private void CreateAdaptiveMetronome()
    {
        // Creates a new AdaptiveMetronomeMain GameObject and assigns it to the script reference.
        GameObject newGameObject = new GameObject("Adaptive Metronome");
        AdaptiveMetronomeMain adaptiveMetronome = newGameObject.AddComponent<AdaptiveMetronomeMain>();
        EditorPrefs.SetBool("AdaptiveMetronomeCreated", true);
        Debug.Log("Created new AdaptiveMetronomeMain GameObject.");

        adaptiveMetronomeScript = adaptiveMetronome;
        adaptiveMetronomeScript.playerReferences = playerReferences;

        Debug.Log("Assigned player references to AdaptiveMetronomeMain.");
    }

    private void CreatePlayers()
    {
        // Creates player GameObjects and initialises their settings.
        for (int i = 0; i < MaxPlayers; i++)
        {
            GameObject playerObject = new GameObject($"Player {i + 1}");
            AdaptiveMetronomePlayer player = playerObject.AddComponent<AdaptiveMetronomePlayer>();

            player.playerData.volume = 1f;
            player.playerData.motorNoiseSTD = 0.25f;
            player.playerData.timeKeeperNoiseSTD = 0.25f;

            for (int j = 0; j < 4; j++)
            {
                player.playerData.alphaBetaPairs[j].x = 0.1f;
                player.playerData.alphaBetaPairs[j].y = 0.1f;
            }

            playerReferences[i] = player;

            Debug.Log($"Created Player {i + 1}");
        }

        playersCreated = true;
    }

    private void UpdateSettings()
    {
        // Updates the global settings in the AdaptiveMetronomeScript.
        adaptiveMetronomeScript.numIntroTones = this.numIntroTones;
        adaptiveMetronomeScript.volume = this.masterVolume;
    }
}
