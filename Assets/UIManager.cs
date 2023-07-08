using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject introTitle;
    [SerializeField] private GameObject objectiveContainer;

    [SerializeField] private GameObject miniObjectiveContainer;
    [SerializeField] private GameObject checkMarkMiniObjective;

    public GameObject levelCompletedContainer;
    public GameObject levelFailedContainer;

    [SerializeField] float animationTime = 3f;
    [SerializeField] float onScreenTime = 3f;


    // [SerializeField] private AudioManager_LV2 audioManager;
    private bool animationsFinished = false;


    public event EventHandler onAnimationsFinished;

    // Start is called before the first frame update
    void Start()
    {
        PlayIntroAnimation();
    }

    // Update is called once per frame
    void Update()
    {

    }


    void PlayIntroAnimation()
    {
        PlayTitleAnimation(true);
        //audioManager.Play("EaglesCry");

        StartCoroutine(CloseTitleOpenObjective(animationTime));
        // StartCoroutine(Test(animationTime * 5));
    }

    void PlayTitleAnimation(bool onScreen)
    {
        float offScreenY = 700;
        float targetY = 0;

        if (!onScreen)
            targetY = offScreenY;


        LeanTween.moveLocalY(introTitle, targetY, animationTime).setEaseOutQuart();
    }


    public void PlayObjectiveAnimation(bool onScreen)
    {
        float offScreenY = -1000;
        float targetY = 0;
        float time = animationTime;

        if (!onScreen)
        {
            targetY = offScreenY;
            DisplayMiniObjective(true);
            time /= 2;
            onAnimationsFinished?.Invoke(this, EventArgs.Empty);
        }
        else
        {
            DisplayMiniObjective(false);
            //audioManager.Play("NotificationSound");
        }



        LeanTween.moveLocalY(objectiveContainer, targetY, time).setEaseOutQuart();

        // play audio here
    }

    public void DisplayMiniObjective(bool onScreen)
    {
        float offScreenX = -1309;
        float targetX = -628;

        if (!onScreen) targetX = offScreenX;

        Debug.Log("Display Mini Objective");
        LeanTween.moveLocalX(miniObjectiveContainer, targetX, animationTime / 2).setEaseOutQuart();

    }

    public void OnLevelCompletion()
    {
        // audioManager.Play("LevelCompleted");
        Debug.Log("Entered UI Level Complete Function");
        LeanTween.scale(checkMarkMiniObjective, new Vector3(1, 1, 1), 0.25f).setEaseOutBack();
        Debug.Log("Played checkmark animation");
        //audioManager.Play("Yoo");
        //audioManager.Play("Stamp");
        Debug.Log("Played audio for completeiom");
        levelCompletedContainer.SetActive(true);
        Debug.Log("GameObject is active");
        LeanTween.scale(levelCompletedContainer, new Vector3(1, 1, 1), 0.5f).setEaseOutElastic();
        Debug.Log("Played main animation and exiting the function");
        StartCoroutine(HideGameObject(onScreenTime, levelCompletedContainer));

    }


    public void OnLevelFailed()
    {
        //audioManager.Play("Failed");
        //audioManager.Play("Failed");
        levelFailedContainer.SetActive(true);
        LeanTween.scale(levelFailedContainer, new Vector3(1, 1, 1), 0.5f).setEaseOutElastic();
        StartCoroutine(HideGameObject(onScreenTime, levelFailedContainer));
        StartCoroutine(ReloadScene(2));
    }

    IEnumerator CloseTitleOpenObjective(float seconds)
    {
        yield return new WaitForSeconds(seconds);

        PlayTitleAnimation(false);
        PlayObjectiveAnimation(true);
    }

    public bool AreAnimationsFinished() => animationsFinished;

    //IEnumerator Test(float seconds)
    //{
    //    yield return new WaitForSeconds(seconds);

    //    OnLevelCompletion();
    //}

    IEnumerator HideGameObject(float delay, GameObject gameObject)
    {
        yield return new WaitForSeconds(delay);
        gameObject.SetActive(false);
    }

    IEnumerator ReloadScene(float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene("Level2");
    }
}
