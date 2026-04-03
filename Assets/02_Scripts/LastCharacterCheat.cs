using Firebase.AppCheck;

using UnityEngine;

public class LastCharacterCheat : MonoBehaviour
{
    private bool oneTimeOnly = true;

    private readonly int[] _cheatPattern = { 1, 2, 3, 0 };
    private int _cheatProgress = 0;
    private bool triggered = false;

    public void OnLastPageCharacterClicked(int localIndex)
    {
        if (oneTimeOnly && triggered) return;

        if(!gameObject.activeInHierarchy) return;

        if(localIndex == _cheatPattern[_cheatProgress])
        {
            _cheatProgress++;

            if(_cheatProgress >= _cheatPattern.Length)
            {
                _cheatProgress = 0;
                triggered = true;

                if(NewAchiveManager.Instance != null)
                {
                    NewAchiveManager.Instance.UnlockAllCharactersByCheat();
                    Debug.Log("모든 캐릭터 해금");
                }
               
            }
        }
        else
        {
            _cheatProgress = (localIndex == _cheatPattern[0]) ? 1 : 0;
        }

       
    }
}
