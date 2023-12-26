using System;
using HarmonyLib;

namespace NoCorpseHigherQuota.Patches;

[HarmonyPatch(typeof(StartOfRound))]
internal class LeavingBodyPatch
{

    [HarmonyPatch("ShipHasLeft")]
    [HarmonyPostfix]
    private static void CancelAnimation(StartOfRound __instance)
    {
        
        
        int total = Math.Abs(__instance.livingPlayers - GameNetworkManager.Instance.connectedPlayers);


        if (total != 0 && GetBodiesInShip() != total)
        {

            for (int i = 0; i < total; i++)
            {
                TimeOfDay.Instance.profitQuota += 30;
            }


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