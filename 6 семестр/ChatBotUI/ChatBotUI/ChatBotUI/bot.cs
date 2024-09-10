using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatBotUI
{
    internal class ChatBot
    {
        private enum State { INIT, SESSION, STORE, ER }
        private State now;
        private string UserName;
        private List<string> phrases;

        public ChatBot()
        {
            now = State.INIT;
            phrases = new List<string>();
        }

        //public void Run()
        //{
        //    do
        //    {
        //        string text = Console.ReadLine();

        //        switch (now)
        //        {
        //            case State.INIT:
        //                ProcessInit(text);
        //                break;
        //            case State.SESSION:
        //                ProcessSession(text);
        //                break;
        //            case State.STORE:
        //                ProcessStore(text);
        //                break;
        //            default:
        //                break;
        //        }
        //    } while (now != State.ER);
        //}

        public string Answer(string text)
        {
            switch (now)
            {
                case State.INIT:
                    return ProcessInit(text);
                case State.SESSION:
                    return ProcessSession(text);
                case State.STORE:
                    ProcessStore(text);
                    break;
                default:
                    break;
            }

            return string.Empty;
        }

        private string ConvertToLower(string input)
        {
            return input.ToLower();
        }

        private string ProcessInit(string text)
        {
            string[] words = text.Split(' ');
            string firstWord = words[0];
            string name = words[1];

            if (firstWord == "login")
            {
                UserName = name;
                now = State.SESSION;
                return "Nice to meet you!";
            }
            else
            {
                now = State.INIT;
                return "Please introduce yourself first!";
            }
        }

        private string ProcessSession(string text)
        {
            text = ConvertToLower(text);
            string[] words = text.Split(' ');
            string firstWord = words[0];

            if (firstWord == "hello!" || firstWord == "hello" || firstWord == "hi!")
            {
                
                now = State.SESSION;
                return "Welcome, " + UserName + "!";
            }
            else if (firstWord == "memorize")
            {
                now = State.STORE;
                return "I'm ready to memorize";
            }
            else if (firstWord == "say")
            {
                int number = int.Parse(words[1]);
                now = State.SESSION;
                if (number >= 0 && number < phrases.Count)
                {
                    string element = phrases[number];
                    return element;
                }
                else
                {           
                    now = State.SESSION;
                    return "No record";
                }
            }
            else if (firstWord == "exit")
            {
                now = State.INIT;
                return "Goodbye!";
            }
            else
            {
                now = State.SESSION;
                return "I don't understand you!";
            }
        }

        private void ProcessStore(string text)
        {
            phrases.Add(text);
            now = State.SESSION;
        }
    }
}
