//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEditor;
//using UnityEngine.SceneManagement;
//[InitializeOnLoad]
//public static class WifiManagerStatic
//{
//    static WifiManagerStatic()
//    {
//        //EditorApplication.update += AddWifiPoint;
//        SceneView.onSceneGUIDelegate += CheckWifi;

//        EditorApplication.update += delegate { mHasAlreadyClicked = false; };
//        GroundLayerMask = 1 << LayerMask.NameToLayer("Ground");
//        WifiLayerMask = 1 << LayerMask.NameToLayer("WifiPoints");
//    }


//    private static bool mHasAlreadyClicked = false;
//    private static RaycastHit hitInfo;
//    private static int GroundLayerMask = 0;
//    private static int WifiLayerMask = 0;
//    private static void CheckWifi(SceneView scene)
//    {
//        if (WifiManager.sIsActive)
//        {
//            if (mHasAlreadyClicked)
//            {
//                Debug.Log("Aha!");
//                mHasAlreadyClicked = false;
//            }
//            else
//            {
//                if (Event.current.shift && Event.current.keyCode == KeyCode.Alpha0 && Event.current.type == EventType.KeyDown)
//                {
//                    //if(Physics.Raycast())
//                    Ray ray = SceneView.lastActiveSceneView.camera.ScreenPointToRay(MousePosCorrected());

//                    if (Physics.Raycast(ray, out hitInfo, GroundLayerMask))
//                    {
//                        Debug.Log("Creating new wifi point on target ");
//                        GameObject.Instantiate(WifiManager.sPrefab, hitInfo.point, Quaternion.identity);
//                    }
//                    else
//                    {
//                        Debug.Log("No ground found!");
//                    }
//                    mHasAlreadyClicked = true;
//                }
//                else if (Event.current.shift && Event.current.keyCode == KeyCode.Alpha2 && Event.current.type == EventType.KeyDown)
//                {
//                    Event.current.type == EventType.m
//                    Ray ray = SceneView.lastActiveSceneView.camera.ScreenPointToRay(MousePosCorrected());

//                    if (Physics.Raycast(ray, out hitInfo, WifiLayerMask))
//                    {
//                        if (hitInfo.collider.CompareTag("WifiPoint"))
//                        {
//                            Debug.Log("Destroying wifi point ");
//                            GameObject.DestroyImmediate(hitInfo.collider.gameObject);
//                        }

//                    }
//                    else
//                    {
//                        Debug.Log("No Wifi point to destroy found!");
//                    }
//                    mHasAlreadyClicked = true;
//                }
//            }
//        }

//    }

//    private static Vector2 MousePosCorrected()
//    {
//        return new Vector2(Event.current.mousePosition.x, SceneView.lastActiveSceneView.camera.pixelHeight - Event.current.mousePosition.y);
//    }

//}
