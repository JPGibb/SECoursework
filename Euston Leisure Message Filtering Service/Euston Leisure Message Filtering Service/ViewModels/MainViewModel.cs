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
using Microsoft.Win32;
using System.Xml.Linq;

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

        public string OpenFileButtonText { get; set; }
        public ICommand OpenFileButtonCommand { get; private set; }

        private Model model = new Model();

        public MainViewModel()
        {
            MessageIdTextBlock = "Message ID";
            MessageBodyTextBlock = "Message Body";

            SubmitButtonText = "Submit";
            SaveButtonText = "Save";
            OpenFileButtonText = "Open File";

            MessageIdTextBox = string.Empty;
            MessageBodyTextBox = string.Empty;
            TrendingListTextBox = string.Empty;

            SaveButtonCommand = new RelayCommand(SaveButtonClick);
            SubmitButtonCommand = new RelayCommand(SubmitButtonClick);
            OpenFileButtonCommand = new RelayCommand(OpenFileButtonClick);
        }

        //Convert all the data in the model to json format
        private void SaveButtonClick()
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Json Files (*.json)|*.json";
            if(sfd.ShowDialog() == true)
            {
                //MessageBox.Show("Saved");
                string output = string.Empty;
                List<Message> messages = model.getMessages();
                output += JsonConvert.SerializeObject(messages, Formatting.Indented);

                output += JsonConvert.SerializeObject(model.getSirList(), Formatting.Indented);
                output += JsonConvert.SerializeObject(model.getHashtags(), Formatting.Indented);
                output += JsonConvert.SerializeObject(model.getMentions(), Formatting.Indented);
                JsonSerializer ser = new JsonSerializer();
                ser.NullValueHandling = NullValueHandling.Ignore;


                File.WriteAllText(sfd.FileName, output);
            }
            
        }

        private void SubmitButtonClick()
        {
            //MessageBox.Show("The submit Button has been clicked");

            processInput(MessageIdTextBox, MessageBodyTextBox);

            TrendingListTextBox = generateReport();
            OnChanged(nameof(TrendingListTextBox));
        }

        private void OpenFileButtonClick()
        {
            //MessageBox.Show("The open file button has been clicked");

            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == true)
            { 
                XDocument xml = XDocument.Load(ofd.FileName);
                var messageIds = xml.Descendants("messageId");
                var messageBodies = xml.Descendants("messageBody");

                for(int i = 0; i < messageIds.Count(); ++i)
                {
                    //string[] s = messageBodies.ElementAt(i).Value.Split('\n', ' ');
                    //foreach(string x in s)
                    //{
                    //    MessageBox.Show(x);
                    //}
                    processInput(messageIds.ElementAt(i).Value.Trim('\r', '\n'), messageBodies.ElementAt(i).Value.Trim('\r', '\n'));
                }
            }
        }

        private void processInput(string messageid, string messagebody)
        {
            if (messageid.Length != 10)
            {
                MessageBox.Show("Error You must enter a Message ID");
                return;
            }
            if (messagebody == string.Empty)
            {
                MessageBox.Show("Error you must enter a message body");
                return;
            }

            switch (Char.ToUpper(messageid[0]))
            {
                case 'S':
                    MessageBox.Show("SMS");

                    string[] s = messagebody.Split('\n');

                    //set sender to the value of the first line of messagebody
                    string sender = s[0].Replace("\n", string.Empty);

                    string temp = string.Empty;
                    for (int i = 1; i < s.Length; ++i)
                    {
                        temp += s[i];
                    }

                    //Ensure that the message body is within limit
                    if (temp.ToCharArray().Length <= 140)
                    {
                        Message m = new Sms(messageid, temp, sender, model.getTextWords());
                        model.addMessage(m);
                    }
                    else
                    {
                        MessageBox.Show("The message Length of a sms can only be 140 characters long");
                    }

                    break;
                case 'E':
                    string[] x = messagebody.Split('\n', ' ');
                    if (x[1].ToUpper().Contains("SIR"))
                    {
                        MessageBox.Show("Serious Incident report");
                        try
                        {
                            MessageBox.Show(messagebody);
                            SeriousIncidentReport sir = new SeriousIncidentReport(messageid, messagebody, x[2]);
                            model.addMessage(sir);
                            model.addSir(sir.getSccNoc());
                        }
                        catch (InvalidEmailException)
                        {
                            MessageBox.Show("An invalid Email has been entered");
                        }
                        catch(FailedToCreateMessageException)
                        {
                            MessageBox.Show("Failed to create message, ensure that all details are valid");
                        }
                    }
                    else
                    {
                        MessageBox.Show("Email");
                        try
                        {
                            model.addMessage(new Email(messageid, messagebody));
                        }
                        catch (InvalidEmailException)
                        {
                            MessageBox.Show("An invalid email address has been entered");
                        }
                        catch (FailedToCreateMessageException)
                        {
                            MessageBox.Show("Failed to create message, ensure that all details are valid");
                        }
                    }
                    break;
                case 'T':
                    MessageBox.Show("Tweet");
                    string[] k = messagebody.Split('\n');

                    try
                    {
                        Tweet t = new Tweet(messageid, messagebody, model.getTextWords());
                        model.addMessage(t);

                        List<string> hashtags = t.findHashtags();

                        if (hashtags.Count > 0)
                        {
                            foreach (string hashtag in hashtags)
                            {
                                model.addHashtag(hashtag);
                            }
                        }

                        List<string> mentions = t.findMentions();

                        if (mentions.Count > 0)
                        {
                            foreach (string mention in mentions)
                            {
                                model.addMention(mention);
                            }
                        }
                    }
                    catch (FailedToCreateMessageException)
                    {
                        MessageBox.Show("Error failed to create that message \nMake sure that the input data is correct");
                    }
                    break;
            }
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
