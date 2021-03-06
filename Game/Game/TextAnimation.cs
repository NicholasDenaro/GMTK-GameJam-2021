using GameEngine;
using GameEngine._2D;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
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

        private int duration;
        private float dx;
        private float dy;

        public int trueWidth;
        public int trueHeight;

        public override Rectangle Bounds => new Rectangle(new Point((int)X, (int)Y), new Size(trueWidth, Height));

        private TextAnimation(int x, int y, string text, Color color, int size, int duration, float dx, float dy)
            : base(Sprite.Sprites["text"], x: x, y: y, 
                  (int)Graphics.FromImage(new Bitmap(1, 1)).MeasureString(text, new Font("Arial", size)).Width,
                  (int)Graphics.FromImage(new Bitmap(1, 1)).MeasureString(text, new Font("Arial", size)).Height)
        {
            this.text = text;
            this.color = color;
            this.font = new Font("Arial", size);

            using Graphics g = Graphics.FromImage(new Bitmap(1,1));
            trueWidth = (int)g.MeasureString(text, this.font).Width;
            trueHeight = (int)g.MeasureString(text, this.font).Height;

            bmp = BitmapExtensions.CreateBitmap(trueWidth, trueHeight);
            bmp.MakeTransparent();
            gfx = Graphics.FromImage(bmp);
            gfx.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;
            
            this.duration = duration;
            this.dx = dx;
            this.dy = dy;
            time = duration;
        }

        private void Tick(Location location, Entity entity)
        {
            this.ChangeCoordsDelta(dx, dy);
            if (--time <= 0)
            {
                DrawAction = Program.invisible;
                bmp.Dispose();
                gfx.Dispose();
                font.Dispose();
                location.RemoveEntity(id);
            }
        }

        private Bitmap Draw()
        {
            gfx.Clear(Color.Transparent);
            using Brush brush = new SolidBrush(Color.FromArgb((int)(time * 255.0 / duration), color));
            using Pen innerPen = new Pen(Color.FromArgb((int)(time * 255.0 / duration), Color.Black), 3);
            using Pen outerPen2 = new Pen(Color.FromArgb((int)(time * 255.0 / duration), Color.Black), 1);
            using Pen outerPen = new Pen(Color.FromArgb((int)(time * 255.0 / duration), Color.White), 5);
            GraphicsPath innerPath = new GraphicsPath();
            innerPath.AddString(text, font.FontFamily, (int)font.Style, gfx.DpiY * font.Size / 72, new Point(0, 0), new StringFormat());

            GraphicsPath outerPath = new GraphicsPath();
            outerPath.AddPath(innerPath, false);

            GraphicsPath outerPath2 = new GraphicsPath();
            outerPath2.AddPath(outerPath, false);

            //gfx.DrawPath(outerPen2, outerPath2);
            gfx.DrawPath(outerPen, outerPath);
            gfx.DrawPath(innerPen, innerPath);
            gfx.DrawString(text, font, brush, 0, 0);

            return bmp;
        }

        public static GEntity<TextAnimation> Create(double x, double y, string text, Color color, int size, int duration, float dx, float dy)
        {
            TextAnimation ani = new TextAnimation((int)x, (int)y, text, color, size, duration, dx, dy);
            ani.DrawAction += ani.Draw;
            GEntity<TextAnimation> entity = new GEntity<TextAnimation>(ani);
            entity.TickAction += ani.Tick;
            ani.id = entity.Id;
            return entity;
        }
    }
}
