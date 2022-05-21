using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Audio : MonoBehaviour
{

    public AudioSource source;
    public List<AudioClip> audioClips = new List<AudioClip>();
    // Start is called before the first frame update
    void Start()
    {
        source = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    public void Play(int sound)
    {
        source.clip = audioClips[sound];
        source.Play();
    }
}
