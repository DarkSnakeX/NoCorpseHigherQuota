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
        int days = __instance.gameStats.daysSpent + 1;
        int totaldead = Math.Abs(__instance.livingPlayers - GameNetworkManager.Instance.connectedPlayers);
        int cuerposMuertosenNave = GetBodiesInShip();
        int baseQuota = TimeOfDay.Instance.profitQuota;

        if (totaldead != 0 && cuerposMuertosenNave != totaldead)
        {
            Iter = 0;
            int quotaAdjustment;

            if (Config.Instance.Configdynamic)
            {
                // Ajuste basado en la cuota actual y otros factores
                quotaAdjustment = (days * 5 + totaldead * Config.Instance.Configcost - cuerposMuertosenNave * (Config.Instance.Configcost / 2)) * (TimeOfDay.Instance.profitQuota / baseQuota);
                Num += Math.Abs(quotaAdjustment);
                TimeOfDay.Instance.profitQuota += Math.Abs(quotaAdjustment);
            }
            else
            {
                // Ajuste fijo basado en el costo por jugador muerto
                for (; Iter < totaldead; Iter++)
                {
                    TimeOfDay.Instance.profitQuota += Config.Instance.Configcost;
                    Num += Config.Instance.Configcost;
                }
            }
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