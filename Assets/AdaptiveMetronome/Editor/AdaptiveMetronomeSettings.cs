using UnityEngine;
using UnityEditor;
using System.IO;
using System.Xml.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;

public class AdaptiveMetronomeSettingsWindow : EditorWindow
{
    private const int MaxPlayers = 4;
    private Vector2 scrollPosition;

    // New fields for the Master Volume, MIDI file, and Backend Reference
    private float masterVolume = 100f;  // Default Master Volume set to 100
    private string midiFilePath = "";
    private UnityEngine.Object adaptiveMetronomeBackend;  // Reference to Adaptive Metronome Backend

    // Store references to runtime players directly
    private AdaptiveMetronomePlayer[] playerReferences = new AdaptiveMetronomePlayer[MaxPlayers];
    private bool playersCreated = false;

    [MenuItem("Window/Adaptive Metronome")]
    public static void ShowWindow()
    {
        GetWindow<AdaptiveMetronomeSettingsWindow>("Adaptive Metronome Settings");
    }

    private void OnEnable()
    {

    }

    private void OnGUI()
    {
        // Check if AdaptiveMetronomeMain exists in the scene
        if (UnityEngine.Object.FindFirstObjectByType<AdaptiveMetronomeMain>() == null)
        {
            // If not, show the Create button
            if (GUILayout.Button("Create Adaptive Metronome"))
            {
                CreateAdaptiveMetronome();
            }
        }
        else
        {
            // Once AdaptiveMetronomeMain exists, show the rest of the settings
            GUILayout.BeginVertical("box");
            GUILayout.Label("Global Settings", EditorStyles.boldLabel);

            // Master Volume
            masterVolume = EditorGUILayout.Slider("Master Volume", masterVolume, 0f, 100f);

            // Load MIDI File
            if (GUILayout.Button("Load MIDI File"))
            {
                midiFilePath = EditorUtility.OpenFilePanel("Load MIDI File", "", "mid,midi");
                if (!string.IsNullOrEmpty(midiFilePath))
                {
                    Debug.Log("MIDI File Loaded: " + midiFilePath);
                }
            }
            GUILayout.Label("MIDI File Path: " + midiFilePath);

            // Reference to Adaptive Metronome Backend
            adaptiveMetronomeBackend = EditorGUILayout.ObjectField("Adaptive Metronome Script", adaptiveMetronomeBackend, typeof(UnityEngine.Object), true);

            GUILayout.EndVertical();
            GUILayout.Space(20);  // Add space between global settings and players

            // Check if all players exist, and if not, show the "Create Players" button
            bool allPlayersExist = playerReferences.Length == MaxPlayers && playerReferences.All(player => player != null);

            if (!allPlayersExist && GUILayout.Button("Create Players"))
            {
                CreatePlayers();
            }

            // Display player parameters once players are created
            if (playersCreated)
            {
                GUILayout.Label("Player Parameters", EditorStyles.boldLabel);
                scrollPosition = GUILayout.BeginScrollView(scrollPosition, false, true);

                for (int i = 0; i < MaxPlayers; i++)
                {
                    GUILayout.BeginVertical("box");
                    GUILayout.Label($"Player {i + 1}", EditorStyles.boldLabel);

                    // Store the reference of the player object in this GUI section
                    playerReferences[i] = (AdaptiveMetronomePlayer)EditorGUILayout.ObjectField("Player Object", playerReferences[i], typeof(AdaptiveMetronomePlayer), true);

                    if (playerReferences[i] != null)
                    {

                        playerReferences[i].playerData.volume = EditorGUILayout.Slider("Volume", playerReferences[i].playerData.volume, 0f, 1f);
                        playerReferences[i].playerData.motorNoiseSTD = EditorGUILayout.FloatField("Motor Noise STD", playerReferences[i].playerData.motorNoiseSTD);
                        playerReferences[i].playerData.timeKeeperNoiseSTD = EditorGUILayout.FloatField("Time Keeper Noise STD", playerReferences[i].playerData.timeKeeperNoiseSTD);

                        GUILayout.Label("Alpha and Beta Pairs");
                        for (int j = 0; j < 4; j++)
                        {
                            GUILayout.BeginHorizontal();
                            playerReferences[i].playerData.alphaBetaPairs[j].x = EditorGUILayout.FloatField($"Alpha {j + 1}", playerReferences[i].playerData.alphaBetaPairs[j].x);
                            playerReferences[i].playerData.alphaBetaPairs[j].y = EditorGUILayout.FloatField($"Beta {j + 1}", playerReferences[i].playerData.alphaBetaPairs[j].y);
                            GUILayout.EndHorizontal();
                        }
                    }

                    GUILayout.EndVertical();
                    GUILayout.Space(10);
                }

                GUILayout.EndScrollView();
            }
        }
    }

    private void CreateAdaptiveMetronome()
    {
        // Create AdaptiveMetronomeMain and Player GameObjects
        GameObject newGameObject = new GameObject("Adaptive Metronome");
        AdaptiveMetronomeMain adaptiveMetronome = newGameObject.AddComponent<AdaptiveMetronomeMain>();
        EditorPrefs.SetBool("AdaptiveMetronomeCreated", true);  // Mark as created in EditorPrefs
        Debug.Log("Created new AdaptiveMetronomeMain GameObject.");

        // Assign the created AdaptiveMetronomeMain to the backend reference
        adaptiveMetronomeBackend = adaptiveMetronome;

        // Assign player references to AdaptiveMetronomeMain
        adaptiveMetronome.playerReferences = playerReferences;

        // Ensure that the player references are set on the AdaptiveMetronomeMain
        Debug.Log("Assigned player references to AdaptiveMetronomeMain.");
    }


    private void CreatePlayers()
    {
        // Create Player GameObjects and assign data
        for (int i = 0; i < MaxPlayers; i++)
        {
            GameObject playerObject = new GameObject($"Player {i + 1}");
            AdaptiveMetronomePlayer player = playerObject.AddComponent<AdaptiveMetronomePlayer>();

            // Initialise player data
            player.playerData.volume = 1f;  // Set default volume to 1
            player.playerData.motorNoiseSTD = 0.25f;  // Set default motorNoiseSTD to 0
            player.playerData.timeKeeperNoiseSTD = 0.25f;  // Set default timeKeeperNoiseSTD to 0

            // Set each alpha and beta pair to 0.1
            for (int j = 0; j < 4; j++)
            {
                player.playerData.alphaBetaPairs[j].x = 0.1f;
                player.playerData.alphaBetaPairs[j].y = 0.1f;
            }

            // Assign the created player to the playerReferences array
            playerReferences[i] = player;

            Debug.Log($"Created Player {i + 1}");
        }

        playersCreated = true;  // Mark players as created
    }

}
