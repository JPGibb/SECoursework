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

    //Main view model
    class MainViewModel : BaseViewModel
    {
        //Text Blocks
        public  string MessageIdTextBlock { get; private set; }
        public string MessageBodyTextBlock { get; private set; }

        //Text bodies
        public string MessageIdTextBox { get; set; }
        public string MessageBodyTextBox { get; set; }
        public string ReportTextBox { get; set; }

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
            ReportTextBox = string.Empty;

            SaveButtonCommand = new RelayCommand(SaveButtonClick);
            SubmitButtonCommand = new RelayCommand(SubmitButtonClick);
            OpenFileButtonCommand = new RelayCommand(OpenFileButtonClick);
        }

        //Serialize all the data in the model in json format
        //When the save button is clicked
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

        //Process info input into text boxes
        //When the submit button is clicked
        private void SubmitButtonClick()
        {
            //MessageBox.Show("The submit Button has been clicked");

            processInput(MessageIdTextBox, MessageBodyTextBox);

            ReportTextBox = generateReport();
            OnChanged(nameof(ReportTextBox));
        }

        //Prompts the user to open a file and with parse all the message within
        //When the open button is clicked
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
                    processInput(messageIds.ElementAt(i).Value.Trim('\r', '\n'), messageBodies.ElementAt(i).Value.Trim('\r', '\n'));
                }
            }
        }

        //Processes the input for the messages, creates new objects and stores them in the model
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
                    //MessageBox.Show("SMS");

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
                        MessageBodyTextBox = m.getDetails();
                        OnChanged(nameof(MessageBodyTextBox));
                        MessageBox.Show(m.getDetails(), "Created New Sms");
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
                        // MessageBox.Show("Serious Incident report");
                        try
                        {
                            // MessageBox.Show(messagebody);
                            SeriousIncidentReport sir = new SeriousIncidentReport(messageid, messagebody, x[2]);
                            model.addMessage(sir);
                            model.addSir(sir.getSccNoi());
                            MessageBodyTextBox = sir.getDetails();
                            OnChanged(nameof(MessageBodyTextBox));
                            MessageBox.Show(sir.getDetails(), "Created New Serious Incident Report");
                        }
                        catch (InvalidEmailException)
                        {
                            MessageBox.Show("An invalid Email has been entered");
                        }
                        catch (FailedToCreateMessageException)
                        {
                            MessageBox.Show("Failed to create message, ensure that all details are valid");
                        }
                        catch (ToManyCharactersException)
                        {
                            MessageBox.Show("Failed to create that message as there are too many characters in the message subject or body");
                        }
                    }
                    else
                    {
                        //MessageBox.Show("Email");
                        try
                        {
                            Email e = new Email(messageid, messagebody);
                            model.addMessage(e);
                            MessageBodyTextBox = e.getDetails();
                            OnChanged(nameof(MessageBodyTextBox));
                            MessageBox.Show(e.getDetails(), "Created New Email");
                        }
                        catch (InvalidEmailException)
                        {
                            MessageBox.Show("An invalid email address has been entered");
                        }
                        catch (FailedToCreateMessageException)
                        {
                            MessageBox.Show("Failed to create message, ensure that all details are valid");
                        }
                        catch (ToManyCharactersException)
                        {
                            MessageBox.Show("Failed to create that message as there are too many characters in the message subject or body");
                        }
                    }
                    break;
                case 'T':
                    //MessageBox.Show("Tweet");
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
                        MessageBodyTextBox = t.getDetails();
                        OnChanged(nameof(MessageBodyTextBox));
                        MessageBox.Show(t.getDetails(), "Created New Tweet");
                    }
                    catch (FailedToCreateMessageException)
                    {
                        MessageBox.Show("Error failed to create that message \nMake sure that the input data is correct");
                    }
                    catch(ToManyCharactersException)
                    {
                        MessageBox.Show("Error, either the sender name is too long or there are to many characters in the message body");
                    }
                    break;
            }
            ReportTextBox = generateReport();
            OnChanged(nameof(ReportTextBox));
        }

        //Gathers information about all the mentions
        private string generateMentionsReport()
        {
            string mentions = string.Empty;

            mentions += "Tagged users:\n";

            Dictionary<string, int> m= model.getMentions();
            if (m.Count() > 0)
            {
                for (int i = 0; i < m.Count; ++i)
                {
                    mentions += "User " + m.ElementAt(i).Key + " was mentioned " + m.ElementAt(i).Value + " Times\n";
                }
            }

            return mentions;
        }

        //Gathers information about all the hashtags
        private string generateHashtagsReport()
        {
            string hashtags = string.Empty;
            Dictionary<string, int> h = model.getHashtags();
            hashtags += "Hashtags:";
            if (h.Count() > 0)
            {
                for (int i = 0; i < h.Count; ++i)
                {
                    hashtags += "Hashtag " + h.ElementAt(i).Key + " was tweeted " + h.ElementAt(i).Value + " Times\n";
                }
            }

            return hashtags;
        }

        //Gathers information about all the Serious Incident Reports
        private string generateSirReport()
        {
            string sirs = string.Empty;

            List<string[]> s = model.getSirList();
          
            sirs += "Serious Incident Reports:\n";

            if (s.Count() > 0)
            {
                foreach(string[] x in s)
                {
                    sirs += "Centre code: " + x[0] + " Nature of incident: " + x[1] + "\n";
                }
            }
            return sirs;
        }

        //Generates a report containing inforamtion about all the mentiosn, hashtags and sirs
        private string generateReport()
        {
            string report = string.Empty;

            report += generateMentionsReport();
            report += "\n";
            report += generateHashtagsReport();
            report += "\n";
            report += generateSirReport();

            return report;
        }
    }
}
