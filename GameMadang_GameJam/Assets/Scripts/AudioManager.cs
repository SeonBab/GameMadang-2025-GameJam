using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Pool;

public class AudioManager : MonoBehaviour
{
    [Header("Mixer & Groups")] [SerializeField]
    private AudioMixer mixer;

    [SerializeField] private AudioMixerGroup bgmGroup;
    [SerializeField] private AudioMixerGroup sfxGroup;

    [Header("BGM Clips")] [SerializeField] private AudioClip stage1BGM;

    [SerializeField] private AudioClip stage2BGM;
    [SerializeField] private AudioClip stage3BGM;

    [Header("BGM Source")] [SerializeField]
    private AudioSource bgmSource;

    [Header("SFX Pool")] [SerializeField] private int poolCapacity = 10;

    [SerializeField] private int poolMaxSize = 20;

    [Header("2D World-Pan Settings")] [Tooltip("월드 X 위치 기준으로 좌/우 팬을 적용할지 여부")] [SerializeField]
    private bool enableWorldPanning = true;

    [Tooltip("카메라(리스너)에서 좌/우로 이 거리만큼 떨어지면 최대 팬(-1~1)에 도달")] [SerializeField]
    private float panWidthWorldUnits = 10f;

    [Tooltip("이 거리에서 볼륨 감쇠가 0이 됨")] [SerializeField]
    private float maxHearingDistance = 15f;

    [Tooltip("거리 t(0~1)에 따른 볼륨 감쇠 곡선 (0=가까움, 1=최대거리)")] [SerializeField]
    private AnimationCurve distanceAttenuation =
        AnimationCurve.Linear(0f, 1f, 1f, 0f);

    private readonly HashSet<AudioSource> activeSfx = new();

    private ObjectPool<AudioSource> sfxPool;

    public static AudioManager Instance { get; private set; }

    #region Unity

    private void Awake()
    {
        if (!Instance)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        InitSfxPool();
    }

    #endregion

    #region Volume API

    public void SetBGMVolume(float linear)
    {
        mixer.SetFloat("BGM", Linear01ToDb(linear));
    }

    public void SetSfxVolume(float linear)
    {
        mixer.SetFloat("SFX", Linear01ToDb(linear));
    }

    private float Linear01ToDb(float linear)
    {
        return linear <= 0.0001f ? -80f : Mathf.Log10(Mathf.Clamp01(linear)) * 20f;
    }

    #endregion

    #region BGM API

    public void PlayBGM(AudioClip clip, float volume = 1f, bool loop = true,
        float fadeSeconds = 0f)
    {
        if (!clip) return;
        if (bgmSource.clip == clip && bgmSource.isPlaying) return;

        if (fadeSeconds > 0f && bgmSource.isPlaying)
        {
            StartCoroutine(Co_CrossfadeBGM(clip, volume, loop, fadeSeconds));
            return;
        }

        bgmSource.outputAudioMixerGroup = bgmGroup;
        bgmSource.clip = clip;
        bgmSource.loop = loop;
        bgmSource.volume = volume;
        bgmSource.Play();
    }

    public void StopBGM(float fadeOutSeconds = 0f)
    {
        if (fadeOutSeconds <= 0f)
            bgmSource.Stop();
        else
            StartCoroutine(Co_FadeOutAndStop(bgmSource, fadeOutSeconds));
    }

    [ContextMenu("Play Stage 1 BGM")]
    public void PlayStage1BGM(float fadeSeconds = 0.3f)
    {
        PlayBGM(stage1BGM, 1f, true, fadeSeconds);
    }

    [ContextMenu("Play Stage 2 BGM")]
    public void PlayStage2BGM(float fadeSeconds = 0.3f)
    {
        PlayBGM(stage2BGM, 1f, true, fadeSeconds);
    }

    [ContextMenu("Play Stage 3 BGM")]
    public void PlayStage3BGM(float fadeSeconds = 0.3f)
    {
        PlayBGM(stage3BGM, 1f, true, fadeSeconds);
    }

    private IEnumerator Co_CrossfadeBGM(AudioClip next, float vol, bool loop, float dur)
    {
        // fade out
        var t = 0f;
        var start = bgmSource.volume;
        while (t < dur * 0.5f)
        {
            t += Time.unscaledDeltaTime;
            bgmSource.volume = Mathf.Lerp(start, 0f, t / (dur * 0.5f));
            yield return null;
        }

        // swap
        bgmSource.Stop();
        bgmSource.clip = next;
        bgmSource.loop = loop;
        bgmSource.volume = 0f;
        bgmSource.Play();

        // fade in
        t = 0f;
        while (t < dur * 0.5f)
        {
            t += Time.unscaledDeltaTime;
            bgmSource.volume = Mathf.Lerp(0f, vol, t / (dur * 0.5f));
            yield return null;
        }

        bgmSource.volume = vol;
    }

