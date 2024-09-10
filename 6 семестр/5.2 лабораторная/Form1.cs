using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;


namespace Lab5_TuringMachineGUI_v1
{
    public partial class Form1 : Form
    {
        private TuringMachine turingMachine;
        private PictureBox tapePictureBox;
        private Button runButton;
        private Label stateLabel;
        private Label headLabel;
        private Timer timer;
        private const int AutoMoveInterval = 500; // Интервал автоматического движения головки (в миллисекундах)
        private Color highlightColor = Color.LightBlue; // Цвет выделения
        private Color defaultBackColor = Color.White; // Исходный цвет фона ячейки


        public Form1()
        {
            InitializeComponent();
            InitializeTuringMachine();
            InitializeTimer();
        }

        private void InitializeTimer()
        {
            timer = new Timer();
            timer.Interval = AutoMoveInterval;
            timer.Tick += Timer_Tick;
        }

        private void InitializeTuringMachine()
        {
            // Создание экземпляра машины Тьюринга
            turingMachine = new TuringMachine();

            // Задание начальной ленты
            turingMachine.Tape = "111111+1 ";
            

            // Задание начального состояния
            turingMachine.State = "q0";

            //public Transition(string currentState, char readSymbol, char writeSymbol, Direction moveDirection, string nextState)

            //public string CurrentState { get; set; } // Текущее состояние автомата
            //public char ReadSymbol { get; set; } // Символ, считанный с ленты
            //public char WriteSymbol { get; set; } // Символ, который нужно записать на ленту
            //public Direction MoveDirection { get; set; } // Направление движения головки ленты
            //public string NextState { get; set; } // Следующее состояние автомата

            // Задание правил перехода
            turingMachine.Transitions = new List<Transition>
            {
                new Transition("q0", ' ', ' ', Direction.Right, "q0"),
                new Transition("q0", '1', '1', Direction.Right, "q0"),
                new Transition("q0", '+', '1', Direction.Right, "q1"),

                // Правила для перехода ко второму числу
                new Transition("q1", '1', '1', Direction.Right, "q1"),
                new Transition("q1", ' ', ' ', Direction.Left, "q2"),

                // Правила для второго числа
                new Transition("q2", '1', ' ', Direction.Left, "q4"),
                new Transition("q4", '1', '1', Direction.Left, "q3"),

                // Правила переходу для заміни "+" на "-"
                //new Transition("q0", '_', '_', Direction.Right, "q3"),
                //new Transition("q0", '+', '+', Direction.Right, "q1"),
                //new Transition("q1", '_', '_', Direction.Left, "q3"),
                //new Transition("q1", '+', '-', Direction.Right, "q0"),

            };

            // Обновление отображения
            UpdateTapePictureBox();
            UpdateStateLabel();
            UpdateHeadLabel();
            UpdateResultLabel();
        }

        private void UpdateResultLabel()
        {
            if (turingMachine.State == "q3")
            {
                int plusIndex = turingMachine.Tape.LastIndexOf('+');
                if (plusIndex != -1)
                {
                    turingMachine.Tape = turingMachine.Tape.Substring(0, plusIndex + 1) + turingMachine.Tape.Substring(plusIndex + 1);
                }

                resultLabel.Text = "Результат: " + turingMachine.Tape;
            }
            else
            {
                resultLabel.Text = string.Empty;
            }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            // Выполнение одного шага машины Тьюринга
            turingMachine.Step();

            if (turingMachine.State == "q3")
            {
                int plusIndex = turingMachine.Tape.LastIndexOf('+');
                if (plusIndex != -1)
                {
                    turingMachine.Tape = turingMachine.Tape.Substring(0, plusIndex + 1) + turingMachine.Tape.Substring(plusIndex + 1);
                }

                // Остановить таймер
                timer.Stop();

                turingMachine.Tape = turingMachine.Tape.Replace(" ", "");
                resultLabel.Text = "Результат: " + turingMachine.Tape;
                //MessageBox.Show("Результат: " + turingMachine.Tape);
                return;
            }

            // Обновление отображения
            UpdateTapePictureBox();
            UpdateStateLabel();
            UpdateHeadLabel();
            UpdateResultLabel();

        }

        private void RunButton_Click(object sender, EventArgs e)
        {
            if (timer.Enabled)
            {
                timer.Stop();
                return;
            }

           // Выполнение одного шага машины Тьюринга
            turingMachine.Step();

            if (turingMachine.State == "q3")
            {
                int plusIndex = turingMachine.Tape.LastIndexOf('+');
                if (plusIndex != -1)
                {
                    turingMachine.Tape = turingMachine.Tape.Substring(0, plusIndex + 1) + turingMachine.Tape.Substring(plusIndex + 1);
                }

                //MessageBox.Show("Результат: " + turingMachine.Tape);
                turingMachine.Tape = turingMachine.Tape.Replace(" ", "");
                resultLabel.Text = "Результат: " + turingMachine.Tape;
                return;
            }

            // Обновление отображения
            UpdateTapePictureBox();
            UpdateStateLabel();
            UpdateHeadLabel();
            UpdateResultLabel();
        }
        
