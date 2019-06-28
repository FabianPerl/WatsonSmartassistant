using UnityEngine;
using System.Collections;
using IBM.Cloud.SDK;
using IBM.Cloud.SDK.Utilities;
using IBM.Cloud.SDK.DataTypes;
using IBM.Watson.SpeechToText.V1;

public class STT_Watson : STT
{
    private readonly string _serviceUrl = "https://gateway-lon.watsonplatform.net/speech-to-text/api";

    /// <summary>
    /// The necessary apikey.
    /// </summary>
    [Tooltip("STT IAM apikey")]
    [SerializeField]
    private string _iamApikey;

    private string _recognizeModel;

    private int _recordingRoutine = 0;

    // Empty is the default mic
    private string _microphoneID = string.Empty;
    private AudioClip _recording;
    private int _recordingBufferSize = 1;
    private int _recordingHZ;
    private Assistant _assistantService;
    private SpeechToTextService _service;
    private bool _stopListeningFlag = false;

    void Awake()
    {
        int unused;

        if (Microphone.devices.Length > 0)
            Microphone.GetDeviceCaps(_microphoneID, out unused, out _recordingHZ);
        else
            Debug.Log("No Microphone initialized");
    }

    void Start()
    {
        LogSystem.InstallDefaultReactors();
        _assistantService = GetComponent<Assistant_Watson>();
    }

    public override IEnumerator CreateService()
    {
        //  Create credential and instantiate service
        Credentials credentials = null;

        //  Authenticate using iamApikey
        TokenOptions tokenOptions = new TokenOptions()
        {
            IamApiKey = _iamApikey
        };

        credentials = new Credentials(tokenOptions, _serviceUrl);

        //  Wait for tokendata
        while (!credentials.HasIamTokenData())
            yield return null;

        _service = new SpeechToTextService(credentials)
        {
            StreamMultipart = true
        };

        Debug.Log("Created STT Service!");
        OnListen();
    }

    public override bool Active
    {
        get { return _service.IsListening; }
        set
        {
            if (value && !_service.IsListening)
            {
                _service.RecognizeModel = (string.IsNullOrEmpty(_recognizeModel) ? "en-US_BroadbandModel" : _recognizeModel);
                _service.DetectSilence = true;
                _service.EnableWordConfidence = true;
                _service.EnableTimestamps = true;
                _service.SilenceThreshold = 0.01f;
                _service.MaxAlternatives = 1;
                _service.EnableInterimResults = true;
                _service.OnError = OnError;
                _service.InactivityTimeout = -1;
                _service.ProfanityFilter = false;
                _service.SmartFormatting = true;
                _service.SpeakerLabels = false;
                _service.WordAlternativesThreshold = null;
                _service.StartListening(OnRecognize, OnRecognizeSpeaker);
            }
            else if (!value && _service.IsListening)
            {
                _service.StopListening();
            }
        }
    }

    private void StartRecording()
    {
        if (_recordingRoutine == 0)
        {
            UnityObjectUtil.StartDestroyQueue();
            _recordingRoutine = Runnable.Run(RecordingHandler());
        }
    }

    private void StopRecording()
    {
        if (_recordingRoutine != 0)
        {
            Microphone.End(_microphoneID);
            Runnable.Stop(_recordingRoutine);
            _recordingRoutine = 0;
        }
    }

    private void OnError(string error)
    {
        if (!string.IsNullOrEmpty(error))
        {
            Active = false;
            Debug.LogError(error);
        }
    }

    public override void RecordAgain()
    {
        Debug.Log("Played Audio received from Watson Text To Speech");
        if (!_stopListeningFlag)
        {
            OnListen();
        }
    }

    private void OnListen()
    {
        Debug.Log("Start Recording");
        Active = true;
        StartRecording();
    }

    private IEnumerator RecordingHandler()
    {
        _recording = Microphone.Start(_microphoneID, true, _recordingBufferSize, _recordingHZ);
        yield return null;

        if (_recording == null)
        {
            StopRecording();
            yield break;
        }

        bool bFirstBlock = true;
        int midPoint = _recording.samples / 2;
        float[] samples = null;

        while (_recordingRoutine != 0 && _recording != null)
        {
            int writePos = Microphone.GetPosition(_microphoneID);
            if (writePos > _recording.samples || !Microphone.IsRecording(_microphoneID))
            {
                Debug.LogError("Microphone disconnected");

                StopRecording();
                yield break;
            }

            if ((bFirstBlock && writePos >= midPoint)
              || (!bFirstBlock && writePos < midPoint))
            {
                // front block is recorded, make a RecordClip and pass it onto our callback.
                samples = new float[midPoint];
                _recording.GetData(samples, bFirstBlock ? 0 : midPoint);

                AudioData record = new AudioData
                {
                    MaxLevel = Mathf.Max(Mathf.Abs(Mathf.Min(samples)), Mathf.Max(samples)),
                    Clip = AudioClip.Create("Recording", midPoint, _recording.channels, _recordingHZ, false)
                };

                record.Clip.SetData(samples, 0);

                _service.OnListen(record);
                bFirstBlock = !bFirstBlock;
            }
            else
            {
                // calculate the number of samples remaining until we ready for a block of audio, 
                // and wait that amount of time it will take to record.
                int remaining = bFirstBlock ? (midPoint - writePos) : (_recording.samples - writePos);
                float timeRemaining = (float)remaining / (float)_recordingHZ;

                yield return new WaitForSeconds(timeRemaining);
            }

        }

        yield break;
    }

    private void OnRecognize(SpeechRecognitionEvent result)
    {
        if (result != null)
        {
            foreach (var res in result.results)
            {
                foreach (var alt in res.alternatives)
                {
                    if (res.final && alt.confidence > 0)
                    {
                        StopRecording();
                        string text = alt.transcript;
                        Debug.Log("Watson hears : " + text + " Confidence: " + alt.confidence);
                        _assistantService.AnswerRequest(text);
                    }
                }
            }
        }
    }

    private void OnRecognizeSpeaker(SpeakerRecognitionEvent result)
    {
        if (result != null)
        {
            foreach (SpeakerLabelsResult labelResult in result.speaker_labels)
            {
                Debug.Log("Confidence: " + labelResult.confidence + ", From: " + labelResult.from + ", to: " + labelResult.to + ", final: " + labelResult.final);
            }
        }
    }
}
