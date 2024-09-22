using System;
using System.Drawing;
using System.Windows.Forms;

namespace HockeyGame
{
    public partial class Form1 : Form
    {
        private Timer gameTimer;
        private Puck puck;
        private Athlete[] athletes;
        private int redScore = 0;
        private int blueScore = 0;
        private Rectangle leftGoal, rightGoal;
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
            // Задаем размеры и положение ворот
            leftGoal = new Rectangle(20, this.ClientSize.Height / 2 - 50, 10, 100);
            rightGoal = new Rectangle(this.ClientSize.Width - 30, this.ClientSize.Height / 2 - 50, 10, 100);

            // Создаем шайбу и хоккеистов
            puck = new Puck(this.ClientSize.Width / 2, this.ClientSize.Height / 2, random);
            athletes = new Athlete[]
            {
                new Athlete(100, 100, Color.Red, rightGoal),
                new Athlete(this.ClientSize.Width - 100, 100, Color.Blue, leftGoal),
                new Athlete(100, this.ClientSize.Height - 100, Color.Red, rightGoal),
                new Athlete(this.ClientSize.Width - 100, this.ClientSize.Height - 100, Color.Blue, leftGoal)
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
                athlete.Update(puck, athletes, this.ClientSize);
            }
            puck.Update(this.ClientSize);

            // Проверка на гол
            CheckGoal();

            // Перерисовываем поле
            this.Invalidate();
        }

        private void CheckGoal()
        {
            // Если шайба в левых воротах (гол для синих)
            if (leftGoal.Contains((int)puck.Position.X, (int)puck.Position.Y))
            {
                blueScore++;
                ResetGame();
            }

            // Если шайба в правых воротах (гол для красных)
            if (rightGoal.Contains((int)puck.Position.X, (int)puck.Position.Y))
            {
                redScore++;
                ResetGame();
            }
        }

