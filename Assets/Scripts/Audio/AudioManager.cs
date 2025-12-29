using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Database")]
    [SerializeField] private AudioDatabase database;

    [Header("Pool Settings")]
    [SerializeField] private int poolSize = 50;

    private Queue<AudioEvent> eventQueue = new Queue<AudioEvent>();
    private List<AudioSource> sourcePool = new List<AudioSource>();

    // BGM 전용 오디오 소스
    private AudioSource bgmSource;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
        CreatePool();
        CreateBGMSource();
    }

    /// <summary>
    /// 초기 AudioSource 풀 생성
    /// </summary>
    private void CreatePool()
    {
        for (int i = 0; i < poolSize; i++)
        {
            var obj = new GameObject("AudioSource_" + i);
            obj.transform.parent = transform;

            AudioSource source = obj.AddComponent<AudioSource>();
            source.playOnAwake = false;
            source.spatialBlend = 1f;      // default: 3D

            sourcePool.Add(source);
        }
    }

    /// <summary>
    /// 외부에서 사운드 요청
    /// </summary>
    public void EnqueueEvent(AudioEvent audioEvent)
    {
        eventQueue.Enqueue(audioEvent);
    }

    private void Update()
    {
        while (eventQueue.Count > 0)
        {
            var e = eventQueue.Dequeue();
            PlayInternal(e);
        }
    }

    /// <summary>
    /// 실제 재생 처리
    /// </summary>
    private void PlayInternal(AudioEvent e)
    {
        AudioData data = database.Get(e.soundId);
        if (data == null)
        {
            Debug.LogWarning($"Audio ID '{e.soundId}' not found in database.");
            return;
        }

        AudioSource src = GetAvailableSource();
        if (src == null) return;

        src.transform.position = e.position;

        src.clip = data.clip;
        src.outputAudioMixerGroup = data.mixerGroup;

        src.volume = data.defaultVolume * e.volume;
        src.spatialBlend = data.is3D ? 1f : 0f;

        src.Play();
    }

    /// <summary>
    /// 사용 가능한 AudioSource 반환
    /// </summary>
    private AudioSource GetAvailableSource()
    {
        foreach (var src in sourcePool)
        {
            if (!src.isPlaying)
                return src;
        }

        // 모든 소스가 재생 중이면 우선순위 낮은 소스 끊기
        return sourcePool[0];
    }


    // ───────────────────────────────────────────────
    //                 BGM 기능 추가
    // ───────────────────────────────────────────────

    /// <summary>
    /// BGM 전용 AudioSource 생성
    /// </summary>
    private void CreateBGMSource()
    {
        var obj = new GameObject("BGMSource");
        obj.transform.parent = transform;

        bgmSource = obj.AddComponent<AudioSource>();
        bgmSource.playOnAwake = false;
        bgmSource.loop = true;               // 기본 Loop
        bgmSource.spatialBlend = 0f;         // BGM은 2D
    }

    /// <summary>
    /// BGM 재생
    /// </summary>
    public void PlayBGM(string bgmId, float volume = 1f)
    {
        AudioData data = database.Get(bgmId);
        if (data == null)
        {
            Debug.LogWarning($"BGM ID '{bgmId}' not found in database.");
            return;
        }

        bgmSource.clip = data.clip;
        bgmSource.outputAudioMixerGroup = data.mixerGroup;
        bgmSource.volume = data.defaultVolume * volume;
        bgmSource.loop = true;  // 항상 loop
        bgmSource.spatialBlend = 0f;

        bgmSource.Play();
    }

    /// <summary>
    /// BGM 정지
    /// </summary>
    public void StopBGM()
    {
        if (bgmSource.isPlaying)
            bgmSource.Stop();
    }

    /// <summary>
    /// BGM 일시정지
    /// </summary>
    public void PauseBGM()
    {
        if (bgmSource.isPlaying)
            bgmSource.Pause();
    }

    /// <summary>
    /// BGM 재개
    /// </summary>
    public void ResumeBGM()
    {
        if (!bgmSource.isPlaying)
            bgmSource.UnPause();
    }
}


