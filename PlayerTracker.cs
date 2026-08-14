// not a chance XD
/*
 * SUCCESSFUL ATTEMPTS BETA:
 * FES (RANDOM GUY IN A MODDED PUB)
 * MERCYVR (YOUTUBER)
*/
/*
Console.WriteLine("-------------- WM PLAYER TRACKER INFO --------------");
Console.WriteLine("Player ID: " + player.UserId);
Console.WriteLine("Player Is Online: " + player.IsOnline);
Console.WriteLine("Player Is In A Room: " + player.IsInRoom);
Console.WriteLine("Player Room: " + player.Room);
Console.WriteLine("-------------- WM PLAYER TRACKER INFO --------------" + Environment.NewLine);
*//*

using System.Timers;
using System.Collections.Generic;
using System.Collections;
using System;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using Photon.Realtime;
using Photon.Pun;

namespace WM
{
    public class PlayerTracker : MonoBehaviourPunCallbacks
    {
        //Top to bottom discord tracking targets
        public static Timer TrackerTimer;
        public static bool OnGUIEnabled = false;
        public static GameObject thing;
        public void Start()
        {
            GameObject.Find("CodeOfConduct").GetComponent<Text>().text = "<color=cyan>< WM PLAYER TRACKER ></color>";
            GameObject.Find("COC Text").GetComponent<Text>().text = "<color=cyan>LOADING PLAYERTRACKER PLEASE WAIT!!!</color>";
            GameObject.Find("COC Text").GetComponent<Text>().lineSpacing = GameObject.Find("Level/forest/lower level/UI/Tree Room Texts/COC Text").GetComponent<Text>().lineSpacing / 2;
            *//*
            for (int i = 0; i < 5; i++)
            {
                GameObject gameobject = Instantiate(GameObject.Find("Level/forest/lower level/UI/Tree Room Texts/COC Text"));
                gameobject.transform.parent = GameObject.Find("Level/forest/lower level/UI/Tree Room Texts/COC Text").transform.parent;
                gameobject.transform.position += -gameobject.transform.up / 2;
            }
            *//*
            Directory.CreateDirectory(@"WM\Players");
            TrackerTimer = new Timer();
            TrackerTimer.Elapsed += new ElapsedEventHandler(CheckTrackedPlayer);
            TrackerTimer.Interval = 1500;
            TrackerTimer.Enabled = true;
        }
        public static void CheckTrackedPlayer(object source, ElapsedEventArgs e)
        {
            OnGUIEnabled = false;
            //Console.WriteLine("WM PLAYERTRACKER => TRACKING PREDETERMINED PLAYERS");
            if (PhotonNetwork.InRoom)
            {
                GameObject.Find("Level/forest/lower level/UI/Tree Room Texts/COC Text").GetComponent<Text>().text = "<color=lime>YOU ARE IN A ROOM, DISCONNECT TO SEARCH FOR TRACKED PLAYERS...</color>";
            }
            else
            {
                PhotonNetwork.FindFriends(preDeterminedPlayers);
            }
        }
        public static void TrackPlayer(string[] players)
        {
            OnGUIEnabled = true;
            PhotonNetwork.FindFriends(players);
        }
        public override void OnConnectedToMaster()
        {
            base.OnConnectedToMaster();
            Log("Connected to master.");
            PhotonNetwork.JoinLobby(TypedLobby.Default);
            StartCoroutine(WM_WaitForSeconds(.8f));
        }
        public override void OnJoinedRoom()
        {
            base.OnJoinedRoom();
            Guid extension = Guid.NewGuid();
            string time = DateTime.Now.ToString("h:mm:ss tt");
            File.AppendAllText("WM/Players/players.auto.wm." + Photon.Pun.PhotonNetwork.CurrentRoom.Name + "_" + extension.ToString().Substring(0, 4) + ".txt", time + ", PLAYERS IN ROOM CODE " + Photon.Pun.PhotonNetwork.CurrentRoom.Name + ": \n");
            foreach (Player plr in PhotonNetwork.PlayerList)
            {
                File.AppendAllText("WM/Players/players.auto.wm." + Photon.Pun.PhotonNetwork.CurrentRoom.Name + "_" + extension.ToString().Substring(0, 4) + ".txt", "Player Name : (" + WMUtils.NormalizeName(true, plr.NickName) + "), Player ID : (" + plr.UserId + "), Player Mods: (" + plr.CustomProperties["mods"] + ")\n");
            }
        }
        public override void OnLeftRoom()
        {
            base.OnLeftRoom();
            GameObject.Find("Level/forest/lower level/UI/Tree Room Texts/COC Text").GetComponent<Text>().text = "<color=cyan>LOADING PLAYERTRACKER PLEASE WAIT!!!</color>";
        }
        public string CheckOnline(bool online)
        {
            if (online.ToString().ToUpper() == "FALSE")
            {
                return "<color=red>OFFLINE</color>";
            }
            else if (online.ToString().ToUpper() == "TRUE")
            {
                return "<color=lime>ONLINE</color>";
            }
            else
            {
                return "<color=red>OFFLINE</color>";
            }
        }
        public string CheckRoom(string Room)
        {
            if (Room == "")
            {
                return "<color=grey>NONE</color>";
            }
            else
            {
                return "<color=cyan>"+Room+"</color>";
            }
        }

        public void ReplaceTextForTrackedPlayer(string number,string nameToReplace, bool presenceToReplace, string codeToReplace)
        {
            GameObject.Find("Level/forest/lower level/UI/Tree Room Texts/COC Text").GetComponent<Text>().text = 
                GameObject.Find("Level/forest/lower level/UI/Tree Room Texts/COC Text").GetComponent<Text>().text.Replace("PLAYERNAME"+ number, nameToReplace);
            //Log("Replaced "+"PLAYERNAME"+number+" WITH "+nameToReplace);
            GameObject.Find("Level/forest/lower level/UI/Tree Room Texts/COC Text").GetComponent<Text>().text = 
                GameObject.Find("Level/forest/lower level/UI/Tree Room Texts/COC Text").GetComponent<Text>().text.Replace("OFFLINE"+ number, CheckOnline(presenceToReplace));
            //Log("Replaced " + "OFFLINE" + number + " WITH " + CheckOnline(presenceToReplace));
            GameObject.Find("Level/forest/lower level/UI/Tree Room Texts/COC Text").GetComponent<Text>().text = 
                GameObject.Find("Level/forest/lower level/UI/Tree Room Texts/COC Text").GetComponent<Text>().text.Replace("CODE"+ number, CheckRoom(codeToReplace));
            //Log("Replaced " + "CODE" + number + " WITH " + CheckRoom(codeToReplace));
        }
        //ADD IDS HERE ADD IDS HERE ADD IDS HERE ADD IDS HERE ADD IDS HERE ADD IDS HERE ADD IDS HERE 
        public static string[] preDeterminedPlayers = { "458CCE7845335ABF", "4CC650026E063F88", "E354E818871BD1D8", "TOREPLACE", "81CA004411B03454", "CC24059E8F10EF1F", "FDCBA2A2AA273C2F", "A48ECC7AFC42BFF9", "DA7574098B62F071", "26BAB92F114978B2", "5AA1231973BE8A62", "6713DA80D2E9BFB5", "7BA61B65CD132F81", "5ED307F3DB6C89B6", "3E833C6953F44B18", "E0E440B3993CB6CF", "D18CAB17B18F8DDD" };
        //ADD IDS HERE ADD IDS HERE ADD IDS HERE ADD IDS HERE ADD IDS HERE ADD IDS HERE ADD IDS HERE 
        public override void OnFriendListUpdate(List<FriendInfo> friendList)
        {
            base.OnFriendListUpdate(friendList);
            //Log("Got Friend List Update!");
            //Log(friendList.ToStringFull());
            GameObject.Find("Level/forest/lower level/UI/Tree Room Texts/COC Text").GetComponent<Text>().text = string.Concat
                (
                "PLAYERNAME1 IS OFFLINE1 IN ROOM: CODE1\n",
                "PLAYERNAME2 IS OFFLINE2 IN ROOM: CODE2\n",
                "PLAYERNAME3 IS OFFLINE3 IN ROOM: CODE3\n",
                "PLAYERNAME4 IS OFFLINE4 IN ROOM: CODE4\n",
                "PLAYERNAME5 IS OFFLINE5 IN ROOM: CODE5\n",
                "PLAYERNAME6 IS OFFLINE6 IN ROOM: CODE6\n",
                "PLAYERNAME7 IS OFFLINE7 IN ROOM: CODE7\n",
                "PLAYERNAME8 IS OFFLINE8 IN ROOM: CODE8\n",
                "PLAYERNAME9 IS OFFLINE9 IN ROOM: CODE9\n",
                "PLAYERNAMEa IS OFFLINEa IN ROOM: CODEa\n",
                "PLAYERNAMEb IS OFFLINEb IN ROOM: CODEb\n",
                "PLAYERNAMEc IS OFFLINEc IN ROOM: CODEc\n",
                "PLAYERNAMEd IS OFFLINEd IN ROOM: CODEd\n",
                "PLAYERNAMEe IS OFFLINEe IN ROOM: CODEe\n",
                "PLAYERNAMEf IS OFFLINEf IN ROOM: CODEf\n",
                "PLAYERNAMEg IS OFFLINEg IN ROOM: CODEg\n",
                "PLAYERNAMEh IS OFFLINEh IN ROOM: CODEh\n",
                "\n\n<color=red>THERES AN ISSUE WHERE THE TRACKER STOPS WORKING COMPLETELY, IF THIS HAPPENS, PLEASE WAIT FOR IT TO START WORKING AGAIN, UNKNOWN BUG WHICH I CANT SEEM TO FIX.</color>"
                );
            foreach (FriendInfo player in friendList)
            {
                if (OnGUIEnabled)
                {
                    Console.WriteLine("-------------- WM PLAYER TRACKER INFO --------------");
                    Console.WriteLine("Player ID: " + player.UserId);
                    Console.WriteLine("Player Is Online: " + player.IsOnline);
                    Console.WriteLine("Player Is In A Room: " + player.IsInRoom);
                    Console.WriteLine("Player Room: " + player.Room);
                    Console.WriteLine("-------------- WM PLAYER TRACKER INFO --------------" + Environment.NewLine);
                    OnGUIEnabled = false;
                }
                else
                {
                    switch (player.UserId)
                    {
                        case "458CCE7845335ABF": //ETHYB
                            ReplaceTextForTrackedPlayer("1", "ETHYB", player.IsOnline, player.Room);
                            break;
                        case "4CC650026E063F88": //OVERSEER
                            ReplaceTextForTrackedPlayer("2", "OVERSEER", player.IsOnline, player.Room);
                            break;
                        case "E354E818871BD1D8": //DEVTHEYTHEM
                            ReplaceTextForTrackedPlayer("3", "DEVTHEYTHEM", player.IsOnline, player.Room);
                            break;
                        case "TOREPLACE": //
                            ReplaceTextForTrackedPlayer("4", "TOREPLACE", player.IsOnline, player.Room);
                            break;
                        case "81CA004411B03454": //MERCYVR
                            ReplaceTextForTrackedPlayer("5", "MERCYVR", player.IsOnline, player.Room);
                            break;
                        case "CC24059E8F10EF1F": //TINOTIN
                            ReplaceTextForTrackedPlayer("6", "TINOTIN", player.IsOnline, player.Room);
                            break;
                        case "FDCBA2A2AA273C2F": //NBTSUBZER0
                            ReplaceTextForTrackedPlayer("7", "NBTSUBZER0", player.IsOnline, player.Room);
                            break;
                        case "A48ECC7AFC42BFF9": //LIAMM OCULUS
                            ReplaceTextForTrackedPlayer("8", "LIAMM QUEST", player.IsOnline, player.Room);
                            break;
                        case "DA7574098B62F071": //LIAMM OCULUS
                            ReplaceTextForTrackedPlayer("9", "LIAMM PC", player.IsOnline, player.Room);
                            break;
                        case "26BAB92F114978B2": //PICKLENICK
                            ReplaceTextForTrackedPlayer("a", "PICKLENICK", player.IsOnline, player.Room);
                            break;
                        case "5AA1231973BE8A62": //NOV | APOLLO
                            ReplaceTextForTrackedPlayer("b", "NOV | APOLLO", player.IsOnline, player.Room);
                            break;
                        case "6713DA80D2E9BFB5": //A HAUNTED ARMY
                            ReplaceTextForTrackedPlayer("c", "A HAUNTED ARMY", player.IsOnline, player.Room);
                            break;
                        case "7BA61B65CD132F81": //SPRAYSIX
                            ReplaceTextForTrackedPlayer("d", "SPRAYSIX", player.IsOnline, player.Room);
                            break;
                        case "5ED307F3DB6C89B6": //ZODOLORD
                            ReplaceTextForTrackedPlayer("e", "ZODOLORD", player.IsOnline, player.Room);
                            break;
                        case "3E833C6953F44B18": //DIAMOND
                            ReplaceTextForTrackedPlayer("f", "DIAMOND", player.IsOnline, player.Room);
                            break;
                        case "E0E440B3993CB6CF": //TINYBEAR
                            ReplaceTextForTrackedPlayer("g", "TINYBEAR", player.IsOnline, player.Room);
                            break;
                        case "D18CAB17B18F8DDD": //CATOVR
                            ReplaceTextForTrackedPlayer("h", "CATOVR", player.IsOnline, player.Room);
                            break;
                    }
                }
            }
        }
        public void Log(string text)
        {
            Console.WriteLine("WM PLAYERTRACKER => " + text);
        }
        public IEnumerator WM_WaitForSeconds(float seconds)
        {
            WMUtils.PleaseWait = true;
            yield return new WaitForSeconds(seconds);
            WMUtils.PleaseWait = false;
        }
    }
}*/