        private void UpdateTapePictureBox()
        {
            // Очистка PictureBox
            tapePictureBox.Image = new Bitmap(tapePictureBox.Width, tapePictureBox.Height);
            Graphics g = Graphics.FromImage(tapePictureBox.Image);
            g.Clear(Color.White);

            // Получение информации о ленте и головке
            string tape = turingMachine.Tape;
            int headIndex = turingMachine.HeadIndex;

            // Отрисовка ленты
            int cellWidth = tapePictureBox.Width / (tape.Length + 1);
            int cellHeight = tapePictureBox.Height;
            for (int i = 0; i < tape.Length; i++)
            {
                Rectangle cellRect = new Rectangle(i * cellWidth, 0, cellWidth, cellHeight);
                g.DrawRectangle(Pens.Black, cellRect);
                g.DrawString(tape[i].ToString(), Font, Brushes.Black, cellRect, new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });
            }

            // Отрисовка головки
            Rectangle headRect = new Rectangle(headIndex * cellWidth, 0, cellWidth, cellHeight);
            Color transparentRed = Color.FromArgb(128, Color.Gray); // Полупрозрачный красный цвет
            using (Brush brush = new SolidBrush(transparentRed))
            {
                g.FillRectangle(brush, headRect);
            }
        }

        private void UpdateStateLabel()
        {
            stateLabel.Text = "Состояние: " + turingMachine.State + " (" + turingMachine.Steps + " steps)";
        }

        private void UpdateHeadLabel()
        {
            headLabel.Text = "Головка автомата: " + turingMachine.HeadIndex;
        }

        private void autoMoveButton_Click(object sender, EventArgs e)
        {
            timer.Start();
        }

        private void buttonInput_Click(object sender, EventArgs e)
        {
            // Получение входного слова из TextBox
            string inputWord = inputTextBox.Text;

            // Проверка, что входное слово не является пустым
            if (string.IsNullOrWhiteSpace(inputWord))
            {
                MessageBox.Show("Введите входное слово.");
                return;
            }

            // Обнуление состояния машины Тьюринга
            turingMachine.Reset();

            // Задание начальной ленты
            turingMachine.Tape = inputWord + ' ';

            // Обновление положения головки
            turingMachine.HeadIndex = 0;

            // Обновление отображения
            UpdateTapePictureBox();
            UpdateStateLabel();
            UpdateHeadLabel();
            InitializeTimer();
            UpdateResultLabel();
        }
    }

    public enum Direction
    {
        Left,
        Right
    }


    public class Transition
    {
        public string CurrentState { get; set; } // Текущее состояние автомата
        public char ReadSymbol { get; set; } // Символ, считанный с ленты
        public char WriteSymbol { get; set; } // Символ, который нужно записать на ленту
        public Direction MoveDirection { get; set; } // Направление движения головки ленты
        public string NextState { get; set; } // Следующее состояние автомата

        public Transition(string currentState, char readSymbol, char writeSymbol, Direction moveDirection, string nextState)
        {
            CurrentState = currentState;
            ReadSymbol = readSymbol;
            WriteSymbol = writeSymbol;
            MoveDirection = moveDirection;
            NextState = nextState;
        }
    }

    public class TuringMachine
    {
        public string Tape { get; set; } // Лента автомата
        public int HeadIndex { get; set; } // Текущий индекс головки ленты
        public string State { get; set; } // Текущее состояние автомата

        public int Steps { get; set; } // Количество выполненных шагов автомата
        public List<Transition> Transitions { get; set; } // Список правил перехода для автомата

        public void Reset()
        {
            Tape = string.Empty; // Сбросить ленту, установив пустую строку
            HeadIndex = 0; // Сбросить индекс головки ленты, установив 0
            State = "q0"; // Сбросить состояние автомата, установив начальное состояние "q0"
            Steps = 0; // Сбросить количество шагов, установив 0
        }

        public void Step()
        {
            // Получить текущий символ на ленте
            char currentSymbol = Tape[HeadIndex];

            // Найти соответствующее правило перехода
            Transition transition = Transitions.FirstOrDefault(t => t.CurrentState == State && t.ReadSymbol == currentSymbol);

            if (transition != null)
            {
                // Записать новый символ на ленту
                StringBuilder tapeBuilder = new StringBuilder(Tape);
                tapeBuilder[HeadIndex] = transition.WriteSymbol;
                Tape = tapeBuilder.ToString();

                // Увеличить количество шагов
                Steps++;

                // Сдвинуть головку ленты в указанном направлении
                if (transition.MoveDirection == Direction.Left)
                {
                    HeadIndex--;
                }
                else
                {
                    HeadIndex++;
                }

                // Изменить текущее состояние
                State = transition.NextState;

                // Проверка, если головка выходит за пределы ленты
                if (HeadIndex < 0 || HeadIndex >= Tape.Length)
                {
                    MessageBox.Show("Результат: " + Tape);
                    return;
                }
            }
            else
            {
                MessageBox.Show("Не найдено правило перехода для текущего состояния и символа.");
                return;
            }
        }
    }
}
