using GameEngine;
using GameEngine._2D;
using System;
using System.Collections.Generic;
using System.Text;

namespace Game
{
    public class GEntity<T>: Entity where T: Description2D
    {
        private T t;

        public GEntity(T description) : base(description)
        {
            t = description;
        }

        public new T Description => t;

    }
}
