
using UnityEngine;
using UnityEngine.SceneManagement;

public class BaBoomAudioManager : MonoBehaviour
{
    public static BaBoomAudioManager Instance
    {
        get;
        private set;
    }

    // ====================================
    // AUDIO SOURCES
    // ====================================

    [Header("Audio Sources")]
    public AudioSource backgroundSource;
    public AudioSource effectsSource;

    // ====================================
    // MUSIC CLIPS
    // ====================================

    [Header("Scene Music")]

    // เพลง Main Menu (ช่องใหม่)
    public AudioClip mainMenuClip;

    // เพลง Select Level
    public AudioClip levelSelectClip;

    // เพลง Gameplay
    // เก็บชื่อ backgroundClip เดิม
    // เพื่อรักษาไฟล์ที่เชื่อมไว้ใน Inspector
    [InspectorName("Gameplay Music Clip")]
    public AudioClip backgroundClip;

    // ====================================
    // SOUND EFFECTS
    // ====================================

    [Header("Sound Effects")]
    public AudioClip explosionClip;
    public AudioClip scoreClip;
    public AudioClip countdownClip;
    public AudioClip winClip;
    public AudioClip loseClip;

    // ====================================
    // MUSIC VOLUME
    // ====================================

    [Header("Music Volume")]

    [Range(0f, 1f)]
    public float mainMenuVolume = 0.3f;

    [Range(0f, 1f)]
    public float selectMusicVolume = 0.35f;

    [Range(0f, 1f)]
    public float backgroundVolume = 0.25f;

    // ====================================
    // SOUND EFFECT VOLUME
    // ====================================

    [Header("Sound Effect Volume")]

    [Range(0f, 1f)]
    public float explosionVolume = 0.55f;

    [Range(0f, 1f)]
    public float scoreVolume = 0.25f;

    [Range(0f, 1f)]
    public float countdownVolume = 0.5f;

    [Range(0f, 1f)]
    public float winVolume = 0.75f;

    [Range(0f, 1f)]
    public float loseVolume = 0.7f;

    // ====================================
    // SOUND COOLDOWNS
    // ====================================

    [Header("Sound Cooldowns")]

    [Min(0f)]
    public float explosionInterval = 0.1f;

    [Min(0f)]
    public float scoreInterval = 0.15f;

    private float lastExplosionTime = -100f;
    private float lastScoreTime = -100f;

    private int lastSceneHandle = -1;

    // ====================================
    // AWAKE
    // ====================================

