using System;
using System.Drawing;

namespace _1lab
{
    public class Leaf
    {
        public PointF Position { get; private set; }

        public Leaf(PointF position)
        {
            Position = position;
        }

        // Устанавливаем случайную позицию для листа
        public void SetRandomPosition()
        {
            Random random = new Random();
            Position = new PointF(random.Next(50, 400), random.Next(50, 400)); // Случайные координаты в пределах формы
        }
    }
}
