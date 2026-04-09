using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    private const string MasterVolumeKey = "Audio.MasterVolume";
    private const string SfxVolumeKey = "Audio.SfxVolume";
    private const string MusicVolumeKey = "Audio.MusicVolume";

    public static AudioManager Instance { get; private set; }

    [Header("Library")]
    [SerializeField] private AudioCueLibrary cueLibrary;

    [Header("Music")]
    [SerializeField] private AudioSource musicSourceA;
    [SerializeField] private AudioSource musicSourceB;
    [SerializeField] [Min(0f)] private float musicFadeDuration = 0.75f;
    [SerializeField] private bool playSceneMusicOnStart = true;
    [SerializeField] private List<string> menuSceneNames = new List<string> { "Menu" };
    [SerializeField] private List<string> gameplaySceneNames = new List<string> { "MainGamePlay", "SampleScene" };

    [Header("2D Audio")]
    [SerializeField] private AudioSource uiSource;
    [SerializeField] private AudioSource sfxSourceTemplate;
    [SerializeField] [Min(1)] private int initialSfxPoolSize = 8;

    [Header("Volume")]
    [SerializeField] [Range(0f, 1f)] private float masterVolume = 1f;
    [SerializeField] [Range(0f, 1f)] private float sfxVolume = 1f;
    [SerializeField] [Range(0f, 1f)] private float musicVolume = 1f;

    private readonly List<AudioSource> sfxPool = new List<AudioSource>();
    private AudioSource activeMusicSource;
    private AudioSource inactiveMusicSource;
    private Coroutine musicFadeRoutine;
    private AudioMusicState currentMusicState = AudioMusicState.None;

    public AudioCueLibrary CueLibrary => cueLibrary;
    public float MasterVolume => masterVolume;
    public float SfxVolume => sfxVolume;
    public float MusicVolume => musicVolume;
    public AudioMusicState CurrentMusicState => currentMusicState;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        EnsureCoreSources();
        LoadSavedVolumes();
        ApplyVolumes();
        WarmupSfxPool();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void Start()
    {
        if (playSceneMusicOnStart)
        {
            PlaySceneMusic(SceneManager.GetActiveScene().name, true);
        }
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void PlaySFX(AudioClip clip)
    {
        PlaySfxInternal(clip, Vector3.zero, false, 1f, 1f, null, 0f);
    }

    public void PlayMusic(AudioClip clip)
    {
        PlayMusic(clip, null, false, 1f);
    }

    public void PlayUI(AudioClip clip)
    {
        if (clip == null || uiSource == null)
        {
            return;
        }

        uiSource.outputAudioMixerGroup = null;
        uiSource.pitch = 1f;
        uiSource.spatialBlend = 0f;
        uiSource.PlayOneShot(clip, masterVolume * sfxVolume);
    }

    public void PlaySFX(AudioCue cue)
    {
        if (cue == null)
        {
            return;
        }

        PlaySfxInternal(cue.GetRandomClip(), Vector3.zero, false, cue.Volume, cue.GetRandomPitch(), cue.OutputGroup, 0f);
    }

    public void PlaySFXAtPoint(AudioCue cue, Vector3 worldPosition)
    {
        if (cue == null)
        {
            return;
        }

        PlaySfxInternal(cue.GetRandomClip(), worldPosition, true, cue.Volume, cue.GetRandomPitch(), cue.OutputGroup, cue.SpatialBlend);
    }

    public void PlayUI(AudioCue cue)
    {
        if (cue == null || uiSource == null)
        {
            return;
        }

        AudioClip clip = cue.GetRandomClip();
        if (clip == null)
        {
            return;
        }

        uiSource.outputAudioMixerGroup = cue.OutputGroup;
        uiSource.pitch = cue.GetRandomPitch();
        uiSource.spatialBlend = 0f;
        uiSource.PlayOneShot(clip, masterVolume * sfxVolume * cue.Volume);
    }

    public void PlayMusic(AudioCue cue, bool restartIfSame = false)
    {
        if (cue == null)
        {
            return;
        }

        PlayMusic(cue.GetRandomClip(), cue.OutputGroup, restartIfSame, cue.Volume);
    }

    public void PlayMusicState(AudioMusicState musicState, bool restartIfSame = false)
    {
        if (cueLibrary == null)
        {
            return;
        }

        AudioCue cue = null;
        switch (musicState)
        {
            case AudioMusicState.MainMenu:
                cue = cueLibrary.MainMenuMusic;
                break;
            case AudioMusicState.Gameplay:
                cue = cueLibrary.GameplayMusic;
                break;
            case AudioMusicState.Boss:
                cue = cueLibrary.BossMusic;
                break;
        }

        if (cue == null)
        {
            return;
        }

        currentMusicState = musicState;
        PlayMusic(cue, restartIfSame);
    }

    public void SetMasterVolume(float value, bool save = true)
    {
        masterVolume = Mathf.Clamp01(value);
        ApplyVolumes();
        SaveVolume(save, MasterVolumeKey, masterVolume);
    }

    public void SetSfxVolume(float value, bool save = true)
    {
        sfxVolume = Mathf.Clamp01(value);
        ApplyVolumes();
        SaveVolume(save, SfxVolumeKey, sfxVolume);
    }

    public void SetMusicVolume(float value, bool save = true)
    {
        musicVolume = Mathf.Clamp01(value);
        ApplyVolumes();
        SaveVolume(save, MusicVolumeKey, musicVolume);
    }

    public void PlayButtonClick()
    {
        PlayUI(cueLibrary != null ? cueLibrary.ButtonClick : null);
    }

    public void PlayButtonHover()
    {
        PlayUI(cueLibrary != null ? cueLibrary.ButtonHover : null);
    }

    public void PlayMenuOpen()
    {
        PlayUI(cueLibrary != null ? cueLibrary.MenuOpen : null);
    }

    public void PlayMenuClose()
    {
        PlayUI(cueLibrary != null ? cueLibrary.MenuClose : null);
    }

    private void PlaySceneMusic(string sceneName, bool forceRestart)
    {
        if (menuSceneNames.Contains(sceneName))
        {
            PlayMusicState(AudioMusicState.MainMenu, forceRestart);
            return;
        }

        if (gameplaySceneNames.Contains(sceneName))
        {
            PlayMusicState(AudioMusicState.Gameplay, forceRestart);
        }
    }

    private void PlayMusic(AudioClip clip, AudioMixerGroup mixerGroup, bool restartIfSame, float clipVolume)
    {
        if (clip == null || activeMusicSource == null || inactiveMusicSource == null)
        {
            return;
        }

        if (!restartIfSame && activeMusicSource.clip == clip && activeMusicSource.isPlaying)
        {
            return;
        }

        inactiveMusicSource.clip = clip;
        inactiveMusicSource.loop = true;
        inactiveMusicSource.outputAudioMixerGroup = mixerGroup;
        inactiveMusicSource.pitch = 1f;
        inactiveMusicSource.volume = 0f;
        inactiveMusicSource.spatialBlend = 0f;
        inactiveMusicSource.Play();

        if (musicFadeRoutine != null)
        {
            StopCoroutine(musicFadeRoutine);
        }

        musicFadeRoutine = StartCoroutine(CrossFadeMusic(activeMusicSource, inactiveMusicSource, clipVolume));
    }

    private void PlaySfxInternal(
        AudioClip clip,
        Vector3 position,
        bool useWorldPosition,
        float clipVolume,
        float pitch,
        AudioMixerGroup mixerGroup,
        float spatialBlend)
    {
        if (clip == null)
        {
            return;
        }

        AudioSource source = GetAvailableSfxSource();
        if (source == null)
        {
            return;
        }

        source.transform.position = useWorldPosition ? position : transform.position;
        source.outputAudioMixerGroup = mixerGroup;
        source.pitch = pitch;
        source.spatialBlend = useWorldPosition ? spatialBlend : 0f;
        source.loop = false;
        source.clip = clip;
        source.volume = masterVolume * sfxVolume * clipVolume;
        source.Play();
    }

    private IEnumerator CrossFadeMusic(AudioSource from, AudioSource to, float clipVolume)
    {
        float duration = Mathf.Max(0.01f, musicFadeDuration);
        float elapsed = 0f;
        float fromStartVolume = from != null ? from.volume : 0f;
        float targetToVolume = masterVolume * musicVolume * clipVolume;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / duration);

            if (from != null)
            {
                from.volume = Mathf.Lerp(fromStartVolume, 0f, t);
            }

            if (to != null)
            {
                to.volume = Mathf.Lerp(0f, targetToVolume, t);
            }

            yield return null;
        }

        if (from != null)
        {
            from.Stop();
            from.clip = null;
            from.volume = 0f;
        }

        if (to != null)
        {
            to.volume = targetToVolume;
        }

        AudioSource previousActive = activeMusicSource;
        activeMusicSource = inactiveMusicSource;
        inactiveMusicSource = previousActive;
        musicFadeRoutine = null;
    }

    private void EnsureCoreSources()
    {
        if (musicSourceA == null)
        {
            musicSourceA = CreateChildSource("Music A");
        }

        if (musicSourceB == null)
        {
            musicSourceB = CreateChildSource("Music B");
        }

        if (uiSource == null)
        {
            uiSource = CreateChildSource("UI");
        }

        if (sfxSourceTemplate == null)
        {
            sfxSourceTemplate = CreateChildSource("SFX Template");
            sfxSourceTemplate.gameObject.SetActive(false);
        }

        ConfigureMusicSource(musicSourceA);
        ConfigureMusicSource(musicSourceB);
        ConfigureUiSource(uiSource);

        activeMusicSource = musicSourceA;
        inactiveMusicSource = musicSourceB;
    }

    private void WarmupSfxPool()
    {
        while (sfxPool.Count < initialSfxPoolSize)
        {
            sfxPool.Add(CreatePooledSfxSource());
        }
    }

    private void LoadSavedVolumes()
    {
        masterVolume = PlayerPrefs.GetFloat(MasterVolumeKey, masterVolume);
        sfxVolume = PlayerPrefs.GetFloat(SfxVolumeKey, sfxVolume);
        musicVolume = PlayerPrefs.GetFloat(MusicVolumeKey, musicVolume);
    }

    private void ApplyVolumes()
    {
        AudioListener.volume = masterVolume;

        if (uiSource != null)
        {
            uiSource.volume = sfxVolume;
        }

        if (activeMusicSource != null && activeMusicSource.isPlaying)
        {
            activeMusicSource.volume = masterVolume * musicVolume;
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (playSceneMusicOnStart)
        {
            PlaySceneMusic(scene.name, false);
        }
    }

    private AudioSource GetAvailableSfxSource()
    {
        for (int i = 0; i < sfxPool.Count; i++)
        {
            if (!sfxPool[i].isPlaying)
            {
                return sfxPool[i];
            }
        }

        AudioSource extraSource = CreatePooledSfxSource();
        sfxPool.Add(extraSource);
        return extraSource;
    }

    private AudioSource CreatePooledSfxSource()
    {
        AudioSource source = Instantiate(sfxSourceTemplate, transform);
        source.gameObject.SetActive(true);
        source.name = "SFX Source";
        source.playOnAwake = false;
        source.loop = false;
        return source;
    }

    private AudioSource CreateChildSource(string sourceName)
    {
        GameObject child = new GameObject(sourceName);
        child.transform.SetParent(transform, false);
        AudioSource source = child.AddComponent<AudioSource>();
        source.playOnAwake = false;
        return source;
    }

    private void SaveVolume(bool shouldSave, string key, float value)
    {
        if (!shouldSave)
        {
            return;
        }

        PlayerPrefs.SetFloat(key, value);
    }

    private static void ConfigureMusicSource(AudioSource source)
    {
        if (source == null)
        {
            return;
        }

        source.playOnAwake = false;
        source.loop = true;
        source.spatialBlend = 0f;
        source.volume = 0f;
    }

    private static void ConfigureUiSource(AudioSource source)
    {
        if (source == null)
        {
            return;
        }

        source.playOnAwake = false;
        source.loop = false;
        source.spatialBlend = 0f;
    }
}