    private IEnumerator Co_FadeOutAndStop(AudioSource src, float dur)
    {
        var t = 0f;
        var start = src.volume;
        while (t < dur)
        {
            t += Time.unscaledDeltaTime;
            src.volume = Mathf.Lerp(start, 0f, t / dur);
            yield return null;
        }

        src.Stop();
        src.volume = start;
    }

    #endregion

    #region SFX Pool & API

    private void InitSfxPool()
    {
        sfxPool = new ObjectPool<AudioSource>(
            () =>
            {
                var go = new GameObject("PooledSFX2D");
                go.transform.SetParent(transform);
                var src = go.AddComponent<AudioSource>();
                src.outputAudioMixerGroup = sfxGroup;

                // === 2D 전용 설정 ===
                src.spatialBlend = 0f; // 완전 2D
                src.panStereo = 0f; // 기본 중앙
                src.playOnAwake = false;
                src.loop = false;

                go.SetActive(false);
                return src;
            },
            src =>
            {
                src.gameObject.SetActive(true);
                activeSfx.Add(src);
            },
            src =>
            {
                src.Stop();
                src.clip = null;
                src.panStereo = 0f;
                src.pitch = 1f;
                src.volume = 1f;
                src.gameObject.SetActive(false);
                activeSfx.Remove(src);
            },
            src => Destroy(src.gameObject),
            false,
            poolCapacity,
            poolMaxSize
        );
    }

    /// <summary>
    ///     UI 등 비위치(완전 2D) 효과음
    /// </summary>
    public void PlaySfx2D(AudioClip clip, float volume = 1f, float pitch = 1f)
    {
        if (!clip) return;
        var src = sfxPool.Get();
        src.clip = clip;
        src.volume = volume;
        src.pitch = pitch;
        src.panStereo = 0f; // 중앙
        src.Play();
        StartCoroutine(ReleaseWhenDone(src));
    }

    /// <summary>
    ///     월드 좌표 기반 2D 효과음(좌우 팬 + 거리 감쇠만 적용)
    /// </summary>
    public void PlaySfxAt2D(AudioClip clip, Vector3 worldPos, float baseVolume = 1f,
        float pitch = 1f)
    {
        if (!clip) return;
        var src = sfxPool.Get();
        src.clip = clip;
        src.pitch = pitch;

        // 리스너는 카메라 기준
        var cam = Camera.main;
        if (cam != null && enableWorldPanning)
        {
            var listenerPos = cam.transform.position;
            var dx = worldPos.x - listenerPos.x;
            var pan = Mathf.Clamp(dx / Mathf.Max(0.001f, panWidthWorldUnits), -1f, 1f);
            src.panStereo = pan;

            // 2D 거리(XY)만으로 감쇠
            var dist = Vector2.Distance(
                new Vector2(worldPos.x, worldPos.y),
                new Vector2(listenerPos.x, listenerPos.y));

            var t = Mathf.Clamp01(dist / Mathf.Max(0.001f, maxHearingDistance));
            var atten = distanceAttenuation.Evaluate(t);
            src.volume = baseVolume * atten;
        }
        else
        {
            src.panStereo = 0f;
            src.volume = baseVolume;
        }

        // 참고: 2D 오디오는 transform 위치가 의미적으론 크지 않지만,
        // 디버깅/정렬용으로 보관
        src.transform.position = worldPos;

        src.Play();
        StartCoroutine(ReleaseWhenDone(src));
    }

    /// <summary>
    ///     현재 재생 중인 모든 SFX를 정지
    /// </summary>
    public void StopAllSfx()
    {
        // 복사본으로 순회 (Release 중에 Set이 바뀌는 걸 방지)
        var list = ListBuffer;
        list.Clear();
        list.AddRange(activeSfx);

        foreach (var src in list) sfxPool.Release(src);
    }

    private static readonly List<AudioSource> ListBuffer = new(64);

    private IEnumerator ReleaseWhenDone(AudioSource src)
    {
        // clip 길이를 이용해 fail-safe (희귀 케이스에서 isPlaying이 즉시 false가 될 수 있음)
        var timeout = (src.clip ? src.clip.length / Mathf.Max(0.01f, src.pitch) : 0.5f) + 0.1f;
        var t = 0f;
        while (src && src.isPlaying && t < timeout)
        {
            t += Time.unscaledDeltaTime;
            yield return null;
        }

        if (src) sfxPool.Release(src);
    }

    #endregion
}