    private void Awake()
    {
        // ป้องกัน AudioManager ซ้ำ
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        // ตรวจ AudioSource
        if (backgroundSource == null ||
            effectsSource == null ||
            backgroundSource == effectsSource)
        {
            Debug.LogError(
                "BaBoomAudioManager: " +
                "Please assign two different AudioSources.",
                this
            );

            enabled = false;
            return;
        }

        Instance = this;

        // เก็บ AudioManager เมื่อเปลี่ยน Scene
        DontDestroyOnLoad(gameObject);

        // ตั้งค่า Music Source
        backgroundSource.playOnAwake = false;
        backgroundSource.loop = true;
        backgroundSource.spatialBlend = 0f;

        // ตั้งค่า Effects Source
        effectsSource.playOnAwake = false;
        effectsSource.loop = false;
        effectsSource.spatialBlend = 0f;
        effectsSource.volume = 1f;

        // ติดตามการเปลี่ยน Scene
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void Start()
    {
        if (Instance != this)
            return;

        UpdateSceneAudio(
            SceneManager.GetActiveScene()
        );
    }

    // ====================================
    // SCENE CHANGE
    // ====================================

    private void OnSceneLoaded(
        Scene scene,
        LoadSceneMode mode)
    {
        UpdateSceneAudio(scene);
    }

    private void UpdateSceneAudio(Scene scene)
    {
        if (Instance != this)
            return;

        if (lastSceneHandle == scene.handle)
            return;

        lastSceneHandle = scene.handle;

        string sceneName = scene.name;

        // หยุดเอฟเฟกต์ค้างจาก Scene ก่อนหน้า
        effectsSource.Stop();

        // MAIN MENU
        if (sceneName == "MainMenu")
        {
            PlayMusic(
                mainMenuClip,
                mainMenuVolume
            );
        }

        // SELECT LEVEL
        else if (sceneName == "LevelSelect")
        {
            PlayMusic(
                levelSelectClip,
                selectMusicVolume
            );
        }

        // GAMEPLAY 1, 2, 3
        else if (
            sceneName == "GamePlay 1" ||
            sceneName == "GamePlay 2" ||
            sceneName == "GamePlay 3")
        {
            PlayMusic(
                backgroundClip,
                backgroundVolume
            );
        }

        // WIN
        else if (sceneName == "Win")
        {
            StopBackground();
            PlayWin();
        }

        // LOSE
        else if (sceneName == "Lose")
        {
            StopBackground();
            PlayLose();
        }

        // อื่น ๆ
        else
        {
            StopBackground();
        }
    }

    // ====================================
    // MUSIC CONTROL
    // ====================================

    private void PlayMusic(
        AudioClip clip,
        float volume)
    {
        if (backgroundSource == null)
            return;

        // ถ้าไม่มีเพลงให้หยุดเสียง
        if (clip == null)
        {
            StopBackground();
            return;
        }

        backgroundSource.volume =
            Mathf.Clamp01(volume);

        backgroundSource.loop = true;

        // ถ้าเปลี่ยนเพลง ให้หยุดเพลงเดิมก่อน
        if (backgroundSource.clip != clip)
        {
            backgroundSource.Stop();
            backgroundSource.clip = clip;
        }

        // เล่นเพลงใหม่
        if (!backgroundSource.isPlaying)
        {
            backgroundSource.Play();
        }
    }

    // ใช้กับระบบเสียงเดิม
    public void PlayBackground()
    {
        PlayMusic(
            backgroundClip,
            backgroundVolume
        );
    }

    public void StopBackground()
    {
        if (backgroundSource != null)
        {
            backgroundSource.Stop();
        }
    }

    // ====================================
    // EXPLOSION SOUND
    // ====================================

    public void PlayExplosion()
    {
        float now = Time.unscaledTime;

        if (now - lastExplosionTime <
            explosionInterval)
        {
            return;
        }

        lastExplosionTime = now;

        PlayEffect(
            explosionClip,
            explosionVolume
        );
    }

    // ====================================
    // SCORE SOUND
    // ====================================

    public void PlayScore()
    {
        float now = Time.unscaledTime;

        if (now - lastScoreTime <
            scoreInterval)
        {
            return;
        }

        lastScoreTime = now;

        PlayEffect(
            scoreClip,
            scoreVolume
        );
    }

    // ====================================
    // COUNTDOWN SOUND
    // ====================================

    public void PlayCountdownTick()
    {
        PlayEffect(
            countdownClip,
            countdownVolume
        );
    }

    // ====================================
    // WIN / LOSE SOUND
    // ====================================

    public void PlayWin()
    {
        PlayEffect(
            winClip,
            winVolume
        );
    }

    public void PlayLose()
    {
        PlayEffect(
            loseClip,
            loseVolume
        );
    }

    // ====================================
    // COMMON SOUND EFFECT PLAYER
    // ====================================

    private void PlayEffect(
        AudioClip clip,
        float volume)
    {
        if (effectsSource == null ||
            clip == null)
        {
            return;
        }

        effectsSource.PlayOneShot(
            clip,
            Mathf.Clamp01(volume)
        );
    }

    // ====================================
    // CLEANUP
    // ====================================

    private void OnDestroy()
    {
        if (Instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            Instance = null;
        }
    }
}
