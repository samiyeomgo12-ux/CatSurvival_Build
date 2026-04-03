using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
public class ToggleButton : MonoBehaviour
{
    [SerializeField] private Button toggleButton;
    [SerializeField] private GameObject activeObject;

    private void Awake()
    {
        toggleButton = GetComponent<Button>();

    }


    public void TogglePanel()
    {
        activeObject.SetActive(!activeObject.activeSelf);
    }

}
