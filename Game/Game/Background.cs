using GameEngine._2D;
using System;
using System.Collections.Generic;
using System.Text;

namespace Game
{
    class Background : Description2D
    {
        private Background(int x, int y, Sprite sprite) : base(sprite, x: x, y: y, sprite.Width, sprite.Height)
        {
        }

        public static GEntity<Background> Create(Sprite sprite, double x, double y)
        {
            Background ani = new Background((int)x, (int)y, sprite);
            GEntity<Background> entity = new GEntity<Background>(ani);
            return entity;
        }
    }
}
