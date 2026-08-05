using BepInEx;
using HarmonyLib;
using System.Collections.Generic;
using System.Collections;
using System;
using GorillaNetworking;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;
using GorillaLocomotion;
using System.IO;
using UnityEngine.XR;

namespace WM
{
    [BepInPlugin(PluginInfo.GUID, PluginInfo.Name, PluginInfo.Version)]
    public class WMMenu : BaseUnityPlugin
    {
        //Normal Monke Mod Menu Bullshit.
        static string[] buttons = new string[] { "Fly", "Invis Monke", "Mod Check (OnGUI)", "Ghost Monke", "Platforms", "Refresh Photon", "Save Current Room Players", "Remove Tutorial PlayerPref", "No Slip", "Iron Monke", "Everything is Ice", "Go To Troll Menu" };
        static bool?[] buttonsActive = new bool?[] { false, false, false, false, false, false, false, false, false, false, false, false };
        static string[] buttonsTroll = new string[] { "Head Fuckery", "Spaz", "Head Spin", "Attach To Player", "Duck.", "Sound Spam", "", "", "", "", "", "Return To Normal Menu" };
        static bool?[] buttonsTrollActive = new bool?[] { false, false, false, false, false, false, false, false, false, false, false, false };
        static bool menuOpen;
        static GameObject menu = null;
        static GameObject canvasObj = null;
        static GameObject referance = null;
        public static int framePressCooldown = 0;
        static bool verified = false;
        static float? maxJumpSpeed = null;

        static bool troll = false;
        static bool normal = true;

        //Visual shit
        string MOTDMessage = "<color=cyan>< WM IS LOADED! >\n\n\nLOADED MODULES : WM MENU, WM GAMEMODE, WM PLAYER TRACKER</color>";

        public static bool onceDuck;

        //Mod GameObjects
        public static GameObject playerToFollow;

        public static GameObject CameraObj;
        public static Camera CameraObjCamera;

        public static LineRenderer pointer;

        public static GameObject rightPlat;
        public static GameObject leftPlat;
        public static bool onceRightSecButton;
        public static bool onceLeftSecButton;

        void Awake()
        {
            PlayerPrefs.SetString("tutorial", "done");
        }
        void OnEnable()
        {
            HarmonyPatches.ApplyHarmonyPatches();
            Utilla.Events.GameInitialized += OnGameInitialized;
        }

