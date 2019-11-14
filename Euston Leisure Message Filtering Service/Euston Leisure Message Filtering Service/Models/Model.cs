using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows;

namespace Euston_Leisure_Message_Filtering_Service.Models
{
    class Model
    {
        Dictionary<string, string> textWords = new Dictionary<string, string>();

        List<Message> messages = new List<Message>();       

        public Model()
        {
            loadTextWords();
        }

        private void loadTextWords()
        {
            try
            {
                StreamReader input_file = new StreamReader(File.OpenRead("../../Resources/textwords.csv"));
                while (!input_file.EndOfStream)
                {
                    string line = input_file.ReadLine();
                    string[] s = line.Split(',');

                    textWords.Add(s[0].ToLower(), s[1]);
                }
            }
            catch(FileNotFoundException)
            {
                MessageBox.Show("Failed to open csv file");
            }
        }

        public void addMessage(Message m)
        {
            messages.Add(m);
            //MessageBox.Show(messages.Count().ToString());
        }

        public Dictionary<string,string> getTextWords()
        {
            return textWords;
        }
    }
}
