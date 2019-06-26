using System.Collections;
using UnityEngine;

public class Samantha : MonoBehaviour
{
    /// <summary>
    /// The assistant services which converts the speech to text, analyzes it in the cloud and sends a result
    /// </summary>
    private Assistant _assistantService;
    private STT _sttService;
    private TTS _ttsService;

    /// <summary>
    /// All Necessary Components
    /// </summary>
    void Awake()
    { 
        _assistantService = GetComponent<Assistant_Watson>();
        _sttService       = GetComponent<STT_Watson>();
        _ttsService       = GetComponent<TTS_Watson>();
    }

    void Start()
    {
        StartCoroutine(CreateServices());
    }

    private IEnumerator CreateServices()
    {
        // TTS Service
        if (_ttsService != null)
            yield return _ttsService.CreateService();
        else
            Debug.Log("Can not Create TTS");

        // Assistant Service
        if (_assistantService != null)
            yield return _assistantService.CreateService();
        else
            Debug.Log("Can not Create Assistant");

        // STT Service
        if (_sttService != null)
            yield return _sttService.CreateService();
        else
            Debug.Log("Can not Create STT");
    }
}
