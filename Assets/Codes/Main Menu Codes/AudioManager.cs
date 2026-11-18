using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicPlayer : MonoBehaviour
{
    private static MusicPlayer instance;
    private AudioSource musicSource;

    [SerializeField] private string[] musicScenes = { "MainMenu", "LevelSelection", "Stage2Scene", "Stage3Scene" };
    [SerializeField] private AudioClip stage2MusicClip;
    [SerializeField] private AudioClip stage3MusicClip;
    private AudioClip defaultMusicClip;
    private string currentMusicScene;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        musicSource = GetComponent<AudioSource>();
        if (musicSource == null)
        {
            Debug.LogError("MusicPlayer requires an AudioSource component.", this);
            return;
        }

        defaultMusicClip = musicSource.clip;
        musicSource.ignoreListenerPause = false;
        DontDestroyOnLoad(gameObject);
        currentMusicScene = SceneManager.GetActiveScene().name;
        SceneManager.sceneLoaded += OnSceneLoaded;
        ApplyMusicForScene(currentMusicScene);
    }

    public static void PauseMusic()
    {
        if (instance != null && instance.musicSource != null)
        {
            instance.musicSource.Pause();
        }
    }

    public static void ResumeMusic()
    {
        if (instance != null && instance.musicSource != null)
        {
            instance.musicSource.UnPause();
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        bool restartingStage = scene.name == currentMusicScene
            && (scene.name == "Stage2Scene" || scene.name == "Stage3Scene");
        currentMusicScene = scene.name;
        ApplyMusicForScene(scene.name, restartingStage);
    }

    private void ApplyMusicForScene(string sceneName, bool restart = false)
    {
        if (System.Array.IndexOf(musicScenes, sceneName) < 0)
        {
            musicSource.Stop();
            return;
        }

        AudioClip clipForScene = sceneName == "Stage2Scene"
            ? stage2MusicClip
            : sceneName == "Stage3Scene"
                ? stage3MusicClip
                : defaultMusicClip;

        if (clipForScene == null)
        {
            musicSource.Stop();
            return;
        }

        if (musicSource.clip != clipForScene || restart)
        {
            musicSource.Stop();
            musicSource.clip = clipForScene;
            musicSource.loop = true;
            musicSource.time = 0f;
            musicSource.Play();
        }
        else if (!musicSource.isPlaying)
        {
            musicSource.Play();
        }
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        if (instance == this) instance = null;
    }
}