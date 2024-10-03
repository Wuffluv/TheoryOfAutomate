using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace Lab5
{
    public partial class Form1 : Form
    {
        // Перечисление состояний светофора
        private enum TrafficLightState
        {
            Red,
            Yellow,
            Green
        }

        private TrafficLightState currentState; // Текущее состояние светофора
        private int timeLeft; // Время до смены состояния
        private string imagePath = @"C:\Users\wolfd\Desktop\Study\Теория Автоматов\Lab5\Img\"; // Путь к изображениям светофора
        private bool isBlinking = false; // Флаг для мигания

        public Form1()
        {
            InitializeComponent();
            InitializeTrafficLight();
        }

        // Инициализация светофора
        private void InitializeTrafficLight()
        {
            // Устанавливаем начальное состояние (красный свет)
            currentState = TrafficLightState.Red;
            timeLeft = 10; // Время до смены с красного на желтый
            label1.Text = $"Время до смены: {timeLeft} секунд";
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage; // Устанавливаем растяжение изображения
            timer1.Interval = 1000; // Таймер будет срабатывать каждую секунду
            timer1.Tick += Timer1_Tick; // Подписываемся на событие таймера
            timer1.Start(); // Запускаем таймер
            UpdateTrafficLightImage(); // Устанавливаем начальное изображение
        }

        // Обработчик таймера
        private void Timer1_Tick(object sender, EventArgs e)
        {
            timeLeft--; // Уменьшаем время
            label1.Text = $"Время до смены: {timeLeft} секунд";

            // Логика мигания для зеленого и желтого света
            if ((currentState == TrafficLightState.Green || currentState == TrafficLightState.Yellow) && timeLeft <= 3)
            {
                isBlinking = !isBlinking; // Переключаем состояние мигания
                UpdateTrafficLightImage();
            }

            // Если время вышло, переключаем состояние
            if (timeLeft <= 0)
            {
                SwitchTrafficLightState();
            }
        }

        // Переключение состояния светофора
        private void SwitchTrafficLightState()
        {
            switch (currentState)
            {
                case TrafficLightState.Red:
                    currentState = TrafficLightState.Green; // Переход на зеленый
                    timeLeft = 10; // Время до смены на желтый
                    break;
                case TrafficLightState.Green:
                    currentState = TrafficLightState.Yellow; // Переход на желтый
                    timeLeft = 5; // Время до смены на красный
                    break;
                case TrafficLightState.Yellow:
                    currentState = TrafficLightState.Red; // Переход на красный
                    timeLeft = 10; // Время до смены на зеленый
                    break;
            }

            isBlinking = false; // Сбрасываем мигание
            UpdateTrafficLightImage(); // Обновляем изображение светофора
        }

        // Метод для обновления изображения светофора в зависимости от состояния
        private void UpdateTrafficLightImage()
        {
            string imageName = string.Empty;

            switch (currentState)
            {
                case TrafficLightState.Red:
                    imageName = "red.png"; // Красный свет (не мигает)
                    break;
                case TrafficLightState.Yellow:
                    imageName = isBlinking ? "yellow_dim.png" : "yellow.png"; // Желтый свет мигает
                    break;
                case TrafficLightState.Green:
                    imageName = isBlinking ? "green_dim.png" : "green.png"; // Зеленый свет мигает
                    break;
            }

            // Если изображение пустое, очищаем PictureBox
            if (!string.IsNullOrEmpty(imageName))
            {
                string fullPath = Path.Combine(imagePath, imageName); // Полный путь к изображению
                if (File.Exists(fullPath))
                {
                    pictureBox1.Image = Image.FromFile(fullPath); // Загружаем изображение в PictureBox
                }
                else
                {
                    MessageBox.Show($"Изображение {imageName} не найдено по пути {fullPath}!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
