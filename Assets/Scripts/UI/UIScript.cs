using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIScript : MonoBehaviour
{

    [SerializeField] private GameObject uiContainer;
    [SerializeField] private float openingTime = 2f;
    [SerializeField] private float closingTime = 2f;

    float closeY = 1119;
    float openY = 29;
    // Start is called before the first frame update
    void Start()
    {
        uiContainer = gameObject;
        PlayOpeningAnimation();
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void PlayOpeningAnimation()
    {
        LeanTween.moveLocalY(gameObject, openY, 2).setEaseOutQuint();
    }


    public void PlayClosingAnimation()
    {
        LeanTween.moveLocalY(gameObject, closeY, closingTime).setEaseOutExpo();
    }
}
