using System;
using System.Drawing;
using System.Windows.Forms;

namespace _1lab
{
    public partial class Form1 : Form
    {
        public static Form1 Instance { get; private set; }
        public Leaf Leaf { get; private set; }
        public Home Home { get; private set; }
        public Mouse Mouse { get; private set; }
        private Ant ant;
        private Timer timer;
        public float SpeedMultiplier { get; private set; } = 1.0f;

        public Form1()
        {
            InitializeComponent();
            Instance = this;

            // Инициализируем объекты Leaf, Home и Mouse
            Leaf = new Leaf(new PointF(100, 100));
            Home = new Home(new PointF(200, 200));
            Mouse = new Mouse(new PointF(300, 300));

            // Инициализируем муравья
            ant = new Ant(50, 50);

            // Настраиваем таймер
            timer = new Timer();
            timer.Interval = 50; // Каждые 50 мс
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            // Обновляем положение муравья
            ant.Update();
            // Перерисовываем форму
            this.Invalidate();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Ускорение муравья
            SpeedMultiplier = Math.Min(3.0f, SpeedMultiplier + 0.5f);
            label1.Text = "Speed: " + SpeedMultiplier;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // Замедление муравья
            SpeedMultiplier = Math.Max(0.5f, SpeedMultiplier - 0.5f);
            label1.Text = "Speed: " + SpeedMultiplier;
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            // Обновляем положение мыши
            Mouse.Position = new PointF(e.X, e.Y);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Graphics g = e.Graphics;

            // Рисуем лист
            g.FillEllipse(Brushes.Green, Leaf.Position.X - 10, Leaf.Position.Y - 10, 20, 20);

            // Рисуем дом
            g.FillEllipse(Brushes.Red, Home.Position.X - 10, Home.Position.Y - 10, 20, 20);

            // Рисуем муравья
            g.FillEllipse(Brushes.Black, ant.position.X - 5, ant.position.Y - 5, 10, 10);
        }
    }
}
