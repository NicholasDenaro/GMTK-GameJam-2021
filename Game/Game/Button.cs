using GameEngine;
using GameEngine._2D;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Game
{
    class Button : Description2D
    {
        private Bitmap bitmapUp;
        private Bitmap bitmapDown;
        private bool isDown;

        private Button(int x, int y) : base(Sprite.Sprites["text"], x, y, 64, 48)
        {
            bitmapUp = BitmapExtensions.CreateBitmap(64, 48);
            bitmapDown = BitmapExtensions.CreateBitmap(64, 48);
            Graphics gfx = Graphics.FromImage(bitmapUp);
            gfx.FillEllipse(Brushes.DarkRed, new Rectangle(0, 16, 64, 32));
            gfx.FillRectangle(Brushes.DarkRed, new Rectangle(0, 16, 64, 16));
            gfx.FillEllipse(Brushes.Red, new Rectangle(0, 0, 64, 32));

            gfx = Graphics.FromImage(bitmapDown);
            gfx.FillEllipse(Brushes.DarkRed, new Rectangle(0, 16, 64, 32));
            gfx.FillEllipse(Brushes.Red, new Rectangle(0, 8, 64, 32));
        }

        private void Tick(Location location, Entity entity)
        {
            if (Program.Engine.Controllers.Skip(1).First()[(int)Program.Actions.CLICK].IsPress())
            {
                isDown = true;
            }
            if (Program.Engine.Controllers.Skip(1).First()[(int)Program.Actions.CLICK].State == HoldState.RELEASE)
            {
                isDown = false;
            }
        }

        private Bitmap Draw()
        {
            return isDown ? bitmapDown : bitmapUp;
        }

        public static Entity Create(int x, int y)
        {
            Button button = new Button(x, y);
            button.DrawAction += button.Draw;
            Entity entity = new Entity(button);
            entity.TickAction += button.Tick;
            return entity;
        }
    }
}
