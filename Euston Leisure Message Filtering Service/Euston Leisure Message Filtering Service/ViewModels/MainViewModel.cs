using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Euston_Leisure_Message_Filtering_Service.Commands;
using Euston_Leisure_Message_Filtering_Service.Models;
using Euston_Leisure_Message_Filtering_Service.Exceptions;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Euston_Leisure_Message_Filtering_Service.ViewModels
{
    class MainViewModel : BaseViewModel
    {
        //Text Blocks
        public  string MessageIdTextBlock { get; private set; }
        public string MessageBodyTextBlock { get; private set; }

        //Text bodies
        public string MessageIdTextBox { get; set; }
        public string MessageBodyTextBox { get; set; }
        public string TrendingListTextBox { get; set; }

        //Buttons
        public string SubmitButtonText { get; set; }
        public ICommand SubmitButtonCommand { get; private set; }

        public string SaveButtonText { get; set; }
        public ICommand SaveButtonCommand { get; private set; }

        private Model model = new Model();

        public MainViewModel()
        {
            MessageIdTextBlock = "Message ID";
            MessageBodyTextBlock = "Message Body";

            SubmitButtonText = "Submit";
            SaveButtonText = "Save";

            MessageIdTextBox = string.Empty;
            MessageBodyTextBox = string.Empty;
            TrendingListTextBox = string.Empty;

            SaveButtonCommand = new RelayCommand(SaveButtonClick);
            SubmitButtonCommand = new RelayCommand(SubmitButtonClick);
        }

        private void SaveButtonClick()
        {
            //MessageBox.Show("Saved");

            List<Message> messages = model.getMessages();
            Message m = messages[0];

            string output = JsonConvert.SerializeObject(m);
            //output += "\n" + JsonConvert.SerializeObject(messages[1]);
            MessageBox.Show(output);

            JsonSerializer ser = new JsonSerializer();
            ser.NullValueHandling = NullValueHandling.Ignore;

            using(StreamWriter sw = new StreamWriter(@"C:\Users\Public\test.json"))
            using (JsonWriter jw = new JsonTextWriter(sw))
            {
                ser.Serialize(jw, m);
            }
        }

        private void SubmitButtonClick()
        {
            //MessageBox.Show("The submit Button has been clicked");

            if(MessageIdTextBox.Length != 10)
            {
                MessageBox.Show("Error You must enter a Message ID");
                return;
            }
            if(MessageBodyTextBox == string.Empty)
            {
                MessageBox.Show("Error you must enter a message body");
                return;
            }

            switch (Char.ToUpper(MessageIdTextBox[0]))
            {
                case 'S':
                    MessageBox.Show("SMS");

                    string[] s = MessageBodyTextBox.Split('\n');

                    //set sender to the value of the first line of MessageBodyTextBox
                    string sender = s[0].Replace("\n", string.Empty);

                    string temp = string.Empty;
                    for(int i = 1; i < s.Length; ++i)
                    {
                        temp += s[i];
                    }

                    //Ensure that the message body is within limit
                    if (temp.ToCharArray().Length <= 140) 
                    {
                        Message m = new Sms(MessageIdTextBox, temp, sender, model.getTextWords());
                        model.addMessage(m);
                    }
                    else
                    {
                        MessageBox.Show("The message Length of a sms can only be 140 characters long");
                    }
                    
                    break;
                case 'E':
                    string[] x = MessageBodyTextBox.Split('\n', ' ');
                    if (x[1].ToUpper().Contains("SIR"))
                    {
                        MessageBox.Show("Serious Incident report");
                        try 
                        {
                            model.addMessage(new SeriousIncidentReport(MessageIdTextBox, MessageBodyTextBox, x[2]));
                        }
                        catch(InvalidEmailException)
                        {
                            MessageBox.Show("An invalid Email has been entered");
                        }
                    }
                    else
                    {
                        MessageBox.Show("Email");
                        try
                        {
                            model.addMessage(new Email(MessageIdTextBox, MessageBodyTextBox));
                        }
                        catch (InvalidEmailException)
                        {
                            MessageBox.Show("An invalid email address has been entered");
                        }
                    }
                    break;
                case 'T':
                    MessageBox.Show("Tweet");
                    string[] k = MessageBodyTextBox.Split('\n');

                    try
                    {
                        Tweet t = new Tweet(MessageIdTextBox, MessageBodyTextBox, model.getTextWords());
                        model.addMessage(t);

                        List<string> hashtags = t.findHashtags();
                                                
                        if(hashtags.Count > 0)
                        {
                            foreach(string hashtag in hashtags)
                            {
                                model.addHashtag(hashtag);
                            }
                        }

                        List<string> mentions = t.findMentions();

                        if(mentions.Count > 0)
                        {
                            foreach (string mention in mentions)
                            {
                                model.addMention(mention);
                            }
                        }
                    }
                    catch(FailedToCreateMessageException)
                    {
                        MessageBox.Show("Error failed to create that message \nMake sure that the input data is correct");
                    }
                    break;
            }

            TrendingListTextBox = generateReport();
            OnChanged(nameof(TrendingListTextBox));
        }

        private string generateReport()
        {
            string report = string.Empty;

            report += "Tagged users:\n";

            Dictionary<string, int> mentions = model.getMentions();

            for(int i = 0; i < mentions.Count; ++i)
            {
                report += "User " + mentions.ElementAt(i).Key + " was mentioned " + mentions.ElementAt(i).Value + " Times\n";
            }

            report += "Hashtags:\n";
            Dictionary<string, int> hashtags = model.getHashtags();

            for(int i = 0; i < hashtags.Count; ++i)
            {
                report += "Hashtag " + hashtags.ElementAt(i).Key + " was tweeted " + hashtags.ElementAt(i).Value + " Times\n";
            }

            return report;
        }       
    }
}
