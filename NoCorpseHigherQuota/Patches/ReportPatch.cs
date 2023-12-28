using HarmonyLib;

namespace NoCorpseHigherQuota.Patches;


[HarmonyPatch(typeof(HUDManager))]
internal class ReportPatch
{
    
    [HarmonyPatch("ApplyPenalty")]
    [HarmonyPostfix]
    private static void CancelAnimation(ref EndOfGameStatUIElements ___statsUIElements)
    {
        ___statsUIElements.penaltyAddition.text += $"\nUnrecovered body/s: " + LeavingBodyPatch.Iter + "\nQuota has increased by " + LeavingBodyPatch.Num;
        
    }
    
}