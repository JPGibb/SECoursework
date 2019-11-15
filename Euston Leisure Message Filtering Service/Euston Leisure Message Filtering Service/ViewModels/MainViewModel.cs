using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Euston_Leisure_Message_Filtering_Service.Commands;
using Euston_Leisure_Message_Filtering_Service.Models;

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

        //Buttons
        public string SubmitButtonText { get; set; }
        public ICommand SubmitButtonCommand { get; private set; }

        private Model model = new Model();

        public MainViewModel()
        {
            MessageIdTextBlock = "Message ID";
            MessageBodyTextBlock = "Message Body";

            SubmitButtonText = "Submit";

            MessageIdTextBox = string.Empty;
            MessageBodyTextBox = string.Empty;

            SubmitButtonCommand = new RelayCommand(SubmitButtonClick);
        }

        private void SubmitButtonClick()
        {
            // MessageBox.Show("The submit Button has been clicked");

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
                    MessageBox.Show("Email");
                    model.addMessage(new Email(MessageIdTextBox, MessageBodyTextBox));
                    break;
                case 'T':
                    MessageBox.Show("Twitter");
                    break;
            }
        }

    }
}
