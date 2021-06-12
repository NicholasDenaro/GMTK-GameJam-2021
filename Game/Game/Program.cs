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

            GEntity<Fighter> left = Fighter.Create(ScreenWidth / 2 - 64, ScreenHeight * 2 / 3 - 32);
            GEntity<Fighter> right = Fighter.Create(ScreenWidth / 2 + 16, ScreenHeight * 2 / 3 - 32);
            GEntity<Enemy> enemy = Enemy.Create(ScreenWidth / 2 - 24, 32, 1);

            left.Description.Target = enemy.Description;
            right.Description.Target = enemy.Description;


            GEntity<UIBar> enemyHealth = UIBar.Create(ScreenWidth / 4, 16, ScreenWidth / 2, 8, Brushes.Red, false, enemy.Description.HealthPercentage);

            GEntity<UIBar> leftHealth = UIBar.Create(8, ScreenHeight / 3, 8, ScreenHeight / 3, Brushes.Red, true, left.Description.HealthPercentage);
            GEntity<UIBar> leftAttackTimer = UIBar.Create(20, ScreenHeight / 3, 8, ScreenHeight / 3, Brushes.Blue, true, left.Description.AttackPercentage);

            GEntity<UIBar> rightHealth = UIBar.Create(ScreenWidth - 16, ScreenHeight / 3, 8, ScreenHeight / 3, Brushes.Red, true, right.Description.HealthPercentage);
            GEntity<UIBar> rightAttackTimer = UIBar.Create(ScreenWidth - 28, ScreenHeight / 3, 8, ScreenHeight / 3, Brushes.Blue, true, right.Description.AttackPercentage);


            Engine.TickEnd += (object sender, GameState state) =>
            {
                if (Program.Engine.Controllers.Skip(1).First()[(int)Program.Actions.CLICK].IsPress())
                {
                    left.Description.AdvanceTimer(10);
                    right.Description.AdvanceTimer(10);
                }
            };

            Engine.SetLocation(new Location(new Description2D(0, 0, ScreenWidth, ScreenHeight)));
            Engine.AddEntity(left);
            Engine.AddEntity(right);
            Engine.AddEntity(enemy);
            Engine.AddEntity(Button.Create(ScreenWidth / 2 - 32, ScreenHeight - 48));

            Engine.AddEntity(enemyHealth);
            Engine.AddEntity(leftHealth);
            Engine.AddEntity(leftAttackTimer);
            Engine.AddEntity(rightHealth);
            Engine.AddEntity(rightAttackTimer);

            while (true) { }
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
