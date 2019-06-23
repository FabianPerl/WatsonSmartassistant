using System.Collections;
using UnityEngine;

public abstract class TTS : MonoBehaviour
{
    public abstract IEnumerator CreateService();
    public abstract void TextToSpeech(string output);

    public void SendToModel(string text, float time)
    {
    }
}
