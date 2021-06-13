using GameEngine.UI;
using GameEngine.UI.AvaloniaUI;
using NAudio.CoreAudioApi;
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

        ////private static IWavePlayer simpleSoundPlayer;
        ////private static MixingSampleProvider simpleSoundProvider;

        static SoundManager()
        {
            soundPlayer = (AvaloniaSoundPlayer)typeof(GameFrame).GetField("soundPlayer", System.Reflection.BindingFlags.NonPublic
                | System.Reflection.BindingFlags.Instance).GetValue(Program.Frame);
            backingSoundPlayer = (IWavePlayer)typeof(AvaloniaSoundPlayer).GetField("player", System.Reflection.BindingFlags.NonPublic
                | System.Reflection.BindingFlags.Instance).GetValue(soundPlayer);
            provider = (MixingSampleProvider)typeof(AvaloniaSoundPlayer).GetField("provider", System.Reflection.BindingFlags.NonPublic
                | System.Reflection.BindingFlags.Instance).GetValue(soundPlayer);

            ////simpleSoundPlayer = new WasapiOut(AudioClientShareMode.Shared, 0);
            ////WaveFormat format = WaveFormat.CreateIeeeFloatWaveFormat(44100, 2);
            ////simpleSoundProvider = new MixingSampleProvider(format);
            ////simpleSoundPlayer.Init(provider);
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

        public static void Play(MML mml)
        {
            foreach (AvaloniaSound sound in new AvaloniaTrack(mml).Channels())
            {
                ////simpleSoundProvider.AddMixerInput((ISampleProvider)sound.GetOutput());
                ////simpleSoundPlayer.Volume = 0.6f;
                ////simpleSoundPlayer.Play();
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
