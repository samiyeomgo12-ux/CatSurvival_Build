using System;
using UnityEngine;
using UnityEngine.UIElements;

public class TimeManager : MonoBehaviour
{

    [SerializeField]private float[] _maxGameTime;
    private int stageIdx;
    public float[] MaxGameTime => _maxGameTime;
    public float GameTime { get; private set; }
    public int currentMaxGameTimeIdx;
    private bool _isLive;
    
    public event Action OnTimeUp;

    public void SetLive(bool isLive) => _isLive = isLive; //파사드에서 주입
    private void Awake()
    {
        stageIdx = PlayerPrefs.GetInt("StageIdx", 0);
    }
    private void Update()
    {
        if (!_isLive) return;

        GameTime += Time.deltaTime;

        currentMaxGameTimeIdx = Mathf.Min(stageIdx, _maxGameTime.Length - 1);

        if(GameTime >= _maxGameTime[currentMaxGameTimeIdx])
        {
            GameTime = _maxGameTime[currentMaxGameTimeIdx];

            if (stageIdx < MaxGameTime.Length - 1)
            {
                stageIdx++;
                PlayerPrefs.SetInt("StageIdx", stageIdx);
                PlayerPrefs.Save();
            }

            OnTimeUp?.Invoke();
        }

    }

    public void GameTimeReset()
    {
        GameTime = 0f;
    }
}
