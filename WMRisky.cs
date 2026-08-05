using Photon.Pun;
using Photon.Realtime;
using GorillaNetworking;
using ExitGames.Client.Photon;

namespace WM
{
    class Risky
    {
        public static void SoundSpam()
        {
            PhotonView.Get(GorillaTagger.Instance.myVRRig).RPC("PlayHandTap", RpcTarget.All, new object[] { 40, false, 1f });
            PhotonView.Get(GorillaTagger.Instance.myVRRig).RPC("PlayHandTap", RpcTarget.All, new object[] { 40, true, 1f });
        }
        public static void RiskyBusiness()
        {
            GorillaGameManager.instance = null;
        }
    }
}
