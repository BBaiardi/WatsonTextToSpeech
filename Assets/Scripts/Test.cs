using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IBM.Watson.TextToSpeech.V1;
using IBM.Cloud.SDK;
using IBM.Cloud.SDK.Utilities;

public class Test : MonoBehaviour
{
    [Header("IAM Auth")]
    [SerializeField]
    private string TextToSpeechIamApiKey;

    private TextToSpeechService textToSpeech;

    // Start is called before the first frame update
    void Start()
    {
        Runnable.Run(CredentialCheck());
    }

    public IEnumerator CredentialCheck()
    {
        Credentials credentials = null;
        TokenOptions tokenOptions = new TokenOptions()
        {
            IamApiKey = TextToSpeechIamApiKey
        };
        credentials = new Credentials(tokenOptions);
        while (!credentials.HasIamTokenData())
        {
            yield return null;
        }
        textToSpeech = new TextToSpeechService(credentials);
        Runnable.Run(CallTextToSpeech("Fábrica de transformadores Siemens Colombia"));
    }

    public IEnumerator CallTextToSpeech(string outputText)
    {
        byte[] synthesizeResponse = null;
        AudioClip clip = null;
        textToSpeech.Synthesize(
            callback: (DetailedResponse<byte[]> response, IBMError error) =>
            {
                synthesizeResponse = response.Result;
                clip = WaveFile.ParseWAV("myClip", synthesizeResponse);
                PlayClip(clip);
            },
            text: outputText,
            voice: "es-LA_SofiaVoice",
            accept: "audio/wav"
            );
        while (synthesizeResponse == null)
        {
            yield return null;
        }
        yield return new WaitForSeconds(clip.length);
    }

    private void PlayClip(AudioClip clip)
    {
        if (Application.isPlaying && clip != null)
        {
            GameObject audioObject = new GameObject("AudioObject");
            AudioSource source = audioObject.AddComponent<AudioSource>();
            source.loop = false;
            source.clip = clip;
            source.Play();
            GameObject.Destroy(audioObject, clip.length);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
