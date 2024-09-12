using System;
using System.Drawing;

namespace _1lab
{
    public class Ant
    {
        public PointF position;
        public PointF velocity;
        public FSM brain;

        private const float MOUSE_THREAT_RADIUS = 50;
        private const float SPEED = 1.0f; // Уменьшенная базовая скорость муравья

        public Ant(float posX, float posY)
        {
            position = new PointF(posX, posY);
            velocity = new PointF(0, 0);
            brain = new FSM();
            brain.SetState(FindLeaf);
        }

        // Метод нахождения листа
        public void FindLeaf()
        {
            SetVelocityTowards(Form1.Instance.Leaf.Position);

            // Если муравей достиг листа
            if (Distance(Form1.Instance.Leaf.Position, position) <= 10)
            {
                // Перемещаем лист в новое случайное место
                Form1.Instance.Leaf.SetRandomPosition();
                brain.SetState(GoHome);
            }

            // Проверяем, если мышь в радиусе угрозы
            if (Distance(Form1.Instance.Mouse.Position, position) <= MOUSE_THREAT_RADIUS)
            {
                brain.SetState(RunAway);
            }
        }

        // Метод возвращения домой
        public void GoHome()
        {
            SetVelocityTowards(Form1.Instance.Home.Position);

            // Если муравей достиг дома
            if (Distance(Form1.Instance.Home.Position, position) <= 10)
            {
                brain.SetState(FindLeaf);
            }

            // Проверяем, если мышь в радиусе угрозы
            if (Distance(Form1.Instance.Mouse.Position, position) <= MOUSE_THREAT_RADIUS)
            {
                brain.SetState(RunAway);
            }
        }

        // Метод убегания от мыши
        public void RunAway()
        {
            SetVelocityAwayFrom(Form1.Instance.Mouse.Position);

            // Если мышь больше не в зоне угрозы
            if (Distance(Form1.Instance.Mouse.Position, position) > MOUSE_THREAT_RADIUS)
            {
                // Вернёмся к поиску листа или движению домой, в зависимости от предыдущего состояния
                brain.SetState(FindLeaf);
            }
        }

        // Метод обновления состояния муравья
        public void Update()
        {
            brain.Update();
            MoveBasedOnVelocity();
        }

        // Движение муравья с учётом скорости
        private void MoveBasedOnVelocity()
        {
            position = new PointF(position.X + velocity.X * Form1.Instance.SpeedMultiplier,
                                  position.Y + velocity.Y * Form1.Instance.SpeedMultiplier);
        }

        // Нормализация вектора скорости, чтобы муравей двигался с постоянной скоростью
        private void SetVelocityTowards(PointF target)
        {
            velocity = new PointF(target.X - position.X, target.Y - position.Y);
            NormalizeVelocity();
        }

        // Убегание от мыши
        private void SetVelocityAwayFrom(PointF target)
        {
            velocity = new PointF(position.X - target.X, position.Y - target.Y);
            NormalizeVelocity();
        }

        // Нормализация вектора скорости
        private void NormalizeVelocity()
        {
            float length = Distance(new PointF(0, 0), velocity);
            if (length > 0)
            {
                velocity = new PointF(velocity.X / length * SPEED, velocity.Y / length * SPEED);
            }
        }

        // Вычисление расстояния между двумя точками
        private float Distance(PointF a, PointF b)
        {
            return (float)Math.Sqrt((a.X - b.X) * (a.X - b.X) + (a.Y - b.Y) * (a.Y - b.Y));
        }
    }
}
