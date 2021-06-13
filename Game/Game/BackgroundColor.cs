using GameEngine._2D;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Game
{
    class BackgroundColor : Description2D
    {
        Bitmap[] bmp;
        public int background;

        public BackgroundColor(int x, int y, int width, int height, Color[] colors) : base(Sprite.Sprites["text"], x, y, width, height)
        {
            bmp = new Bitmap[colors.Length];
            for (int i = 0; i < colors.Length; i++)
            {
                bmp[i] = new Bitmap(width, height);
                using Graphics gfx = Graphics.FromImage(bmp[i]);
                gfx.FillRectangle(new SolidBrush(colors[i]), 0, 0, width, height);
            }

            DrawAction += Draw;
        }

        public Bitmap Draw()
        {
            return bmp[background];
        }
    }
}
