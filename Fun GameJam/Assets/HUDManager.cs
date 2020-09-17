using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDManager : MonoBehaviour
{
    public Image mCurrentBeingDownloaded;
    public float mTimeToFolder = 0.5f;
    public AnimationCurve mFileToFolderCurve;
    public Transform mDownloadIconRef;
    public Transform mDownloadFolder;
    public Material mDownloadBarMat;
    public GameObject mWifiSignalLost;
    public Text mTimerCountdown;



    private static HUDManager sInstance;
    private Sprite mNextDownloadIcon;
    private void Awake()
    {

        sInstance = this;
        GameManager.OnGamePreparation += delegate { mWifiSignalLost.SetActive(false); };
        GameManager.OnFileDonwloaded += delegate { mNextDownloadIcon = GameManager.sCurrentFileBeingDownloaded.mSprite; };
        SignalScanner.OnBigDisconnection += delegate { mWifiSignalLost.SetActive(true); };
        SignalScanner.OnBigReconnection += delegate { mWifiSignalLost.SetActive(false); };
    }

    private void Update()
    {
        mDownloadBarMat.SetFloat("_DownloadRatio", GameManager.sCurrentDownloadProgress);
        if (GameManager.sCountsAsPlaying)
        {
            float seconds = Mathf.Floor(GameManager.sTimeLeft);
            float decimals = Mathf.Floor((GameManager.sTimeLeft % 1f) * 100);
            mTimerCountdown.text = seconds.ToString() + "." + decimals.ToString(); 
        }
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
