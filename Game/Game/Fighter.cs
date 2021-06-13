using GameEngine;
using GameEngine._2D;
using GameEngine.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Game
{
    class Fighter : Description2D, IAttacker
    {
        private Animation animation;
        public int Level { get; private set; }
        private int exp;
        private int damage;
        private int speed;
        private int attacks;
        private float hpHeal;
        private int maxHealth;
        public float Health { get; private set; }

        public int SkillPoints { get; private set; }

        private Bitmap bmp;
        private Graphics gfx;

        public int AttackTimer { get; set; }

        public Enemy Target { get; set; }

        public int screen;

        public static readonly float SecondsPerAttack = 2.0f;

        private Point defaultPosition;

        private Fighter(int x, int y, Sprite sprite) : base(sprite, x: x, y: y, 48, 64)
        {
            Level = 1;
            exp = 0;
            damage = 1;
            speed = 1;
            attacks = 1;
            hpHeal = 0;
            Health = 10;
            maxHealth = (int)Health;
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
                this.hpHeal += 0.2f / Program.TPS;
                this.SkillPoints--;
            }
        }

        private int TickTimeForAttack()
        {
            return (int)Math.Max((Program.TPS * SecondsPerAttack - speed), 0);
        }

        private void ResetAttackTimer()
        {
            AttackTimer = TickTimeForAttack();
        }

        public void AdvanceTimer(int i)
        {
            //AttackTimer -= i;
            Attack(Target, manualAttack: true);
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
                Program.AddEntity(TextAnimation.Create(X, Y, "Level up!", Color.White, 15, Program.TPS, 0, -1));
            }
            else
            {
                Program.AddEntity(TextAnimation.Create(X + Width / 2, Y - 4, $"{exp}xp", Color.Yellow, 10, Program.TPS, 0.5f, -1));
            }
        }

        private void Tick(Location location, Entity entity)
        {
            if (!(animation?.IsDone() ?? true))
            {
                animation.Tick(this);
            }

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

            if (Health < maxHealth)
            {
                Health += hpHeal;
                
                if (Health > maxHealth)
                {
                    Health = maxHealth;
                }
            }

            AttackTimer--;
            if (AttackTimer <= 0)
            {
                while (!(animation?.IsDone() ?? true))
                {
                    animation.Tick(this);
                }

                if (Attack(Target))
                {
                    ResetAttackTimer();
                }
            }
        }

        public void SetDefaultPosition(int x, int y)
        {
            defaultPosition = new Point(x, y);
        }

        public bool Damage(int damage)
        {
            this.Health -= damage;
            return this.Health <= 0;
        }

        private bool Attack(Enemy enemy, bool manualAttack = false)
        {
            if (enemy.Health <= 0)
            {
                return false;
            }

            if (!manualAttack)
            {
                if (Program.IsSplit)
                {
                    animation = AnimationManager.Instance["attack"].CreateNew();
                    Program.AddEntity(SpriteAnimation.Create(Target.X + Target.Width / 2, Target.Y + Target.Height / 2, Sprite.Sprites["slash"], new int[] { 2, 1, 3, 2, 1 }));
                }
                else
                {
                    if (X < Program.ScreenWidth / 2)
                    {
                        animation = AnimationManager.Instance["left attack center"].CreateNew();
                        Program.AddEntity(SpriteAnimation.Create(Target.X + Target.Width * 3 / 4, Target.Y + Target.Height / 2, Sprite.Sprites["slash"], new int[] { 2, 1, 3, 2, 1 }));
                    }
                    else
                    {
                        animation = AnimationManager.Instance["right attack center"].CreateNew();
                        Program.AddEntity(SpriteAnimation.Create(Target.X, Target.Y + Target.Height / 2, Sprite.Sprites["slash"], new int[] { 2, 1, 3, 2, 1 }));
                    }
                }
            }

            int x = (int)enemy.X + enemy.Width / 2 + Program.Random.Next(-16, 16);
            int y = (int)enemy.Y - 4 + Program.Random.Next(-4, 4);

            int damage = manualAttack ? (1 + this.damage / 5) : this.damage;
            bool killed = false;

            Color color = manualAttack ? Color.White : (X < Program.ScreenWidth / 2) ? Color.LawnGreen : Color.Red;
            for (int i = 0; i < (manualAttack ? 1 : attacks); i++)
            {
                Program.AddEntity(TextAnimation.Create(x, y, "" + damage, color, 15, 8, 1.5f, -4));
                x += 3;
                y -= 8;
                killed |= enemy.Damage(damage);
            }

            if (killed)
            {
                Program.GiveExp(screen, enemy.Level);
                Program.Killed(screen);
            }

            return true;
        }

        private Bitmap Draw()
        {
            return bmp;
        }

        public static GEntity<Fighter> Create(int x, int y, Sprite sprite)
        {
            Fighter fighter = new Fighter(x, y, sprite);
            //fighter.DrawAction = fighter.Draw;
            GEntity<Fighter> entity = new GEntity<Fighter>(fighter);
            entity.TickAction += fighter.Tick;
            return entity;
        }

        public static void AniAttack(IDescription d)
        {
            Fighter fighter = d as Fighter;
            if (fighter == null)
            {
                return;
            }

            if (fighter.animation.TicksLeft() > 13)
            {
                fighter.ChangeCoordsDelta(0, 1);
            }
            else if (fighter.animation.TicksLeft() > 10)
            {
                fighter.ChangeCoordsDelta(0, -(fighter.animation.TicksLeft() - 10) * 3);
            }
            else
            {
                fighter.ChangeCoordsDelta(0, 2);
            }
        }

        public static void AniLeftAttackCenter(IDescription d)
        {
            Fighter fighter = d as Fighter;
            if (fighter == null)
            {
                return;
            }

            if (fighter.animation.TicksLeft() > 13)
            {
                fighter.ChangeCoordsDelta(-1, 1);
            }
            else if (fighter.animation.TicksLeft() > 10)
            {
                fighter.ChangeCoordsDelta((fighter.animation.TicksLeft() - 10), -(fighter.animation.TicksLeft() - 10) * 3);
            }
            else
            {
                fighter.ChangeCoordsDelta(-0.66, 2);
            }
        }

        public static void AniRightAttackCenter(IDescription d)
        {
            Fighter fighter = d as Fighter;
            if (fighter == null)
            {
                return;
            }

            if (fighter.animation.TicksLeft() > 13)
            {
                fighter.ChangeCoordsDelta(1, 1);
            }
            else if (fighter.animation.TicksLeft() > 10)
            {
                fighter.ChangeCoordsDelta(-(fighter.animation.TicksLeft() - 10), -(fighter.animation.TicksLeft() - 10) * 3);
            }
            else
            {
                fighter.ChangeCoordsDelta(0.66, 2);
            }
        }

        public static void AniResetPosition(IDescription d)
        {
            Fighter fighter = d as Fighter;
            if (fighter == null)
            {
                return;
            }

            fighter.SetCoords(fighter.defaultPosition.X, fighter.defaultPosition.Y);
            fighter.animation = null;
        }
    }
}
