using System;
using System.Collections.Generic;
using System.Speech.Recognition;
using System.Speech.Synthesis;
using System.Diagnostics;

namespace StartingWithSpeechRecognition
{
    class Program
    {
        static SpeechRecognitionEngine recognizer = null;
        static Dictionary<string, bool> recognizedCommands = new Dictionary<string, bool>();

        static void Main(string[] args)
        {
            recognizer = new SpeechRecognitionEngine();

            bool onYoutube = false;

            string[] commands = { "hello",  "go to youtube", "go to calender", "search on youtube" }; //list of commands using an array
            //of all the commands the user wants to do.
            foreach (var command in commands) //for each loop, going through each command.
            {
                recognizedCommands[command] = false;
                Grammar commandGrammar = new Grammar(new GrammarBuilder(command)); //Using the speech library, adds the grammar for the
                //the bot for the command. Set of commands for the recognizer and what commands to do.
                recognizer.LoadGrammar(commandGrammar); //loads it to the bot.
            }

            recognizer.SpeechRecognized += Recognizer_SpeechRecognized;
            recognizer.SpeechRecognitionRejected += Recognizer_SpeechRecognitionRejected;

            recognizer.SetInputToDefaultAudioDevice();
            recognizer.RecognizeAsync(RecognizeMode.Multiple);

            RespondWithSpeech("Hello there, my name is Botimus Prime!");
            Console.WriteLine("Say a command to the microphone and it will execute.");
            Console.WriteLine("Press any key to exit");
            Console.ReadKey();

            recognizer.Dispose();
        }

        static async void Recognizer_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            string command = e.Result.Text;
            if (recognizedCommands.ContainsKey(command) && !recognizedCommands[command])
            {
                recognizedCommands[command] = true; 

                if (command == "hello")
                {
                    RespondWithSpeech("Hello Mazen!");
                    RespondWithSpeech("How can I assist you?");
                }
                else if (command == "go to youtube")
                {
                    RespondWithSpeech("Going to Youtube!");
                    OpenYouTube();

                }
                else if (command == "go to calender")
                {
                    RespondWithSpeech("Going to your calender");
                    openCalender();
                }
                else if (command.StartsWith("search on youtube"))
                {
                    string searchTerm = command.Substring("search on youtube".Length).Trim();
                    if(!string.IsNullOrEmpty(searchTerm))
                    {
                        RespondWithSpeech($"Searching for {searchTerm} on Youtube.");
                        string videoUrl = await YoutubeFeatures.Youtube.searchOnYoutube(searchTerm);
                        if (videoUrl != null)
                        {
                            RespondWithSpeech("Found a video! Opening it now!");
                            Process.Start(new ProcessStartInfo {
                                FileName = videoUrl,
                                UseShellExecute = true
                            });
                        }
                        else
                        {
                            RespondWithSpeech("Sorry, I couldn't find any video for your search.");

                        }
                    }
                    else
                    {
                        RespondWithSpeech("Please specify what video you want to search for.");
                    }

                }
                else
                {
                    RespondWithSpeech($"Command '{command}' recognized!");
                }

                Console.WriteLine($"Recognized: '{command}'");
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

    static void openCalender()
    {
        Process.Start(new ProcessStartInfo{
            FileName = "outlookcal:", UseShellExecute = true
        });
    }

        static void OpenYouTube() //This method  plays the youtube or goes to the youtube link.
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "https://www.youtube.com",
                UseShellExecute = true
            });
        }
    }
}

