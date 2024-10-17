using System;
using System.Windows.Forms;

namespace lab6
{
    public partial class Form1 : Form
    {
        // Определение состояний
        enum AccountState
        {
            GoodAccount,  // Счет хороший
            Overdrawn     // Превышены расходы по счету
        }

        private AccountState currentState; // Текущее состояние счета
        private decimal balance;           // Баланс счета
        private decimal debt;              // Долг по счету

        public Form1()
        {
            InitializeComponent();
            currentState = AccountState.GoodAccount; // Начальное состояние
            balance = 0;                             // Начальный баланс
            debt = 0;                                // Долг по умолчанию отсутствует
            UpdateStateLabel();
            UpdateBalanceLabel();
            UpdateDebtLabel();
        }

        // Метод для обновления текста состояния
        private void UpdateStateLabel()
        {
            switch (currentState)
            {
                case AccountState.GoodAccount:
                    label1.Text = "Счет Хороший";
                    break;
                case AccountState.Overdrawn:
                    label1.Text = "Превышены Расходы по Счету";
                    break;
            }
        }

        // Метод для обновления текста баланса
        private void UpdateBalanceLabel()
        {
            label2.Text = "Баланс: " + balance.ToString("C");
        }

        // Метод для обновления текста долга
        private void UpdateDebtLabel()
        {
            label3.Text = "Долг: " + debt.ToString("C");
        }

        // Закрытие счета (обнуление баланса и долга)
        private void button2_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Счет закрыт.");
            currentState = AccountState.GoodAccount; // Сброс состояния
            balance = 0;                             // Обнуление баланса
            debt = 0;                                // Обнуление долга
            UpdateStateLabel();
            UpdateBalanceLabel();
            UpdateDebtLabel();
            textBox1.Clear();  // Очистка поля ввода
        }

        // Вклад (внесение денег только на баланс, долг не изменяется)
        private void button3_Click(object sender, EventArgs e)
        {
            decimal depositAmount;
            if (decimal.TryParse(textBox1.Text, out depositAmount) && depositAmount > 0)
            {
                balance += depositAmount; // Вклад всегда увеличивает баланс
                UpdateStateLabel();
                UpdateBalanceLabel();
            }
            else
            {
                MessageBox.Show("Введите корректную сумму для вклада.");
            }

            textBox1.Clear();  // Очистка поля ввода
        }

        // Обычное снятие денег
        private void button4_Click(object sender, EventArgs e)
        {
            decimal withdrawalAmount;
            if (decimal.TryParse(textBox1.Text, out withdrawalAmount) && withdrawalAmount > 0)
            {
                if (balance >= withdrawalAmount)
                {
                    balance -= withdrawalAmount; // Снятие с баланса
                }
                else
                {
                    debt += (withdrawalAmount - balance); // Добавляем долг
                    balance = 0;
                    currentState = AccountState.Overdrawn; // Меняем состояние на перерасход
                }

                UpdateStateLabel();
                UpdateBalanceLabel();
                UpdateDebtLabel();
            }
            else
            {
                MessageBox.Show("Введите корректную сумму для снятия.");
            }

            textBox1.Clear();  // Очистка поля ввода
        }

        // Погашение долга
        private void button6_Click(object sender, EventArgs e)
        {
            if (currentState == AccountState.Overdrawn)
            {
                // Если на балансе достаточно средств для погашения долга
                if (balance >= debt)
                {
                    balance -= debt;  // Списываем долг с баланса
                    debt = 0;         // Обнуляем долг
                    currentState = AccountState.GoodAccount; // Состояние счета становится хорошим
                    MessageBox.Show("Долг успешно погашен с баланса.");
                }
                else
                {
                    MessageBox.Show("Недостаточно средств на балансе для погашения долга.");
                }
            }
            else
            {
                MessageBox.Show("У вас нет долга.");
            }

            // Обновляем все соответствующие поля
            UpdateStateLabel();
            UpdateBalanceLabel();
            UpdateDebtLabel();   // Обновляем отображение долга
            textBox1.Clear();    // Очистка поля ввода
        }


    }
}