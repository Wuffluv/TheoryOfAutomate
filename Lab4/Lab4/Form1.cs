using System;
using System.Drawing; // Для работы с изображениями
using System.IO; // Для работы с путями файлов
using System.Windows.Forms;

namespace Lab4
{
    public partial class Form1 : Form
    {
        private LampState currentState;
        private string imagePath = @"C:\Images\"; // Путь к папке с изображениями

        public Form1()
        {
            InitializeComponent();
            currentState = LampState.Off;

            // Привязка обработчиков событий
            button1.Click += new EventHandler(button1_Click);
            button2.Click += new EventHandler(button2_Click);

            // Устанавливаем режим отображения изображения в PictureBox
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;

            UpdateLampState();
        }

        private void button1_Click(object sender, EventArgs e) //  "On"
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

        private void button2_Click(object sender, EventArgs e) //  "Off"
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
                    LoadImage("lamp_off.png"); // Загружаем изображение выключенной лампочки
                    break;
                case LampState.On:
                    label1.Text = "Лампочка включена";
                    LoadImage("lamp_on.png"); // Загружаем изображение включенной лампочки
                    break;
                case LampState.BurnOut:
                    label1.Text = "Лампочка перегорела!";
                    // Если нет изображения для перегоревшей лампочки, используем выключенное изображение
                    LoadImage("lamp_off.png");
                    button1.Enabled = false; // Лампочку нельзя снова включить
                    break;
            }
        }

        // Метод для загрузки изображения в PictureBox
        private void LoadImage(string imageName)
        {
            string fullPath = Path.Combine(imagePath, imageName); // Объединяем путь к папке с именем файла        
            pictureBox1.Image = Image.FromFile(fullPath); // Загружаем изображение  
        }

        private enum LampState
        {
            Off,
            On,
            BurnOut
        }
    }
}
