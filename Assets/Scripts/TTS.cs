using System;
using System.Collections;
using UnityEngine;

public abstract class TTS : MonoBehaviour
{
    public abstract IEnumerator CreateService();
    public abstract void TextToSpeech(string output);

    private Animation_Controller _animController;

    void Awake()
    {
        _animController = GetComponent<Animation_Controller>();
    }

    public void SendToModel(string text, AudioClip clip)
    {
        _animController.Animate(text, clip);
    }

}