        void OnDisable()
        {
            HarmonyPatches.RemoveHarmonyPatches();
            Utilla.Events.GameInitialized -= OnGameInitialized;
        }
        void OnGameInitialized(object sender, EventArgs e)
        {
            CameraObj = new GameObject();
            CameraObj.name = "< WM FIRST PERSON >";
            CameraObj.transform.SetParent(Player.Instance.headCollider.transform,false);
            CameraObjCamera = CameraObj.AddComponent<Camera>();
            CameraObjCamera.stereoTargetEye = StereoTargetEyeMask.None;
            CameraObjCamera.enabled = false;
            Player.Instance.gameObject.AddComponent<PlayerTracker>();
            StartCoroutine(WaitSeconds(0.2f));
            CameraObjCamera.enabled = true;
            CameraObjCamera.nearClipPlane = 0.08f;
            CameraObjCamera.fieldOfView = 120;
            GameObject.Find("Third Person Camera/Shoulder Camera").GetComponent<Camera>().enabled = false;
        }
        void Update()
        {
            verified = true;
            try
            {
                if (true)
                {
                    if (maxJumpSpeed == null)
                    {
                        maxJumpSpeed = Player.Instance.maxJumpSpeed;
                        verified = true;
                    }

                    List<InputDevice> list = new List<InputDevice>();
                    InputDevices.GetDeviceAtXRNode(XRNode.RightHand).TryGetFeatureValue(CommonUsages.primary2DAxisClick, out menuOpen);

                    if (menuOpen && menu == null)
                    {
                        Draw();
                        if (referance == null)
                        {
                            referance = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                            GameObject.Destroy(referance.GetComponent<MeshRenderer>());
                            referance.transform.parent = Player.Instance.rightHandTransform;
                            referance.transform.localPosition = new Vector3(0f, -0.1f, 0f);
                            referance.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
                        }
                    }
                    else if (!menuOpen && menu != null)
                    {
                        GameObject.Destroy(menu);
                        menu = null;
                        GameObject.Destroy(referance);
                        referance = null;
                    }

                    if (menuOpen && menu != null)
                    {
                        menu.transform.position = Player.Instance.leftHandTransform.position + Player.Instance.leftHandTransform.forward / 10;
                        menu.transform.rotation = Player.Instance.leftHandTransform.rotation;
                    }

                    if (verified)
                    {
                        if (buttonsActive[0] == true)
                        {
                            InputDevices.GetDeviceAtXRNode(XRNode.RightHand).TryGetFeatureValue(CommonUsages.secondaryButton, out bool Freeze);
                            InputDevices.GetDeviceAtXRNode(XRNode.LeftHand).TryGetFeatureValue(CommonUsages.gripButton, out bool lFly);
                            InputDevices.GetDeviceAtXRNode(XRNode.RightHand).TryGetFeatureValue(CommonUsages.gripButton, out bool rFly);
                            if (Freeze)
                            {
                                Player.Instance.bodyCollider.attachedRigidbody.velocity = new Vector3(0,0.01f,0);
                            }
                            if (lFly)
                            {
                                Player.Instance.transform.position += Player.Instance.leftHandTransform.forward * 1.1f;
                                Player.Instance.bodyCollider.attachedRigidbody.velocity = Vector3.zero;
                            }
                            if (rFly)
                            {
                                Player.Instance.transform.position += Player.Instance.rightHandTransform.forward * 1.1f;
                                Player.Instance.bodyCollider.attachedRigidbody.velocity = Vector3.zero;
                            }
                        }
                        if (buttonsActive[1] == true)
                        {
                            InputDevices.GetDeviceAtXRNode(XRNode.LeftHand).TryGetFeatureValue(CommonUsages.primaryButton, out bool invisMonke);
                            if (PhotonNetwork.InRoom)
                            {
                                if (invisMonke)
                                {
                                    GorillaTagger.Instance.myVRRig.enabled = false;
                                    GorillaTagger.Instance.myVRRig.transform.position = new Vector3(Player.Instance.headCollider.transform.position.x, -646.46464f, Player.Instance.headCollider.transform.position.z);
                                }
                                else
                                {
                                    GorillaTagger.Instance.myVRRig.enabled = true;
                                }
                            }
                            else
                            {
                                Log(true, "NOT IN ROOM!");
                            }
                        }
                        if (buttonsActive[2] == true)
                        {
                            WMUtils.ModCheck = true;
                        }
                        else
                        {
                            WMUtils.ModCheck = false;
                        }
                        if (buttonsActive[3] == true)
                        {
                            InputDevices.GetDeviceAtXRNode(XRNode.LeftHand).TryGetFeatureValue(CommonUsages.primaryButton, out bool ghostMonke);
                            if (PhotonNetwork.InRoom)
                            {
                                if (ghostMonke)
                                {
                                    GorillaTagger.Instance.myVRRig.enabled = false;
                                }
                                else
                                {
                                    GorillaTagger.Instance.myVRRig.enabled = true;
                                }
                            }
                            else
                            {
                                Log(true, "NOT IN ROOM!");
                            }
                        }
                        if (buttonsActive[4] == true)
                        {
                            leftPlat.GetComponent<Renderer>().material.mainTexture = Resources.Load<Material>("objects/forest/materials/dirt").mainTexture;
                            leftPlat.GetComponent<Renderer>().material.SetColor("_Color", new Color(PlayerPrefs.GetFloat("redValue", 0), PlayerPrefs.GetFloat("greenValue", 0), PlayerPrefs.GetFloat("blueValue", 0)));
                            rightPlat.GetComponent<Renderer>().material.mainTexture = Resources.Load<Material>("objects/forest/materials/dirt").mainTexture;
                            rightPlat.GetComponent<Renderer>().material.SetColor("_Color", new Color(PlayerPrefs.GetFloat("redValue", 0), PlayerPrefs.GetFloat("greenValue", 0), PlayerPrefs.GetFloat("blueValue", 0)));
                            InputDevices.GetDeviceAtXRNode(XRNode.RightHand).TryGetFeatureValue(CommonUsages.triggerButton, out bool rightButton);
                            InputDevices.GetDeviceAtXRNode(XRNode.LeftHand).TryGetFeatureValue(CommonUsages.triggerButton, out bool leftButton);
                            if (rightButton) { if (!onceRightSecButton) { rightPlat.transform.position = Player.Instance.rightHandFollower.position + new Vector3(0, -0.05f, 0); onceRightSecButton = true; } } else { if (onceRightSecButton) { rightPlat.transform.position = new Vector3(0, -6464, 0); onceRightSecButton = false; } }
                            if (leftButton) { if (!onceLeftSecButton) { leftPlat.transform.position = Player.Instance.leftHandFollower.position + new Vector3(0, -0.05f, 0); onceLeftSecButton = true; } } else { if (onceLeftSecButton) { leftPlat.transform.position = new Vector3(0, -6464, 0); onceLeftSecButton = false; } }
                        }
                        if (buttonsActive[5] == true)
                        {
                            StartCoroutine(RefreshPhoton());
                            buttonsActive[5] = false;
                            GameObject.Destroy(menu);
                            menu = null;
                            Draw();
                        }
                        if (buttonsActive[6] == true)
                        {
                            buttonsActive[6] = false;
                            if (PhotonNetwork.InRoom)
                            {
                                Guid extension = Guid.NewGuid();
                                string time = DateTime.Now.ToString("h:mm:ss tt");
                                Log(false, "Wrote all player names and Id's to : " + Application.streamingAssetsPath + "/WM/players.wm." + PhotonNetwork.CurrentRoom.Name + "_" + extension.ToString().Substring(0, 4) + ".txt");
                                File.AppendAllText("WM/Players/players.wm." + PhotonNetwork.CurrentRoom.Name + "_" + extension.ToString().Substring(0, 4) + ".txt", time + ", PLAYERS IN ROOM CODE " + PhotonNetwork.CurrentRoom.Name + ": \n");
                                foreach (Photon.Realtime.Player plr in PhotonNetwork.PlayerList)
                                {
                                    File.AppendAllText("WM/Players/players.wm." + PhotonNetwork.CurrentRoom.Name + "_" + extension.ToString().Substring(0, 4) + ".txt", "Player Name : (" + WMUtils.NormalizeName(true, plr.NickName) + "), Player ID : (" + plr.UserId + "), Player Mods: ("+ plr.CustomProperties["mods"] +")\n");
                                }
                            }
                            GameObject.Destroy(menu);
                            menu = null;
                            Draw();
                        }
                        if (buttonsActive[7] == true)
                        {
                            PlayerPrefs.DeleteKey("tutorial");
                            buttonsActive[7] = false;
                            GameObject.Destroy(menu);
                            menu = null;
                            Draw();
                        }
                        if (buttonsActive[8] == true)
                        {
                            //theres a harmonypatch that does this anyway lmaoooooo
                        }
                        if (buttonsActive[9] == true)
                        {
                            InputDevices.GetDeviceAtXRNode(XRNode.RightHand).TryGetFeatureValue(CommonUsages.gripButton, out bool ironRight);
                            InputDevices.GetDeviceAtXRNode(XRNode.LeftHand).TryGetFeatureValue(CommonUsages.gripButton, out bool ironLeft);
                            if (ironRight)
                            {
                                Player.Instance.bodyCollider.attachedRigidbody.velocity += Player.Instance.rightHandTransform.right / 5;
                            }
                            if (ironLeft)
                            {
                                Player.Instance.bodyCollider.attachedRigidbody.velocity += -Player.Instance.leftHandTransform.right / 5;
                            }
                        }
                        if (buttonsActive[10] == true)
                        {
                            //theres a harmonypatch that does this anyway lmaoooooo part 2
                        }
                        if (buttonsActive[11] == true)
                        {
                            buttonsActive[11] = false;
                            GameObject.Destroy(menu);

                            normal = false;
                            troll = true;

                            menu = null;
                            Draw();
                        }

                        if (buttonsTrollActive[0] == true)
                        {
                            if (PhotonNetwork.InRoom)
                            {
                                GorillaTagger.Instance.myVRRig.head.rigTarget.eulerAngles = GorillaTagger.Instance.myVRRig.head.rigTarget.eulerAngles + new Vector3(180, 180, 0);
                            }
                            else
                            {
                                GorillaTagger.Instance.offlineVRRig.head.rigTarget.eulerAngles = GorillaTagger.Instance.offlineVRRig.head.rigTarget.eulerAngles + new Vector3(180, 180, 0);
                            }
                        }

                        if (buttonsTrollActive[1] == true)
                        {
                            if (PhotonNetwork.InRoom)
                            {
                                GorillaTagger.Instance.myVRRig.head.rigTarget.eulerAngles = new Vector3(UnityEngine.Random.Range(0, 360), UnityEngine.Random.Range(0, 360), UnityEngine.Random.Range(0, 360));
                                GorillaTagger.Instance.myVRRig.leftHand.rigTarget.eulerAngles = new Vector3(UnityEngine.Random.Range(0, 360), UnityEngine.Random.Range(0, 360), UnityEngine.Random.Range(0, 360));
                                GorillaTagger.Instance.myVRRig.rightHand.rigTarget.eulerAngles = new Vector3(UnityEngine.Random.Range(0, 360), UnityEngine.Random.Range(0, 360), UnityEngine.Random.Range(0, 360));
                            }
                            else
                            {
                                GorillaTagger.Instance.offlineVRRig.head.rigTarget.eulerAngles = new Vector3(UnityEngine.Random.Range(0, 360), UnityEngine.Random.Range(0, 360), UnityEngine.Random.Range(0, 360));
                                GorillaTagger.Instance.offlineVRRig.leftHand.rigTarget.eulerAngles = new Vector3(UnityEngine.Random.Range(0, 360), UnityEngine.Random.Range(0, 360), UnityEngine.Random.Range(0, 360));
                                GorillaTagger.Instance.offlineVRRig.rightHand.rigTarget.eulerAngles = new Vector3(UnityEngine.Random.Range(0, 360), UnityEngine.Random.Range(0, 360), UnityEngine.Random.Range(0, 360));
                            }
                        }

                        if (buttonsTrollActive[2] == true)
                        {
                            if (PhotonNetwork.InRoom)
                            {
                                GorillaTagger.Instance.myVRRig.head.trackingRotationOffset.y += 15;
                            }
                            else
                            {
                                GorillaTagger.Instance.offlineVRRig.head.trackingRotationOffset.y += 15;
                            }

                        }
                        else
                        {
                            if (PhotonNetwork.InRoom)
                            {
                                GorillaTagger.Instance.myVRRig.head.trackingRotationOffset.y = 0;
                            }
                            else
                            {
                                GorillaTagger.Instance.offlineVRRig.head.trackingRotationOffset.y = 0;
                            }
                        }
                        if (buttonsTrollActive[3] == true)
                        {
                            RaycastHit hit;
                            InputDevices.GetDeviceAtXRNode(XRNode.RightHand).TryGetFeatureValue(CommonUsages.triggerButton, out bool attachToPlayer);
                            InputDevices.GetDeviceAtXRNode(XRNode.RightHand).TryGetFeatureValue(CommonUsages.primaryButton, out bool detachFromPlayer);
                            if (Physics.Raycast(GorillaTagger.Instance.offlineVRRig.rightHand.rigTarget.transform.position, GorillaTagger.Instance.offlineVRRig.rightHand.rigTarget.transform.up * 20, out hit, Mathf.Infinity, LayerMask.GetMask("Gorilla Tag Collider")))
                            {
                                pointer.material.color = new Color(1, 1, 1);
                                pointer.SetPositions(new Vector3[] {
                                    GorillaTagger.Instance.offlineVRRig.rightHand.rigTarget.transform.position,
                                    hit.point
                                });
                                if (attachToPlayer)
                                {
                                    Player.Instance.enabled = false;
                                    playerToFollow = hit.transform.gameObject;
                                    Player.Instance.enabled = true;
                                }
                            }
                            else
                            {
                                pointer.material.color = new Color(PlayerPrefs.GetFloat("redValue", 0), PlayerPrefs.GetFloat("greenValue", 0), PlayerPrefs.GetFloat("blueValue", 0));
                                pointer.SetPositions(new Vector3[] {
                                    GorillaTagger.Instance.offlineVRRig.rightHand.rigTarget.transform.position,
                                    GorillaTagger.Instance.offlineVRRig.rightHand.rigTarget.transform.position+GorillaTagger.Instance.offlineVRRig.rightHand.rigTarget.transform.up * 999
                                });
                            }
                            if (playerToFollow != null)
                            {
                                Player.Instance.transform.position = playerToFollow.transform.position - Player.Instance.bodyCollider.transform.position + Player.Instance.transform.position + new Vector3(0, 1.5f, 0);
                                Player.Instance.bodyCollider.attachedRigidbody.velocity = new Vector3(0, 1, 0);
                            }
                            if (detachFromPlayer)
                            {
                                playerToFollow = null;
                            }
                        }
                        else
                        {
                            playerToFollow = null;
                            pointer.SetPositions(new Vector3[]
                            {
                                    Vector3.zero,
                                    Vector3.zero
                            });
                        }
                        if (buttonsTrollActive[4] == true)
                        {
                            InputDevices.GetDeviceAtXRNode(XRNode.RightHand).TryGetFeatureValue(CommonUsages.triggerButton, out bool duck);
                            if (duck && !onceDuck)
                            {
                                if (GorillaTagger.Instance.myVRRig != null)
                                {
                                    PhotonView.Get(GorillaTagger.Instance.myVRRig).RPC("PlayHandTap", RpcTarget.All, new object[]
                                    {
                                    75,
                                    false,
                                    1f
                                    });
                                }
                                else
                                {
                                    GorillaTagger.Instance.offlineVRRig.PlayHandTapLocal(75, false, 1f);
                                }
                                GorillaTagger.Instance.StartVibration(false, 0.2f, Time.deltaTime);
                            }
                            if (!duck && onceDuck)
                            {
                                if (GorillaTagger.Instance.myVRRig != null)
                                {
                                    PhotonView.Get(GorillaTagger.Instance.myVRRig).RPC("PlayHandTap", RpcTarget.Others, new object[]
                                    {
                                    76,
                                    false,
                                    1f
                                    });
                                }
                                else
                                {
                                    GorillaTagger.Instance.offlineVRRig.PlayHandTapLocal(76, false, 1f);
                                }
                                GorillaTagger.Instance.StartVibration(false, 0.2f, Time.deltaTime);
                            }
                            onceDuck = duck;
                        }
                        if (buttonsTrollActive[5] == true)
                        {
                            InputDevices.GetDeviceAtXRNode(XRNode.RightHand).TryGetFeatureValue(CommonUsages.triggerButton, out bool spam);
                            if (spam)
                            {
                                if (GorillaTagger.Instance.myVRRig != null)
                                {
                                    Risky.SoundSpam();
                                    Risky.SoundSpam();
                                    Risky.SoundSpam();
                                    Risky.SoundSpam();
                                }
                            }
                        }
                        if (buttonsTrollActive[11] == true)
                        {
                            buttonsTrollActive[11] = false;
                            GameObject.Destroy(menu);

                            normal = true;
                            troll = false;

                            menu = null;
                            Draw();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                File.AppendAllText("error_log.wmError", e.ToString());
            }
        }
        IEnumerator RefreshPhoton()
        {
            PhotonNetworkController.Instance.FullDisconnect();
            GorillaComputer.instance.screenText.Text = "RECONNECTING...";
            GorillaTagger.Instance.offlineVRRig.PlayHandTapLocal(68,false,1);
            GorillaTagger.Instance.offlineVRRig.PlayHandTapLocal(68,true,1);
            yield return new WaitForSeconds(0.3f);
            PhotonNetworkController.Instance.InitiateConnection();
        }
        static void AddButton(float offset, string text)
        {
            GameObject newBtn = GameObject.CreatePrimitive(PrimitiveType.Cube);
            newBtn.GetComponent<Renderer>().enabled = false;
            GameObject.Destroy(newBtn.GetComponent<Rigidbody>());
            newBtn.GetComponent<BoxCollider>().isTrigger = true;
            newBtn.transform.parent = menu.transform;
            newBtn.transform.rotation = Quaternion.identity;
            newBtn.transform.localScale = new Vector3(0.09f, 0.8f, 0.08f);
            newBtn.transform.localPosition = new Vector3(0.56f, 0f, 0.28f - offset);
            newBtn.AddComponent<BtnCollider>().relatedText = text;
            int index = -1;
            if (normal)
            {
                for (int i = 0; i < buttons.Length; i++)
                {
                    if (text == buttons[i])
                    {
                        index = i;
                        break;
                    }
                }
            }
            if (troll)
            {
                for (int i = 0; i < buttonsTroll.Length; i++)
                {
                    if (text == buttonsTroll[i])
                    {
                        index = i;
                        break;
                    }
                }
            }

            GameObject titleObj = new GameObject();
            titleObj.transform.parent = canvasObj.transform;
            Text title = titleObj.AddComponent<Text>();
            title.font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
            title.fontStyle = FontStyle.Bold;
            title.text = "< " + text + " >";
            title.fontSize = 1;
            title.alignment = TextAnchor.MiddleCenter;
            title.resizeTextForBestFit = true;
            title.resizeTextMinSize = 0;
            RectTransform titleTransform = title.GetComponent<RectTransform>();
            titleTransform.localPosition = Vector3.zero;
            titleTransform.sizeDelta = new Vector2(0.35f, 0.03f);
            titleTransform.localPosition = new Vector3(0.058f, 0f, 0.111f - (offset / 2.55f));
            titleTransform.rotation = Quaternion.Euler(new Vector3(180f, 90f, 90f));

            if (normal)
            {
                if (buttonsActive[index] == false)
                {
                    title.color = Color.white;
                }
                else if (buttonsActive[index] == true)
                {
                    title.color = Color.cyan;
                }
                else
                {
                    title.color = Color.red;
                }
            }
            if (troll)
            {
                if (buttonsTrollActive[index] == false)
                {
                    title.color = Color.white;
                }
                else if (buttonsTrollActive[index] == true)
                {
                    title.color = Color.cyan;
                }
                else
                {
                    title.color = Color.red;
                }
            }
        }

        public static void Draw()
        {
            Log(false, "Opening Menu.");
            menu = GameObject.CreatePrimitive(PrimitiveType.Cube);
            GameObject.Destroy(menu.GetComponent<Rigidbody>());
            GameObject.Destroy(menu.GetComponent<BoxCollider>());
            GameObject.Destroy(menu.GetComponent<Renderer>());
            menu.transform.localScale = new Vector3(0.1f, 0.3f, 0.4f);

            GameObject background = GameObject.CreatePrimitive(PrimitiveType.Cube);
            GameObject.Destroy(background.GetComponent<Rigidbody>());
            GameObject.Destroy(background.GetComponent<BoxCollider>());
            background.transform.parent = menu.transform;
            background.transform.rotation = Quaternion.identity;
            background.transform.localScale = new Vector3(0.1f, 1.2f, 1.4f);
            background.GetComponent<Renderer>().material.mainTexture = Resources.Load<Material>("objects/forest/materials/dirt").mainTexture;
            background.GetComponent<Renderer>().material.SetColor("_Color", new Color(PlayerPrefs.GetFloat("redValue",0), PlayerPrefs.GetFloat("greenValue", 0), PlayerPrefs.GetFloat("blueValue", 0)));
            background.transform.position = new Vector3(0.05f, 0f, -0.04f);

            canvasObj = new GameObject();
            canvasObj.transform.parent = menu.transform;
            Canvas canvas = canvasObj.AddComponent<Canvas>();
            CanvasScaler canvasScale = canvasObj.AddComponent<CanvasScaler>();
            canvasObj.AddComponent<GraphicRaycaster>();
            canvas.renderMode = RenderMode.WorldSpace;
            canvasScale.dynamicPixelsPerUnit = 1000;

            GameObject titleObj = new GameObject();
            titleObj.transform.parent = canvasObj.transform;
            Text title = titleObj.AddComponent<Text>();
            title.font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
            title.fontStyle = FontStyle.BoldAndItalic;
            if (normal)
            {
                Log(false, "Mode : Normal.");
                title.text = "< WM NORMAL >";
            }
            if (troll)
            {
                Log(false, "Mode : Troll.");
                title.text = "< WM TROLL >";
            }
            title.fontSize = 1;
            title.alignment = TextAnchor.MiddleCenter;
            title.resizeTextForBestFit = true;
            title.resizeTextMinSize = 0;
            RectTransform titleTransform = title.GetComponent<RectTransform>();
            titleTransform.localPosition = Vector3.zero;
            titleTransform.sizeDelta = new Vector2(0.28f, 0.05f);
            titleTransform.position = new Vector3(0.06f, 0f, 0.205f);
            titleTransform.rotation = Quaternion.Euler(new Vector3(180f, 90f, 90f));

            if (normal)
            {
                Log(false, "Inserting Normal Buttons.");
                for (int i = 0; i < buttons.Length; i++)
                {
                    AddButton(i * 0.091f, buttons[i]);
                }
            }
            if (troll)
            {
                Log(false, "Inserting Troll Buttons.");
                for (int i = 0; i < buttonsTroll.Length; i++)
                {
                    AddButton(i * 0.091f, buttonsTroll[i]);
                }
            }
        }

        public static void Toggle(string relatedText)
        {
            int index = -1;
            if (normal)
            {
                for (int i = 0; i < buttons.Length; i++)
                {
                    if (relatedText == buttons[i])
                    {
                        index = i;
                        break;
                    }
                }

                if (buttonsActive[index] != null)
                {
                    buttonsActive[index] = !buttonsActive[index];

                    GameObject.Destroy(menu);
                    menu = null;
                    Draw();
                }
            }
            if (troll)
            {
                for (int i = 0; i < buttonsTroll.Length; i++)
                {
                    if (relatedText == buttonsTroll[i])
                    {
                        index = i;
                        break;
                    }
                }

                if (buttonsTrollActive[index] != null)
                {
                    buttonsTrollActive[index] = !buttonsTrollActive[index];

                    GameObject.Destroy(menu);
                    menu = null;
                    Draw();
                }
            }
        }
        public static void Log(bool err, string msg)
        {
            if (err)
            {
                Debug.LogError("< WM ERROR > : " + msg);
            }
            else
            {
                Debug.Log("< WM LOG > : " + msg);
            }
        }

        public IEnumerator WaitSeconds(float seconds)
        {
            yield return new WaitForSeconds(seconds);
        }

        [HarmonyPatch(typeof(GorillaLocomotion.Player), "GetSlidePercentage")]
        class NoSlip
        {
            static void Postfix(ref float __result)
            {
                if (buttonsActive[8] == true)
                {
                    __result = 0f;
                }
            }
        }
        [HarmonyPatch(typeof(GorillaLocomotion.Player), "GetSlidePercentage")]
        class Slip
        {
            static void Postfix(ref float __result)
            {
                if (buttonsActive[10] == true)
                {
                    __result = 1f;
                }
            }
        }
    }
    class BtnCollider : MonoBehaviour
    {
        public string relatedText;

        private void OnTriggerEnter(Collider collider)
        {
            Debug.Log("collision detected" + collider, collider);
            if (Time.frameCount >= WMMenu.framePressCooldown + 30)
            {
                Debug.Log("buttan press");
                GorillaTagger.Instance.StartVibration(false, GorillaTagger.Instance.tapHapticStrength / 2f, GorillaTagger.Instance.tapHapticDuration);
                GorillaTagger.Instance.offlineVRRig.PlayHandTapLocal(67, false, .5f);
                /*
                if (PhotonNetwork.InRoom && GorillaTagger.Instance.myVRRig != null)
                {
                    Photon.Pun.PhotonView.Get(GorillaTagger.Instance.myVRRig).RPC("PlayHandTap", Photon.Pun.RpcTarget.Others, new object[]
                    {
                            67,
                            false,
                            .5f
                    });
                }
                */
                WMMenu.Toggle(relatedText);
                WMMenu.framePressCooldown = Time.frameCount;
            }
        }
    }

    [HarmonyPatch(typeof(Player))]
    [HarmonyPatch("Awake", MethodType.Normal)]
    class Startup
    {
        static void Prefix(Player __instance)
        {
            WMMenu.pointer = __instance.gameObject.AddComponent<LineRenderer>();
            WMMenu.pointer.material = new Material(Shader.Find("Standard"));
            WMMenu.pointer.material.color = new Color(PlayerPrefs.GetFloat("redValue", 0), PlayerPrefs.GetFloat("greenValue", 0), PlayerPrefs.GetFloat("blueValue", 0));
            WMMenu.pointer.startWidth = 0.02f;
            WMMenu.pointer.endWidth = 0.02f;
            WMMenu.Log(false, "Startup Patch Starting.");
            WMMenu.Log(false, "Creating Platforms.");
            WMMenu.leftPlat = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            WMMenu.leftPlat.name = "< WM LEFT PLATFORM >";
            UnityEngine.Object.Destroy(WMMenu.leftPlat.GetComponent<SphereCollider>());
            WMMenu.leftPlat.AddComponent<BoxCollider>();
            WMMenu.leftPlat.GetComponent<Renderer>().material.SetColor("_Color", Color.white);
            WMMenu.leftPlat.transform.localScale = new Vector3(0.4f, 0.01f, 0.4f);
            WMMenu.rightPlat = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            WMMenu.rightPlat.name = "< WM RIGHT PLATFORM >";
            UnityEngine.Object.Destroy(WMMenu.rightPlat.GetComponent<SphereCollider>());
            WMMenu.rightPlat.AddComponent<BoxCollider>();
            WMMenu.rightPlat.GetComponent<Renderer>().material.SetColor("_Color", Color.white);
            WMMenu.rightPlat.transform.localScale = new Vector3(0.4f, 0.01f, 0.4f);
            WMMenu.Log(false, "Created Platforms.");
            WMMenu.Log(false, "Creating Pointer.");
        }
    }
}
