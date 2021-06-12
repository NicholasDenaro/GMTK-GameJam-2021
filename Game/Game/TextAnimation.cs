using GameEngine;
using GameEngine._2D;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Game
{
    class TextAnimation : Description2D
    {
        string text;
        Color color;
        Bitmap bmp;
        Graphics gfx;
        int time;
        Guid id;
        Font font;

        private TextAnimation(int x, int y, string text, Color color, int size) : base(Sprite.Sprites["text"], x: x, y: y, 100, 50)
        {
            this.text = text;
            this.color = color;
            this.font = new Font("Arial", size);
            bmp = BitmapExtensions.CreateBitmap(100, 50);
            bmp.MakeTransparent();
            gfx = Graphics.FromImage(bmp);
            time = Program.TPS;
        }

        private void Tick(Location location, Entity entity)
        {
            this.ChangeCoordsDelta((time % 4 == 0) ? 1 : 0, -1);
            if (--time <= 0)
            {
                location.RemoveEntity(id);
            }
        }

        private Bitmap Draw()
        {
            gfx.Clear(Color.Transparent);
            using Brush brush = new SolidBrush(Color.FromArgb((int)(time * 255.0 / Program.TPS), color));
            gfx.DrawString(text, font, brush, 0, 0);

            return bmp;
        }

        public static GEntity<TextAnimation> Create(double x, double y, string text, Color color, int size)
        {
            TextAnimation ani = new TextAnimation((int)x, (int)y, text, color, size);
            ani.DrawAction += ani.Draw;
            GEntity<TextAnimation> entity = new GEntity<TextAnimation>(ani);
            entity.TickAction += ani.Tick;
            ani.id = entity.Id;
            return entity;
        }
    }
}
