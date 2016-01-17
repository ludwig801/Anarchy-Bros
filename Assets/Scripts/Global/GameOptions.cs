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
}
