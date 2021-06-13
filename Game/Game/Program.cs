using GameEngine;
using GameEngine._2D;
using GameEngine.UI;
using GameEngine.UI.AvaloniaUI;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using static GameEngine._2D.Description2D;

namespace Game
{
    class Program
    {
        public static readonly int ScreenWidth = 320;
        public static readonly int ScreenHeight = 240;
        public static readonly int Scale = 2;
        public static GameEngine.GameEngine Engine;

        public static int TPS = 30;

        private static GEntity<Fighter> left;
        private static GEntity<Fighter> right;

        private static GEntity<Button> joinedButton;
        private static GEntity<Button> leftButton;
        private static GEntity<Button> rightButton;

        public static GEntity<Button> leftHealthButton;
        public static GEntity<Button> leftDamageButton;
        public static GEntity<Button> leftSpeedButton;

        public static GEntity<Button> rightHealthButton;
        public static GEntity<Button> rightDamageButton;
        public static GEntity<Button> rightSpeedButton;

        private static GEntity<Button> PreviousLevelButton;
        private static GEntity<Button> NextLevelButton;

        private static GEntity<Description2D> JoinedBackground;
        private static GEntity<Description2D> LeftBackground;
        private static GEntity<Description2D> RightBackground;

        private static List<GEntity<Enemy>> enemies = new List<GEntity<Enemy>>();

        public static bool IsSplit { get; private set; }

        public static Random Random { get; private set; } = new Random();

        private static Bitmap blank = new Bitmap(1, 1);
        public static DirectDraw invisible = () => blank;

        public static readonly int ProgressionKills = 10;
        public static int JoinedLevel = 1;
        public static int FurthestLevel = JoinedLevel;
        public static int LeftLevel = 1;
        public static int RightLevel = 1;
        public static int JoinedLevelKills = 0;
        public static int LeftLevelKills = 0;
        public static int RightLevelKills = 0;
        public static GEntity<UIBar> joinedProgress;
        public static GEntity<UIBar> leftProgress;
        public static GEntity<UIBar> rightProgress;
        public static bool autoProgress = true;

        public static int NextTarget;

