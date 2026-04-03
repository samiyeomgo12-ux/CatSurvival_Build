/*using System.Collections;
using System.IO.Compression;
using UnityEngine;

public class Character : MonoBehaviour
{
    public static float Speed
    {
        get
        {
            switch (GameManager.instance.playerId)
            {

                case 0: return 1.1f; //치즈냥
                case 1: return 1f; //흰냥이
                case 2: return 1f;//깜냥이
                case 3: return 1.2f; //보라냥
                default: return 1f;

            }
        }

    }

    public static float WeaponSpeed
    {
        get
        {
            switch (GameManager.instance.playerId)
            {
                case 1: return 1.1f; //흰냥이 불화살 발사 속도 
                case 3: return 1.2f; //보라냥 불화살 발사 속도
                default: return 1f;
            }
        }
    }

    public static float WeaponRate
    {
        get
        {
            switch(GameManager.instance.playerId)
            {
                case 1: return 0.9f; //흰냥이 더 빠르게 재장전 
                case 3: return 0.9f;//보라냥 더 빠르게 재장전 
                default: return 1f;
            }
        }

    }
    public static float Damage
    {
        get 
        {
            switch(GameManager.instance.playerId)
            {
                case 2: return 1.1f; //깜냥이
                case 3: return 1.2f;//보라냥 
                default: return 1;
            }
        }
    }

    public static int Count
    {
        get
        {
            switch(GameManager.instance.playerId)
            {
                case 2: return 1; //
                default: return 0; //

            }
        }
    }


        *//* public static float Speed
         {
             get { return GameManager.instance.playerId == 0 ? 1.1f : 1f; }
         }

         public static float WeaponSpeed
         {
             get { return GameManager.instance.playerId == 1 ? 1.1f : 1f; }
         }

         public static float WeaponRate //원거리 무기 
         {
             get { return GameManager.instance.playerId == 1 ? 0.9f : 1f; }
         }
         public static float Damage 
         {
             get { return GameManager.instance.playerId == 1 ? 1.2f : 1f; }
         }

         public static int Count 
         {
             get { return GameManager.instance.playerId == 3 ? 1 : 0; }
         }
     *//*
    
}
*/