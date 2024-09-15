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
        private const float SPEED = 1.0f;

        public Ant(float posX, float posY)
        {
            position = new PointF(posX, posY);
            velocity = new PointF(0, 0);
            brain = new FSM();
            brain.SetState(FindLeaf);
        }

        public void FindLeaf()
        {
            SetVelocityTowards(Form1.Instance.Leaf.Position);

            if (Distance(Form1.Instance.Leaf.Position, position) <= 10)
            {
                Form1.Instance.Leaf.SetRandomPosition();
                brain.SetState(GoHome);
            }

            if (Distance(Form1.Instance.Mouse.Position, position) <= MOUSE_THREAT_RADIUS)
            {
                brain.SetState(RunAway);
            }
        }

        public void GoHome()
        {
            SetVelocityTowards(Form1.Instance.Home.Position);

            if (Distance(Form1.Instance.Home.Position, position) <= 10)
            {
                brain.SetState(FindLeaf);
            }

            if (Distance(Form1.Instance.Mouse.Position, position) <= MOUSE_THREAT_RADIUS)
            {
                brain.SetState(RunAway);
            }
        }

        public void RunAway()
        {
            SetVelocityAwayFrom(Form1.Instance.Mouse.Position);

            if (Distance(Form1.Instance.Mouse.Position, position) > MOUSE_THREAT_RADIUS)
            {
                brain.PopState(); // Возвращаемся к предыдущему состоянию
            }
        }

        public void Update()
        {
            brain.Update();
            MoveBasedOnVelocity();
        }

        private void MoveBasedOnVelocity()
        {
            position = new PointF(position.X + velocity.X * Form1.Instance.SpeedMultiplier,
                                  position.Y + velocity.Y * Form1.Instance.SpeedMultiplier);
        }

        private void SetVelocityTowards(PointF target)
        {
            velocity = new PointF(target.X - position.X, target.Y - position.Y);
            NormalizeVelocity();
        }

        private void SetVelocityAwayFrom(PointF target)
        {
            velocity = new PointF(position.X - target.X, position.Y - target.Y);
            NormalizeVelocity();
        }

        private void NormalizeVelocity()
        {
            float length = Distance(new PointF(0, 0), velocity);
            if (length > 0)
            {
                velocity = new PointF(velocity.X / length * SPEED, velocity.Y / length * SPEED);
            }
        }

        private float Distance(PointF a, PointF b)
        {
            return (float)Math.Sqrt((a.X - b.X) * (a.X - b.X) + (a.Y - b.Y) * (a.Y - b.Y));
        }
    }
}
