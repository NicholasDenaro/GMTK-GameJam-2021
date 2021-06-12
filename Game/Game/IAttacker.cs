using System;
using System.Collections.Generic;
using System.Text;

namespace Game
{
    interface IAttacker
    {

        int AttackTimer { get; set; }

        public void AdvanceTimer()
        {

        }
    }
}
