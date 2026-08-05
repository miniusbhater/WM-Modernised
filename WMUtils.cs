using BepInEx;
using System.Collections;
using System.Timers;
using System;
using UnityEngine.UI;
using System.Linq;
using GorillaNetworking;
using UnityEngine;
using Photon.Pun;
using Utilla;

namespace WM
{
    [BepInPlugin(PluginInfo.GUID + ".utils", PluginInfo.Name + " UTILS", PluginInfo.Version)]
    public class WMUtils : BaseUnityPlugin
    {
        static Material playerColorMat = new Material(Shader.Find("Standard"));
        private static Timer computerMaterialTimer;
        private static Timer LoopJoinTimer;
        public static bool ModCheck = false;
        public static bool trackConfirmation = false;
        public static bool PleaseWait = false;
        public string playerToTrack = "";
        public static string roomToJoin = "";
        public void Awake()
        {
            Events.GameInitialized += GameInitialized;
        }
        static void SetScreens()
        {
            foreach (GorillaLevelScreen i in GorillaComputer.instance.levelScreens)
            {
                i.goodMaterial = playerColorMat;
            }
            GameObject.Find("Level/forest/lower level/StaticUnlit/screen").GetComponent<MeshRenderer>().material = playerColorMat;
            GameObject.Find("Level/forest/lower level/StaticUnlit/motdscreen").GetComponent<MeshRenderer>().material = playerColorMat;
            GameObject.Find("Level/forest/lower level/StaticUnlit/screen").GetComponent<MeshRenderer>().sharedMaterial = playerColorMat;
            GameObject.Find("Level/forest/lower level/StaticUnlit/motdscreen").GetComponent<MeshRenderer>().sharedMaterial = playerColorMat;
            GameObject.Find("Level/forest/campgroundstructure/scoreboard/REMOVE board").GetComponent<MeshRenderer>().material = playerColorMat;
            GameObject.Find("Level/forest/campgroundstructure/scoreboard/REMOVE board").GetComponent<MeshRenderer>().sharedMaterial = playerColorMat;
            GorillaNetworking.GorillaComputer.instance.computerScreenRenderer.material = playerColorMat;
            GorillaNetworking.GorillaComputer.instance.computerScreenRenderer.sharedMaterial = playerColorMat;
        }
        private void GameInitialized(object sender, EventArgs e)
        {
            computerMaterialTimer = new Timer();
            computerMaterialTimer.Elapsed += new ElapsedEventHandler(SetComputerMats);
            computerMaterialTimer.Interval = 200;
            computerMaterialTimer.Enabled = true;
            LoopJoinTimer = new Timer();
            LoopJoinTimer.Elapsed += new ElapsedEventHandler(LoopJoin);
            LoopJoinTimer.Interval = 1500;
            LoopJoinTimer.Enabled = false;
        }
        private static void SetComputerMats(object source, ElapsedEventArgs e)
        {
            playerColorMat.color = new Color(PlayerPrefs.GetFloat("redValue", 0)/2f, PlayerPrefs.GetFloat("greenValue", 0)/2f, PlayerPrefs.GetFloat("blueValue", 0)/2f);
            SetScreens();
        }
        private static void LoopJoin(object source, ElapsedEventArgs e)
        {
            GorillaNetworking.PhotonNetworkController.Instance.AttemptToJoinSpecificRoom(roomToJoin);
        }
        public void OnGUI()
        {
            GUI.backgroundColor = GUI.color = new Color(PlayerPrefs.GetFloat("redValue", 0), PlayerPrefs.GetFloat("greenValue", 0), PlayerPrefs.GetFloat("blueValue", 0));
            GUI.color = new Color(PlayerPrefs.GetFloat("redValue", 0), PlayerPrefs.GetFloat("greenValue", 0), PlayerPrefs.GetFloat("blueValue", 0));
            if (PhotonNetwork.InRoom)
            {
                GUI.Label(new Rect(220, 0, 999, 30), "Current Room : " + PhotonNetwork.CurrentRoom.Name);
            }
            else
            {
                GUI.Label(new Rect(220, 0, 999, 30), "Current Room : NONE");
            }
            playerToTrack = GUI.TextField(new Rect(20, 20, 200, 30), playerToTrack, 999);
            roomToJoin = GUI.TextField(new Rect(220, 20, 200, 30), roomToJoin, 999);
            if (GUI.Button(new Rect(20, 60, 200, 30), "Get Player Info"))
            {
                if (!PleaseWait)
                {
                    trackConfirmation = false;
                    PlayerTracker.TrackPlayer(new string[] { playerToTrack });
                    StartCoroutine(ShowConfirmation());
                }
            }
            if (GUI.Button(new Rect(220, 60, 100, 30), "Join Room"))
            {
                GorillaNetworking.PhotonNetworkController.Instance.AttemptToJoinSpecificRoom(roomToJoin);
            }
            GUI.Label(new Rect(440, 60, 999, 999), "Loop Join Enabled : " + LoopJoinTimer.Enabled);
            if (GUI.Button(new Rect(320, 60, 100, 30), "Toggle Loop"))
            {
                LoopJoinTimer.Enabled = !LoopJoinTimer.Enabled;
            }
            if (GUI.Button(new Rect(220, 90, 200, 30), "Disconnect"))
            {
                GorillaNetworking.PhotonNetworkController.Instance.AttemptDisconnect();
            }
            if (PleaseWait)
            {
                trackConfirmation = false;
                GUI.Label(new Rect(20, 100, 200, 300), "Connected to Master, Please wait.");
            }
            if (trackConfirmation)
            {
                GUI.Label(new Rect(20, 100, 200, 300), "Look at BepInEx Console for player info. Retry if unsuccessful.");
            }
            if (ModCheck)
            {
                if (PhotonNetwork.InRoom)
                {
                    GUILayout.BeginVertical();
                    foreach (Photon.Realtime.Player p in PhotonNetwork.PlayerList)
                    {
                        GUILayout.Label("<b>( USERID : <color=green>" + p.UserId + "</color>, NAME : <color=cyan>" + p.NickName + "</color>, UTILLA MODS GUIDS : <color=yellow>" + p.CustomProperties["mods"] + "</color>)</b>\n");
                    }
                    GUILayout.EndHorizontal();
                }
            }
        }
        public IEnumerator ShowConfirmation()
        {
            yield return new WaitForSeconds(.1f);
            trackConfirmation = true;
            yield return new WaitForSeconds(4f);
            trackConfirmation = false;
        }
        public void Update()
        {
            VRRig[] vrRigs = (VRRig[])GameObject.FindObjectsOfType(typeof(VRRig));
            foreach (VRRig rig in vrRigs)
            {
                if (!rig.isOfflineVRRig && !rig.isMyPlayer && !rig.photonView.IsMine)
                {
                    rig.playerText.text = NormalizeName(true, rig.photonView.Owner.NickName) + "\nUSERID: "+rig.photonView.Owner.UserId;
                }
            }
        }
        public static string NormalizeName(bool doIt, string text)
        {
            if (doIt)
            {
                text = new string(Array.FindAll<char>(text.ToCharArray(), (char c) => char.IsLetterOrDigit(c)));
                if (text.Length > 12)
                {
                    text = text.Substring(0, 10);
                }
                text = text.ToUpper();
            }
            return text;
        }
    }
}
