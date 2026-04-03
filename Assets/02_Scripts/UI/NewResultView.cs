using UnityEngine;

public class NewResultView : MonoBehaviour
{
    public GameObject[] titles;
    
    public void Lose()
    {
        titles[0].SetActive(true);
        titles[1].SetActive(false);
    }
    public void Win()
    {
        titles[0].SetActive(false);
        titles[1].SetActive(true);
    }
  
}
