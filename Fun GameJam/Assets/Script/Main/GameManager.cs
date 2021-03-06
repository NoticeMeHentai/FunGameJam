﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class File
{
    public string mName = "";
    public Sprite mSprite;
    public float mFileSize = 50f;
    public int mAmountDownload = 0;
}

public class GameManager : MonoBehaviour
{
    public bool mHasMenu = false;
    [Header("Gameplay")]
    [SerializeField]
    public File[] mFileTypes;
    public int mMaxLives = 3;
    public float mMaxTime = 120f;


    private float mCurrentPoints = 0;
    private float mCurrentDownloadProgress = 0;
    private File mCurrentFileBeingDownloaded;
    private bool mCountsAsInPlay = false;
    private float mCurrentTimeLeft;
    private int mCurrentLivesLeft = 0;
    private bool mGameHasStarted = false;
    private float mTotalScore = 0;

    public static bool sGameHasStarted => sInstance.mGameHasStarted;
    public static float sTimeLeft => sInstance.mCurrentTimeLeft;
    public static float sCurrentPoints => sInstance.mCurrentPoints;
    public static float sCurrentDownloadProgress => Mathf.Clamp01(sInstance.mCurrentDownloadProgress/sInstance.mCurrentFileBeingDownloaded.mFileSize);
    public static File sCurrentFileBeingDownloaded => sInstance.mCurrentFileBeingDownloaded;
    public static bool sCountsAsPlaying { get { if (sInstance != null) return sInstance.mCountsAsInPlay; else return false; } }
    private static GameManager sInstance;
    public static File[] sDifferentFiles => sInstance.mFileTypes;
    public static float sScore => sInstance.mTotalScore;

    private void Awake()
    {
        mCountsAsInPlay = false;
        sInstance = this;
        OnGamePreparation += delegate
         {
             HUDManager.Enable(true);
             int randomIndex = Random.Range(0, mFileTypes.Length);
             mCurrentFileBeingDownloaded = mFileTypes[randomIndex];
             mCurrentTimeLeft = mMaxTime;
             mCurrentLivesLeft = mMaxLives;
         };

        OnGameReady += delegate { mCountsAsInPlay = true; mGameHasStarted = true; };
        SignalScanner.OnBigDisconnection += delegate 
        {
            mCountsAsInPlay = false;
            mCurrentLivesLeft -= 1;
            if (mCurrentLivesLeft == 0) OnGameOverNoLivesLeft();
        };
        SignalScanner.OnBigReconnection += delegate { mCountsAsInPlay = true; };

        OnFileDonwloaded += delegate 
        {
            mCurrentFileBeingDownloaded.mAmountDownload++;
            mTotalScore += mCurrentFileBeingDownloaded.mFileSize;
            int newRandomIndex = Random.Range(0, mFileTypes.Length);
            mCurrentFileBeingDownloaded = mFileTypes[newRandomIndex];
            mCurrentDownloadProgress = 0;
        };
    }

    private void Start()
    {
            StartCoroutine(TemporaryStart()); 
        if (!mHasMenu)
        {
        }
    }

    public delegate void Notify();
    /// <summary>
    /// Event to prepare the maps and other ressources
    /// </summary>
    public static Notify OnGamePreparation;
    /// <summary>
    /// Event to start the game when the map and ressources are ready
    /// </summary>
    public static Notify OnGameReady;
    /// <summary>
    /// When the player loses
    /// </summary>
    public static Notify OnGameOverNoLivesLeft;
    /// <summary>
    /// When the player wins
    /// </summary>
    public static Notify OnGameOverTimeRanOut;
    /// <summary>
    /// When the player restarts the game
    /// </summary>
    public static Notify OnRestart;

    public static Notify OnFileDonwloaded;
    float progress;
    private void Update()
    {
        if (sGameHasStarted && SignalScanner.sIsBigConnected)
        {
            mCurrentDownloadProgress += SignalScanner.sCurrentDownloadingSpeed * Time.deltaTime;
            progress = mCurrentDownloadProgress / mCurrentFileBeingDownloaded.mFileSize;
            Shader.SetGlobalFloat("_DownloadRatio", progress);
            if (mCurrentDownloadProgress >= mCurrentFileBeingDownloaded.mFileSize) OnFileDonwloaded();
        }
        if (mCountsAsInPlay)
        {
            mCurrentTimeLeft -= Time.deltaTime;
            if (mCurrentTimeLeft < 0) OnGameOverTimeRanOut();
        }
    }

    private IEnumerator TemporaryStart()
    {
        if (OnGamePreparation != null) OnGamePreparation();
        yield return null;
        if (OnGameReady != null) OnGameReady();
    }
}
