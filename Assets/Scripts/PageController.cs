using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class PageController : MonoBehaviour
{
    public VideoPlayer pageVideoPlayer;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void StartVideo() {
        pageVideoPlayer.Play();
    }

    public void StopVideo() { 
        pageVideoPlayer.Stop();
    }
}
