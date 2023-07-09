using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public GameObject canvas;
    public Button playButton;

    private void Start()
    {
        canvas.SetActive(true);
        canvas.transform.localScale = Vector3.zero;
        playButton.onClick.AddListener(StartGame);

    
    }

    private void StartGame()
    {
        canvas.SetActive(false);
    }

}
