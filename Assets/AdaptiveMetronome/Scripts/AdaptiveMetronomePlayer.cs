using System;using UnityEngine;
using UnityEngine.Events;


[Serializable]
public class AdaptiveMetronomePlayerData
{
    public float volume;
    public float motorNoiseSTD;
    public float timeKeeperNoiseSTD;
    public Vector2[] alphaBetaPairs = new Vector2[4];
}


public class AdaptiveMetronomePlayer : MonoBehaviour
{
    public AdaptiveMetronomePlayerData playerData = new AdaptiveMetronomePlayerData();
    

    [Header("Note Play Event")]
    public UnityEvent onNotePlay;

    private void Start()
    {
        // Ensure the UnityEvent is initialized if it's null
        if (onNotePlay == null)
            onNotePlay = new UnityEvent();
    }

    public AdaptiveMetronomePlayerData ExportData()
    {
        return playerData;
    }

    public void ImportData(AdaptiveMetronomePlayerData newData)
    {
        playerData = newData;
    }

    // This method is called when the player needs to play a note
    public void PlayNote()
    {
        // Call all functions assigned to the onNotePlay UnityEvent
        onNotePlay.Invoke();
    }
}

