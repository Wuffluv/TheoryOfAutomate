using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace HockeyGame
{
    // Главная форма приложения
    public partial class Form1 : Form
    {
        private Timer gameTimer; // Таймер для обновления игры
        private Puck puck; // Объект шайбы
        private Athlete[] athletes; // Массив хоккеистов
        private int redScore = 0; // Счет команды красных
        private int blueScore = 0; // Счет команды синих
        private Rectangle leftGoal, rightGoal; // Прямоугольники, представляющие ворота
        private Random random = new Random(); // Генератор случайных чисел

        public Form1()
        {
            InitializeComponent();
            InitializeGame(); // Инициализация игры
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            // Этот метод может использоваться для дополнительной отрисовки, если потребуется
        }

        private void InitializeGame()
        {
            // Включаем двойную буферизацию для предотвращения мерцания
            this.DoubleBuffered = true;

            // Задаем размеры и положение ворот
            leftGoal = new Rectangle(20, this.ClientSize.Height / 2 - 50, 10, 100); // Левые ворота
            rightGoal = new Rectangle(this.ClientSize.Width - 30, this.ClientSize.Height / 2 - 50, 10, 100); // Правые ворота

            // Создаем шайбу и хоккеистов
            puck = new Puck(this.ClientSize.Width / 2, this.ClientSize.Height / 2, random); // Создаем шайбу в центре поля
            athletes = new Athlete[]
            {
                new Athlete(100, 100, Color.Red, rightGoal), // Хоккеист красной команды
                new Athlete(this.ClientSize.Width - 100, 100, Color.Blue, leftGoal), // Хоккеист синей команды
                new Athlete(100, this.ClientSize.Height - 100, Color.Red, rightGoal), // Еще один хоккеист красной команды
                new Athlete(this.ClientSize.Width - 100, this.ClientSize.Height - 100, Color.Blue, leftGoal) // Еще один хоккеист синей команды
            };

            // Инициализируем таймер игры
            gameTimer = new Timer();
            gameTimer.Interval = 10; // Интервал обновления (примерно 100 кадров в секунду)
            gameTimer.Tick += GameTimer_Tick; // Привязываем обработчик события
            gameTimer.Start(); // Запускаем таймер
        }

        private void GameTimer_Tick(object sender, EventArgs e)
        {
            // Обновляем позиции хоккеистов и шайбы
            foreach (var athlete in athletes)
            {
                athlete.Update(puck, athletes, this.ClientSize);
            }
            puck.Update(this.ClientSize);

            // Проверяем, был ли забит гол
            CheckGoal();

            // Перерисовываем поле
            this.Invalidate();
        }

        private void CheckGoal()
        {
            // Создаем прямоугольник, представляющий шайбу для проверки столкновений
            RectangleF puckRect = new RectangleF(
                puck.Position.X - puck.Radius,
                puck.Position.Y - puck.Radius,
                puck.Radius * 2,
                puck.Radius * 2);

            // Проверяем пересечение шайбы с левыми воротами (гол для синих)
            if (puckRect.IntersectsWith(leftGoal))
            {
                blueScore++; // Увеличиваем счет синих
                ResetGame(); // Сбрасываем игру
            }
            // Проверяем пересечение шайбы с правыми воротами (гол для красных)
            else if (puckRect.IntersectsWith(rightGoal))
            {
                redScore++; // Увеличиваем счет красных
                ResetGame(); // Сбрасываем игру
            }
        }

        private void ResetGame()
        {
            // Возвращаем шайбу на центр и сбрасываем ее состояние
            puck = new Puck(this.ClientSize.Width / 2, this.ClientSize.Height / 2, random);
            puck.Possessor = null;

            // Сбрасываем состояние всех хоккеистов
            foreach (var athlete in athletes)
            {
                athlete.Reset();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Закрываем форму при нажатии на кнопку
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            ResetGame();
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

            // Отображаем текущий счет
            using (Font font = new Font("Arial", 16))
            {
                e.Graphics.DrawString($"Red: {redScore}  Blue: {blueScore}", font, Brushes.Black, this.ClientSize.Width / 2 - 50, 20);
            }
        }
    }

    // Класс, представляющий шайбу
    public class Puck
    {
        public PointF Position { get; set; } // Позиция шайбы
        public float Radius { get; private set; } = 10; // Радиус шайбы
        public PointF Velocity { get; private set; } // Скорость шайбы
        private Random random; // Генератор случайных чисел для направления
        public Athlete Possessor { get; set; } // Хоккеист, владеющий шайбой

        public Puck(float x, float y, Random random)
        {
            Position = new PointF(x, y); // Устанавливаем начальную позицию
            this.random = random;
            SetRandomDirection(); // Устанавливаем случайное направление
            Possessor = null; // Шайба никому не принадлежит
        }

        public void SetRandomDirection()
        {
            // Генерируем случайное направление для шайбы
            float angle = (float)(random.NextDouble() * 2 * Math.PI);
            float speed = 4;  // Устанавливаем скорость шайбы
            Velocity = new PointF((float)Math.Cos(angle) * speed, (float)Math.Sin(angle) * speed); // Вычисляем компоненты скорости
        }

        public void Update(Size clientSize)
        {
            // Если шайбой никто не владеет
            if (Possessor == null)
            {
                // Обновляем позицию шайбы
                Position = new PointF(Position.X + Velocity.X, Position.Y + Velocity.Y);

                // Проверяем столкновения с краями окна и меняем направление при необходимости
                if (Position.X - Radius < 0 || Position.X + Radius > clientSize.Width)
                    Velocity = new PointF(-Velocity.X, Velocity.Y);
                if (Position.Y - Radius < 0 || Position.Y + Radius > clientSize.Height)
                    Velocity = new PointF(Velocity.X, -Velocity.Y);
            }
            else
            {
                // Если шайба у хоккеиста, следует за ним
                Position = Possessor.Position;
            }
        }

        public void Draw(Graphics g)
        {
            // Рисуем шайбу
            g.FillEllipse(Brushes.Black, Position.X - Radius, Position.Y - Radius, Radius * 2, Radius * 2);
        }
    }

    // Определяем возможные действия хоккеиста
    public enum AthleteActionType
    {
        MoveToPosition, // Двигаться к определенной позиции
        ChasePuck,      // Преследовать шайбу
        MoveToGoal,     // Двигаться к воротам
        Support,        // Поддерживать товарища по команде
        Idle            // Без действия
    }

    // Класс действия хоккеиста
    public class AthleteAction
    {
        public AthleteActionType ActionType { get; set; } // Тип действия
        public PointF TargetPosition { get; set; } // Целевая позиция, если необходимо

        public AthleteAction(AthleteActionType actionType, PointF targetPosition = default(PointF))
        {
            ActionType = actionType;
            TargetPosition = targetPosition;
        }
    }

    // Класс, представляющий хоккеиста
    public class Athlete
    {
        public PointF Position { get; private set; } // Позиция хоккеиста
        public float Radius { get; private set; } = 15; // Радиус хоккеиста
        public Color Color { get; private set; } // Цвет команды хоккеиста
        private Rectangle targetGoal; // Цель — ворота противника
        private bool hasPuck = false; // Владеет ли хоккеист шайбой
        private const float MaxSpeed = 6f; // Максимальная скорость
        private const float MinDistance = 20f; // Минимальное расстояние для избежания столкновений
        private const float SpeedMultiplier = 2.5f; // Множитель скорости

        private PointF initialPosition; // Начальная позиция хоккеиста

        // Стек действий хоккеиста
        private Stack<AthleteAction> actionStack = new Stack<AthleteAction>();

        public Athlete(float x, float y, Color color, Rectangle goal)
        {
            Position = new PointF(x, y); // Устанавливаем начальную позицию
            initialPosition = Position;
            Color = color; // Устанавливаем цвет команды
            targetGoal = goal; // Устанавливаем цель (ворота противника)

            // Изначально хоккеист преследует шайбу
            actionStack.Push(new AthleteAction(AthleteActionType.ChasePuck));
        }

        public void Reset()
        {
            // Сбрасываем состояние хоккеиста
            hasPuck = false;
            Position = initialPosition;
            actionStack.Clear();
            actionStack.Push(new AthleteAction(AthleteActionType.ChasePuck)); // Начинаем преследовать шайбу
        }

        public void Update(Puck puck, Athlete[] athletes, Size clientSize)
        {
            // Обработка текущего действия из стека
            if (actionStack.Count > 0)
            {
                AthleteAction currentAction = actionStack.Peek(); // Получаем текущее действие
                switch (currentAction.ActionType)
                {
                    case AthleteActionType.ChasePuck:
                        PerformChasePuck(puck); // Преследуем шайбу
                        break;
                    case AthleteActionType.MoveToGoal:
                        PerformMoveToGoal(); // Двигаемся к воротам
                        break;
                    case AthleteActionType.Support:
                        PerformSupport(); // Поддерживаем товарища
                        break;
                    case AthleteActionType.MoveToPosition:
                        PerformMoveToPosition(currentAction.TargetPosition); // Двигаемся к определенной позиции
                        break;
                    case AthleteActionType.Idle:
                        // Ничего не делаем
                        break;
                }
            }
            else
            {
                // Если действий нет, начинаем преследовать шайбу
                actionStack.Push(new AthleteAction(AthleteActionType.ChasePuck));
            }

            // Избегаем столкновений с другими хоккеистами
            AvoidCollisionWithAthletes(athletes);

            // Ограничиваем движение в пределах окна
            RestrictWithinBounds(clientSize);
        }

        private void PerformChasePuck(Puck puck)
        {
            if (hasPuck)
            {
                // Если владеем шайбой, начинаем двигаться к воротам
                actionStack.Pop();
                actionStack.Push(new AthleteAction(AthleteActionType.MoveToGoal));
                return;
            }

            if (puck.Possessor == null)
            {
                // Двигаемся к шайбе
                MoveTowards(puck.Position);

                // Проверяем, можем ли завладеть шайбой
                float distance = GetDistance(Position, puck.Position);
                if (distance < Radius + puck.Radius)
                {
                    hasPuck = true; // Теперь владеем шайбой
                    puck.Possessor = this;
                }
            }
            else
            {
                if (puck.Possessor.Color != this.Color)
                {
                    // Преследуем противника с шайбой
                    MoveTowards(puck.Possessor.Position);
                }
                else
                {
                    // Наш товарищ владеет шайбой, переходим в поддержку
                    actionStack.Pop();
                    actionStack.Push(new AthleteAction(AthleteActionType.Support));
                }
            }
        }

        private void PerformMoveToGoal()
        {
            if (!hasPuck)
            {
                // Если потеряли шайбу, возвращаемся к преследованию
                actionStack.Pop();
                actionStack.Push(new AthleteAction(AthleteActionType.ChasePuck));
                return;
            }

            // Двигаемся к центру ворот противника
            float goalCenterX = targetGoal.X + targetGoal.Width / 2;
            float goalCenterY = targetGoal.Y + targetGoal.Height / 2;
            PointF goalCenter = new PointF(goalCenterX, goalCenterY);

            MoveTowards(goalCenter);

            // Проверяем, достигли ли ворот
            if (targetGoal.Contains((int)Position.X, (int)Position.Y))
            {
                hasPuck = false; // Больше не владеем шайбой
                actionStack.Pop();
                actionStack.Push(new AthleteAction(AthleteActionType.ChasePuck)); // Возвращаемся к преследованию шайбы
            }
        }

        private void PerformSupport()
        {
            // Двигаемся в позицию поддержки
            float supportX = targetGoal.X + (Color == Color.Red ? -100 : 100);
            float supportY = targetGoal.Y + targetGoal.Height / 2;
            PointF supportPosition = new PointF(supportX, supportY);

            MoveTowards(supportPosition);

            // Здесь можно добавить дополнительную логику, например, пас
        }

        private void PerformMoveToPosition(PointF targetPosition)
        {
            // Двигаемся к заданной позиции
            MoveTowards(targetPosition);

            // Проверяем, достигли ли позиции
            float distance = GetDistance(Position, targetPosition);
            if (distance < 5f)
            {
                actionStack.Pop(); // Удаляем действие из стека
            }
        }

        private void MoveTowards(PointF target)
        {
            // Вычисляем направление движения
            float dx = target.X - Position.X;
            float dy = target.Y - Position.Y;
            float distance = (float)Math.Sqrt(dx * dx + dy * dy);

            if (distance > 0.1)
            {
                dx /= distance; // Нормализуем вектор
                dy /= distance;
            }

            dx *= SpeedMultiplier; // Применяем множитель скорости
            dy *= SpeedMultiplier;

            // Ограничиваем скорость
            if (Math.Abs(dx) > MaxSpeed) dx = MaxSpeed * Math.Sign(dx);
            if (Math.Abs(dy) > MaxSpeed) dy = MaxSpeed * Math.Sign(dy);

            // Обновляем позицию
            Position = new PointF(Position.X + dx, Position.Y + dy);
        }

        private void AvoidCollisionWithAthletes(Athlete[] athletes)
        {
            // Избегаем столкновений с другими хоккеистами
            foreach (var athlete in athletes)
            {
                if (athlete != this)
                {
                    float dx = Position.X - athlete.Position.X;
                    float dy = Position.Y - athlete.Position.Y;
                    float distance = (float)Math.Sqrt(dx * dx + dy * dy);
                    if (distance < MinDistance && distance > 0)
                    {
                        dx /= distance; // Нормализуем вектор
                        dy /= distance;
                        Position = new PointF(Position.X + dx, Position.Y + dy); // Отталкиваемся
                    }
                }
            }
        }

        private void RestrictWithinBounds(Size clientSize)
        {
            // Ограничиваем движение хоккеиста в пределах окна
            if (Position.X - Radius < 0) Position = new PointF(Radius, Position.Y);
            if (Position.X + Radius > clientSize.Width) Position = new PointF(clientSize.Width - Radius, Position.Y);
            if (Position.Y - Radius < 0) Position = new PointF(Position.X, Radius);
            if (Position.Y + Radius > clientSize.Height) Position = new PointF(Position.X, clientSize.Height - Radius);
        }

        private float GetDistance(PointF p1, PointF p2)
        {
            // Вычисляем расстояние между двумя точками
            float dx = p1.X - p2.X;
            float dy = p1.Y - p2.Y;
            return (float)Math.Sqrt(dx * dx + dy * dy);
        }

        public void Draw(Graphics g)
        {
            // Рисуем хоккеиста
            using (Brush brush = new SolidBrush(Color))
            {
                g.FillEllipse(brush, Position.X - Radius, Position.Y - Radius, Radius * 2, Radius * 2);
            }
        }
    }
}
