using GameEngine;
using System;
using System.Collections.Generic;
using System.Text;

namespace Game
{
    class CustomAnimation
    {
        private TickHandler handle;

        public CustomAnimation(TickHandler ani)
        {
            handle = ani;
            Program.Engine.TickEnd += ani; //(object sender, GameState state) => { }
        }
    }
}
