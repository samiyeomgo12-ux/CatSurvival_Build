using UnityEngine;

public class NewCharacter : MonoBehaviour
{
    public static float Speed
    {
        get
        {
            switch (GameFacade.Instance.PlayerId)
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
            switch (GameFacade.Instance.PlayerId)
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
            switch (GameFacade.Instance.PlayerId)
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
            switch (GameFacade.Instance.PlayerId)
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
            switch (GameFacade.Instance.PlayerId)
            {
                case 2: return 1; //
                default: return 0; //

            }
        }
    }
}
