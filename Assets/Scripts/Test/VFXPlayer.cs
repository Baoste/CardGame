using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFXPlayer : MonoBehaviour
{
    public GameObject musicRhythmVFX;

    private GameObject musicRhythmVFXPlayer = null;

    public void PlayMusicRhythmVFX()
    {
        if (musicRhythmVFXPlayer == null)
        {
            Debug.Log("Play");
            musicRhythmVFXPlayer = Instantiate(musicRhythmVFX, transform.position, Quaternion.identity);
            musicRhythmVFXPlayer.transform.parent = transform;
            musicRhythmVFXPlayer.transform.position += Vector3.up * 0.3f;
        }
        else
        {
            Debug.Log("Stop");
            Destroy(musicRhythmVFXPlayer);
            musicRhythmVFXPlayer = null;
        }
    }
}