        static void Main(string[] args)
        {
            var wbuilder = new AvaloniaWindowBuilder();
            GameBuilder builder = new GameBuilder()
                .GameEngine(new FixedTickEngine(TPS))
                .GameFrame(new GameFrame(wbuilder, 0, 0, ScreenWidth, ScreenHeight, Scale, Scale))
                .GameView(new GameView2D(ScreenWidth, ScreenHeight, Scale, Scale, Color.SkyBlue))
                .StartingLocation(new Location())
                .Controller(new WindowsKeyController(keyMap.ToDictionary(kvp => (int)kvp.Key, kvp => (int)kvp.Value)))
                .Controller(new WindowsMouseController(mouseMap.ToDictionary(kvp => (int)kvp.Key, kvp => (int)kvp.Value)))
                .Build();

            Engine = builder.Engine;
            GameFrame frame = builder.Frame;

            new Sprite("text", 0, 0);
            new Sprite("grasslands", "Resources/Backgrounds/grassland.PNG", 320, 160);
            new Sprite("grasslands2", "Resources/Backgrounds/grassland2.PNG", 160, 160);
            new Sprite("chicken", "Resources/mobs/chicken.png", 64, 64);
            new Sprite("fighter1", "Resources/Characters/leftCharacter.png", 39, 64);
            new Sprite("fighter2", "Resources/Characters/rightCharacter.png", 43, 64);
            new Sprite("attackbutton", "Resources/UI/attackButton.png", 64, 48);
            new Sprite("attackbuttonsmall", "Resources/UI/attackButtonSmall.png", 32, 24);
            new Sprite("leftArrow", "Resources/UI/guiLeftArrow.png", 21, 16);
            new Sprite("rightArrow", "Resources/UI/guiRightArrow.png", 21, 16);

            Engine.SetLocation(new Location(new Description2D(0, 0, ScreenWidth, ScreenHeight)));

            JoinedBackground = new GEntity<Description2D>(new Description2D(Sprite.Sprites["grasslands"], 0, 80));
            LeftBackground = new GEntity<Description2D>(new Description2D(Sprite.Sprites["grasslands2"], 0, 80));
            RightBackground = new GEntity<Description2D>(new Description2D(Sprite.Sprites["grasslands2"], 160, 80));

            Program.AddEntity(JoinedBackground);
            Program.AddEntity(LeftBackground);
            Program.AddEntity(RightBackground);

            Engine.Start();

            left = Fighter.Create(ScreenWidth / 2 - 64, ScreenHeight * 2 / 3 - 32, Sprite.Sprites["fighter1"]);
            right = Fighter.Create(ScreenWidth / 2 + 16, ScreenHeight * 2 / 3 - 32, Sprite.Sprites["fighter2"]);

            GEntity<UIBar> leftHealth = UIBar.Create(8, ScreenHeight / 3, 8, ScreenHeight / 3, Brushes.Red, true, left.Description.HealthPercentage);
            GEntity<UIBar> leftAttackTimer = UIBar.Create(20, ScreenHeight / 3, 8, ScreenHeight / 3, Brushes.Blue, true, left.Description.AttackPercentage);
            GEntity<UIBar> leftExp = UIBar.Create(20, ScreenHeight - 12, ScreenWidth / 4, 8, Brushes.Cyan, false, left.Description.ExpPercentage);
            GEntity<Counter> leftLevel = Counter.Create(20, ScreenHeight - 12 - 20, () => $"Level:{left.Description.Level},sp:{left.Description.SkillPoints}");

            GEntity<UIBar> rightHealth = UIBar.Create(ScreenWidth - 16, ScreenHeight / 3, 8, ScreenHeight / 3, Brushes.Red, true, right.Description.HealthPercentage);
            GEntity<UIBar> rightAttackTimer = UIBar.Create(ScreenWidth - 28, ScreenHeight / 3, 8, ScreenHeight / 3, Brushes.Blue, true, right.Description.AttackPercentage);
            GEntity<UIBar> rightExp = UIBar.Create(ScreenWidth - ScreenWidth / 4 - 28, ScreenHeight - 12, ScreenWidth / 4, 8, Brushes.Cyan, false, right.Description.ExpPercentage);
            GEntity<Counter> rightLevel = Counter.Create(ScreenWidth - ScreenWidth / 4 - 28, ScreenHeight - 12 - 20, () => $"Level:{right.Description.Level},sp:{right.Description.SkillPoints}");

            // Devtool
            Engine.TickEnd += (object sender, GameState state) =>
            {
                if (Program.Engine.Controllers.First()[(int)Program.Actions.ACTION].IsPress())
                {
                    JoinTogether();
                }
                if (Program.Engine.Controllers.First()[(int)Program.Actions.CANCEL].IsPress())
                {
                    Splitup();
                }
            };

            Program.AddEntity(left);
            Program.AddEntity(right);

            JoinTogether();

            Program.AddEntity(leftHealth);
            Program.AddEntity(leftAttackTimer);
            Program.AddEntity(leftExp);
            Program.AddEntity(leftLevel);

            Program.AddEntity(rightHealth);
            Program.AddEntity(rightAttackTimer);
            Program.AddEntity(rightExp);
            Program.AddEntity(rightLevel);

            while (true) { }
        }

        public static void AddEntity(Entity entity)
        {
            if (!Engine.Location.Entities.Contains(entity))
            {
                Engine.AddEntity(entity);
            }
        }

        public static void RemoveEntity(Entity entity)
        {
            if (Engine.Location.Entities.Contains(entity))
            {
                Engine.Location.RemoveEntity(entity.Id);
            }
        }

        public static void ShowNextLevelButton()
        {
            if (NextLevelButton == null)
            {
                NextLevelButton = Button.Create(ScreenWidth - 24, 0, Color.Wheat, () => {
                    if (JoinedLevel < FurthestLevel)
                    {
                        JoinedLevel++;
                        LeftLevel = JoinedLevel;
                        RightLevel = JoinedLevel;
                        if (JoinedLevel % 10 == 0)
                        {
                            JoinTogether();
                        }
                    }
                    else
                    {
                        autoProgress = true;
                    }
                }, 3, Sprite.Sprites["rightArrow"]);
                Program.Engine.AddEntity(NextLevelButton);
            }
        }

