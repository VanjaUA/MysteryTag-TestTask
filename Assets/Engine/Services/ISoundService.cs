#region

using UnityEngine;

#endregion

namespace MergeGridGame.Services
{
    public interface ISoundService
    {
        void PlayMusic(AudioClip clip, bool loop = true);
        void StopMusic();

        void PlaySound(AudioClip clip);

        void SetMusicVolume(float volume);
        void SetSfxVolume(float volume);

        float GetMusicVolume();
        float GetSfxVolume();
    }
}
