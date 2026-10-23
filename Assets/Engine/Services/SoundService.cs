#region

using UnityEngine;

#endregion

namespace MergeGridGame.Services
{
    public class SoundService : ISoundService
    {
        private AudioSource _musicSource;
        private AudioSource _sfxSource;

        private float _musicVolume = 0.3f;
        private float _sfxVolume = 0.3f;

        private float _pitchMin = 0.9f;
        private float _pitchMax = 1.1f;

        public SoundService(Transform audioRoot)
        {
            _musicSource = new GameObject("MusicSource").AddComponent<AudioSource>();
            _musicSource.transform.SetParent(audioRoot);
            _musicSource.loop = true;

            _sfxSource = new GameObject("SfxSource").AddComponent<AudioSource>();
            _sfxSource.transform.SetParent(audioRoot);
        }

        public void PlayMusic(AudioClip clip, bool loop = true)
        {
            if (_musicSource.clip == clip && _musicSource.isPlaying) 
            {
                return;
            }

            _musicSource.clip = clip;
            _musicSource.loop = loop;
            _musicSource.volume = _musicVolume;
            _musicSource.Play();
        }

        public void StopMusic()
        {
            _musicSource.Stop();
            _musicSource.clip = null;
        }

        public void PlaySound(AudioClip clip)
        {
            _sfxSource.pitch = Random.Range(_pitchMin, _pitchMax);
            _sfxSource.PlayOneShot(clip, _sfxVolume);
        }

        public void SetMusicVolume(float volume)
        {
            _musicVolume = Mathf.Clamp01(volume);
            _musicSource.volume = _musicVolume;
        }

        public void SetSfxVolume(float volume)
        {
            _sfxVolume = Mathf.Clamp01(volume);
        }

        public float GetMusicVolume() => _musicVolume;
        public float GetSfxVolume() => _sfxVolume;
    }
}
