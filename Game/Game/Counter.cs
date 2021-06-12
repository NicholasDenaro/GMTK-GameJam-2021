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
        float value;
        Bitmap bmp;
        Graphics gfx;

        private Counter(string name, int baseValue, int pos) : base(Sprite.Sprites["text"], x: 0, y: pos, Program.ScreenWidth, Program.ScreenHeight)
        {
            this.name = name;
            this.value = baseValue;
            bmp = BitmapExtensions.CreateBitmap(Program.ScreenWidth, Program.ScreenHeight);
            gfx = Graphics.FromImage(bmp);
        }

        public void Tick(Location loc, Entity entity)
        {
            ////Increment();
            ////if (Program.Engine.Controllers.First()[(int)Program.Actions.ACTION].IsPress())
            ////{
            ////    Increment(TPS);
            ////    //frame.PlaySound(new AvaloniaSound(new MML("l8cdefgab1").GetChannel(0)));
            ////}
            ////if (Program.Engine.Controllers.Skip(1).First()[(int)Program.Actions.CLICK].IsPress())
            ////{
            ////    Increment(100 * TPS);
            ////}

        }
        public void Increment(int i = 1)
        {
            //value += i / TPS.0f;
        }

        public new Image Image()
        {
            return bmp;
        }

        private Bitmap Draw()
        {
            string text = $"{name}: {(int)value}";

            gfx.Clear(Color.Transparent);
            gfx.DrawString(text, new Font("Arial", 15), Brushes.White, 0, 0);

            return bmp;
        }

        public static Entity Create(string name, int baseValue, int pos)
        {
            Counter counter = new Counter(name, baseValue, pos);
            counter.DrawAction += counter.Draw;
            Entity entity = new Entity(counter);
            entity.TickAction += counter.Tick;
            return entity;
        }
    }
}
