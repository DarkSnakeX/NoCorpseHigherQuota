using System;
using HarmonyLib;

namespace NoCorpseHigherQuota.Patches;

[HarmonyPatch(typeof(StartOfRound))]
internal class LeavingBodyPatch
{
    
    public static int num = 0;
    public static int iter = 0;

    [HarmonyPatch("ShipHasLeft")]
    [HarmonyPostfix]
    private static void CancelAnimation(StartOfRound __instance)
    {
        

        int total = Math.Abs(__instance.livingPlayers - GameNetworkManager.Instance.connectedPlayers);


        if (total != 0 && GetBodiesInShip() != total)
        {
            num = 0;
            iter = 0;
            for (; iter < total; iter++)
            {
                TimeOfDay.Instance.profitQuota += NoCorpseHigherQuota.configcost.Value;
                num += NoCorpseHigherQuota.configcost.Value;
            }
            /*HUDManager.Instance.AddTextToChatOnServer("<color=yellow>Lost body/s</color> - <color=blue>The quota has increased by </color><color=red>" + num + "</color><color=blue> more.</color>");*/
            
        }

    }
    
    


    
    private static int GetBodiesInShip()
    {
        int num = 0;
        DeadBodyInfo[] array = UnityEngine.Object.FindObjectsOfType<DeadBodyInfo>();
        for (int i = 0; i < array.Length; i++)
        {
            if (array[i].isInShip)
            {
                num++;
            }
        }
        return num;
    }
    
}