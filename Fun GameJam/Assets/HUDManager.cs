using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDManager : MonoBehaviour
{
    public Image mCurrentBeingDownloaded;
    public float mDownloadingTextSpeed = 1f;
    public float mTimeToFolder = 0.5f;
    public AnimationCurve mFileToFolderCurve;
    public Transform mDownloadIconRef;
    public Transform mDownloadFolder;
    public Material mDownloadBarMat;
    public GameObject mWifiSignalLost;
    public Text mDownloadingText;
    public Text mTimerCountdown;
    public Text mDisconectionCountdown;



    private static HUDManager sInstance;
    private Sprite mNextDownloadIcon;
    private void Awake()
    {

        sInstance = this;
        GameManager.OnGamePreparation += delegate { mWifiSignalLost.SetActive(false); mDisconectionCountdown.enabled = false; };
        GameManager.OnFileDonwloaded += delegate { mNextDownloadIcon = GameManager.sCurrentFileBeingDownloaded.mSprite; };
        SignalScanner.OnBigDisconnection += delegate { mWifiSignalLost.SetActive(true); };
        SignalScanner.OnBigReconnection += delegate { mWifiSignalLost.SetActive(false); };
        SignalScanner.OnDirectDisconnection += delegate { mDownloadingText.enabled = false; mDisconectionCountdown.enabled = true; };
        SignalScanner.OnDirectReconnection += delegate { mDownloadingText.enabled = true; mDisconectionCountdown.enabled = false; };
    }

    private void Update()
    {
        if (GameManager.sCountsAsPlaying)
        {
            Shader.SetGlobalFloat("_DownloadRatio", GameManager.sCurrentDownloadProgress);
            //mDownloadBarMat.SetFloat("_DownloadRatio", GameManager.sCurrentDownloadProgress);
            mDownloadingText.text = "Downloading" + new string('.', Mathf.FloorToInt((Time.time * mDownloadingTextSpeed) % 3) + 1);
            mTimerCountdown.text = GetSecondsText(GameManager.sTimeLeft);
            mDisconectionCountdown.text = GetSecondsText(SignalScanner.sTimeLeftUntilBigDisconnection);
        }

    }

    public static void Enable(bool value)
    {
        sInstance.gameObject.SetActive(value);
    }

    private string GetSecondsText(float timeLeft)
    {
        float seconds = Mathf.Floor(timeLeft);
        float decimals = Mathf.Floor((timeLeft % 1f) * 100);
        return seconds.ToString() + "." + decimals.ToString();
    }
    private IEnumerator FileToFolderEnumerator()
    {
        float currentTime = 0;
        while (currentTime < mTimeToFolder)
        {
            float progress = mFileToFolderCurve.Evaluate(currentTime / mTimeToFolder);
            mCurrentBeingDownloaded.transform.position = Vector3.Lerp(mDownloadIconRef.position, mDownloadFolder.position, progress);
            currentTime += Time.deltaTime;
            yield return null;
        }
        mCurrentBeingDownloaded.transform.position = mDownloadIconRef.position;
        mCurrentBeingDownloaded.sprite = mNextDownloadIcon;

    }



}
