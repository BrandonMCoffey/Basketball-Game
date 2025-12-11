using Sirenix.OdinInspector;
using UnityEngine;

namespace CoffeyUtils.Sound
{
    public class SoundManager : MonoBehaviour
    {
        private const string DefaultManagerName = "Sound Manager";
        private const string DefaultMusicPoolName = "Music Manager";
        public const string DefaultMusicPlayerName = "Music Player";
        private const string DefaultSfxPoolName = "SFX Manager";
        public const string DefaultSfxPlayerName = "SFX Player";
        
        [SerializeField] private bool _scenePersistent;
        [SerializeField] private SoundLibrarySO _soundLibrary;
        public SoundLibrarySO SoundLibrary => _soundLibrary;
        [SerializeField, ReadOnly] private MusicPool _musicPool;
        [SerializeField, ReadOnly] private SfxPool _sfxPool;

        public static MusicPool Music => Instance._musicPool;
        public static SfxPool Sfx => Instance._sfxPool;

        #region Singleton

        private static SoundManager _instance;
        public static SoundManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<SoundManager>();
                    if (_instance == null)
                    {
                        _instance = new GameObject(DefaultManagerName, typeof(SoundManager)).GetComponent<SoundManager>();
                        _instance.CreateSoundSystemComponents();
                    }
                }
                return _instance;
            }
        }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            _instance = this;
            CreateSoundSystemComponents();
            if (_scenePersistent)
            {
                transform.SetParent(null);
                DontDestroyOnLoad(gameObject);
            }
        }
        
        #endregion

        [Button]
        private void CreateSoundSystemComponents()
        {
            if (_musicPool == null)
            {
                _musicPool = GetComponentInChildren<MusicPool>();
                if (_musicPool == null)
                {
                    _musicPool = new GameObject(DefaultMusicPoolName, typeof(MusicPool)).GetComponent<MusicPool>();
                    _musicPool.transform.SetParent(transform);
                }
            }
            _musicPool.BuildInitialPool();
            if (_sfxPool == null)
            {
                _sfxPool = GetComponentInChildren<SfxPool>();
                if (_sfxPool == null)
                {
                    _sfxPool = new GameObject(DefaultSfxPoolName, typeof(SfxPool)).GetComponent<SfxPool>();
                    _sfxPool.transform.SetParent(transform);
                }
            }
            _sfxPool.BuildInitialPool();
        }


        public static SfxPlayer PlaySfx(AudioClip clip) => Sfx.Play(clip);
        public static SfxPlayer PlaySfx(Sfx2dProp properties2d) => Sfx.Play(properties2d);
        // Redundant way of playing sfx if you prefer
        public static SfxPlayer PlaySfx(SfxBase sfx) => sfx.PlayGetPlayer();
        public static SfxPlayer PlaySfx(SfxReference sfx) => sfx.Play();

        public static void StopAllSfx() => Sfx.StopAll();

        public static MusicPlayer PlayMusicNow(MusicTrack track) => Music.Play(track);
        public static MusicPlayer PlayMusicNow(MusicTrack track, float delay) => Music.Play(track);
        public static void QueueMusic(MusicTrack track) => Music.QueueTrack(track);
        public static void StopAllMusic() => Music.StopAll();


        // Sai's Weird Code...
        public static SfxPlayer PlaySfx(SFXEventsEnum sfxEvent)
        {
            if (Instance.SoundLibrary == null) return null;
            
            var sfxClip = Instance.SoundLibrary.GetSFXClip(sfxEvent);
            if (sfxClip == null) return null;
            
            return sfxClip.PlayGetPlayer();
        }
        public static MusicPlayer PlayMusicNow(MusicTracksEnum musicTrack) => PlayMusicNow(Instance.SoundLibrary.GetMusicTrack(musicTrack));
        public static MusicPlayer PlayMusicNow(MusicTracksEnum musicTrack, float delay) => PlayMusicNow(Instance.SoundLibrary.GetMusicTrack(musicTrack), delay);
    }
}