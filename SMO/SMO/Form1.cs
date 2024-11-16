using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace SMO
{
    public partial class Form1 : Form
    {
        private PetriNet petriNet; // Сеть Петри для управления всей системой
        private Timer timer = new Timer(); // Таймер для обновления сети в реальном времени
        private int clientId = 1; // Идентификатор для нового клиента
        private List<int> processedClients = new List<int>(); // Список обработанных клиентов
        private List<int> discardedClients = new List<int>(); // Список отброшенных клиентов

        public Form1()
        {
            InitializeComponent();
            SetupForm(); // Настраиваем элементы формы

            // Создаем сеть Петри
            InitializePetriNet();

            timer.Interval = 1000; // Таймер срабатывает каждые 1000 мс
            timer.Tick += Timer_Tick; // Привязываем обработчик таймера
            timer.Start(); // Запускаем таймер
        }

        private void SetupForm()
        {
            // Настраиваем текст и обработчики для элементов управления на форме
            button1.Text = "Добавить клиента";
            button1.Click += button1_Click;
            labelProcessed.Text = "Обработанные";
            labelDiscarded.Text = "Отброшенные";
        }

        private void InitializePetriNet()
        {
            petriNet = new PetriNet(); // Создаем объект сети Петри

            // Создаем места (places) в сети Петри
            petriNet.AddPlace("Очередь");
            petriNet.AddPlace("Процессор1");
            petriNet.AddPlace("Процессор2");
            petriNet.AddPlace("Завершено");

            // Создаем переходы (transitions) между местами
            petriNet.AddTransition("ИзОчередиВПроцессор1", new[] { "Очередь" }, new[] { "Процессор1" });
            petriNet.AddTransition("ИзОчередиВПроцессор2", new[] { "Очередь" }, new[] { "Процессор2" });
            petriNet.AddTransition("ИзПроцессора1ВЗавершено", new[] { "Процессор1" }, new[] { "Завершено" });
            petriNet.AddTransition("ИзПроцессора2ВЗавершено", new[] { "Процессор2" }, new[] { "Завершено" });
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Добавляем клиента (токен) в место "Очередь"
            if (petriNet.GetTokensCount("Очередь") < 6) // Ограничиваем длину очереди
            {
                petriNet.AddToken("Очередь", clientId++);
            }
            else
            {
                // Если очередь переполнена, клиент добавляется в список отброшенных
                discardedClients.Add(clientId);
                listBoxDiscarded.Items.Add(clientId++);
            }

            DrawStateMachine(); // Перерисовываем граф
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            // Выводим активные переходы в консоль для диагностики
            var enabledTransitions = petriNet.GetEnabledTransitions();
            Console.WriteLine($"Активные переходы: {string.Join(", ", enabledTransitions.Select(t => t.Name))}");

            // Активируем переходы в сети Петри
            petriNet.FireTransitions();

            // Перемещаем обработанные токены в список обработанных
            var completedTokens = petriNet.GetTokensFromPlace("Завершено");
            foreach (var token in completedTokens)
            {
                if (!processedClients.Contains(token))
                {
                    processedClients.Add(token); // Добавляем клиента в список обработанных
                    listBoxProcessed.Items.Add(token); // Отображаем клиента в списке на форме
                }
            }

            DrawStateMachine(); // Обновляем граф
        }

        private void DrawStateMachine()
        {
            Bitmap bmp = new Bitmap(pictureBox1.Width, pictureBox1.Height); // Создаем изображение для графа
            Graphics g = Graphics.FromImage(bmp);

            // Рисуем места (places) на графе
            DrawPlace(g, "Очередь", "Очередь", new Point(50, 250));
            DrawPlace(g, "Процессор1", "Процессор 1", new Point(200, 100));
            DrawPlace(g, "Процессор2", "Процессор 2", new Point(500, 100));
            DrawPlace(g, "Завершено", "Завершено", new Point(650, 250));

            pictureBox1.Image = bmp; // Отображаем граф на форме
        }

        private void DrawPlace(Graphics g, string placeName, string label, Point position)
        {
            // Рисуем место (круг) на графе
            Brush brush = new SolidBrush(Color.LightSkyBlue);
            g.FillEllipse(brush, position.X - 20, position.Y - 20, 50, 50);
            g.DrawEllipse(Pens.Black, position.X - 20, position.Y - 20, 50, 50);

            // Отображаем текст с названием места и количеством токенов
            g.DrawString(label, this.Font, Brushes.Black, position.X - 25, position.Y - 40);
            g.DrawString($"Токены: {petriNet.GetTokensCount(placeName)}", this.Font, Brushes.Black, position.X - 25, position.Y + 30);
        }
    }

    // Класс Place - Место в сети Петри
    public class Place
    {
        public string Name { get; } // Название места
        private List<int> tokens; // Список токенов в месте

        public Place(string name)
        {
            Name = name;
            tokens = new List<int>(); // Инициализируем список токенов
        }

        // Добавляем токен в место
        public void AddToken(int tokenId) => tokens.Add(tokenId);

        // Убираем токен из места
        public int RemoveToken()
        {
            if (tokens.Count > 0)
            {
                int token = tokens[0];
                tokens.RemoveAt(0); // Удаляем первый токен
                return token;
            }
            return -1; // Если токенов нет, возвращаем -1
        }

        public int TokensCount => tokens.Count; // Количество токенов в месте
        public List<int> Tokens => new List<int>(tokens); // Копия списка токенов
    }

    // Класс Transition - Переход в сети Петри
    public class Transition
    {
        public string Name { get; } // Название перехода
        private List<Place> inputPlaces; // Входные места
        private List<Place> outputPlaces; // Выходные места

        public Transition(string name, List<Place> inputs, List<Place> outputs)
        {
            Name = name;
            inputPlaces = inputs;
            outputPlaces = outputs;
        }

        // Проверяем, можно ли выполнить переход
        public bool IsEnabled() => inputPlaces.All(p => p.TokensCount > 0);

        // Выполняем переход
        public void Fire()
        {
            if (IsEnabled())
            {
                foreach (var place in inputPlaces)
                {
                    int token = place.RemoveToken();
                    if (token != -1)
                    {
                        foreach (var output in outputPlaces)
                        {
                            output.AddToken(token); // Перемещаем токен в выходное место
                        }
                    }
                }
            }
        }
    }

    // Класс PetriNet - Управляет всей сетью Петри
    public class PetriNet
    {
        private Dictionary<string, Place> places; // Список мест
        private List<Transition> transitions; // Список переходов

        public PetriNet()
        {
            places = new Dictionary<string, Place>(); // Инициализируем места
            transitions = new List<Transition>(); // Инициализируем переходы
        }

        public void AddPlace(string name) => places[name] = new Place(name); // Добавляем место

        public void AddTransition(string name, string[] inputNames, string[] outputNames)
        {
            var inputs = inputNames.Select(n => places[n]).ToList();
            var outputs = outputNames.Select(n => places[n]).ToList();
            transitions.Add(new Transition(name, inputs, outputs)); // Добавляем переход
        }

        public void AddToken(string placeName, int tokenId) => places[placeName].AddToken(tokenId); // Добавляем токен в место

        public int GetTokensCount(string placeName) => places[placeName].TokensCount; // Получаем количество токенов в месте

        public List<int> GetTokensFromPlace(string placeName) => places[placeName].Tokens; // Получаем список токенов из места

        // Получаем список активных переходов
        public List<Transition> GetEnabledTransitions() => transitions.Where(t => t.IsEnabled()).ToList();

        // Активируем переходы
        public void FireTransitions()
        {
            foreach (var transition in GetEnabledTransitions())
            {
                transition.Fire(); // Выполняем активный переход
            }
        }
    }
}
