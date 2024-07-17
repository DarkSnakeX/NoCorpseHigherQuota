using HarmonyLib;
using NoCorpseHigherQuota.Configs;

namespace NoCorpseHigherQuota.Patches;

[HarmonyPatch(typeof(TimeOfDay))]
internal class NewQuotaPatch
{
    
    
    
    [HarmonyPatch("SetNewProfitQuota")]
    [HarmonyPrefix]
    private static void CalculateProfitQuota()
    {
        if (Config.Instance.Confignewquota || LeavingBodyPatch.Num == 0) return;
        TimeOfDay.Instance.profitQuota -= LeavingBodyPatch.Num;
        LeavingBodyPatch.Num = 0;
    }
    

}