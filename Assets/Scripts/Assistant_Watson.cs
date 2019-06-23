using System.Collections;
using IBM.Cloud.SDK;
using IBM.Watson.Assistant.V2;
using IBM.Watson.Assistant.V2.Model;
using UnityEngine;

public class Assistant_Watson : Assistant
{
    private readonly string _serviceUrl = "https://gateway-lon.watsonplatform.net/assistant/api";

    /// <summary>
    /// The assistant identifier. Is needed for each assistant
    /// </summary>
    [Tooltip("The assistant ID")]
    [SerializeField]
    private string _assistantId;

    [Tooltip("The IAM apikey")]
    [SerializeField]
    private string _iamApikey;

    private AssistantService _service;
    private TTS _ttsService;
    private bool _sessionCreated = false;
    private string _sessionId;

    void Start()
    {
        LogSystem.InstallDefaultReactors();
        _ttsService = GetComponent<TTS_Watson>();
    }

    public override IEnumerator CreateService()
    {
        Credentials assistant_credentials = null;

        TokenOptions tokenOptions = new TokenOptions()
        {
            IamApiKey = _iamApikey
        };

        assistant_credentials = new Credentials(tokenOptions, _serviceUrl);

        // Wait for tokendata
        while (!assistant_credentials.HasIamTokenData())
            yield return null;

        _service = new AssistantService("2019-02-28", assistant_credentials);
        _service.CreateSession(OnCreateSession, _assistantId);

        // wait till session is created
        while (!_sessionCreated)
            yield return null;

        Debug.Log("Created Assistant Service!");
    }

    public override void AnswerRequest(string spokenText)
    {
        if (!string.IsNullOrEmpty(spokenText))
        {
            var input = new MessageInput()
            {
                Text = spokenText
            };

            _service.Message(OnMessage, _assistantId, _sessionId, input);
        }
    }

    private void OnMessage(DetailedResponse<MessageResponse> response, IBMError error)
    {
        if (response != null)
        {
            //getIntent
            if (response.Result.Output.Intents.Count > 0)
            {
                string intent = response.Result.Output.Intents[0].Intent;
                Debug.Log(intent);
            }

            //get Watson Output
            if (response.Result.Output.Generic.Count > 0)
            {
                string outputText2 = response.Result.Output.Generic[0].Text;
                Debug.Log(outputText2);
                _ttsService.TextToSpeech(outputText2);
            }
        }
    }

    private void OnCreateSession(DetailedResponse<SessionResponse> response, IBMError error)
    {
        if (response != null)
        {
            Debug.Log("Session created: " + response.Result.SessionId);
            _sessionId = response.Result.SessionId;
            _sessionCreated = true;
        }
    }
}
