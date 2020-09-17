using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public bool mHasMenu = false;

    private static GameManager sInstance;
    private void Awake()
    {
        sInstance = this;
    }

    private void Start()
    {
        if (!mHasMenu)
        {
            StartCoroutine(TemporaryStart()); 
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
    public static Notify OnGameOver;
    /// <summary>
    /// When the player wins
    /// </summary>
    public static Notify OnGameWin;
    /// <summary>
    /// When the player restarts the game
    /// </summary>
    public static Notify OnRestart;


    private IEnumerator TemporaryStart()
    {
        if (OnGamePreparation != null) OnGamePreparation();
        yield return null;
        if (OnGameReady != null) OnGameReady();
    }
}
