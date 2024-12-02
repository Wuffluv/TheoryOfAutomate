using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace lab6
{
    public partial class Form1 : Form
    {
        // Модель сети Петри
        private class PetriNet
        {
            public Dictionary<string, int> Places { get; set; } = new Dictionary<string, int>();
            public Dictionary<string, Func<bool>> Transitions { get; set; } = new Dictionary<string, Func<bool>>();

            // Метод для срабатывания перехода
            public string Fire(string transitionName)
            {
                if (Transitions.ContainsKey(transitionName) && Transitions[transitionName]())
                {
                    return transitionName;
                }
                return null;
            }
        }

        private PetriNet accountNet = new PetriNet();
        private decimal balance;
        private decimal debt;
        private string activeTransition = null;
        private Timer animationTimer;

        public Form1()
        {
            InitializeComponent();

            // Инициализация таймера для анимации
            animationTimer = new Timer();
            animationTimer.Interval = 500; // Длительность анимации в миллисекундах (0.5 секунды)
            animationTimer.Tick += AnimationTimer_Tick;

            // Инициализация сети Петри
            accountNet.Places["GoodAccount"] = 1;  // Начальное состояние — хороший счет (1 фишка)
            accountNet.Places["Overdrawn"] = 0;    // Нет долгов (0 фишек)

            // Определение переходов
            accountNet.Transitions["Deposit"] = () =>
            {
                if (debt > 0 && balance >= debt)
                {
                    balance -= debt; // Погашаем долг с баланса
                    debt = 0;        // Обнуляем долг
                    accountNet.Places["Overdrawn"] = 0; // Убираем фишку из "Overdrawn"
                    accountNet.Places["GoodAccount"] = 1; // Ставим фишку в "GoodAccount"
                }
                else if (debt == 0)
                {
                    accountNet.Places["GoodAccount"] = 1; // Ставим фишку в "GoodAccount"
                }
                // Если долг остается, фишки не меняем
                return true;
            };

            accountNet.Transitions["Withdraw"] = () =>
            {
                if (balance > 0 || debt > 0)
                {
                    accountNet.Places["GoodAccount"] = 0; // Убираем фишку из "GoodAccount"
                    accountNet.Places["Overdrawn"] = 1;   // Ставим фишку в "Overdrawn"
                    return true;
                }
                return false;
            };

            accountNet.Transitions["RepayDebt"] = () =>
            {
                if (debt > 0 && balance >= debt)
                {
                    balance -= debt; // Погашаем долг с баланса
                    debt = 0;        // Обнуляем долг
                    accountNet.Places["Overdrawn"] = 0;   // Убираем фишку из "Overdrawn"
                    accountNet.Places["GoodAccount"] = 1; // Ставим фишку в "GoodAccount"
                    return true;
                }
                return false;
            };

            UpdateUI();     // Обновляем интерфейс
            DrawPetriNet(); // Рисуем сеть Петри
        }

        // Обработчик таймера анимации
        private void AnimationTimer_Tick(object sender, EventArgs e)
        {
            activeTransition = null;    // Сбрасываем активный переход
            animationTimer.Stop();      // Останавливаем таймер
            DrawPetriNet();             // Перерисовываем граф
        }

        // Метод для обновления пользовательского интерфейса
        private void UpdateUI()
        {
            // Обновление состояния счета
            if (accountNet.Places["GoodAccount"] > 0)
            {
                label1.Text = "Счет Хороший";
            }
            else if (accountNet.Places["Overdrawn"] > 0)
            {
                label1.Text = "Превышены Расходы по Счету";
            }

            // Обновление баланса и долга
            label2.Text = "Баланс: " + balance.ToString("C");
            label3.Text = "Долг: " + debt.ToString("C");
        }

        // Метод для рисования сети Петри
        private void DrawPetriNet()
        {
            Bitmap bitmap = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                g.Clear(Color.White);

                // Рисуем места
                int placeRadius = 30;
                Point goodAccountPosition = new Point(50, 100);
                Point overdrawnPosition = new Point(200, 100);

                DrawPlace(g, goodAccountPosition, "GoodAccount", accountNet.Places["GoodAccount"], placeRadius);
                DrawPlace(g, overdrawnPosition, "Overdrawn", accountNet.Places["Overdrawn"], placeRadius);

                // Рисуем переходы
                Point depositPosition = new Point(125, 50);
                Point withdrawPosition = new Point(125, 150);
                DrawTransition(g, depositPosition, "Deposit");
                DrawTransition(g, withdrawPosition, "Withdraw");

                // Рисуем связи
                g.DrawLine(Pens.Black, goodAccountPosition.X + placeRadius, goodAccountPosition.Y + placeRadius / 2,
                    depositPosition.X, depositPosition.Y + 15);
                g.DrawLine(Pens.Black, overdrawnPosition.X, overdrawnPosition.Y + placeRadius / 2,
                    depositPosition.X, depositPosition.Y + 15);
                g.DrawLine(Pens.Black, goodAccountPosition.X + placeRadius, goodAccountPosition.Y + placeRadius / 2,
                    withdrawPosition.X, withdrawPosition.Y + 15);
                g.DrawLine(Pens.Black, overdrawnPosition.X, overdrawnPosition.Y + placeRadius / 2,
                    withdrawPosition.X, withdrawPosition.Y + 15);
            }

            pictureBox1.Image = bitmap;
        }

        // Метод для рисования места
        private void DrawPlace(Graphics g, Point position, string name, int tokens, int radius)
        {
            g.DrawEllipse(Pens.Black, position.X, position.Y, radius, radius);
            if (tokens > 0)
            {
                g.FillEllipse(Brushes.Blue, position.X, position.Y, radius, radius);
            }
            g.DrawString($"{name} ({tokens})", DefaultFont, Brushes.Black, position.X - 10, position.Y - 20);
        }

        // Метод для рисования перехода
        private void DrawTransition(Graphics g, Point position, string name)
        {
            Brush brush = Brushes.Gray;
            if (name == activeTransition)
            {
                brush = Brushes.Green; // Подсвечиваем активный переход
            }
            g.FillRectangle(brush, position.X - 10, position.Y, 20, 30);
            g.DrawRectangle(Pens.Black, position.X - 10, position.Y, 20, 30);
            g.DrawString(name, DefaultFont, Brushes.Black, position.X - 30, position.Y - 20);
        }

        // Обработка события кнопки "Вклад"
        private void button3_Click(object sender, EventArgs e)
        {
            if (decimal.TryParse(textBox1.Text, out var depositAmount) && depositAmount > 0)
            {
                balance += depositAmount;      // Увеличиваем баланс
                var firedTransition = accountNet.Fire("Deposit"); // Срабатывает переход "Deposit"
                if (firedTransition != null)
                {
                    activeTransition = firedTransition;
                    animationTimer.Start();
                }
                DrawPetriNet();
                UpdateUI();                    // Обновляем интерфейс
            }
            else
            {
                MessageBox.Show("Введите корректную сумму для вклада.");
            }
            textBox1.Clear(); // Очищаем поле ввода
        }

        // Обработка события кнопки "Снятие"
        private void button4_Click(object sender, EventArgs e)
        {
            if (decimal.TryParse(textBox1.Text, out var withdrawalAmount) && withdrawalAmount > 0)
            {
                if (balance >= withdrawalAmount)
                {
                    balance -= withdrawalAmount; // Снимаем с баланса
                }
                else
                {
                    debt += (withdrawalAmount - balance); // Увеличиваем долг
                    balance = 0;                          // Обнуляем баланс
                }
                var firedTransition = accountNet.Fire("Withdraw"); // Срабатывает переход "Withdraw"
                if (firedTransition != null)
                {
                    activeTransition = firedTransition;
                    animationTimer.Start();
                }
                DrawPetriNet();
                UpdateUI();                   // Обновляем интерфейс
            }
            else
            {
                MessageBox.Show("Введите корректную сумму для снятия.");
            }
            textBox1.Clear(); // Очищаем поле ввода
        }

        // Обработка события кнопки "Погашение долга"
        private void button6_Click(object sender, EventArgs e)
        {
            var firedTransition = accountNet.Fire("RepayDebt"); // Срабатывает переход "RepayDebt"
            if (firedTransition != null)
            {
                activeTransition = firedTransition;
                animationTimer.Start();
                MessageBox.Show("Долг успешно погашен.");
            }
            else
            {
                MessageBox.Show("Недостаточно средств для погашения долга.");
            }
            DrawPetriNet();
            UpdateUI();     // Обновляем интерфейс
            textBox1.Clear(); // Очищаем поле ввода
        }

        // Обработка события кнопки "Закрытие счета"
        private void button2_Click(object sender, EventArgs e)
        {
            if (debt > 0)
            {
                MessageBox.Show("Невозможно закрыть счет при наличии долга.");
                return;
            }
            balance = 0;                             // Обнуляем баланс
            accountNet.Places["GoodAccount"] = 0;    // Убираем фишку из "GoodAccount"
            MessageBox.Show("Счет успешно закрыт.");
            UpdateUI();     // Обновляем интерфейс
            textBox1.Clear(); // Очищаем поле ввода
        }

        // Обработка события кнопки "Разрешенное снятие при долге"
        private void button5_Click(object sender, EventArgs e)
        {
            if (accountNet.Places["Overdrawn"] > 0) // Проверяем состояние счета
            {
                decimal allowedWithdrawal = 100;     // Разрешенная сумма
                balance -= allowedWithdrawal;        // Уменьшаем баланс
                debt += allowedWithdrawal;           // Увеличиваем долг
                MessageBox.Show($"Разрешенное снятие: {allowedWithdrawal.ToString("C")}.");
                UpdateUI();     // Обновляем интерфейс
            }
            else
            {
                MessageBox.Show("Разрешенное снятие возможно только при перерасходе.");
            }
            textBox1.Clear(); // Очищаем поле ввода
        }
    }
}
