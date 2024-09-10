using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ChatBotUI
{
    public partial class Form1 : Form
    {
        ChatBot chatBot;
        public Form1()
        {
            InitializeComponent();
            chatBot = new ChatBot();
        }

        private void button_send_Click(object sender, EventArgs e)
        {
            string request = textBox1.Text;
            richTextBox1.AppendText(chatBot.Answer(request));
        }
    }
}
