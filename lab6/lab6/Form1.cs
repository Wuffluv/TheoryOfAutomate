using System;
using System.Windows.Forms;

namespace lab6
{
    public partial class Form1 : Form
    {
        // Определение состояний счета
        enum AccountState
        {
            GoodAccount,  // Счет хороший (нет долгов)
            Overdrawn     // Превышены расходы по счету (есть долг)
        }

        private AccountState currentState; // Текущее состояние счета
        private decimal balance;           // Текущий баланс счета
        private decimal debt;              // Текущий долг по счету

        public Form1()
        {
            InitializeComponent();
            currentState = AccountState.GoodAccount; // Начальное состояние счета — хороший счет
            balance = 0;                             // Начальный баланс — 0
            debt = 0;                                // Долг отсутствует при старте
            UpdateStateLabel();  // Обновление метки состояния счета
            UpdateBalanceLabel(); // Обновление метки баланса счета
            UpdateDebtLabel();    // Обновление метки долга
        }

        // Метод для обновления отображения состояния счета
        private void UpdateStateLabel()
        {
            switch (currentState)
            {
                case AccountState.GoodAccount:
                    label1.Text = "Счет Хороший"; // Вывод состояния "Счет хороший"
                    break;
                case AccountState.Overdrawn:
                    label1.Text = "Превышены Расходы по Счету"; // Вывод состояния "Счет в долге"
                    break;
            }
        }

        // Метод для обновления отображения баланса счета
        private void UpdateBalanceLabel()
        {
            label2.Text = "Баланс: " + balance.ToString("C"); // Вывод баланса в формате валюты
        }

        // Метод для обновления отображения долга
        private void UpdateDebtLabel()
        {
            label3.Text = "Долг: " + debt.ToString("C"); // Вывод долга в формате валюты
        }

        // Закрытие счета (сброс баланса и долга)
        private void button2_Click(object sender, EventArgs e)
        {
            if (debt > 0)
            {
                MessageBox.Show("Невозможно закрыть счет при наличии долга"); //Запрещает закрывать счет при наличии долга
                return;
            }
            MessageBox.Show("Счет закрыт.");
            currentState = AccountState.GoodAccount; // Сброс состояния на "хороший счет"
            balance = 0;                             // Обнуление баланса
            debt = 0;                                // Обнуление долга
            UpdateStateLabel();  // Обновление метки состояния счета
            UpdateBalanceLabel(); // Обновление метки баланса
            UpdateDebtLabel();    // Обновление метки долга
            textBox1.Clear();     // Очистка поля ввода
        }

        // Вклад (внесение денег на баланс)
        private void button3_Click(object sender, EventArgs e)
        {
            decimal depositAmount;
            if (decimal.TryParse(textBox1.Text, out depositAmount) && depositAmount > 0)
            {
                balance += depositAmount; // Увеличение баланса на сумму вклада
                UpdateStateLabel();       // Обновление состояния счета (если было изменено)
                UpdateBalanceLabel();     // Обновление отображения баланса
            }
            else
            {
                MessageBox.Show("Введите корректную сумму для вклада."); // Проверка на корректность ввода
            }

            textBox1.Clear();  // Очистка поля ввода
        }

        // Обычное снятие денег
        private void button4_Click(object sender, EventArgs e)
        {
            decimal withdrawalAmount;
            if (decimal.TryParse(textBox1.Text, out withdrawalAmount) && withdrawalAmount > 0)
            {
                if (debt > 0) // Если есть долг
                {
                    if (withdrawalAmount >= debt)
                    {
                        // Снимаем с суммы долг и оставшиеся деньги снимаем с баланса
                        withdrawalAmount -= debt;
                        debt = 0; // Долга нет
                        currentState = AccountState.GoodAccount; // Меняем состояние счета
                    }
                    else
                    {
                        // Уменьшаем долгг
                        debt -= withdrawalAmount;
                        withdrawalAmount = 0; ////
                    }
                }
                
                if (withdrawalAmount > 0)
                {
                    if (balance >= withdrawalAmount)
                    {
                        balance -= withdrawalAmount; 
                    }
                    else
                    {
                        debt += (withdrawalAmount - balance); 
                        balance = 0;
                        currentState = AccountState.Overdrawn;  //Обновление состояния на перерасход

                    }
                }

                UpdateStateLabel();
                UpdateBalanceLabel();
                UpdateDebtLabel(); // Обновляем отображение долга
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
            if (currentState == AccountState.Overdrawn) // Проверяем, есть ли долг
            {
                // Если на балансе достаточно средств для погашения долга
                if (balance >= debt)
                {
                    balance -= debt;  // Списание долга с баланса
                    debt = 0;         // Обнуление долга
                    currentState = AccountState.GoodAccount; // Изменение состояния на "хороший счет"
                    MessageBox.Show("Долг успешно погашен с баланса.");
                }
                else
                {
                    MessageBox.Show("Недостаточно средств на балансе для погашения долга.");
                }
            }
            else
            {
                MessageBox.Show("У вас нет долга."); // Сообщение, если долга нет
            }

            UpdateStateLabel();   // Обновление состояния счета
            UpdateBalanceLabel(); // Обновление баланса
            UpdateDebtLabel();    // Обновление долга
            textBox1.Clear();     // Очистка поля ввода
        }

        // Разрешенное снятие денег при перерасходе
        private void button5_Click(object sender, EventArgs e)
        {
            if (currentState == AccountState.Overdrawn) // Проверяем, находится ли счет в долге
            {
                decimal allowedWithdrawal = 500; // Фиксированная сумма для разрешенного снятия

                if (balance >= allowedWithdrawal)
                {
                    balance -= allowedWithdrawal; // Списание с баланса разрешенной суммы
                    MessageBox.Show($"Снятие разрешено. Вы сняли {allowedWithdrawal.ToString("C")}.");
                }
                else if (balance > 0)  // Если на балансе меньше фиксированной суммы
                {
                    allowedWithdrawal = balance; // Снимаем оставшиеся средства
                    balance = 0;                  // Обнуляем баланс
                    MessageBox.Show($"Снятие разрешено. Вы сняли оставшиеся {allowedWithdrawal.ToString("C")}.");
                }
                else
                {
                    MessageBox.Show("У вас нет средств на счете для разрешенного снятия.");
                }

                UpdateBalanceLabel(); // Обновляем баланс на интерфейсе
            }
            else
            {
                MessageBox.Show("Счет в порядке, разрешенное снятие не требуется.");
            }

            textBox1.Clear();  // Очистка поля ввода
        }
    }
}
