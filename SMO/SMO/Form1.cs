using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace SMO
{
    public partial class Form1 : Form
    {
        // Позиции каждого состояния на графе
        private Dictionary<string, Point> statePositions;
        // Очередь для хранения клиентов
        private Queue<int> queue = new Queue<int>();
        // Списки для обработанных и отброшенных клиентов
        private List<int> processedClients = new List<int>();
        private List<int> discardedClients = new List<int>();
        // Таймер для обновления графа
        private Timer timer = new Timer();
        // Переменные для идентификации клиентов и их состояний
        private int clientId = 1;
        private int currentClientId = -1;
        private int currentClientState = -1;
        private Random random = new Random();

        public Form1()
        {
            InitializeComponent();
            SetupForm();
            InitializeStatePositions();

            // Установка интервала таймера и его запуск
            timer.Interval = 1000;
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        // Метод для настройки начальных параметров формы
        private void SetupForm()
        {
            button1.Text = "Добавить клиента";
            button1.Click += button1_Click;
            labelProcessed.Text = "Обработанные";
            labelDiscarded.Text = "Отброшенные";
        }

        // Метод для инициализации позиций состояний на графе
        private void InitializeStatePositions()
        {
            // позиции для каждого состояния
            statePositions = new Dictionary<string, Point>
            {
                { "Start", new Point(50, 250) },
                { "Processor1", new Point(200, 100) },
                { "Processor2", new Point(500, 100) },
                { "Queue3", new Point(200, 400) },
                { "Queue2", new Point(350, 400) },
                { "Queue1", new Point(500, 400) },
                { "End", new Point(650, 250) }
            };
        }

        // Обработчик кнопки для добавления клиента
        private void button1_Click(object sender, EventArgs e)
        {
            
            bool isQueueUnavailable = random.Next(100) < 20;

            // Если очередь недоступна, клиент отбрасывается
            if (isQueueUnavailable)
            {
                discardedClients.Add(clientId);
                listBoxDiscarded.Items.Add($"{clientId++} (Очередь недоступна)");
            }
            // Проверка, переполнена ли очередь
            else if (queue.Count >= 6)
            {
                // Добавление клиента в список отброшенных, если очередь переполнена
                discardedClients.Add(clientId);
                listBoxDiscarded.Items.Add($"{clientId++} (Очередь переполнена)");
            }
            else
            {
                // Добавление клиента в очередь
                queue.Enqueue(clientId++);
            }
            DrawStateMachine();
        }


        // Обработчик события таймера для обновления графа
        private void Timer_Tick(object sender, EventArgs e)
        {
            // Если очередь не пуста и нет клиента в обработке, извлекаем нового клиента из очереди
            if (queue.Count > 0 && currentClientState == -1)
            {
                currentClientId = queue.Dequeue();
                currentClientState = 0; // Устанавливаем начальное состояние "Start"
            }

            // Если текущий клиент находится в обработке
            if (currentClientState != -1)
            {
                ProcessClient(); // Переход к следующему состоянию
                DrawStateMachine(); // Обновление графа
            }
        }

        // Метод для обработки клиента через конечный автомат
        private void ProcessClient()
        {
            // смен а  состояний конечного автомата
            switch (currentClientState)
            {
                case 0:
                    // Переход из начального состояния Start в одно из состояний
                    currentClientState = GetNextState(new[] { 1, 2, 3, 4 });
                    break;
                case 1:
                    // Переход от состояния Processor1 в Processor2 или Queue
                    currentClientState = GetNextState(new[] { 2, 5 });
                    break;
                case 2:
                case 5:
                    // Завершение обработки и переход в End
                    currentClientState = 6;
                    break;
                case 3:
                    // Переход между очередями
                    currentClientState = 4;
                    break;
                case 4:
                    // Переход в очередь перед вторым процессором
                    currentClientState = 5;
                    break;
                case 6:
                    // Завершение обработки клиента
                    processedClients.Add(currentClientId);
                    listBoxProcessed.Items.Add(currentClientId);
                    currentClientState = -1; // Клиент завершил путь по состояниям
                    break;
            }
        }

        // Метод для выбора следующего состояния случайным образом из списка возможных
        private int GetNextState(int[] possibleStates)
        {
            return possibleStates[random.Next(possibleStates.Length)];
        }





        /// <summary>
        /// Далее ниже отрисовка графа
        /// </summary>

        // Метод для отрисовки всего графа состояний и переходов
        private void DrawStateMachine()
        {
            Bitmap bmp = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            Graphics g = Graphics.FromImage(bmp);

            DrawTransitions(g); // Рисуем все переходы
            DrawStates(g); // Рисуем все состояния

            pictureBox1.Image = bmp;
        }

        // Метод для отрисовки всех переходов между состояниями
        private void DrawTransitions(Graphics g)
        {
            Pen pen = new Pen(Color.DarkSlateGray, 2);

            DrawTransition(g, pen, "Start", "Processor1");
            DrawTransition(g, pen, "Start", "Processor2");
            DrawTransition(g, pen, "Start", "Queue3");
            DrawTransition(g, pen, "Start", "End");
            DrawTransition(g, pen, "Queue3", "Queue2");
            DrawTransition(g, pen, "Queue2", "Queue1");
            DrawTransition(g, pen, "Queue1", "Processor1");
            DrawTransition(g, pen, "Queue1", "Processor2");
            DrawTransition(g, pen, "Processor1", "End");
            DrawTransition(g, pen, "Processor2", "End");
        }

        // Метод для отрисовки каждого состояния на графе
        private void DrawStates(Graphics g)
        {
            DrawState(g, "Start", queue.Count < 6 ? "Ожидание" : "Заполнено", Color.LightCoral, queue.Count);
            DrawState(g, "Processor1", currentClientState == 1 ? $"{currentClientId} (6с)" : "Свободен", Color.Wheat);
            DrawState(g, "Processor2", currentClientState == 2 ? $"{currentClientId} (3с)" : "Свободен", Color.Wheat);
            DrawState(g, "Queue3", currentClientState == 3 ? currentClientId.ToString() : "Пусто", Color.LightSkyBlue);
            DrawState(g, "Queue2", currentClientState == 4 ? currentClientId.ToString() : "Пусто", Color.LightSkyBlue);
            DrawState(g, "Queue1", currentClientState == 5 ? currentClientId.ToString() : "Пусто", Color.LightSkyBlue);
            DrawState(g, "End", "Завершено", Color.MediumSeaGreen);
        }

        // Метод для рисования отдельного состояния
        private void DrawState(Graphics g, string stateName, string text, Color color, int queueCount = 0)
        {
            Point pos = statePositions[stateName];
            Brush brush = new SolidBrush(color);
            g.FillEllipse(brush, pos.X - 20, pos.Y - 20, 50, 50);
            g.DrawEllipse(Pens.Black, pos.X - 20, pos.Y - 20, 50, 50);
            g.DrawString(text, this.Font, Brushes.Black, pos.X - 15, pos.Y - 10);

            // Если это начальное состояние "Start", показываем количество клиентов в очереди
            if (stateName == "Start" && queueCount > 0)
            {
                g.DrawString($"Очередь: {queueCount}", this.Font, Brushes.Black, pos.X - 15, pos.Y + 30);
            }
        }

        // Метод для рисования перехода между состояниями с добавлением стрелок
        private void DrawTransition(Graphics g, Pen pen, string fromState, string toState)
        {
            Point fromPos = statePositions[fromState];
            Point toPos = statePositions[toState];
            g.DrawLine(pen, fromPos, toPos);
            DrawArrow(g, fromPos, toPos, pen);
        }

        // Метод для добавления стрелок на концах переходов
        private void DrawArrow(Graphics g, Point fromPos, Point toPos, Pen pen)
        {
            float arrowSize = 10;
            float angle = (float)Math.Atan2(toPos.Y - fromPos.Y, toPos.X - fromPos.X);

            PointF arrowEnd = new PointF(
                toPos.X - 20 * (float)Math.Cos(angle),
                toPos.Y - 20 * (float)Math.Sin(angle)
            );

            PointF arrowLeft = new PointF(
                arrowEnd.X - arrowSize * (float)Math.Cos(angle - Math.PI / 6),
                arrowEnd.Y - arrowSize * (float)Math.Sin(angle - Math.PI / 6)
            );
            PointF arrowRight = new PointF(
                arrowEnd.X - arrowSize * (float)Math.Cos(angle + Math.PI / 6),
                arrowEnd.Y - arrowSize * (float)Math.Sin(angle + Math.PI / 6)
            );

            g.DrawPolygon(pen, new PointF[] { arrowEnd, arrowLeft, arrowRight });
        }
    }
}
