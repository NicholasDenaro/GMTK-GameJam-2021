using GameEngine;
using GameEngine._2D;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Game
{
    class UIBar : Description2D
    {
        private Bitmap bmp;
        private bool vertical;
        private Func<float> percentage;
        private Brush brush;
        private Graphics gfx;

        public UIBar(int x, int y, int width, int height, Brush brush, bool vertical, Func<float> percentage) : base(Sprite.Sprites["text"], x, y, width, height)
        {
            this.brush = brush;
            this.vertical = vertical;
            this.percentage = percentage;
            bmp = BitmapExtensions.CreateBitmap(width, height);
            gfx = Graphics.FromImage(bmp);
        }

        private void Tick(Location location, Entity entity)
        {

        }

        private Bitmap Draw()
        {
            gfx.Clear(Color.Transparent);
            if (vertical)
            {
                gfx.FillRectangle(brush, new Rectangle(0, this.Height - (int)(this.Height * this.percentage()), this.Width, (int)(this.Height * this.percentage())));
            }
            else
            {
                gfx.FillRectangle(brush, new Rectangle(0, 0, (int)(this.Width * this.percentage()), this.Height));
            }

            gfx.DrawRectangle(Pens.Black, new Rectangle(0, 0, this.Width, this.Height));

            return bmp;
        }

        public static GEntity<UIBar> Create(int x, int y, int width, int height, Brush brush, bool vertical, Func<float> percentage)
        {
            UIBar fighter = new UIBar(x, y, width, height, brush, vertical, percentage);
            fighter.DrawAction = fighter.Draw;
            GEntity<UIBar> entity = new GEntity<UIBar>(fighter);
            entity.TickAction += fighter.Tick;
            return entity;
        }
    }
}
