using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Result : MonoBehaviour
{
    public GameObject[] titles;

    public void Lose()
    {
        titles[0].SetActive(true); //게임 오버 띄우기
        titles[1].SetActive(false); //이겼다 타이틀 비활성화 
    }
    public void Win()
    {
        titles[1].SetActive(true);
        titles[0].SetActive(false);
    }

}
