using System;
using System.Collections.Generic;

namespace _1lab
{
    public class FSM
    {
        private Stack<Action> stateStack; // стек состояний

        public FSM()
        {
            stateStack = new Stack<Action>();
        }

        // Устанавливаем текущее состояние
        public void SetState(Action state)
        {
            stateStack.Push(state); // сохраняем текущее состояние в стек
        }

        // Возвращаемся к предыдущему состоянию
        public void PopState()
        {
            if (stateStack.Count > 0)
            {
                stateStack.Pop(); // удаляем текущее состояние
            }
        }

        // Выполняем текущее состояние
        public void Update()
        {
            if (stateStack.Count > 0)
            {
                stateStack.Peek()?.Invoke();
            }
        }
    }
}
