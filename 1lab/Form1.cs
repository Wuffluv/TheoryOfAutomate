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
            timer.Interval = 25; // Каждые 50 мс
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

            // Рисуем листочек
            DrawLeaf(g, Leaf.Position);

            // Рисуем домик
            DrawHome(g, Home.Position);

            // Рисуем муравья (3 части тела и 6 ножек)
            DrawAnt(g, ant.position);
        }

        private void DrawHome(Graphics g, PointF position)
        {
            // Размеры домика
            float houseWidth = 40;
            float houseHeight = 30;

            // Рисуем корпус домика (прямоугольник)
            RectangleF houseBody = new RectangleF(position.X - houseWidth / 2, position.Y - houseHeight / 2, houseWidth, houseHeight);
            g.FillRectangle(Brushes.Brown, houseBody);

            // Рисуем крышу домика (треугольник)
            PointF[] roofPoints =
            {
        new PointF(position.X - houseWidth / 2, position.Y - houseHeight / 2),  // Левый угол крыши
        new PointF(position.X + houseWidth / 2, position.Y - houseHeight / 2),  // Правый угол крыши
        new PointF(position.X, position.Y - houseHeight)  // Верхушка крыши
    };
            g.FillPolygon(Brushes.DarkRed, roofPoints);

            // Рисуем дверь домика
            float doorWidth = 10;
            float doorHeight = 15;
            g.FillRectangle(Brushes.Black, position.X - doorWidth / 2, position.Y, doorWidth, doorHeight);
        }

        private void DrawLeaf(Graphics g, PointF position)
        {
            // Размеры листа
            float leafWidth = 30;
            float leafHeight = 20;

            // Рисуем форму листа (эллипс)
            g.FillEllipse(Brushes.Green, position.X - leafWidth / 2, position.Y - leafHeight / 2, leafWidth, leafHeight);

            // Рисуем центральную жилку листа
            g.DrawLine(Pens.DarkGreen, position.X, position.Y - leafHeight / 2, position.X, position.Y + leafHeight / 2);

            // Рисуем боковые жилки
            g.DrawLine(Pens.DarkGreen, position.X, position.Y, position.X - leafWidth / 4, position.Y - leafHeight / 4);
            g.DrawLine(Pens.DarkGreen, position.X, position.Y, position.X + leafWidth / 4, position.Y - leafHeight / 4);
            g.DrawLine(Pens.DarkGreen, position.X, position.Y, position.X - leafWidth / 4, position.Y + leafHeight / 4);
            g.DrawLine(Pens.DarkGreen, position.X, position.Y, position.X + leafWidth / 4, position.Y + leafHeight / 4);
        }

        private void DrawAnt(Graphics g, PointF position)
        {
            // Размеры муравья
            float bodyWidth = 20;
            float bodyHeight = 10;

            // Определяем три части тела муравья
            PointF head = new PointF(position.X, position.Y);
            PointF thorax = new PointF(position.X + bodyWidth / 2, position.Y);
            PointF abdomen = new PointF(position.X + bodyWidth, position.Y);

            // Рисуем тело (голова, грудь и брюшко)
            g.FillEllipse(Brushes.Black, head.X - 5, head.Y - 5, 10, 10); // Голова
            g.FillEllipse(Brushes.Black, thorax.X - 5, thorax.Y - 5, 12, 12); // Грудь
            g.FillEllipse(Brushes.Black, abdomen.X - 5, abdomen.Y - 5, 14, 14); // Брюшко

            // Рисуем усики
            g.DrawLine(Pens.Black, head.X - 5, head.Y - 5, head.X - 15, head.Y - 10);
            g.DrawLine(Pens.Black, head.X - 5, head.Y - 5, head.X - 15, head.Y + 10);

            // Рисуем ножки (по 3 ножки с каждой стороны)
            DrawLegs(g, thorax);
        }

        private void DrawLegs(Graphics g, PointF thorax)
        {
            // Длина ножек
            float legLength = 10;

            // Рисуем по 3 ножки с каждой стороны
            g.DrawLine(Pens.Black, thorax.X - 5, thorax.Y, thorax.X - 15, thorax.Y - legLength); // Верхняя левая
            g.DrawLine(Pens.Black, thorax.X - 5, thorax.Y, thorax.X - 15, thorax.Y + legLength); // Нижняя левая
            g.DrawLine(Pens.Black, thorax.X - 5, thorax.Y, thorax.X - 15, thorax.Y);             // Средняя левая

            g.DrawLine(Pens.Black, thorax.X + 5, thorax.Y, thorax.X + 15, thorax.Y - legLength); // Верхняя правая
            g.DrawLine(Pens.Black, thorax.X + 5, thorax.Y, thorax.X + 15, thorax.Y + legLength); // Нижняя правая
            g.DrawLine(Pens.Black, thorax.X + 5, thorax.Y, thorax.X + 15, thorax.Y);             // Средняя правая
        }
    }
}
