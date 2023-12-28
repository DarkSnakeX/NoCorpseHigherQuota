using System;
using HarmonyLib;

namespace NoCorpseHigherQuota.Patches;

[HarmonyPatch(typeof(StartOfRound))]
internal class LeavingBodyPatch
{
    
    public static int Num;
    public static int Iter;

    [HarmonyPatch("ShipHasLeft")]
    [HarmonyPostfix]
    private static void SumQuota(StartOfRound __instance)
    {
        int days = __instance.gameStats.daysSpent+1;
        int totaldead = Math.Abs(__instance.livingPlayers - GameNetworkManager.Instance.connectedPlayers);
        int cuerposMuertosenNave = GetBodiesInShip();

        if (totaldead != 0 && cuerposMuertosenNave < totaldead)
        {
            Num = 0;
            Iter = 0;
            
                if (Config.Configdynamic.Value)
                {
                    Num = Math.Abs(days*5+totaldead*Config.Configcost.Value-cuerposMuertosenNave*(Config.Configcost.Value/2));
                    TimeOfDay.Instance.profitQuota += Num;
                }
                else
                {
                    Num = Config.Configcost.Value;
                    TimeOfDay.Instance.profitQuota += Num;
                }
            
            
            
            /*HUDManager.Instance.AddTextToChatOnServer("<color=yellow>Lost body/s</color> - <color=blue>The quota has increased by </color><color=red>" + num + "</color><color=blue> more.</color>");*/
            
        }

    }
    
    


    
    private static int GetBodiesInShip()
    {
        int num = 0;
        DeadBodyInfo[] array = UnityEngine.Object.FindObjectsOfType<DeadBodyInfo>();
        foreach (var t in array)
        {
            if (t.isInShip)
            {
                num++;
            }
        }
        return num;
    }
    
}