using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Rendering;

public class NewPlayerManager : MonoBehaviour
{
    [Header("플레이어 정보")]
    [SerializeField] private NewPlayer _player;
    [SerializeField] private GameObject _playerObj;
    [SerializeField] private CharacterSO _characterData;
    [SerializeField] private float _playerSpeed;
    [SerializeField] private int _playerId;
    [SerializeField] private float _playerHp;
    [SerializeField] private float _playerMaxHp;
    [SerializeField] private int _level;
    [SerializeField] private int _kill;
    [SerializeField] private int _exp;
    [SerializeField] private int[] nextExp = { 3, 5, 10, 100, 150, 210, 280, 360, 450, 600 };

    public NewPlayer Player => _player;
    public CharacterSO CharacterData => _characterData;
    public int PlayerId => _playerId;
    public float PlayerSpeed => _playerSpeed;
    public float PlayerHp => _playerHp;
    public float PlayerMaxHp => _playerMaxHp; 
    public int Level => _level;
    public int Kill => _kill;
    public int Exp => _exp;
    public int[] NextExp => nextExp;
    private float _magnetRadius = 0f;
    private float _magnetEndTime = 0f;

    public float MagnetRadius => _magnetRadius;

    public event Action OnLevelup;
    public event Action OnHpChanged;

    public void Init(CharacterSO characterData)
    {
        _characterData = characterData;
        _playerId = characterData.characterID;
        _playerHp = _playerMaxHp;
        _level = 0; 
        _kill = 0;
        _exp = 0;

        _playerSpeed = 3f * _characterData.characterSpeed;
    }
    
    public void PlayerActive()
    {
        _playerObj.SetActive(true);
    }
    public void AddExp(int amount)
    {
        if (amount <= 0) return;

        _exp += amount;

        while(_level < nextExp.Length && _exp >= nextExp[_level])
        {
            _exp -= nextExp[_level];
            _level++;
            OnLevelup?.Invoke();

        }        
    }

    public void AddKill() => _kill++;

    public void SpeedUp(float rate)
    {
        float speed = 3 * _characterData.characterSpeed;
        _playerSpeed = speed + speed * rate;
    }
    public void TakeDamage(float amount)
    {
        _playerHp = Mathf.Max(0f, _playerHp - amount);

        OnHpChanged?.Invoke();
    }

    public void ActivateMagnet(float radius, float duration)
    {
        _magnetRadius = Mathf.Max(_magnetRadius, radius);
        _magnetEndTime = Mathf.Max(_magnetEndTime, Time.time + Mathf.Max(0.1f, duration));
    }

    public bool IsMagnetActive()
    {
        if (Time.time > _magnetEndTime)
        {
            _magnetRadius = 0f;
            return false;
        }
        return _magnetRadius > 0f;
    }

    public void HealFull()
    {
        _playerHp = _playerMaxHp;
        OnHpChanged?.Invoke();
        
        
    }
}
