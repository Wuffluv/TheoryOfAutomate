using System;
using System.Windows.Forms;

namespace Lab6
{
    public partial class Form1 : Form
    {
        private enum AccountState
        {
            GoodStanding,
            Overdrawn
        }

        private AccountState currentState;
        private decimal accountBalance;

        public Form1()
        {
            InitializeComponent();
            currentState = AccountState.GoodStanding;
            accountBalance = 0;
            UpdateUI();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (currentState == AccountState.GoodStanding)
            {
                decimal amount = GetAmount();
                accountBalance -= amount;
                if (accountBalance < 0)
                {
                    currentState = AccountState.Overdrawn;
                }
                UpdateUI();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            decimal amount = GetAmount();
            accountBalance += amount;
            if (currentState == AccountState.Overdrawn && accountBalance >= 0)
            {
                currentState = AccountState.GoodStanding;
            }
            UpdateUI();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (currentState == AccountState.Overdrawn)
            {
                decimal amount = GetAmount();
                accountBalance -= amount;
                UpdateUI();
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            currentState = AccountState.GoodStanding;
            accountBalance = 0;
            UpdateUI();
        }

        private decimal GetAmount()
        {
            decimal amount;
            if (decimal.TryParse(textBox1.Text, out amount))
            {
                return amount;
            }
            else
            {
                MessageBox.Show("Invalid amount");
                return 0;
            }
        }

        private void UpdateUI()
        {
            label1.Text = "Current State: " + currentState;
            label2.Text = "Balance: " + accountBalance.ToString("C");
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}