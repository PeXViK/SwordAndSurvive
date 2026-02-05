using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public GameObject infoPanel;

    void Start()
    {
        
    }

    public void ShowInfo()
    {
        infoPanel.SetActive(true);
    }

    public void CloseInfo()
    {
        infoPanel.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.Escape))
        {
            CloseInfo();
        }
    }
}