        public static void Splitup()
        {
            IsSplit = true;
            LeftBackground.Description.DrawAction -= invisible;
            RightBackground.Description.DrawAction -= invisible;
            JoinedBackground.Description.DrawAction = invisible;
            if (JoinedLevel % 10 == 0)
            {
                JoinedLevel--;
                autoProgress = false;

                ShowNextLevelButton();
            }

            if (PreviousLevelButton == null)
            {
                PreviousLevelButton = Button.Create(0, 0, Color.Wheat, () => {
                    if (JoinedLevel > 1)
                    {
                        JoinedLevel--;
                        LeftLevel = JoinedLevel;
                        RightLevel = JoinedLevel;

                        if (JoinedLevel % 10 == 0)
                        {
                            JoinTogether();
                        }

                        autoProgress = false;

                        if (IsSplit)
                        {
                            SummonEnemy(1);
                            SummonEnemy(2);
                        }
                        else
                        {
                            SummonEnemy(0);
                        }

                        ShowNextLevelButton();
                    }
                }, 3, Sprite.Sprites["leftArrow"]);
                Program.AddEntity(PreviousLevelButton);
            }

            LeftLevel = JoinedLevel;
            RightLevel = JoinedLevel;
            LeftLevelKills = JoinedLevelKills;
            RightLevelKills = JoinedLevelKills;

            left.Description.SetCoords(ScreenWidth / 4 - 24, ScreenHeight * 2 / 3 - 32);
            left.Description.screen = 1;

            right.Description.SetCoords(ScreenWidth * 3 / 4 - 24, ScreenHeight * 2 / 3 - 32);
            right.Description.screen = 2;

            if (joinedButton != null)
            {
                Engine.Location.RemoveEntity(joinedButton.Id);
                Engine.Location.RemoveEntity(joinedProgress.Id);
            }

            if (leftButton == null)
            {
                Action leftAdvanceTimer = () => left.Description.AdvanceTimer(TPS);
                Action rightAdvanceTimer = () => right.Description.AdvanceTimer(TPS);

                leftButton = Button.Create(ScreenWidth / 4 - 16, ScreenHeight - 48, Color.Red, leftAdvanceTimer, 2, Sprite.Sprites["attackbuttonsmall"]);
                rightButton = Button.Create(ScreenWidth * 3 / 4 - 16, ScreenHeight - 48, Color.Red, rightAdvanceTimer, 2, Sprite.Sprites["attackbuttonsmall"]);
                leftProgress = UIBar.Create(20, 4, ScreenWidth / 4, 8, Brushes.Wheat, false, () => LeftLevelKills / 10.0f, () => $"Level: {LeftLevel}, {LeftLevelKills}/10");
                rightProgress = UIBar.Create(ScreenWidth - ScreenWidth / 4 - 28, 4, ScreenWidth / 4, 8, Brushes.Wheat, false, () => RightLevelKills / 10.0f, () => $"Level: {RightLevel}, {RightLevelKills}/10");
            }

            RemoveEnemies();

            SummonEnemy(1);
            SummonEnemy(2);

            if (!Engine.Location.Entities.Contains(leftButton))
            {
                Program.AddEntity(leftButton);
                Program.AddEntity(rightButton);
                Program.AddEntity(leftProgress);
                Program.AddEntity(rightProgress);
            }
        }
        
