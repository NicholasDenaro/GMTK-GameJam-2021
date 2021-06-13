using GameEngine.UI;
using GameEngine.UI.AvaloniaUI;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Game
{
    static class SoundManager
    {
        private static AvaloniaSoundPlayer soundPlayer;
        private static IWavePlayer backingSoundPlayer;
        private static MixingSampleProvider provider;

        static SoundManager()
        {
            soundPlayer = (AvaloniaSoundPlayer)typeof(GameFrame).GetField("soundPlayer", System.Reflection.BindingFlags.NonPublic
                | System.Reflection.BindingFlags.Instance).GetValue(Program.Frame);
            backingSoundPlayer = (IWavePlayer)typeof(AvaloniaSoundPlayer).GetField("player", System.Reflection.BindingFlags.NonPublic
                | System.Reflection.BindingFlags.Instance).GetValue(soundPlayer);
            provider = (MixingSampleProvider)typeof(AvaloniaSoundPlayer).GetField("provider", System.Reflection.BindingFlags.NonPublic
                | System.Reflection.BindingFlags.Instance).GetValue(soundPlayer);
        }

        public static void PlayLoopedMML(MML mml)
        {
            foreach (AvaloniaSound sound in new AvaloniaTrack(mml).Channels())
            {
                SinWaveSound wav = (SinWaveSound)typeof(AvaloniaSound).GetField("wav", System.Reflection.BindingFlags.NonPublic
                    | System.Reflection.BindingFlags.Instance).GetValue(sound);
                typeof(SinWaveSound).GetField("loop", System.Reflection.BindingFlags.NonPublic
                    | System.Reflection.BindingFlags.Instance).SetValue(wav, true);
                Program.Frame.PlaySound(sound);
            }
        }

        public static void Stop()
        {
            backingSoundPlayer.Stop();
        }

        public static void Clear()
        {
            provider.RemoveAllMixerInputs();
        }
    }
}
