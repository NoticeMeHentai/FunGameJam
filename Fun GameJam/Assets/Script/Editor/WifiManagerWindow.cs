using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class WifiManagerWindow : EditorWindow
{
    [MenuItem("Window/Wifi Manager", priority =0)]
    public static void ShowWindow()
    {
        GetWindow<WifiManagerWindow>("Wifi Manager");
    }

    private static GUIStyle _ToggleButtonStyleNormal = null;
    private static GUIStyle _ToggleButtonStyleToggled = null;

    private enum WifiModificationStatus
    {
        None,
        Placing,
        Removing,
        Unifying,
    }
    private static WifiModificationStatus _CurrentModificationStatus;
    private static WifiPoint _CurrentSelectedPoint;

    private GameObject _WifiPointPrefab;

    private bool mHasDoneAction = false;
    private bool _IsActive = false;

    private void OnGUI()
    {
        //Initialize
        if (_ToggleButtonStyleNormal == null)
        {
            _ToggleButtonStyleNormal = "Button";
            _ToggleButtonStyleToggled = new GUIStyle(_ToggleButtonStyleNormal);
            _ToggleButtonStyleToggled.normal.background = _ToggleButtonStyleToggled.active.background;
        }

        _WifiPointPrefab = EditorGUILayout.ObjectField(_WifiPointPrefab, typeof(GameObject),true) as GameObject;
        _IsActive = GUILayout.Toggle(_IsActive, "IsActive");
        //if (GUILayout.Button("Active", _IsActive ? _ToggleButtonStyleToggled : _ToggleButtonStyleNormal)) _IsActive = !_IsActive;
        EditorGUILayout.TextArea("Shift + Left Click = Add \n Shift + Right Click = Relay \n Ctrl + Left Click = Delete");
        //Window
        //GUILayout.BeginHorizontal();
        //if (GUILayout.Button("None", _CurrentModificationStatus== WifiModificationStatus.None ? _ToggleButtonStyleToggled : _ToggleButtonStyleNormal))
        //{
        //    _CurrentModificationStatus = WifiModificationStatus.None;
        //}

        //if (GUILayout.Button("Placing", _CurrentModificationStatus == WifiModificationStatus.Placing ? _ToggleButtonStyleToggled : _ToggleButtonStyleNormal))
        //{
        //    _CurrentModificationStatus = WifiModificationStatus.Placing;
        //}
        //GUILayout.EndHorizontal();

        //GUILayout.BeginHorizontal();
        //if (GUILayout.Button("Removing", _CurrentModificationStatus == WifiModificationStatus.Removing ? _ToggleButtonStyleToggled : _ToggleButtonStyleNormal))
        //{
        //    _CurrentModificationStatus = WifiModificationStatus.None;
        //}

        //if (GUILayout.Button("Unifying", _CurrentModificationStatus == WifiModificationStatus.Unifying ? _ToggleButtonStyleToggled : _ToggleButtonStyleNormal))
        //{
        //    _CurrentModificationStatus = WifiModificationStatus.None;
        //}
        //GUILayout.EndHorizontal();

        if (_IsActive && !mHasDoneAction)
        {
            if (Event.current.shift && Event.current.button == 0) AddWifiPoint();
            else if (Event.current.control && Event.current.button == 1) RemoveWifiPoint();
            else if (Event.current.shift && Event.current.button == 1) RelayWifiPoints();
        }

    }

    private void Update()
    {
        mHasDoneAction = false;
    }

    private void AddWifiPoint()
    {
        mHasDoneAction = true;
        Debug.Log("Adding Wifi point");
        if (_WifiPointPrefab!=null)
        Instantiate(_WifiPointPrefab);
    }

    private void RemoveWifiPoint()
    {
        mHasDoneAction = true;
        Debug.Log("Removing Wifi point");
    }

    private bool mTest = false;
    private void RelayWifiPoints()
    {
        mTest = !mTest;
        mHasDoneAction = true;
        Debug.Log("Relaying Wifi point "+mTest.ToString());
    }
}
