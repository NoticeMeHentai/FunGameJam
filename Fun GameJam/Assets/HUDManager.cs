using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDManager : MonoBehaviour
{
    public Image mCurrentBeingDownloaded;
    public float mDownloadingTextSpeed = 1f;
    public float mTimeToFolder = 0.5f;
    public float mBlinkingWifiTime = 0.2f;
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
        GameManager.OnFileDonwloaded += delegate { StartCoroutine(nameof(FileToFolderEnumerator)); mNextDownloadIcon = GameManager.sCurrentFileBeingDownloaded.mSprite; };
        SignalScanner.OnBigDisconnection += delegate { mWifiSignalLost.SetActive(true); StopCoroutine(nameof(BlinkingWifi)); };
        SignalScanner.OnBigReconnection += delegate { mWifiSignalLost.SetActive(false); StopCoroutine(nameof(BlinkingWifi)); };
        SignalScanner.OnDirectDisconnection += delegate { mDownloadingText.enabled = false; mDisconectionCountdown.enabled = true; StartCoroutine(nameof(BlinkingWifi)); };
        SignalScanner.OnDirectReconnection += delegate { mDownloadingText.enabled = true; mDisconectionCountdown.enabled = false; StopCoroutine(nameof(BlinkingWifi));  mWifiSignalLost.SetActive(false); };
    }

    private void Update()
    {
        if (GameManager.sCountsAsPlaying)
        {
            mDownloadBarMat.SetFloat("_DownloadRatio", GameManager.sCurrentDownloadProgress);
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

    private IEnumerator BlinkingWifi()
    {
        float currentTime = 0;
        bool toggle = true;
        bool test = true;
        while (test)
        {
            while (currentTime < mBlinkingWifiTime)
            {
                currentTime += Time.deltaTime;
                yield return null;
            }
            toggle = !toggle;
            mWifiSignalLost.SetActive(toggle);
            currentTime %= 1f;
            yield return null;
        }
    }



}
