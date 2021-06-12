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
        string name;
        Func<int> value;
        Bitmap bmp;
        Graphics gfx;

        private Counter(int x, int y, string name, Func<int> value) : base(Sprite.Sprites["text"], x: x, y: y, Program.ScreenWidth, 20)
        {
            this.name = name;
            this.value = value;
            bmp = BitmapExtensions.CreateBitmap(Program.ScreenWidth, Program.ScreenHeight);
            gfx = Graphics.FromImage(bmp);
        }

        public void Tick(Location loc, Entity entity)
        {
        }

        ////public new Image Image()
        ////{
        ////    return bmp;
        ////}

        private Bitmap Draw()
        {
            string text = $"{name}: {value()}";

            gfx.Clear(Color.Transparent);
            gfx.DrawString(text, new Font("Arial", 10), Brushes.Black, 0, 0);

            return bmp;
        }

        public static GEntity<Counter> Create(int x, int y, string name, Func<int> value)
        {
            Counter counter = new Counter(x, y, name, value);
            counter.DrawAction += counter.Draw;
            GEntity<Counter> entity = new GEntity<Counter>(counter);
            entity.TickAction += counter.Tick;
            return entity;
        }
    }
}
