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

        private Button(int x, int y, Action pressAction) : base(Sprite.Sprites["text"], x, y, 64, 48)
        {
            this.pressAction = pressAction;
            bitmapUp = BitmapExtensions.CreateBitmap(64, 48);
            bitmapDown = BitmapExtensions.CreateBitmap(64, 48);
            Graphics gfx = Graphics.FromImage(bitmapUp);
            gfx.FillEllipse(Brushes.DarkRed, new Rectangle(0, 16, 64, 32));
            gfx.FillRectangle(Brushes.DarkRed, new Rectangle(0, 16, 64, 16));
            gfx.FillEllipse(Brushes.Red, new Rectangle(0, 0, 64, 32));

            gfx = Graphics.FromImage(bitmapDown);
            gfx.FillEllipse(Brushes.DarkRed, new Rectangle(0, 16, 64, 32));
            gfx.FillEllipse(Brushes.Red, new Rectangle(0, 8, 64, 32));
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

        public static GEntity<Button> Create(int x, int y, Action pressAction)
        {
            Button button = new Button(x, y, pressAction);
            button.DrawAction += button.Draw;
            GEntity<Button> entity = new GEntity<Button>(button);
            entity.TickAction += button.Tick;
            return entity;
        }
    }
}
