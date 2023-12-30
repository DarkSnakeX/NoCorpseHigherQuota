using System;
using HarmonyLib;
using NoCorpseHigherQuota.Configs;

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

        if (totaldead != 0 && cuerposMuertosenNave != totaldead)
        {
            Iter = 0;
            
                if (Config.Instance.Configdynamic)
                {
                    Num += Math.Abs(days*5+totaldead*Config.Instance.Configcost-cuerposMuertosenNave*(Config.Instance.Configcost/2));
                    TimeOfDay.Instance.profitQuota += Math.Abs(days*5+totaldead*Config.Instance.Configcost-cuerposMuertosenNave*(Config.Instance.Configcost/2));
                }
                else
                {
                    for (; Iter < totaldead; Iter++)
                    {
                        TimeOfDay.Instance.profitQuota += Config.Instance.Configcost;
                        Num += Config.Instance.Configcost;
                    }
                }
            
            
            
            /*HUDManager.Instance.AddTextToChatOnServer("<color=yellow>Lost body/s</color> - <color=blue>The quota has increased by </color><color=red>" + num + "</color><color=blue> more.</color>");*/
            
        }
        else
        {
            NoCorpseHigherQuota.Mls.LogMessage("No deaths :D");
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