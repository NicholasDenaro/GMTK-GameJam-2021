using GameEngine;
using GameEngine._2D;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Game
{
    class Fighter : Description2D, IAttacker
    {
        private int level;
        private int exp;
        private int damage;
        private int speed;
        private int attacks;
        private int maxHealth;
        public int Health { get; private set; }

        private Bitmap bmp;
        private Graphics gfx;

        public int AttackTimer { get; set; }

        public Enemy Target { get; set; }

        private Fighter(int x, int y) : base(Sprite.Sprites["text"], x: x, y: y, Program.ScreenWidth, Program.ScreenHeight)
        {
            level = 1;
            exp = 0;
            damage = 1;
            speed = 1;
            attacks = 1;
            Health = 10;
            maxHealth = Health;
            AttackTimer = Program.TPS * 5 / (1 + (speed / 3));
            bmp = BitmapExtensions.CreateBitmap(48, 64);
            gfx = Graphics.FromImage(bmp);
            gfx.FillRectangle(Brushes.OrangeRed, 0, 0, 48, 64);
        }

        public void AdvanceTimer(int i)
        {
            AttackTimer -= i;
        }

        public float AttackPercentage()
        {
            return AttackTimer / (Program.TPS * 5.0f / (1 + (speed / 3)));
        }
        public float HealthPercentage()
        {
            return Health * 1.0f / maxHealth;
        }

        private void Tick(Location location, Entity entity)
        {
            AttackTimer--;
            if (AttackTimer <= 0)
            {
                Attack(Target);
                AttackTimer = Program.TPS * 5 / (1 + (speed / 3));
            }
        }

        public void Damage(int damage)
        {
            this.Health -= damage;
        }

        private void Attack(Enemy enemy)
        {
            int damage = this.damage;
            enemy.Damage(damage);
        }

        private Bitmap Draw()
        {
            return bmp;
        }

        public static GEntity<Fighter> Create(int x, int y)
        {
            Fighter fighter = new Fighter(x, y);
            fighter.DrawAction = fighter.Draw;
            GEntity<Fighter> entity = new GEntity<Fighter>(fighter);
            entity.TickAction += fighter.Tick;
            return entity;
        }
    }
}
