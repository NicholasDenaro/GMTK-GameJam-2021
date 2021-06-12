using GameEngine;
using GameEngine._2D;
using GameEngine.UI;
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
        private Action pressAction;
        private bool isDown;

        private Button(int x, int y, Color color, Action pressAction, int scaleDown)
            : base(Sprite.Sprites["text"], x, y, 64 / scaleDown, 48 / scaleDown)
        {
            this.pressAction = pressAction;
            bitmapUp = BitmapExtensions.CreateBitmap(64 / scaleDown, 48 / scaleDown);
            bitmapDown = BitmapExtensions.CreateBitmap(64 / scaleDown, 48 / scaleDown);
            Graphics gfx = Graphics.FromImage(bitmapUp);

            Brush b = new SolidBrush(color);
            Brush b2 = new SolidBrush(Color.FromArgb(color.A, color.R / 2, color.G / 2, color.B / 2));
            gfx.FillEllipse(b2, new Rectangle(0, 16 / scaleDown, 64 / scaleDown, 32 / scaleDown));
            gfx.FillRectangle(b2, new Rectangle(0, 16 / scaleDown, 64 / scaleDown, 16 / scaleDown));
            gfx.FillEllipse(b, new Rectangle(0, 0, 64 / scaleDown, 32 / scaleDown));

            gfx = Graphics.FromImage(bitmapDown);
            gfx.FillEllipse(b2, new Rectangle(0, 16 / scaleDown, 64 / scaleDown, 32 / scaleDown));
            gfx.FillEllipse(b, new Rectangle(0, 8 / scaleDown, 64 / scaleDown, 32 / scaleDown));
        }

        private void Tick(Location location, Entity entity)
        {
            if (Program.Engine.Controllers.Skip(1).First()[(int)Program.Actions.CLICK].IsPress())
            {
                WindowsMouseController wmc = Program.Engine.Controllers.Skip(1).First() as WindowsMouseController;
                MouseControllerInfo mci = wmc[(int)Program.Actions.MOUSEINFO].Info as MouseControllerInfo;
                if (this.Bounds.Contains(new Point(mci.X, mci.Y)))
                {
                    isDown = true;
                    pressAction();
                }
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

        public static GEntity<Button> Create(int x, int y, Color color, Action pressAction, int scaleDown = 1)
        {
            Button button = new Button(x, y, color, pressAction, scaleDown);
            button.DrawAction += button.Draw;
            GEntity<Button> entity = new GEntity<Button>(button);
            entity.TickAction += button.Tick;
            return entity;
        }
    }
}
