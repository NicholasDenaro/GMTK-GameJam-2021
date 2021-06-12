using GameEngine;
using GameEngine._2D;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Game
{
    class Enemy : Description2D, IAttacker
    {
        private int maxHealth;
        public int Health { get; private set; }
        private int damage;
        private int speed;
        public int AttackTimer { get; set; }
        private Bitmap bmp;

        private Enemy(int x, int y, int level) : base(Sprite.Sprites["text"], x: x, y: y, Program.ScreenWidth, Program.ScreenHeight)
        {
            this.Health = (int)Math.Pow(1.2, level) * 10;
            this.maxHealth = Health;
            this.damage = (int)Math.Pow(1.01, level);
            this.speed = 1 + level / 10;
            bmp = BitmapExtensions.CreateBitmap(48, 64);
            Graphics gfx = Graphics.FromImage(bmp);
            gfx.FillRectangle(Brushes.MediumPurple, 0, 0, 48, 64);
        }

        public float HealthPercentage()
        {
            return Health * 1.0f / maxHealth;
        }

        public void Damage(int damage)
        {
            this.Health -= damage;
        }

        private void Tick(Location location, Entity entity)
        {
            AttackTimer--;

            if (AttackTimer <= 0)
            {
                AttackTimer = Program.TPS * 5 / (1 + (speed / 3));
            }
        }

        private Bitmap Draw()
        {
            return bmp;
        }

        public static GEntity<Enemy> Create(int x, int y, int level)
        {
            Enemy enemy = new Enemy(x, y, level);
            enemy.DrawAction += enemy.Draw;
            GEntity<Enemy> entity = new GEntity<Enemy>(enemy);
            entity.TickAction += enemy.Tick;
            return entity;
        }
    }
}
