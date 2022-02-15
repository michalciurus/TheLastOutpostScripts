using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySoundInSpace : MonoBehaviour
{
    public string soundName;


    public void Play()
    {
        AudioSource.PlayClipAtPoint((AudioClip) Resources.Load("Sounds/" + soundName), transform.position);
    }

}
