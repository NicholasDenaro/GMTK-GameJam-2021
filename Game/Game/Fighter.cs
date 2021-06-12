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
        public int Level { get; private set; }
        private int exp;
        private int damage;
        private int speed;
        private int attacks;
        private int maxHealth;
        public int Health { get; private set; }

        public int SkillPoints { get; private set; }

        private Bitmap bmp;
        private Graphics gfx;

        public int AttackTimer { get; set; }

        public Enemy Target { get; set; }

        public int screen;

        public static readonly float SecondsPerAttack = 2.0f;

        private Fighter(int x, int y) : base(Sprite.Sprites["text"], x: x, y: y, 48, 64)
        {
            Level = 1;
            exp = 0;
            damage = 1;
            speed = 1;
            attacks = 1;
            Health = 10;
            maxHealth = Health;
            ResetAttackTimer();
            bmp = BitmapExtensions.CreateBitmap(48, 64);
            gfx = Graphics.FromImage(bmp);
            gfx.FillRectangle(Brushes.OrangeRed, 0, 0, 48, 64);
        }

        public void UpgradeDamage()
        {
            if (this.SkillPoints > 0)
            {
                this.damage++;
                this.SkillPoints--;
            }
        }

        public void UpgradeSpeed()
        {
            if (this.SkillPoints > 0)
            {
                this.speed++;
                this.SkillPoints--;
                if (this.speed == 4)
                {
                    this.speed = 1;
                    this.attacks++;
                }
            }
        }

        public void UpgradeHealth()
        {
            if (this.SkillPoints > 0)
            {
                this.maxHealth = (int)(this.maxHealth * 1.25);
                this.SkillPoints--;
            }
        }

        private int TickTimeForAttack()
        {
            return (int)(Program.TPS * SecondsPerAttack / (speed * 0.5f));
        }

        private void ResetAttackTimer()
        {
            AttackTimer = TickTimeForAttack();
        }

        public void AdvanceTimer(int i)
        {
            AttackTimer -= i;
        }

        public float AttackPercentage()
        {
            return AttackTimer * 1.0f / TickTimeForAttack();
        }

        public float HealthPercentage()
        {
            return Health * 1.0f / maxHealth;
        }

        public float ExpPercentage()
        {
            return exp * 1.0f / ExpNeeded();
        }

        private int ExpNeeded()
        {
            return (int)(Math.Pow(1.1, this.Level) * 10);
        }

        public void GiveExp(int exp)
        {
            this.exp += exp;
            if (this.exp >= ExpNeeded())
            {
                this.exp -= ExpNeeded();
                this.Level++;
                this.SkillPoints++;
                Program.AddEntity(TextAnimation.Create(X, Y, "Level up!", Color.White, 15));
            }
            else
            {
                Program.AddEntity(TextAnimation.Create(X + Width / 2, Y - 4, $"{exp}xp", Color.White, 10));
            }
        }

        private void Tick(Location location, Entity entity)
        {
            if (Health <= 0)
            {
                if (!Program.IsSplit)
                {
                    Program.Splitup();
                }
                else
                {
                    Program.SummonEnemy(screen);
                }

                Health = maxHealth;
            }

            AttackTimer--;
            if (AttackTimer <= 0)
            {
                Attack(Target);
                ResetAttackTimer();
            }
        }

        public bool Damage(int damage)
        {
            this.Health -= damage;
            return this.Health <= 0;
        }

        private void Attack(Enemy enemy)
        {
            if (enemy.Health <= 0)
            {
                return;
            }

            int x = (int)enemy.X + enemy.Width / 2 + Program.Random.Next(-16, 16);
            int y = (int)enemy.Y - 4 + Program.Random.Next(-4, 4);

            int damage = this.damage;
            bool killed = false;
            for (int i = 0; i < attacks; i++)
            {
                Program.AddEntity(TextAnimation.Create(x, y, "" + damage, Color.White, 15));
                x += 3;
                y -= 8;
                killed |= enemy.Damage(damage);
            }

            if (killed)
            {
                Program.GiveExp(screen, enemy.Level);
                Program.Killed(screen);
            }

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
