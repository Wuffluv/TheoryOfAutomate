using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;


namespace _1lab
{
    public class Ant
    {
       public Vector2 position;
       public Vector2 velocity;
       public FSM brain;

        public Ant(float posX, float posY)
        {
            position = new Vector2(posX, posY);
            velocity = new Vector2(-1, -1);
            brain = new FSM();
            // Начинаем с поиска листка.
            brain.SetState(FindLeaf);
        }
        /// <summary>
        /// Состояние "findLeaf".
        /// Заставляет муравья искать листья.
        /// </summary>
        public void FindLeaf()
        {
            // Перемещает муравья к листу.
            Vector2 velocity = new Vector3(Form1.Instance.Leaf.x - position.x,
                Form1.Instance.Leaf.y - position.y);

            if (Vector3.Distance(Form1.Instance.Leaf.position, this.position) <= 10)
            {
                // Муравей только что подобрал листок, время
                // возвращаться домой!
                brain.SetState(GoHome);
            }

            if (Vector3.Distance(Form1.Mouse.position, this.position) <= MOUSE_THREAT_RADIUS)
            {
                // Курсор мыши находится рядом. Бежим!
                // Меняем состояние автомата на RunAway()
                brain.SetState(RunAway);
            }
        }

        /// <summary>
        /// Состояние "goHome".
        /// Заставляет муравья идти в муравейник.
        /// </summary>
        public void GoHome()
        {
            // Перемещает муравья к дому
            velocity = new Vector3(Form1.Instance.Home.x - position.x,
                Form1.Instance.Home.y - position.y);

            if (Vector3.Distance(Form1.Instance.Home, this.transform.position) <= 10)
            {
                // Муравей уже дома. Пора искать новый лист.
                brain.SetState(FindLeaf);
            }
        }

        /// <summary>
        /// Состояние "runAway".
        /// Заставляет муравья убегать от курсора мыши.
        /// </summary>
        public void RunAway()
        {
            // Перемещает муравья подальше от курсора
            velocity = new Vector3(position.x - Form1.mouse.x, position.y - Form1.mouse.y, 0);

            // Курсор все еще рядом?
            if (Vector3.Distance(Form1.mouse, this.transform.position) > MOUSE_THREAT_RADIUS)
            {
                // Нет, уже далеко. Пора возвращаться к поискам листочков.
                brain.SetState(FindLeaf);
            }
        }

        public void Update()
        {
            // Обновление конечного автомата. Эта функция будет
            // вызывать функцию активного состояния: FindLeaf(), GoHome() или RunAway().
            brain.Update();
            // Применение скорости для движения муравья.
            MoveBasedOnVelocity();
        }

        private void MoveBasedOnVelocity()
        {
            // Implement movement logic based on velocity
        }

    }
}
