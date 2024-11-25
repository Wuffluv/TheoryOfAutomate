using System;
using System.Collections.Generic;
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
            public bool Fire(string transitionName)
            {
                if (Transitions.ContainsKey(transitionName) && Transitions[transitionName]())
                {
                    return true;
                }
                return false;
            }

            // Метод добавления фишек
            public void AddTokens(string place, int count)
            {
                if (Places.ContainsKey(place))
                {
                    Places[place] += count;
                }
            }

            // Метод удаления фишек
            public void RemoveTokens(string place, int count)
            {
                if (Places.ContainsKey(place) && Places[place] >= count)
                {
                    Places[place] -= count;
                }
            }
        }

        private PetriNet accountNet = new PetriNet();
        private decimal balance;
        private decimal debt;

        public Form1()
        {
            InitializeComponent();

            // Инициализация сети Петри
            accountNet.Places["GoodAccount"] = 1;  // Начальное состояние — хороший счет (есть одна фишка)
            accountNet.Places["Overdrawn"] = 0;   // Нет долгов (нет фишек)

            // Определение переходов
            accountNet.Transitions["Deposit"] = () =>
            {
                if (accountNet.Places["Overdrawn"] > 0)
                {
                    accountNet.RemoveTokens("Overdrawn", 1); // Убираем фишку из места "Overdrawn"
                }
                accountNet.AddTokens("GoodAccount", 1); // Добавляем фишку в место "GoodAccount"
                return true;
            };

            accountNet.Transitions["Withdraw"] = () =>
            {
                if (balance > 0 || debt > 0)
                {
                    if (accountNet.Places["GoodAccount"] > 0)
                    {
                        accountNet.RemoveTokens("GoodAccount", 1); // Убираем фишку из места "GoodAccount"
                    }
                    accountNet.AddTokens("Overdrawn", 1); // Добавляем фишку в место "Overdrawn"
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
                    accountNet.RemoveTokens("Overdrawn", 1); // Убираем фишку из места "Overdrawn"
                    accountNet.AddTokens("GoodAccount", 1);  // Добавляем фишку в место "GoodAccount"
                    return true;
                }
                return false;
            };

            UpdateUI(); // Обновляем пользовательский интерфейс
        }

        // Метод для обновления пользовательского интерфейса
        private void UpdateUI()
        {
            // Обновление состояния
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

        // Обработка события кнопки "Вклад"
        private void button3_Click(object sender, EventArgs e)
        {
            if (decimal.TryParse(textBox1.Text, out var depositAmount) && depositAmount > 0)
            {
                balance += depositAmount; // Увеличиваем баланс на сумму вклада
                accountNet.Fire("Deposit"); // Переход "Deposit"
                UpdateUI(); // Обновляем пользовательский интерфейс
            }
            else
            {
                MessageBox.Show("Введите корректную сумму для вклада.");
            }
            textBox1.Clear(); // Очистка поля ввода
        }

        // Обработка события кнопки "Снятие"
        private void button4_Click(object sender, EventArgs e)
        {
            if (decimal.TryParse(textBox1.Text, out var withdrawalAmount) && withdrawalAmount > 0)
            {
                if (balance >= withdrawalAmount)
                {
                    balance -= withdrawalAmount; // Снимаем деньги с баланса
                }
                else
                {
                    debt += (withdrawalAmount - balance); // Увеличиваем долг на разницу
                    balance = 0; // Обнуляем баланс
                }
                accountNet.Fire("Withdraw"); // Переход "Withdraw"
                UpdateUI(); // Обновляем пользовательский интерфейс
            }
            else
            {
                MessageBox.Show("Введите корректную сумму для снятия.");
            }
            textBox1.Clear(); // Очистка поля ввода
        }

        // Обработка события кнопки "Погашение долга"
        private void button6_Click(object sender, EventArgs e)
        {
            if (accountNet.Fire("RepayDebt")) // Переход "RepayDebt"
            {
                MessageBox.Show("Долг успешно погашен.");
            }
            else
            {
                MessageBox.Show("Недостаточно средств для погашения долга.");
            }
            UpdateUI(); // Обновляем пользовательский интерфейс
            textBox1.Clear(); // Очистка поля ввода
        }

        // Обработка события кнопки "Закрытие счета"
        private void button2_Click(object sender, EventArgs e)
        {
            if (debt > 0)
            {
                MessageBox.Show("Невозможно закрыть счет при наличии долга.");
                return;
            }
            balance = 0; // Обнуляем баланс
            accountNet.Places["GoodAccount"] = 0; // Убираем фишки из места "GoodAccount"
            MessageBox.Show("Счет успешно закрыт.");
            UpdateUI(); // Обновляем пользовательский интерфейс
            textBox1.Clear(); // Очистка поля ввода
        }

        // Обработка события кнопки "Разрешенное снятие при долге"
        private void button5_Click(object sender, EventArgs e)
        {
            if (accountNet.Places["Overdrawn"] > 0) // Проверяем, находится ли счет в состоянии перерасхода
            {
                decimal allowedWithdrawal = 100; // Разрешенная сумма для снятия
                balance -= allowedWithdrawal; // Уменьшаем баланс на разрешенную сумму
                debt += allowedWithdrawal; // Увеличиваем долг
                MessageBox.Show($"Разрешенное снятие: {allowedWithdrawal.ToString("C")}.");
                UpdateUI(); // Обновляем пользовательский интерфейс
            }
            else
            {
                MessageBox.Show("Разрешенное снятие возможно только при перерасходе.");
            }
            textBox1.Clear(); // Очистка поля ввода
        }
    }
}
