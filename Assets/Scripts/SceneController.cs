﻿using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SceneController : MonoBehaviour
{
    #region PARAMS

    [Header("Links")]
    [SerializeField] public GameObject ruleteObj;

    [SerializeField] private Image _player;
    [SerializeField] private Text _timerText;
    [SerializeField] private Text _scoreText;

    [Header("Params")]
    [SerializeField] private float _timeForMatch;
    [SerializeField] private float _ruleteSpeed;
    [SerializeField] private float _scoreModifide;

    #endregion

    [HideInInspector] public Color[] holeColors = new Color[]
    {
        Color.white,
        Color.grey,
        Color.red,
        Color.green,
        Color.blue,
        Color.cyan,
        Color.black,
        Color.yellow
    };

    [HideInInspector] public bool[] holeActiveList;

    #region PRIVATE

    private bool _isPlayGame;
    private float _playerScore;
    private Vector2 _playerStartPosition;

    #endregion

    #region METHODS

    private void Start()
    {
        _isPlayGame = true;
        _playerScore = 0;
        holeActiveList = new bool[holeColors.Length];

        _playerStartPosition = _player.gameObject.transform.position;
        _player.color = holeColors[Random.Range(0, holeColors.Length)];
        GenerateWorld();
        StartCoroutine(UpdateTimeUI());
    }

    private void Update()
    {
        if (_isPlayGame)
        {
            ruleteObj.transform.Rotate(new Vector3(0, 0, 1), _ruleteSpeed);
        }
    }

    private void GenerateWorld()
    {
        for (int holeId = 0; holeId < ruleteObj.transform.childCount; holeId++)
        {
            holeActiveList[holeId] = true;
            ruleteObj.transform.GetChild(holeId).GetComponent<HoleController>().CreateHole(holeId, holeColors[holeId], this);
        }
    }

    private void GameEnd()
    {
        Debug.LogWarning("Game End / You Score : " + _playerScore);

        _isPlayGame = false;

        for (int holeId = 0; holeId < ruleteObj.transform.childCount; holeId++)
        {
            if (!ruleteObj.transform.GetChild(holeId).GetComponent<HoleController>().isActive) _playerScore += 1 * _scoreModifide;
        }

        _scoreText.text = "Score : " + _playerScore;
    }

    private IEnumerator UpdateTimeUI()
    {
        while (_timeForMatch > 0)
        {
            _timerText.text = "Timer : " + _timeForMatch;
            yield return new WaitForSecondsRealtime(1f);
            _timeForMatch -= 1;
        }

        GameEnd();
    }

    public void ReactivatePlayer(int holeId, bool isWin)
    {
        if (isWin) holeActiveList[holeId] = false;
        ReactivatePlayer();
    }

    public void ReactivatePlayer()
    {
        _player.color = GetNewPlayerColor();
        _player.gameObject.GetComponent<Rigidbody2D>().position = _playerStartPosition;
        _player.gameObject.transform.position = _playerStartPosition;
    }

    public Color GetNewPlayerColor()
    {
        for (int colorId = 0; colorId < holeColors.Length; colorId++)
        {
            if (holeActiveList[colorId])
            {
                return holeColors[colorId];
            }
        }

        return Color.white;
    }

    #endregion
}