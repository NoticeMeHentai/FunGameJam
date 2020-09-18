using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public GameObject mPauseMenu;
    public GameObject mEndFailMenu;
    public GameObject mEndWinMenu;
    public GameObject mDownloadBar;
    public GameObject mWifiLostLogo;
    private Text mDownloadText;
    public bool mDownloading = true;
    public float mDownloadSpeed = 3.0f;
    public bool mWifiLost = false;

    private bool mWifiLostLogoSetActive = false;

    public GameObject mMainMenuFirstSelected;
    public GameObject mOptionMenuFirstSelected;
    public GameObject mPauseMenuFirstSelected;
    public GameObject mEndFailMenuFirstSelected;
    public GameObject mEndWinMenuFirstSelected;

    public static bool sIsPaused = false;
    public static bool sInMainMenu = true;
    public bool mEndFail = false;
    public bool mEndWin = false;
    [Range(0.0f,1.0f)] public float mDownloadRatio = 0.0f;

    private void Start()
    {
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(mMainMenuFirstSelected);
        mDownloadText = mDownloadBar.transform.GetChild(0).GetComponent<Text>();
    }
    private void Update()
    {
        if(mWifiLost && !mWifiLostLogoSetActive)
        {
            mWifiLostLogoSetActive = true;
            mWifiLostLogo.SetActive(true);
        }
        if (!mWifiLost && mWifiLostLogoSetActive)
        {
            mWifiLostLogoSetActive = false;
            mWifiLostLogo.SetActive(false);
        }
        mDownloadBar.GetComponent<Image>().material.SetFloat("_DownloadRatio", mDownloadRatio);
        if(mDownloading)
        {
            mDownloadText.text = "Downloading" + new string('.', Mathf.FloorToInt((Time.time* mDownloadSpeed) % 3) + 1);
        }
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
        mDownloadText = mDownloadBar.transform.GetChild(0).GetComponent<Text>();
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