        public static void JoinTogether()
        {
            IsSplit = false;
            LeftLevelKills = 0;
            RightLevelKills = 0;
            left.Description.SetCoords(ScreenWidth / 2 - 64, ScreenHeight * 2 / 3 - 32);
            right.Description.SetCoords(ScreenWidth / 2 + 16, ScreenHeight * 2 / 3 - 32);
            left.Description.screen = 0;
            right.Description.screen = 0;

            LeftBackground.Description.DrawAction = invisible;
            RightBackground.Description.DrawAction = invisible;
            JoinedBackground.Description.DrawAction -= invisible;

            if (leftButton != null)
            {
                Program.RemoveEntity(leftButton);
                Program.RemoveEntity(leftProgress);
            }
            if (rightButton != null)
            {
                Program.RemoveEntity(rightButton);
                Program.RemoveEntity(rightProgress);
            }

            if (joinedButton == null)
            {
                Action bothAdvanceTimer = () =>
                {
                    left.Description.AdvanceTimer(TPS);
                    right.Description.AdvanceTimer(TPS);
                };

                joinedButton = Button.Create(ScreenWidth / 2 - 32, ScreenHeight - 48, Color.Red, bothAdvanceTimer, 1, Sprite.Sprites["attackbutton"]);
                joinedProgress = UIBar.Create(20, 4, ScreenWidth * 7 / 8, 8, Brushes.Wheat, false, () => JoinedLevelKills / 10.0f, () => $"Level: {JoinedLevel}, {JoinedLevelKills}/10");
            }

            RemoveEnemies();

            SummonEnemy(0);

            if (!Engine.Location.Entities.Contains(joinedButton))
            {
                Program.AddEntity(joinedButton);
                Program.AddEntity(joinedProgress);
            }
        }

        private static void RemoveEnemies(Func<GEntity<Enemy>, bool> filter = null)
        {
            if (filter == null)
            {
                filter = _ => true;
            }
            foreach (GEntity<Enemy> enemy in enemies)
            {
                if (filter(enemy))
                {
                    enemy.Description.Destroy();
                }
            }

            enemies.RemoveAll((e) => filter(e));
        }

        public static void SummonEnemy(int screen)
        {
            GEntity<Enemy> enemy;
            GEntity<UIBar> enemyHealth;
            if (screen == 1)
            {
                enemy = Enemy.Create(ScreenWidth * 1 / 4 - 24, 32, LeftLevel, screen);
                enemyHealth = UIBar.Create(
                    ScreenWidth / 8, 24,
                    ScreenWidth / 4, 8,
                    Brushes.Red,
                    false,
                    enemy.Description.HealthPercentage);
                left.Description.Target = enemy.Description;
                enemy.Description.SetTargets(left.Description);

            }
            else if (screen == 2)
            {
                enemy = Enemy.Create(ScreenWidth * 3 / 4 - 24, 32, RightLevel, screen);
                enemyHealth = UIBar.Create(
                    ScreenWidth * 5 / 8, 24,
                    ScreenWidth / 4, 8,
                    Brushes.Red,
                    false,
                    enemy.Description.HealthPercentage);
                right.Description.Target = enemy.Description;
                enemy.Description.SetTargets(right.Description);
            }
            else
            {
                enemy = Enemy.Create(ScreenWidth / 2 - 24, 32, JoinedLevel, screen);
                enemyHealth = UIBar.Create(ScreenWidth / 3, 24, ScreenWidth / 3, 8, Brushes.Red, false, enemy.Description.HealthPercentage);
                left.Description.Target = enemy.Description;
                right.Description.Target = enemy.Description;
                enemy.Description.SetTargets(left.Description, right.Description);
            }
            RemoveEnemies((e) => e.Description.screen == screen);

            Program.AddEntity(enemy);
            enemies.Add(enemy);

            enemy.Description.HealthBar = enemyHealth;
            Program.AddEntity(enemyHealth);
        }

        public static void GiveExp(int screen, int exp)
        {
            if (screen == 1)
            {
                Fighter f = left.Description;
                left.Description.GiveExp(exp);

                if (f.SkillPoints > 0)
                {
                    ShowSkillButtons(false);
                }
            }
            else if (screen == 2)
            {
                Fighter f = left.Description;
                right.Description.GiveExp(exp);

                if (f.SkillPoints > 0)
                {
                    ShowSkillButtons(true);
                }
            }
            else
            {
                Fighter f = left.Description;
                left.Description.GiveExp(exp);

                if (f.SkillPoints > 0)
                {
                    ShowSkillButtons(false);
                }

                f = right.Description;
                right.Description.GiveExp(exp);

                if (f.SkillPoints > 0)
                {
                    ShowSkillButtons(true);
                }
            }
        }

