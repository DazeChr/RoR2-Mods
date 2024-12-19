using System;
using System.Globalization;
using BepInEx;
using HarmonyLib;
using RoR2;
using RoR2.UI;
using UnityEngine;

namespace ShowPromoteButtonNew
{
    [BepInPlugin("dazechr.showpromotebutton", "Show Promote Button Mod", "1.0.0")]
    public class ShowPromoteButtonMod : BaseUnityPlugin
    {
        void Awake()
        {
            var harmony = new Harmony("dazechr.showpromotebutton");
            harmony.PatchAll();
        }
    }

    [HarmonyPatch(typeof(LobbyUserListElement), nameof(LobbyUserListElement.RefreshCrownAndPromoteButton))]
    public static class RefreshCrownAndPromoteButtonPatch
    {
        public static void Postfix(LobbyUserListElement __instance)
        {

            bool isHost = PlatformSystems.lobbyManager.ownsLobby;

            if (!isHost)
                return;

            bool isThisUserHost = PlatformSystems.lobbyManager.IsLobbyOwner(__instance.id);

            GameObject promoteButton = __instance.elementChildLocator.FindChild("PromoteButton")?.gameObject;

            if (__instance.id == default(RoR2.PlatformID))
            {
                return;
            }

            if (promoteButton)
            {
                promoteButton.SetActive(!isThisUserHost);

                if (!isThisUserHost)
                {
                    MPButton buttonComponent = promoteButton.GetComponent<MPButton>();
                    if (buttonComponent)
                    {
                        buttonComponent.onClick.RemoveAllListeners();
                        buttonComponent.onClick.AddListener(delegate()
                        {
                            RoR2.Console.instance.SubmitCmd(null,string.Format(CultureInfo.InvariantCulture, "steam_lobby_assign_owner {0}", __instance.id),false);
                        });
                    }
                }
            }
        }
    }
}
