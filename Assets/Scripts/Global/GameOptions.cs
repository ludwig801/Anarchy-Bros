using UnityEngine;
using UnityEngine.UI;

public class GameOptions : MonoBehaviour
{
    public AudioSource MainAudioSource;
    public Text SoundEffectsVolumeLabel, MusicVolumeLabel;
    public Slider SoundEffectsSlider, MusicSlider;
    public MultiOption BodyCount, Difficulty;
    public int UpdatesPerSecond;

    void Start()
    {
        SoundEffectsVolumeLabel.text = "SFX";
        MusicVolumeLabel.text = "Music";
    }

    public void Save()
    {
        PlayerPrefs.SetInt("BodyCount", BodyCount.Value);
        PlayerPrefs.SetInt("Difficulty", Difficulty.Value);
        PlayerPrefs.SetFloat("SFX", SoundEffectsSlider.value);
        PlayerPrefs.SetFloat("Music", MusicSlider.value);

        PlayerPrefs.Save();
    }

    public void Load()
    {
        BodyCount.Value = PlayerPrefs.GetInt("BodyCount", 0);
        Difficulty.Value = PlayerPrefs.GetInt("Difficulty", 0);
        SoundEffectsSlider.value = PlayerPrefs.GetFloat("SFX", 0f);
        MusicSlider.value = PlayerPrefs.GetFloat("Music", 0f);
    }
}
