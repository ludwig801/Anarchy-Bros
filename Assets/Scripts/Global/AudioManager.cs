using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour
{
    public AudioSource[] Audios;
    public AudioClip Menu, Deploy, Game;

    GameManager GameManager
    {
        get
        {
            if (_gameManager == null)
            {
                _gameManager = GameManager.Instance;
            }
            return _gameManager;
        }
    }

    GlobalManager _globalManager;
    GameManager _gameManager;
    int _currentAudioSource;
    bool _isFading;

    void Start()
    {
        _globalManager = GlobalManager.Instance;
        _isFading = false;
        _currentAudioSource = 0;
    }

    void Update()
    {
        if (!_isFading)
        {
            Audios[_currentAudioSource].volume = _globalManager.MusicValue;
        }

        if (_globalManager.InGame && GameManager != null)
        {
            switch (GameManager.CurrentState)
            {
                case GameStates.Pause:
                    //StartCoroutine(PlaySound(Deploy));
                    break;

                case GameStates.Place:
                    StartCoroutine(PlaySound(Deploy));
                    break;

                case GameStates.Play:
                    StartCoroutine(PlaySound(Game));
                    break;
            }
        }
        else
        {
            StartCoroutine(PlaySound(Menu));
        }
    }

    IEnumerator PlaySound(AudioClip clip)
    {
        if (_isFading) yield break;

        if (Audios[_currentAudioSource].clip == clip && Audios[_currentAudioSource].isPlaying) yield break;

        _isFading = true;

        int nextAudioSource = (_currentAudioSource + 1) % Audios.Length;
        Audios[nextAudioSource].volume = 0;
        Audios[nextAudioSource].clip = clip;
        Audios[nextAudioSource].Play();

        while (Audios[_currentAudioSource].volume > 0.01f && Audios[nextAudioSource].volume < 0.99f)
        {
            Audios[_currentAudioSource].volume = Mathf.Lerp(Audios[_currentAudioSource].volume, 0f, Time.unscaledDeltaTime * 2.5f);
            Audios[nextAudioSource].volume = Mathf.Lerp(Audios[nextAudioSource].volume, _globalManager.MusicValue, Time.unscaledDeltaTime * 2.5f);

            yield return null;
        }

        Audios[_currentAudioSource].volume = 0;
        Audios[_currentAudioSource].Stop();

        Audios[nextAudioSource].volume = 1f;

        _currentAudioSource = nextAudioSource;

        _isFading = false;
    }
}
