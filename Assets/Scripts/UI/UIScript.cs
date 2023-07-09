using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIScript : MonoBehaviour
{

    [SerializeField] private GameObject uiContainer;
    [SerializeField] private GameObject turnUI;
    [SerializeField] private float openingTime = 2f;
    [SerializeField] private float closingTime = 2f;

    float closeY = 1119;
    float openY = 29;

    [SerializeField] GameObject playerText;
    [SerializeField] GameObject computerText;

    float[] textYLocations = { 42.8f, -52.8f };
    int counter = 0;
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
        turnUI.SetActive(true); 
    }

    public void ReverseUIElements()
    {
        LeanTween.moveLocalY(playerText, textYLocations[(counter + 1) % 2], 1);
        LeanTween.moveLocalY(computerText, textYLocations[counter], 1);

        counter = (counter + 1) % 2;
    }
}
