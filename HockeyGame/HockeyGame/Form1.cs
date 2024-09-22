using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HockeyGame
{
    public partial class Form1 : Form
    {
        private Timer gameTimer;
        private Puck puck;
        private Athlete[] athletes;
        private Random random = new Random();
        public Form1()
        {
            InitializeComponent();
            InitializeGame();
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void InitializeGame()
        {
            // Создаем шайбу и хоккеистов
            puck = new Puck(this.ClientSize.Width / 2, this.ClientSize.Height / 2);
            athletes = new Athlete[]
            {
                new Athlete(100, 100, Color.Red),
                new Athlete(this.ClientSize.Width - 100, 100, Color.Blue),
                new Athlete(100, this.ClientSize.Height - 100, Color.Red),
                new Athlete(this.ClientSize.Width - 100, this.ClientSize.Height - 100, Color.Blue)
            };

            // Инициализируем таймер игры
            gameTimer = new Timer();
            gameTimer.Interval = 16; // примерно 60 кадров в секунду
            gameTimer.Tick += GameTimer_Tick;
            gameTimer.Start();
        }

        private void GameTimer_Tick(object sender, EventArgs e)
        {
            // Обновляем позиции хоккеистов и шайбы
            foreach (var athlete in athletes)
            {
                athlete.Update(puck);
            }
            puck.Update(this.ClientSize);

            // Перерисовываем поле
            this.Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            // Рисуем хоккеистов
            foreach (var athlete in athletes)
            {
                athlete.Draw(e.Graphics);
            }

            // Рисуем шайбу
            puck.Draw(e.Graphics);
        }
    }

    public class Puck
    {
        public PointF Position { get; private set; }
        public float Radius { get; private set; } = 10;
        public PointF Velocity { get; private set; }

        public Puck(float x, float y)
        {
            Position = new PointF(x, y);
            Velocity = new PointF(2, 2);
        }

        public void Update(Size clientSize)
        {
            Position = new PointF(Position.X + Velocity.X, Position.Y + Velocity.Y);

            // Проверка на столкновение с краями экрана
            if (Position.X - Radius < 0 || Position.X + Radius > clientSize.Width)
                Velocity = new PointF(-Velocity.X, Velocity.Y);
            if (Position.Y - Radius < 0 || Position.Y + Radius > clientSize.Height)
                Velocity = new PointF(Velocity.X, -Velocity.Y);
        }

        public void Draw(Graphics g)
        {
            g.FillEllipse(Brushes.Black, Position.X - Radius, Position.Y - Radius, Radius * 2, Radius * 2);
        }
    }

    public class Athlete
    {
        public PointF Position { get; private set; }
        public float Radius { get; private set; } = 15;
        public Color Color { get; private set; }
        private PointF velocity;

        public Athlete(float x, float y, Color color)
        {
            Position = new PointF(x, y);
            Color = color;
            velocity = new PointF(1, 1);
        }

        public void Update(Puck puck)
        {
            // Простое движение к шайбе
            float dx = puck.Position.X - Position.X;
            float dy = puck.Position.Y - Position.Y;
            float distance = (float)Math.Sqrt(dx * dx + dy * dy);

            if (distance > 0)
            {
                dx /= distance;
                dy /= distance;
                Position = new PointF(Position.X + dx, Position.Y + dy);
            }
        }

        public void Draw(Graphics g)
        {
            using (Brush brush = new SolidBrush(Color))
            {
                g.FillEllipse(brush, Position.X - Radius, Position.Y - Radius, Radius * 2, Radius * 2);
            }
        }
    }
}

