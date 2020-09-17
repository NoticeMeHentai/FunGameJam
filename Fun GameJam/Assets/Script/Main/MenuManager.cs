using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class MenuManager : MonoBehaviour
{
    public GameObject mPauseMenu;
    public GameObject mEndFailMenu;
    public GameObject mEndWinMenu;

    public GameObject mMainMenuFirstSelected;
    public GameObject mOptionMenuFirstSelected;
    public GameObject mPauseMenuFirstSelected;
    public GameObject mEndFailMenuFirstSelected;
    public GameObject mEndWinMenuFirstSelected;

    public static bool sIsPaused = false;
    public static bool sInMainMenu = true;
    public bool mEndFail = false;
    public bool mEndWin = false;

    private void Start()
    {
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(mMainMenuFirstSelected);
    }
    private void Update()
    {
        if (Input.GetAxis("HorizontalMenu") > 0)
        {
            Debug.Log("a");

        }    

        if (!sInMainMenu)
        {
            if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Joystick1Button7))
            {
                if (sIsPaused)
                {
                    sIsPaused = false;
                    mPauseMenu.SetActive(false);
                    Time.timeScale = 0.0f;
                }
                else
                {
                    sIsPaused = true;
                    mPauseMenu.SetActive(true);
                    Time.timeScale = 1.0f;
                    EventSystem.current.SetSelectedGameObject(null);
                    EventSystem.current.SetSelectedGameObject(mPauseMenuFirstSelected);

                }
            }
        }
        if (mEndFail)
        {
            EndFail();
        }
        if (mEndWin)
        {
            EndWin();
        }
        
    }
    public void GoToOptions()
    {
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(mOptionMenuFirstSelected);
    }

    public void PlayGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1, LoadSceneMode.Additive);
        sInMainMenu = false;
        Time.timeScale = 1.0f;
    }

    public void Resume()
    {
        sIsPaused = false;
        Time.timeScale = 1.0f;
    }
    public void BackToMainMenu()
    {
        sIsPaused = false;
        sInMainMenu = true;
        SceneManager.UnloadSceneAsync(1);
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(mMainMenuFirstSelected);
    }

    public void EndFail()
    {
        mEndFail = false;
        mEndFailMenu.SetActive(true);
        Time.timeScale = 0.0f;
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(mEndFailMenuFirstSelected);
    }

    public void EndWin()
    {
        mEndWin = false;
        mEndWinMenu.SetActive(true);
        Time.timeScale = 0.0f;
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(mEndWinMenuFirstSelected);
    }

    public void Quit ()
    {
        Debug.Log("QUIT");
        Application.Quit();
    }

}
