using System;
using System.Collections;
using UnityEngine;

public abstract class TTS : MonoBehaviour
{
    public abstract IEnumerator CreateService();
    public abstract void TextToSpeech(string output);

    AnimationController animController;

    void Awake()
    {
        animController = GetComponent<AnimationController>();
    }

    public void SendToModel(string text, float time)
    {
        animController.speak(text, time);
    }

}
