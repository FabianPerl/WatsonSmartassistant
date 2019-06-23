using System;
using System.Collections;
using IBM.Cloud.SDK;
using IBM.Cloud.SDK.Utilities;
using IBM.Watson.TextToSpeech.V1;
using UnityEngine;

public class TTS_Watson : TTS
{
    private readonly string _serviceUrl = "https://gateway-lon.watsonplatform.net/text-to-speech/api";

    /// <summary>
    /// The apikey for the assistent.
    /// </summary>
    [Tooltip("The IAM apikey")]
    [SerializeField]
    private string _iamApikey;

    private TextToSpeechService _service;
    private STT _sttService;
    private AudioSource _source;

    void Start()
    {
        LogSystem.InstallDefaultReactors();
        _source = GetComponent<AudioSource>();
        _sttService = GetComponent<STT_Watson>();
        _source.spatialBlend = 0.0f;
        _source.playOnAwake = false;
        _source.volume = 1.0f;
        _source.loop = false;
    }

    public override IEnumerator CreateService()
    {
        // Create credential and instantiate service
        Credentials credentials = null;

        //Authenticate using iamApikey
        TokenOptions tokenOptions = new TokenOptions()
        {
            IamApiKey = _iamApikey
        };

        credentials = new Credentials(tokenOptions, _serviceUrl);

        // Wait for tokendata
        while (!credentials.HasIamTokenData())
            yield return null;

        _service = new TextToSpeechService(credentials);

        Debug.Log("Created TTS Service!");
    }

    public override void TextToSpeech(string outputText)
    {
        Debug.Log("Send to TTS Watson: " + outputText);

        if (!string.IsNullOrEmpty(outputText))
        {
            byte[] synthesizeResponse = null;
            AudioClip clip = null;

            _service.Synthesize(
                callback: (DetailedResponse<byte[]> response, IBMError error) =>
                {
                    if (response != null && response.Result != null && response.Result.Length > 0)
                    {
                        synthesizeResponse = response.Result;
                        clip = WaveFile.ParseWAV("answer", synthesizeResponse);
                        ManageOutput(clip, outputText);
                    }
                },
                text: outputText,
                voice: "en-US_AllisonVoice",
                accept: "audio/wav"
            );
        }
    }

    private void ManageOutput(AudioClip clip, string outputText)
    {
        Debug.Log("Received AudioClip from Watson TTS!");

        if (Application.isPlaying && clip != null)
        {
            _source.clip = clip;
            SendToModel(outputText, _source.clip.length);
            _source.Play();
            
            Runnable.Run(WaitSecondsAndExecute(_source.clip.length));
        }
    }

    private IEnumerator WaitSecondsAndExecute(float length)
    {
        yield return new WaitForSeconds(length);
        _source.Stop();
        _sttService.RecordAgain();
    }
}
