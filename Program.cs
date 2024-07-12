
using System;
using System.Collections.Generic;
using System.Speech.Recognition;
using System.Speech.Synthesis;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net;

namespace StartingWithSpeechRecognition
{
    class Program
    {
        static HttpClient client = new HttpClient();
        static SpeechRecognitionEngine recognizer = null;
        static Dictionary<string, bool> recognizedCommands = new Dictionary<string, bool>();

        static async Task Main(string[] args)
        {
            recognizer = new SpeechRecognitionEngine();

            string[] commands = { "hello", "go to youtube", "go to calendar", "search on youtube", "pause", "play", "rewind", "forward", "goodbye" };

            foreach (var command in commands)
            {
                recognizedCommands[command] = false;
                Grammar commandGrammar = new Grammar(new GrammarBuilder(command));
                recognizer.LoadGrammar(commandGrammar);
            }

            recognizer.SpeechRecognized += Recognizer_SpeechRecognized;
            recognizer.SpeechRecognitionRejected += Recognizer_SpeechRecognitionRejected;

            recognizer.SetInputToDefaultAudioDevice();
            recognizer.RecognizeAsync(RecognizeMode.Multiple);

            RespondWithSpeech("Hello there, my name is Botimus Prime!");
            Console.WriteLine("Say a command to the microphone and it will execute.");
            Console.WriteLine("Say 'goodbye' to exit.");

            bool running = true;

            while (running)
            {
                while(!recognizedCommands.ContainsValue(true))
                {
                    await Task.Delay(1000000000);
                }
                string input = Console.ReadLine()?.ToLower();

                if (input == "goodbye")
                {
                    running = false;
                    break;
                }
                else
                {
                    if (recognizedCommands.ContainsKey(input))
                    {
                        ExecuteCommand(input);
                    }
                    else
                    {
                        RespondWithSpeech($"Command '{input}' not recognized. Please try again.");
                    }
                }
            }

            recognizer.Dispose();
        }

        static async void Recognizer_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            string command = e.Result.Text.ToLower();

            if (recognizedCommands.ContainsKey(command) && !recognizedCommands[command])
            {
                recognizedCommands[command] = true;
                ExecuteCommand(command);
            }
        }

        static async void ExecuteCommand(string command)
        {
            switch (command)
            {
                case "hello":
                    RespondWithSpeech("Hello Mazen!");
                    RespondWithSpeech("How can I assist you?");
                    break;
                case "go to youtube":
                    RespondWithSpeech("Going to Youtube!");
                    OpenYouTube();
                    break;
                case "go to calendar":
                    RespondWithSpeech("Going to your calendar");
                    OpenCalendar();
                    break;
                case "search on youtube":
                    RespondWithSpeech("Please enter the search term for YouTube:");
                    string searchTerm = Console.ReadLine()?.Trim();
                    if (!string.IsNullOrEmpty(searchTerm))
                    {
                        RespondWithSpeech($"Searching for {searchTerm} on YouTube.");
                        string videoUrl = await YoutubeFeatures.Youtube.searchOnYoutube(searchTerm);
                        if (!string.IsNullOrEmpty(videoUrl))
                        {
                            RespondWithSpeech("Found a video! Opening it now!");
                            Process.Start(new ProcessStartInfo
                            {
                                FileName = videoUrl,
                                UseShellExecute = true
                            });
                        }
                        else
                        {
                            RespondWithSpeech("Could not find video");
                        }
                    }
                    else
                    {
                        RespondWithSpeech("Please specify the video");
                    }
                    break;
                case "pause":
                    RespondWithSpeech("Pausing the video.");
                    await pauseVideo();
                    break;
                case "play":
                    RespondWithSpeech("Playing the video.");
                    break;
                case "rewind":
                    RespondWithSpeech("Rewinding the video.");
                    break;
                case "forward":
                    RespondWithSpeech("Forwarding the video.");
                    break;
                case "goodbye":
                    RespondWithSpeech("Goodbye!");
                    Environment.Exit(0);
                    break;
                default:
                    RespondWithSpeech($"Command '{command}' recognized!");
                    break;
            }

            Console.WriteLine($"Recognized: '{command}'");
        }

        static async Task pauseVideo()
        {
            try{
                var response = await client.PostAsync("http://localhost:5000/pause", null);
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine($"Error: {e.Message}");
            }
        }

        static void Recognizer_SpeechRecognitionRejected(object sender, SpeechRecognitionRejectedEventArgs e)
        {
            Console.WriteLine("Speech not recognized");
        }

        static void RespondWithSpeech(string message)
        {
            using (SpeechSynthesizer synthesizer = new SpeechSynthesizer())
            {
                synthesizer.Speak(message);
            }
        }

        static void OpenCalendar()
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "outlookcal:",
                UseShellExecute = true
            });
        }

        static void OpenYouTube()
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "https://www.youtube.com",
                UseShellExecute = true
            });
        }

    }
}

