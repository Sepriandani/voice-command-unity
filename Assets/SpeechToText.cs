using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;

public class SpeechToText : MonoBehaviour
{
    public Text OutputText;

    private string speechKey = "3b65334c446f4bd180641ba13a26e9bc";
    private string speechRegion = "eastus";

    static void OutputSpeechRecognitionResult(SpeechRecognitionResult speechRecognitionResult)
    {
        switch (speechRecognitionResult.Reason)
        {
            case ResultReason.RecognizedSpeech:
                Debug.Log($"RECOGNIZED: Text={speechRecognitionResult.Text}");
                break;
            case ResultReason.NoMatch:
                Debug.Log($"NOMATCH: Speech could not be recognized.");
                break;
            case ResultReason.Canceled:
                var cancellation = CancellationDetails.FromResult(speechRecognitionResult);
                Debug.Log($"CANCELED: Reason={cancellation.Reason}");

                if (cancellation.Reason == CancellationReason.Error)
                {
                    Debug.Log($"CANCELED: ErrorCode={cancellation.ErrorCode}");
                    Debug.Log($"CANCELED: ErrorDetails={cancellation.ErrorDetails}");
                    Debug.Log($"CANCELED: Did you set the speech resource key and region values?");
                }
                break;
        }
    }

    static void OutputSpeechSynthesisResult(SpeechSynthesisResult speechSynthesisResult, string text)
    {
        switch (speechSynthesisResult.Reason)
        {
            case ResultReason.SynthesizingAudioCompleted:
                Debug.Log($"Speech synthesized for text: [{text}]");
                break;
            case ResultReason.Canceled:
                var cancellation = SpeechSynthesisCancellationDetails.FromResult(speechSynthesisResult);
                Debug.Log($"CANCELED: Reason={cancellation.Reason}");

                if (cancellation.Reason == CancellationReason.Error)
                {
                    Debug.Log($"CANCELED: ErrorCode={cancellation.ErrorCode}");
                    Debug.Log($"CANCELED: ErrorDetails=[{cancellation.ErrorDetails}]");
                    Debug.Log($"CANCELED: Did you set the speech resource key and region values?");
                }
                break;
            default:
                break;
        }
    }
    void Start()
    {
        OutputText.text = "";
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public async void ButtonOnClick()
    {
        var speechConfig = SpeechConfig.FromSubscription(speechKey, speechRegion);
        speechConfig.SpeechRecognitionLanguage = "id-ID";
        speechConfig.SpeechSynthesisVoiceName = "id-ID-GadisNeural";

        using var audioConfig = AudioConfig.FromDefaultMicrophoneInput();
        using var speechRecognizer = new SpeechRecognizer(speechConfig, audioConfig);

        Debug.Log("Speak into your microphone.");
        OutputText.text = "Katakan sesuatu!";
        var speechRecognitionResult = await speechRecognizer.RecognizeOnceAsync();
        OutputSpeechRecognitionResult(speechRecognitionResult);
        OutputText.text = speechRecognitionResult.Text;

        string text = null;

        if (speechRecognitionResult.Text == "Apa itu teleskop?")
        {
            text = File.ReadAllText("C:\\Users\\user\\Documents\\project-silvi\\voice-command\\materi\\teleskop.txt");
        }
        else if (speechRecognitionResult.Text == "Apa itu kamera?")
        {
            text = File.ReadAllText("C:\\Users\\user\\Documents\\project-silvi\\voice-command\\materi\\kamera.txt");
        }
        else
        {
            Debug.Log("Perintah yang anda katakan belum benar");
        }

        if(text !=null)
        {
            using (var speechSynthesizer = new SpeechSynthesizer(speechConfig))
            {

                var speechSynthesisResult = await speechSynthesizer.SpeakTextAsync(text);
                OutputSpeechSynthesisResult(speechSynthesisResult, text);
            }
        }
        

    }
}
