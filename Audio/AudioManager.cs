using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Audio;
using System;
using System.Collections.Generic;

namespace StarSmuggler.Audio
{
    public class AudioManager
    {
        private ContentManager content;

        // Music
        private Song currentSong;
        private Song nextSong;
        private float fadeSpeed = 0.5f;
        private float targetVolume = 1.0f;
        private bool fadingOut = false;
        private bool fadingIn = false;

        // Sound effects
        private Dictionary<string, SoundEffect> soundEffects = new();
        private float sfxVolume = 1.0f;

        public void Initialize(ContentManager contentManager)
        {
            content = contentManager;
            MediaPlayer.Volume = 1.0f;
            MediaPlayer.IsRepeating = true;
        }

        // MUSIC

        public void PlaySong(string songName)
        {
            if (currentSong != null && songName == currentSong.Name)
                return;

            nextSong = content.Load<Song>($"Music/{songName}");
            fadingOut = true;
            fadingIn = false;
        }

        public void Update(GameTime gameTime)
        {
            float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (fadingOut)
            {
                MediaPlayer.Volume -= fadeSpeed * delta;
                if (MediaPlayer.Volume <= 0f)
                {
                    MediaPlayer.Stop();
                    currentSong = nextSong;
                    MediaPlayer.Play(currentSong);
                    fadingOut = false;
                    fadingIn = true;
                }
            }
            else if (fadingIn)
            {
                MediaPlayer.Volume += fadeSpeed * delta;
                if (MediaPlayer.Volume >= targetVolume)
                {
                    MediaPlayer.Volume = targetVolume;
                    fadingIn = false;
                }
            }
        }

        // SFX

        public void LoadSfx(string name)
        {
            var sfx = content.Load<SoundEffect>($"FX/{name}");
            soundEffects[name] = sfx;
        }

        public void PlaySfx(string name)
        {
            if (soundEffects.TryGetValue(name, out var sfx))
            {
                var instance = sfx.CreateInstance();
                instance.Volume = sfxVolume;
                instance.Play();
            }
        }

        public void SetSfxVolume(float volume)
        {
            sfxVolume = MathHelper.Clamp(volume, 0f, 1f);
        }

        public void SetMusicVolume(float volume)
        {
            targetVolume = MathHelper.Clamp(volume, 0f, 1f);
            MediaPlayer.Volume = targetVolume;
        }
    }
}
