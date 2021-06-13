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
        private BarColor color;
        private Graphics gfx;
        private int barHeight;

        public UIBar(int x, int y, int width, int height, BarColor color, bool vertical, Func<float> percentage, Func<string> text)
            : base(Sprite.Sprites["text"], x, y, width, height + (text != null ? 10 : 0))
        {
            this.barHeight = height;
            this.color = color;
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

        public Bitmap Draw()
        {
            gfx.Clear(Color.Transparent);
            if (vertical)
            {
                //gfx.FillRectangle(brush, new Rectangle(0, this.Height - (int)(this.barHeight * this.percentage()), this.Width, (int)(this.barHeight * this.percentage())));
                gfx.DrawImage(Sprite.Sprites["plankV"].Images[0], new Rectangle(0, 0, Width, Height), new Rectangle(0, 0, 8, 96), GraphicsUnit.Pixel);
                gfx.DrawImage(Sprite.Sprites["barsV"].Images[(int)color], new Rectangle(1, 2 + (int)((Height - 4) * (1 - this.percentage())), Width - 2, (int)((Height - 4) * (this.percentage()))), new Rectangle(0, 0, 6, 92), GraphicsUnit.Pixel);
            }
            else
            {
                gfx.DrawImage(Sprite.Sprites["plankH"].Images[0], new Rectangle(0, 0, Width, Height - (text != null ? 10 : 0)), new Rectangle(0, 0, 96, 8), GraphicsUnit.Pixel);
                gfx.DrawImage(Sprite.Sprites["barsH"].Images[(int)color], new Rectangle(2, 1, (int)((Width - 4) * this.percentage()), Height - 2 - (text != null ? 10 : 0)), new Rectangle(0, 0, 92, 6), GraphicsUnit.Pixel);
                //gfx.FillRectangle(brush, new Rectangle(0, 0, (int)(this.Width * this.percentage()), this.barHeight));
            }

            gfx.DrawRectangle(Pens.Black, new Rectangle(0, 0, this.Width, this.barHeight));
            if (text != null)
            {
                var sp = Sprite.Sprites["plank"];
                gfx.DrawImage(sp.GetImage(0), new Rectangle(0, this.barHeight, 80, 10), new Rectangle(0, 0, sp.Width, sp.Height), GraphicsUnit.Pixel);
                gfx.DrawString(text?.Invoke() ?? "", new Font("Arial", 6), Brushes.White, 0, this.barHeight - 1);
            }

            return bmp;
        }

        public static GEntity<UIBar> Create(int x, int y, int width, int height, BarColor color, bool vertical, Func<float> percentage, Func<string> text = null)
        {
            UIBar fighter = new UIBar(x, y, width, height, color, vertical, percentage, text);
            fighter.DrawAction = fighter.Draw;
            GEntity<UIBar> entity = new GEntity<UIBar>(fighter);
            entity.TickAction += fighter.Tick;
            return entity;
        }
    }

    enum BarColor { Red = 3, Blue = 1, Cyan = 2, Yellow = 0 }
}
