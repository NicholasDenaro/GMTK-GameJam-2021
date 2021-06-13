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
        private Func<string> text;
        private Brush brush;
        private Graphics gfx;
        private int barHeight;

        public UIBar(int x, int y, int width, int height, Brush brush, bool vertical, Func<float> percentage, Func<string> text)
            : base(Sprite.Sprites["text"], x, y, width, height + (text != null ? 10 : 0))
        {
            this.barHeight = height;
            this.brush = brush;
            this.vertical = vertical;
            this.percentage = percentage;
            this.text = text;
            bmp = BitmapExtensions.CreateBitmap(width, height + (text != null ? 10 : 0));
            gfx = Graphics.FromImage(bmp);
            gfx.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;
        }

        private void Tick(Location location, Entity entity)
        {

        }

        private Bitmap Draw()
        {
            gfx.Clear(Color.Transparent);
            if (vertical)
            {
                gfx.FillRectangle(brush, new Rectangle(0, this.Height - (int)(this.barHeight * this.percentage()), this.Width, (int)(this.barHeight * this.percentage())));
            }
            else
            {
                gfx.FillRectangle(brush, new Rectangle(0, 0, (int)(this.Width * this.percentage()), this.barHeight));
            }

            gfx.DrawRectangle(Pens.Black, new Rectangle(0, 0, this.Width, this.barHeight));
            gfx.DrawString(text?.Invoke() ?? "", new Font("Arial", 6), Brushes.White, 0, this.barHeight);

            return bmp;
        }

        public static GEntity<UIBar> Create(int x, int y, int width, int height, Brush brush, bool vertical, Func<float> percentage, Func<string> text = null)
        {
            UIBar fighter = new UIBar(x, y, width, height, brush, vertical, percentage, text);
            fighter.DrawAction = fighter.Draw;
            GEntity<UIBar> entity = new GEntity<UIBar>(fighter);
            entity.TickAction += fighter.Tick;
            return entity;
        }
    }
}
