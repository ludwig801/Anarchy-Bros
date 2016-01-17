using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class GameUIManager : MonoBehaviour
{
    public RectTransform EnemiesHealth, TowersHealth;
    public RectTransform HealthElementPrefab;
    public RectTransform PlayButton;
    public RectTransform PauseButton;
    public RectTransform ResumeButton;
    public RectTransform GoBackButton;
    public RectTransform PlaceTextPanel;
    public RectTransform GameOverPanel;
    public Text GameOverScore;
    public List<HealthElement> HealthElements;

    GameManager _gameManager;
    Text _placeText;

    void Start()
    {
        _gameManager = GameManager.Instance;

        _placeText = PlaceTextPanel.GetComponentInChildren<Text>();
    }

    void Update()
    {
        switch (_gameManager.CurrentState)
        {
            case GameStates.Pause:
                EnemiesHealth.gameObject.SetActive(true);
                TowersHealth.gameObject.SetActive(true);
                GameOverPanel.gameObject.SetActive(false);
                GoBackButton.gameObject.SetActive(true);
                ResumeButton.gameObject.SetActive(true);
                PauseButton.gameObject.SetActive(false);
                PlayButton.gameObject.SetActive(false);
                PlaceTextPanel.gameObject.SetActive(true);
                _placeText.text = "Score: " + _gameManager.Score.ToString();
                break;

            case GameStates.Place:
                EnemiesHealth.gameObject.SetActive(false);
                TowersHealth.gameObject.SetActive(true);
                GameOverPanel.gameObject.SetActive(false);
                GoBackButton.gameObject.SetActive(true);
                ResumeButton.gameObject.SetActive(false);
                PauseButton.gameObject.SetActive(false);
                PlaceTextPanel.gameObject.SetActive(true);
                if (_gameManager.Towers.AliveTowers >= _gameManager.Towers.MaxNumTowers)
                {
                    PlayButton.gameObject.SetActive(true);
                }
                else
                {
                    PlayButton.gameObject.SetActive(false);
                }
                _placeText.text = "Place Towers (" + (_gameManager.Towers.MaxNumTowers - _gameManager.Towers.AliveTowers) + " left)";
                break;

            case GameStates.Play:
                EnemiesHealth.gameObject.SetActive(true);
                TowersHealth.gameObject.SetActive(true);
                GameOverPanel.gameObject.SetActive(false);
                GoBackButton.gameObject.SetActive(false);
                ResumeButton.gameObject.SetActive(false);
                PauseButton.gameObject.SetActive(true);
                PlayButton.gameObject.SetActive(false);
                PlaceTextPanel.gameObject.SetActive(true);
                _placeText.text = "Score: " + _gameManager.Score.ToString();
                break;

            case GameStates.GameOver:
                EnemiesHealth.gameObject.SetActive(false);
                TowersHealth.gameObject.SetActive(false);
                GoBackButton.gameObject.SetActive(false);
                ResumeButton.gameObject.SetActive(false);
                PauseButton.gameObject.SetActive(false);
                PlayButton.gameObject.SetActive(false);
                PlaceTextPanel.gameObject.SetActive(false);
                GameOverPanel.gameObject.SetActive(true);
                GameOverScore.text = "Your score: " + _gameManager.Score.ToString();
                break;
        }
    }

    public HealthElement FindEnemyHealthElement()
    {
        for (int i = 0; i < HealthElements.Count; i++)
        {
            if (!HealthElements[i].gameObject.activeSelf)
            {
                HealthElements[i].gameObject.SetActive(true);
                return HealthElements[i];
            }
        }

        HealthElement elem = Instantiate(HealthElementPrefab).GetComponent<HealthElement>();
        elem.name = "Health Indicator";
        elem.transform.SetParent(EnemiesHealth);
        HealthElements.Add(elem);

        return elem;
    }

    public HealthElement FindTowerHealthElement()
    {
        for (int i = 0; i < HealthElements.Count; i++)
        {
            if (!HealthElements[i].gameObject.activeSelf)
            {
                HealthElements[i].gameObject.SetActive(true);
                return HealthElements[i];
            }
        }

        HealthElement elem = Instantiate(HealthElementPrefab).GetComponent<HealthElement>();
        elem.name = "Health Indicator";
        elem.transform.SetParent(TowersHealth);
        HealthElements.Add(elem);

        return elem;
    }

    public void CreateEnemyHealthElement(PieceBehavior requester)
    {
        HealthElement elem = FindEnemyHealthElement();
        elem.Target = requester;
    }

    public void CreateTowerHealthElement(PieceBehavior requester)
    {
        HealthElement elem = FindTowerHealthElement();
        elem.Target = requester;
    }
}