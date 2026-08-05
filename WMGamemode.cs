using Utilla;
using BepInEx;

namespace WM
{
    [BepInPlugin(PluginInfo.GUID+".gamemode",PluginInfo.Name+" GAMEMODE",PluginInfo.Version)]
    [BepInDependency("org.legoandmars.gorillatag.utilla", "1.5.0")] // Make sure to add Utilla 1.5.0 as a dependency!
    [ModdedGamemode("wm", "WM", Utilla.Models.BaseGamemode.Casual)] // Enable callbacks in a new custom gamemode using MyGameManager
    public class WMGamemode : BaseUnityPlugin { }
}
