using System;
using System.Drawing;

namespace _1lab
{
    public class Ant
    {
        public PointF position;  // Текущая позиция муравья
        public PointF velocity;  // Скорость муравья (вектор направления движения)
        public FSM brain;        // Состояние муравья (автомат состояний)

        private const float MOUSE_THREAT_RADIUS = 100;  // Радиус, в котором муравей боится мыши
        private const float SPEED = 3.0f;  // Базовая скорость муравья

        public Ant(float posX, float posY)
        {
            position = new PointF(posX, posY);  // Инициализируем позицию муравья
            velocity = new PointF(0, 0);  // Начальная скорость 0
            brain = new FSM();  // Создаем автомат состояний для муравья
            brain.SetState(FindLeaf);  // Начальное состояние - поиск листа
        }

        // Состояние: Поиск листа
        public void FindLeaf()
        {
            SetVelocityTowards(Form1.Instance.Leaf.Position);  // Устанавливаем скорость в сторону листа

            // Если муравей добрался до листа
            if (Distance(Form1.Instance.Leaf.Position, position) <= 10)
            {
                Form1.Instance.Leaf.SetRandomPosition();  // Лист перемещается в случайное место
                brain.SetState(GoHome);  // Переход в состояние "Идти домой"
            }

            // Если мышь находится слишком близко
            if (Distance(Form1.Instance.Mouse.Position, position) <= MOUSE_THREAT_RADIUS)
            {
                brain.SetState(RunAway);  // Переход в состояние "Убегать"
            }
        }

        // Состояние: Идти домой
        public void GoHome()
        {
            SetVelocityTowards(Form1.Instance.Home.Position);  // Устанавливаем скорость в сторону домика

            // Если муравей достиг домика
            if (Distance(Form1.Instance.Home.Position, position) <= 10)
            {
                brain.SetState(FindLeaf);  // Переход в состояние "Поиск листа"
            }

            // Если мышь находится слишком близко
            if (Distance(Form1.Instance.Mouse.Position, position) <= MOUSE_THREAT_RADIUS)
            {
                brain.SetState(RunAway);  // Переход в состояние "Убегать"
            }
        }

        // Состояние: Убегать от мыши
        public void RunAway()
        {
            SetVelocityAwayFrom(Form1.Instance.Mouse.Position);  // Устанавливаем скорость в сторону от мыши

            // Если мышь больше не представляет угрозу
            if (Distance(Form1.Instance.Mouse.Position, position) > MOUSE_THREAT_RADIUS)
            {
                brain.PopState();  // Возвращаемся к предыдущему состоянию
            }
        }

        // Обновление состояния муравья
        public void Update()
        {
            brain.Update();  // Обновляем текущее состояние
            MoveBasedOnVelocity();  // Перемещаем муравья в зависимости от его скорости
        }

        // Движение муравья на основе скорости
        private void MoveBasedOnVelocity()
        {
            position = new PointF(position.X + velocity.X * Form1.Instance.SpeedMultiplier,
                                  position.Y + velocity.Y * Form1.Instance.SpeedMultiplier);
        }

        // Устанавливаем направление движения в сторону цели
        private void SetVelocityTowards(PointF target)
        {
            velocity = new PointF(target.X - position.X, target.Y - position.Y);  // Рассчитываем вектор движения
            NormalizeVelocity();  // Нормализуем скорость
        }

        // Устанавливаем направление движения от мыши
        private void SetVelocityAwayFrom(PointF target)
        {
            velocity = new PointF(position.X - target.X, position.Y - target.Y);  // Рассчитываем вектор движения от угрозы
            NormalizeVelocity();  // Нормализуем скорость
        }

        // Нормализация скорости (приведение длины вектора к базовой скорости)
        private void NormalizeVelocity()
        {
            float length = Distance(new PointF(0, 0), velocity);  // Вычисляем длину вектора скорости
            if (length > 0)
            {
                velocity = new PointF(velocity.X / length * SPEED, velocity.Y / length * SPEED);  // Нормализуем вектор
            }
        }

        // Вычисление расстояния между двумя точками
        private float Distance(PointF a, PointF b)
        {
            return (float)Math.Sqrt((a.X - b.X) * (a.X - b.X) + (a.Y - b.Y) * (a.Y - b.Y));
        }
    }
}
