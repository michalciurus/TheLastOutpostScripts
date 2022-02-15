using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PlaySoundOnTrigger : MonoBehaviour
{
    public string soundName;
    public float time = 0;

    // Start is called before the first frame update
    void Start()
    {
        var audioSource = GetComponent<AudioSource>();
        audioSource.clip = (AudioClip) Resources.Load("Sounds/" + soundName);
        audioSource.time = time;
    }

    public void TriggerSound(bool turnOn)
    {
        var audioSource = GetComponent<AudioSource>();

        if (turnOn)
        {
            audioSource.Play();
        }
        else
        {
            audioSource.Stop();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
