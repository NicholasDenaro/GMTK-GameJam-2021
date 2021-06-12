using GameEngine;
using GameEngine._2D;
using GameEngine.UI;
using GameEngine.UI.AvaloniaUI;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;

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

        private static List<GEntity<Enemy>> enemies = new List<GEntity<Enemy>>();

        public static bool IsSplit { get; private set; }

        public static Random Random { get; private set; } = new Random();

        public static readonly int ProgressionKills = 10;
        public static int Level = 1;
        public static int JoinedLevelKills = 0;
        public static int LeftLevelKills = 0;
        public static int RightLevelKills = 0;
        public static GEntity<UIBar> joinedProgress;
        public static GEntity<UIBar> leftProgress;
        public static GEntity<UIBar> rightProgress;

        static void Main(string[] args)
        {
            var wbuilder = new AvaloniaWindowBuilder();
            GameBuilder builder = new GameBuilder()
                .GameEngine(new FixedTickEngine(TPS))
                .GameFrame(new GameFrame(wbuilder, 0, 0, ScreenWidth, ScreenHeight, Scale, Scale))
                .GameView(new GameView2D(ScreenWidth, ScreenHeight, Scale, Scale, Color.DarkSlateGray))
                .StartingLocation(new Location())
                .Controller(new WindowsKeyController(keyMap.ToDictionary(kvp => (int)kvp.Key, kvp => (int)kvp.Value)))
                .Controller(new WindowsMouseController(mouseMap.ToDictionary(kvp => (int)kvp.Key, kvp => (int)kvp.Value)))
                .Build();

            Engine = builder.Engine;
            GameFrame frame = builder.Frame;

            new Sprite("text", 0, 0);

            Engine.Start();

            left = Fighter.Create(ScreenWidth / 2 - 64, ScreenHeight * 2 / 3 - 32);
            right = Fighter.Create(ScreenWidth / 2 + 16, ScreenHeight * 2 / 3 - 32);

            GEntity<UIBar> leftHealth = UIBar.Create(8, ScreenHeight / 3, 8, ScreenHeight / 3, Brushes.Red, true, left.Description.HealthPercentage);
            GEntity<UIBar> leftAttackTimer = UIBar.Create(20, ScreenHeight / 3, 8, ScreenHeight / 3, Brushes.Blue, true, left.Description.AttackPercentage);
            GEntity<UIBar> leftExp = UIBar.Create(20, ScreenHeight - 12, ScreenWidth / 4, 8, Brushes.Cyan, false, left.Description.ExpPercentage);
            GEntity<Counter> leftLevel = Counter.Create(20, ScreenHeight - 12 - 20, "Level", () => left.Description.Level);

            GEntity<UIBar> rightHealth = UIBar.Create(ScreenWidth - 16, ScreenHeight / 3, 8, ScreenHeight / 3, Brushes.Red, true, right.Description.HealthPercentage);
            GEntity<UIBar> rightAttackTimer = UIBar.Create(ScreenWidth - 28, ScreenHeight / 3, 8, ScreenHeight / 3, Brushes.Blue, true, right.Description.AttackPercentage);
            GEntity<UIBar> rightExp = UIBar.Create(ScreenWidth - ScreenWidth / 4 - 28, ScreenHeight - 12, ScreenWidth / 4, 8, Brushes.Cyan, false, right.Description.ExpPercentage);
            GEntity<Counter> rightLevel = Counter.Create(ScreenWidth - ScreenWidth / 4 - 28, ScreenHeight - 12 - 20, "Level", () => right.Description.Level);

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

            Engine.SetLocation(new Location(new Description2D(0, 0, ScreenWidth, ScreenHeight)));
            Engine.AddEntity(left);
            Engine.AddEntity(right);

            JoinTogether();

            Engine.AddEntity(leftHealth);
            Engine.AddEntity(leftAttackTimer);
            Engine.AddEntity(leftExp);
            Engine.AddEntity(leftLevel);

            Engine.AddEntity(rightHealth);
            Engine.AddEntity(rightAttackTimer);
            Engine.AddEntity(rightExp);
            Engine.AddEntity(rightLevel);

            while (true) { }
        }

        public static void Splitup()
        {
            left.Description.SetCoords(ScreenWidth / 4 - 24, ScreenHeight * 2 / 3 - 32);
            left.Description.screen = 1;

            right.Description.SetCoords(ScreenWidth * 3 / 4 - 24, ScreenHeight * 2 / 3 - 32);
            right.Description.screen = 2;

            if (joinedButton != null)
            {
                Engine.Location.RemoveEntity(joinedButton.Id);
            }

            if (leftButton == null)
            {
                Action leftAdvanceTimer = () => left.Description.AdvanceTimer(TPS);
                Action rightAdvanceTimer = () => right.Description.AdvanceTimer(TPS);

                leftButton = Button.Create(ScreenWidth / 4 - 32, ScreenHeight - 48, leftAdvanceTimer);
                rightButton = Button.Create(ScreenWidth * 3 / 4 - 32, ScreenHeight - 48, rightAdvanceTimer);
            }

            RemoveEnemies();

            SummonEnemy(1);
            SummonEnemy(2);

            if (!Engine.Location.Entities.Contains(leftButton))
            {
                Engine.AddEntity(leftButton);
                Engine.AddEntity(rightButton);
            }

            IsSplit = true;
        }
        
        public static void JoinTogether()
        {
            left.Description.SetCoords(ScreenWidth / 2 - 64, ScreenHeight * 2 / 3 - 32);
            right.Description.SetCoords(ScreenWidth / 2 + 16, ScreenHeight * 2 / 3 - 32);
            left.Description.screen = 0;
            right.Description.screen = 0;

            if (leftButton != null)
            {
                Engine.Location.RemoveEntity(leftButton.Id);
            }
            if (rightButton != null)
            {
                Engine.Location.RemoveEntity(rightButton.Id);
            }

            if (joinedButton == null)
            {
                Action bothAdvanceTimer = () =>
                {
                    left.Description.AdvanceTimer(TPS);
                    right.Description.AdvanceTimer(TPS);
                };

                joinedButton = Button.Create(ScreenWidth / 2 - 32, ScreenHeight - 48, bothAdvanceTimer);
            }

            RemoveEnemies();

            SummonEnemy(0);

            if (!Engine.Location.Entities.Contains(joinedButton))
            {
                Engine.AddEntity(joinedButton);
            }
            IsSplit = false;
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
                enemy = Enemy.Create(ScreenWidth * 1 / 4 - 24, 32, 1, screen);
                enemyHealth = UIBar.Create(
                    ScreenWidth / 8, 16,
                    ScreenWidth / 4, 8,
                    Brushes.Red,
                    false,
                    enemy.Description.HealthPercentage);
                left.Description.Target = enemy.Description;
                enemy.Description.SetTargets(left.Description);

            }
            else if (screen == 2)
            {
                enemy = Enemy.Create(ScreenWidth * 3 / 4 - 24, 32, 1, screen);
                enemyHealth = UIBar.Create(
                    ScreenWidth * 5 / 8, 16,
                    ScreenWidth / 4, 8,
                    Brushes.Red,
                    false,
                    enemy.Description.HealthPercentage);
                right.Description.Target = enemy.Description;
                enemy.Description.SetTargets(right.Description);
            }
            else
            {
                enemy = Enemy.Create(ScreenWidth / 2 - 24, 32, 1, screen);
                enemyHealth = UIBar.Create(ScreenWidth / 4, 16, ScreenWidth / 2, 8, Brushes.Red, false, enemy.Description.HealthPercentage);
                left.Description.Target = enemy.Description;
                right.Description.Target = enemy.Description;
                enemy.Description.SetTargets(left.Description, right.Description);
            }
            RemoveEnemies((e) => e.Description.screen == screen);

            Engine.AddEntity(enemy);
            enemies.Add(enemy);

            enemy.Description.HealthBar = enemyHealth;
            Engine.AddEntity(enemyHealth);
        }

        public static void GiveExp(int screen, int exp)
        {
            if (screen == 1)
            {
                Fighter f = left.Description;
                left.Description.GiveExp(exp);
            }
            else if (screen == 2)
            {
                Fighter f = left.Description;
                right.Description.GiveExp(exp);
            }
            else
            {
                Fighter f = left.Description;
                left.Description.GiveExp(exp);

                f = right.Description;
                right.Description.GiveExp(exp);
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
