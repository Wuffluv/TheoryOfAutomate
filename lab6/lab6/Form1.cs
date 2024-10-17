using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        private decimal balance;            // Баланс счета
        private decimal debt;               // Долг по счету

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

        // Открытие счета
        private void button1_Click(object sender, EventArgs e)
        {
            currentState = AccountState.GoodAccount;
            balance = 0;
            debt = 0;
            UpdateStateLabel();
            UpdateBalanceLabel();
            UpdateDebtLabel();
            MessageBox.Show("Счет открыт.");
            textBox1.Clear();  // Очистка поля ввода
        }

        // Закрытие счета
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

        // Вклад (внесение денег)
        private void button3_Click(object sender, EventArgs e)
        {
            decimal depositAmount;
            if (decimal.TryParse(textBox1.Text, out depositAmount) && depositAmount > 0)
            {
                if (currentState == AccountState.Overdrawn)
                {
                    // Погашаем долг в первую очередь
                    if (depositAmount >= debt)
                    {
                        depositAmount -= debt;
                        debt = 0;
                        currentState = AccountState.GoodAccount;
                    }
                    else
                    {
                        debt -= depositAmount;
                        depositAmount = 0;
                    }
                }

                balance += depositAmount;
                UpdateStateLabel();
                UpdateBalanceLabel();
                UpdateDebtLabel();
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
                    balance -= withdrawalAmount;
                }
                else
                {
                    debt += (withdrawalAmount - balance);
                    balance = 0;
                    currentState = AccountState.Overdrawn;
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

        // Разрешенное снятие денег при перерасходе
        private void button5_Click(object sender, EventArgs e)
        {
            if (currentState == AccountState.Overdrawn)
            {
                MessageBox.Show("Разрешено снятие денег при наличии долга.");
            }
            else
            {
                MessageBox.Show("Счет в порядке, разрешенное снятие не требуется.");
            }

            textBox1.Clear();  // Очистка поля ввода
        }

        // Погашение долга
        private void button6_Click(object sender, EventArgs e)
        {
            if (currentState == AccountState.Overdrawn)
            {
                decimal paymentAmount;
                if (decimal.TryParse(textBox1.Text, out paymentAmount) && paymentAmount > 0)
                {
                    if (paymentAmount >= debt)
                    {
                        paymentAmount -= debt;
                        debt = 0;
                        currentState = AccountState.GoodAccount;
                        balance += paymentAmount; // Остаток после погашения идет на баланс
                    }
                    else
                    {
                        debt -= paymentAmount;
                    }
                    UpdateStateLabel();
                    UpdateBalanceLabel();
                    UpdateDebtLabel();
                }
                else
                {
                    MessageBox.Show("Введите корректную сумму для погашения долга.");
                }
            }
            else
            {
                MessageBox.Show("У вас нет долга.");
            }

            textBox1.Clear();  // Очистка поля ввода
        }
    }
}
