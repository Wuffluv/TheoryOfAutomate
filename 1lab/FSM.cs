using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _1lab
{
    public class FSM
    {
        private Action activeState; // указатель на активное состояние автомата

        public FSM()
        {

        }

        public void SetState(Action state)
        {
            activeState = state;
        }

        public void Update()
        {
            activeState?.Invoke();
        }
    }
}
