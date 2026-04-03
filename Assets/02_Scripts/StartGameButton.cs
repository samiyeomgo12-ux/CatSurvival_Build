using UnityEngine;

public class StartGameButton : MonoBehaviour
{


    public void OnClickStartGame(CharacterSO characterData)
    {
        if(!GameFacade.Instance.LifeManager.CanPlay())
        {
            Debug.Log("하트 부족 게임 실행 불가");
            return;
        }

       
        GameFacade.Instance.GameStart(characterData ); //게임 시작 호출해주자 
    }
}
