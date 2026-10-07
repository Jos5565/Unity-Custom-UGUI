using UnityEngine;
using UGUICUSTOM;

public class ToggleTest : MonoBehaviour
{
    public ButtonToggle toggle;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        toggle.isOn = true;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
