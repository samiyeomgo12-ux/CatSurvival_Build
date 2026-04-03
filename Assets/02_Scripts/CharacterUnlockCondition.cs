using UnityEngine;

public enum UnlockType
{
    None, 
    Kill, 
    Time
}
[System.Serializable]
public class CharacterUnlockCondition
{
    public UnlockType unlockType;
    public bool isLock;
    public int unlockKillCount;
    public int unlockSurviveTime; //분 기준 ex) 5분

    public bool CheckUnlockCondition()
    {
        switch(unlockType)
        {
            case UnlockType.None:
                return true;
            case UnlockType.Kill:
               return GameFacade.Instance.Kill >= unlockKillCount;
            case UnlockType.Time:
               return GameFacade.Instance.GameTime >= unlockSurviveTime*60;
            default: 
                return false;                
        }
    }
    

    public string GetUnlockDescription()
    {
        switch(unlockType)
        {
            case UnlockType.Kill:
                return $"적 {unlockKillCount} 처치 시 해금";
            case UnlockType.Time:
                return $"{unlockSurviveTime}분 생존 시 해금";
            default:
                return "";

        }
    }

}
