using System;
using BepInEx.Configuration;
using GameNetcodeStuff;
using HarmonyLib;
using Unity.Collections;
using Unity.Netcode;

namespace NoCorpseHigherQuota.Configs;

[Serializable]
public class Config : SyncedInstance<Config>
{

    public int Configcost { get; private set; }
    
    public bool Configdynamic { get; private set; }
    
    public Config(ConfigFile cfg)
    {
        
        InitInstance(this);
        
        Configcost = cfg.Bind(
            "Costs",
            "Priceperbody",
            30,
            "This value is the cost per body lost."
        ).Value;

        Configdynamic = cfg.Bind(
            "Dynamic",
            "Allow",
            true,
            "This allow/deny if you want to dynamically increase the quota per unrecovered body depending on how much days have passed. If False it will only count the unrecovered corpses."
                    
        ).Value;
    }
    
    public static void RequestSync() {
        if (!IsClient) return;

        using FastBufferWriter stream = new(IntSize, Allocator.Temp);
        MessageManager.SendNamedMessage("ModName_OnRequestConfigSync", 0uL, stream);
    }
    
    public static void OnRequestSync(ulong clientId, FastBufferReader _) {
        if (!IsHost) return;

        NoCorpseHigherQuota.Mls.LogInfo($"Config sync request received from client: {clientId}");

        byte[] array = SerializeToBytes(Instance);
        int value = array.Length;

        using FastBufferWriter stream = new(value + IntSize, Allocator.Temp);

        try {
            stream.WriteValueSafe(in value);
            stream.WriteBytesSafe(array);

            MessageManager.SendNamedMessage("ModName_OnReceiveConfigSync", clientId, stream);
        } catch(Exception e) {
            NoCorpseHigherQuota.Mls.LogInfo($"Error occurred syncing config with client: {clientId}\n{e}");
        }
    }
    
    
    public static void OnReceiveSync(ulong _, FastBufferReader reader) {
        if (!reader.TryBeginRead(IntSize)) {
            NoCorpseHigherQuota.Mls.LogError("Config sync error: Could not begin reading buffer.");
            return;
        }

        reader.ReadValueSafe(out int val);
        if (!reader.TryBeginRead(val)) {
            NoCorpseHigherQuota.Mls.LogError("Config sync error: Host could not sync.");
            return;
        }

        byte[] data = new byte[val];
        reader.ReadBytesSafe(ref data, val);

        SyncInstance(data);

        NoCorpseHigherQuota.Mls.LogInfo("Successfully synced config with host.");
    }
    
    [HarmonyPostfix]
    [HarmonyPatch(typeof(PlayerControllerB), "ConnectClientToPlayerObject")]
    public static void InitializeLocalPlayer() {
        if (IsHost) {
            MessageManager.RegisterNamedMessageHandler("ModName_OnRequestConfigSync", OnRequestSync);
            Synced = true;

            return;
        }

        Synced = false;
        MessageManager.RegisterNamedMessageHandler("ModName_OnReceiveConfigSync", OnReceiveSync);
        RequestSync();
    }
    
    [HarmonyPostfix]
    [HarmonyPatch(typeof(GameNetworkManager), "StartDisconnect")]
    public static void PlayerLeave() {
        Config.RevertSync();
    }
    
    
}