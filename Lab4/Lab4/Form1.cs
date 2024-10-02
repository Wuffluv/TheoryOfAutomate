using System;
using System.Windows.Forms;

namespace Lab4
{
    public partial class Form1 : Form
    {
        private LampState currentState;

        public Form1()
        {
            InitializeComponent();
            currentState = LampState.Off;
            UpdateLampState();
        }

        private void button1_Click(object sender, EventArgs e) // Это кнопка "On"
        {
            if (currentState == LampState.Off)
            {
                currentState = LampState.On;
            }
            else if (currentState == LampState.On)
            {
                // Лампочка может перегореть
                currentState = LampState.BurnOut;
            }
            UpdateLampState();
        }

        private void button2_Click(object sender, EventArgs e) // Это кнопка "Off"
        {
            if (currentState == LampState.On)
            {
                currentState = LampState.Off;
            }
            UpdateLampState();
        }

        private void UpdateLampState()
        {
            switch (currentState)
            {
                case LampState.Off:
                    label1.Text = "Лампочка выключена";
                    pictureBox1.Image = Properties.Resources.lamp_off; // Лампочка выключена
                    break;
                case LampState.On:
                    label1.Text = "Лампочка включена";
                    pictureBox1.Image = Properties.Resources.lamp_on; // Лампочка включена
                    break;
                case LampState.BurnOut:
                    label1.Text = "Лампочка перегорела!";
                    pictureBox1.Image = Properties.Resources.lamp_off; // Лампочка не горит (перегорела)
                    button1.Enabled = false; // Лампочку нельзя снова включить
                    break;
            }
        }

        private enum LampState
        {
            Off,
            On,
            BurnOut
        }
    }
}