        public static void ShowSkillButtons(bool right)
        {
            if (!right)
            {
                if (leftHealthButton == null)
                {
                    leftHealthButton = Button.Create(4, ScreenHeight - 40, Color.Red, () => Program.left.Description.UpgradeHealth(), 4);
                    leftDamageButton = Button.Create(20, ScreenHeight - 40, Color.White, () => Program.left.Description.UpgradeDamage(), 4);
                    leftSpeedButton = Button.Create(36, ScreenHeight - 40, Color.LightBlue, () => Program.left.Description.UpgradeSpeed(), 4);
                    Program.AddEntity(leftHealthButton);
                    Program.AddEntity(leftDamageButton);
                    Program.AddEntity(leftSpeedButton);
                }
            }
            else
            {
                if (rightHealthButton == null)
                {
                    rightHealthButton = Button.Create(ScreenWidth - 52, ScreenHeight - 40, Color.Red, () => Program.right.Description.UpgradeHealth(), 4);
                    rightDamageButton = Button.Create(ScreenWidth - 36, ScreenHeight - 40, Color.White, () => Program.right.Description.UpgradeDamage(), 4);
                    rightSpeedButton = Button.Create(ScreenWidth - 20, ScreenHeight - 40, Color.LightBlue, () => Program.right.Description.UpgradeSpeed(), 4);
                    Program.AddEntity(rightHealthButton);
                    Program.AddEntity(rightDamageButton);
                    Program.AddEntity(rightSpeedButton);
                }
            }
        }

        public static void Killed(int screen)
        {
            if (screen == 1)
            {
                LeftLevelKills++;
            }
            else if (screen == 2)
            {
                RightLevelKills++;
            }
            else
            {
                JoinedLevelKills++;
                if (JoinedLevelKills >= 10 && autoProgress || JoinedLevel % 10 == 0)
                {
                    JoinedLevelKills = 0;
                    JoinedLevel++;
                    FurthestLevel = JoinedLevel;
                }
            }

            if (RightLevelKills >= 10 && LeftLevelKills >= 10)
            {
                if (autoProgress)
                {
                    JoinedLevelKills = 0;
                    JoinedLevel++;
                    FurthestLevel = JoinedLevel;
                }

                JoinTogether();
            }
        }

        public static Dictionary<Avalonia.Input.Key, Actions> keyMap
            = new Dictionary<Avalonia.Input.Key, Actions>(new[]
            {
                new KeyValuePair<Avalonia.Input.Key, Actions>(Avalonia.Input.Key.F2, Actions.DIAGS),
                new KeyValuePair<Avalonia.Input.Key, Actions>(Avalonia.Input.Key.Z, Actions.CANCEL),
                new KeyValuePair<Avalonia.Input.Key, Actions>(Avalonia.Input.Key.R, Actions.RESTART),
                new KeyValuePair<Avalonia.Input.Key, Actions>(Avalonia.Input.Key.X, Actions.ACTION),
                new KeyValuePair<Avalonia.Input.Key, Actions>(Avalonia.Input.Key.Up, Actions.UP),
                new KeyValuePair<Avalonia.Input.Key, Actions>(Avalonia.Input.Key.Down, Actions.DOWN),
                new KeyValuePair<Avalonia.Input.Key, Actions>(Avalonia.Input.Key.Left, Actions.LEFT),
                new KeyValuePair<Avalonia.Input.Key, Actions>(Avalonia.Input.Key.Right, Actions.RIGHT),
            });

        public static Dictionary<Avalonia.Input.PointerUpdateKind, Actions> mouseMap
            = new Dictionary<Avalonia.Input.PointerUpdateKind, Actions>(new[]{
                new KeyValuePair<Avalonia.Input.PointerUpdateKind, Actions>(Avalonia.Input.PointerUpdateKind.LeftButtonPressed, Actions.CLICK),
                new KeyValuePair<Avalonia.Input.PointerUpdateKind, Actions>(Avalonia.Input.PointerUpdateKind.Other, Actions.MOUSEINFO),
            });

        public enum Actions { UP, DOWN, LEFT, RIGHT, ACTION, CANCEL, ESCAPE, RESTART, DIAGS, MOUSEINFO, CLICK };
    }
}
