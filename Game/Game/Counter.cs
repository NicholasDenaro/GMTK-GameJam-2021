using GameEngine;
using GameEngine._2D;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game
{
    class Counter : Description2D
    {
        Func<string> value;
        Bitmap bmp;
        Graphics gfx;

        private Counter(int x, int y, Func<string> value) : base(Sprite.Sprites["plank"], x: x, y: y, 100, 20)
        {
            this.value = value;
            bmp = BitmapExtensions.CreateBitmap(Program.ScreenWidth, Program.ScreenHeight);
            gfx = Graphics.FromImage(bmp);
            gfx.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;
        }

        private Bitmap Draw()
        {
            gfx.Clear(Color.Transparent);
            gfx.DrawImage(Sprite.GetImage(0), new Rectangle(0, 0, Width, Height), new Rectangle(0, 0, Sprite.Width, Sprite.Height), GraphicsUnit.Pixel);
            gfx.DrawString(value(), new Font("Arial", 10, FontStyle.Bold), Brushes.White, 2, 0);

            return bmp;
        }

        public static GEntity<Counter> Create(int x, int y, Func<string> value)
        {
            Counter counter = new Counter(x, y, value);
            counter.DrawAction += counter.Draw;
            GEntity<Counter> entity = new GEntity<Counter>(counter);
            return entity;
        }
    }
}
