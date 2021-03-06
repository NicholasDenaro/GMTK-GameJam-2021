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
        public int Level { get; private set; }
        private int maxHealth;
        public int Health { get; private set; }
        private int damage;
        private int speed;
        public int AttackTimer { get; set; }
        private Bitmap bmp;
        private Guid Id;
        public int screen;
        public GEntity<UIBar> HealthBar { get; set; }

        private Fighter[] targets;

        private bool removed;

        private Enemy(int x, int y, int level, int screen, Sprite sprite) : base(sprite, x: x, y: y, 64, 64)
        {
            this.screen = screen;
            bmp = new Bitmap(Image());
            Summon(level, sprite);
        }

        public void Summon(int level, Sprite sprite)
        {
            this.Level = level;
            this.Health = (int)Math.Pow(1.2, level) * 10 * (this.Level == 10 ? 20 : 1); // Boss health
            this.maxHealth = Health;
            this.damage = (int)Math.Pow(1.1, level);
            this.speed = 1 + level / 10;
            AttackTimer = Program.TPS * 5 / (1 + (speed / 3));
            using Graphics gfx = Graphics.FromImage(bmp);
            gfx.Clear(Color.Transparent);
            gfx.DrawImage(sprite.Images[0], 0, 0);
            Sprite border = Sprite.Sprites["border"];
            gfx.DrawImage(border.Images[0], new Rectangle(0, 0, Width, Height), new Rectangle(0, 0, border.Width, border.Height), GraphicsUnit.Pixel);
            if (this.Level % 10 == 0)
            {
                Sprite plank = Sprite.Sprites["plank"];
                gfx.DrawImage(plank.Images[0], new Rectangle(0, Height - 16, 16, 16), new Rectangle(0, 0, plank.Width, plank.Height), GraphicsUnit.Pixel);
                Sprite skull = Sprite.Sprites["skull"];
                gfx.DrawImage(skull.Images[0], new Rectangle(0, Height - 16, 16, 16), new Rectangle(0, 0, skull.Width, skull.Height), GraphicsUnit.Pixel);
            }
            if (HealthBar != null)
            {
                HealthBar.Description.DrawAction = HealthBar.Description.Draw;
            }

            DrawAction = Draw;
        }

        public float HealthPercentage()
        {
            return Health * 1.0f / maxHealth;
        }

        public bool Damage(int damage)
        {
            this.Health -= damage;

            return this.Health <= 0;
        }

        private void Attack(Fighter fighter)
        {
            int damage = this.damage;
            fighter.Damage(damage);
        }

        public void SetTargets(params Fighter[] targets)
        {
            this.targets = targets;
        }

        private void Tick(Location location, Entity entity)
        {
            //if (removed)
            //{
            //    return;
            //}

            if (DrawAction == Program.invisible)
            {
                return;
            }

            if (Health <= 0)
            {
                //DrawAction = Program.invisible;
                //bmp.Dispose();
                //Destroy();
                Program.SummonEnemy(screen);
            }

            AttackTimer--;

            if (AttackTimer <= 0)
            {
                Attack(targets[Program.NextTarget++ % targets.Length]);
                AttackTimer = Program.TPS * 5 / (1 + (speed / 3));
            }
        }

        ////public void Destroy()
        ////{
        ////    if (!removed)
        ////    {
        ////        Program.Engine.Location.RemoveEntity(this.Id);
        ////        Program.Engine.Location.RemoveEntity(this.HealthBar.Id);
        ////        removed = true;
        ////    }
        ////}

        private Bitmap Draw()
        {
            return bmp;
        }

        public static GEntity<Enemy> Create(int x, int y, int level, int screen, Sprite sprite)
        {
            Enemy enemy = new Enemy(x, y, level, screen, sprite);
            enemy.DrawAction += enemy.Draw;
            GEntity<Enemy> entity = new GEntity<Enemy>(enemy);
            entity.TickAction += enemy.Tick;
            enemy.Id = entity.Id;
            return entity;
        }
    }
}