        private void ResetGame()
        {
            // Возвращаем шайбу на центр и сбрасываем состояние
            puck = new Puck(this.ClientSize.Width / 2, this.ClientSize.Height / 2, random);
            puck.Possessor = null;

            // Сбрасываем состояние хоккеистов
            foreach (var athlete in athletes)
            {
                athlete.Reset();
            }
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

            // Рисуем ворота
            e.Graphics.FillRectangle(Brushes.White, leftGoal);
            e.Graphics.FillRectangle(Brushes.White, rightGoal);

            // Отображаем счёт
            using (Font font = new Font("Arial", 16))
            {
                e.Graphics.DrawString($"Red: {redScore}  Blue: {blueScore}", font, Brushes.Black, this.ClientSize.Width / 2 - 50, 20);
            }
        }
    }

    public class Puck
    {
        public PointF Position { get; set; }
        public float Radius { get; private set; } = 10;
        public PointF Velocity { get; private set; }
        private Random random;

        public Athlete Possessor { get; set; }

        public Puck(float x, float y, Random random)
        {
            Position = new PointF(x, y);
            this.random = random;
            SetRandomDirection();
            Possessor = null;
        }

        public void SetRandomDirection()
        {
            // Случайное направление шайбы
            float angle = (float)(random.NextDouble() * 2 * Math.PI);
            float speed = 4;  // Скорость шайбы
            Velocity = new PointF((float)Math.Cos(angle) * speed, (float)Math.Sin(angle) * speed);
        }

        public void Update(Size clientSize)
        {
            // Если шайба не у игрока, она движется самостоятельно
            if (Possessor == null)
            {
                Position = new PointF(Position.X + Velocity.X, Position.Y + Velocity.Y);

                // Проверка на столкновение с краями экрана
                if (Position.X - Radius < 0 || Position.X + Radius > clientSize.Width)
                    Velocity = new PointF(-Velocity.X, Velocity.Y);
                if (Position.Y - Radius < 0 || Position.Y + Radius > clientSize.Height)
                    Velocity = new PointF(Velocity.X, -Velocity.Y);
            }
            else
            {
                // Если шайба у игрока, она следует за ним
                Position = Possessor.Position;
            }
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
        private Rectangle targetGoal; // Цель — ворота противника
        private bool hasPuck = false;
        private const float MaxSpeed = 6f; // Максимальная скорость
        private const float MinDistance = 20f; // Минимальное расстояние для избежания столкновений
        private const float SpeedMultiplier = 2.5f; // Множитель скорости

        private PointF initialPosition;

        public Athlete(float x, float y, Color color, Rectangle goal)
        {
            Position = new PointF(x, y);
            initialPosition = Position;
            Color = color;
            targetGoal = goal;
        }

        public void Reset()
        {
            hasPuck = false;
            Position = initialPosition;
        }

        public void Update(Puck puck, Athlete[] athletes, Size clientSize)
        {
            // Если хоккеист владеет шайбой
            if (hasPuck)
            {
                MoveTowardsGoal();

                // Если хоккеист достиг ворот, сбрасываем владение шайбой
                if (targetGoal.Contains((int)Position.X, (int)Position.Y))
                {
                    hasPuck = false;
                    puck.Possessor = null;
                }
            }
            else
            {
                // Движение к шайбе или игроку с шайбой
                MoveTowardsPuck(puck);
            }

            // Избегаем столкновений с другими хоккеистами
            AvoidCollisionWithAthletes(athletes);

            // Ограничиваем движение по полю
            RestrictWithinBounds(clientSize);
        }

        private void MoveTowardsPuck(Puck puck)
        {
            if (puck.Possessor == null)
            {
                // Движение к шайбе
                float dx = puck.Position.X - Position.X;
                float dy = puck.Position.Y - Position.Y;
                float distance = (float)Math.Sqrt(dx * dx + dy * dy);

                if (distance < Radius + puck.Radius)
                {
                    hasPuck = true;
                    puck.Possessor = this;
                }

                if (distance > 0.1)
                {
                    dx /= distance;
                    dy /= distance;
                }

                dx *= SpeedMultiplier;
                dy *= SpeedMultiplier;

                Position = new PointF(Position.X + dx, Position.Y + dy);
            }
            else
            {
                if (puck.Possessor.Color != this.Color)
                {
                    // Преследуем игрока противника с шайбой
                    float dx = puck.Possessor.Position.X - Position.X;
                    float dy = puck.Possessor.Position.Y - Position.Y;
                    float distance = (float)Math.Sqrt(dx * dx + dy * dy);

                    if (distance > 0.1)
                    {
                        dx /= distance;
                        dy /= distance;
                    }

                    dx *= SpeedMultiplier;
                    dy *= SpeedMultiplier;

                    Position = new PointF(Position.X + dx, Position.Y + dy);
                }
                else
                {
                    // Товарищ по команде владеет шайбой
                    // Можно реализовать логику поддержки или занять позицию для паса
                    MoveTowardsSupportPosition();
                }
            }
        }

        private void MoveTowardsGoal()
        {
            // Движение к центру ворот противника
            float goalCenterX = targetGoal.X + targetGoal.Width / 2;
            float goalCenterY = targetGoal.Y + targetGoal.Height / 2;

            float dx = goalCenterX - Position.X;
            float dy = goalCenterY - Position.Y;
            float distance = (float)Math.Sqrt(dx * dx + dy * dy);

            if (distance > 0.1)
            {
                dx /= distance;
                dy /= distance;
            }

            dx *= SpeedMultiplier;
            dy *= SpeedMultiplier;

            // Ограничение скорости
            if (Math.Abs(dx) > MaxSpeed) dx = MaxSpeed * Math.Sign(dx);
            if (Math.Abs(dy) > MaxSpeed) dy = MaxSpeed * Math.Sign(dy);

            Position = new PointF(Position.X + dx, Position.Y + dy);
        }

        private void MoveTowardsSupportPosition()
        {
            // Простая логика для движения к поддерживающей позиции
            // Например, игрок движется к определенной точке на поле

            // Определяем позицию немного сбоку от ворот противника
            float supportX = targetGoal.X + (Color == Color.Red ? -50 : 50);
            float supportY = targetGoal.Y + targetGoal.Height / 2;

            float dx = supportX - Position.X;
            float dy = supportY - Position.Y;
            float distance = (float)Math.Sqrt(dx * dx + dy * dy);

            if (distance > 0.1)
            {
                dx /= distance;
                dy /= distance;
            }

            dx *= SpeedMultiplier / 2; // Движемся медленнее, когда поддерживаем
            dy *= SpeedMultiplier / 2;

            Position = new PointF(Position.X + dx, Position.Y + dy);
        }

        private void AvoidCollisionWithAthletes(Athlete[] athletes)
        {
            foreach (var athlete in athletes)
            {
                if (athlete != this)
                {
                    float dx = Position.X - athlete.Position.X;
                    float dy = Position.Y - athlete.Position.Y;
                    float distance = (float)Math.Sqrt(dx * dx + dy * dy);
                    if (distance < MinDistance && distance > 0)
                    {
                        dx /= distance;
                        dy /= distance;
                        Position = new PointF(Position.X + dx, Position.Y + dy);
                    }
                }
            }
        }

        private void RestrictWithinBounds(Size clientSize)
        {
            if (Position.X - Radius < 0) Position = new PointF(Radius, Position.Y);
            if (Position.X + Radius > clientSize.Width) Position = new PointF(clientSize.Width - Radius, Position.Y);
            if (Position.Y - Radius < 0) Position = new PointF(Position.X, Radius);
            if (Position.Y + Radius > clientSize.Height) Position = new PointF(Position.X, clientSize.Height - Radius);
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
