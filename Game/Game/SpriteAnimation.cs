using GameEngine;
using GameEngine._2D;
using GameEngine.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Game
{
    class SpriteAnimation : Description2D
    {
        private Guid id;
        private int ticks;
        private int[] timings;
        private int index;

        private SpriteAnimation(int x, int y, Sprite sprite, int[] timings) : base(sprite, x: x, y: y, sprite.Width, sprite.Height)
        {
            this.timings = timings;
            ticks = 0;
            index = 0;
            ImageIndex = 0;
        }

        private void Tick(Location location, Entity entity)
        {
            ImageIndex = index;

            ticks++;
            if (ticks >= timings[index])
            {
                ticks = 0;
                index++;
            }

            if (index >= timings.Length)
            {
                Program.RemoveEntity(entity);
            }
        }

        public static GEntity<SpriteAnimation> Create(double x, double y, Sprite sprite, int[] timings)
        {
            SpriteAnimation ani = new SpriteAnimation((int)x, (int)y, sprite, timings);
            GEntity<SpriteAnimation> entity = new GEntity<SpriteAnimation>(ani);
            entity.TickAction += ani.Tick;
            ani.id = entity.Id;
            return entity;
        }
    }
}
