using BepInEx.Configuration;

namespace NoCorpseHigherQuota;

public class Config
{
    public static ConfigEntry<int> Configcost;
        
    public static ConfigEntry<bool> Configdynamic;
    
    public Config(ConfigFile cfg)
    {
        Configcost = cfg.Bind(
            "Costs",
            "Priceperbody",
            30,
            "This value is the cost per body lost that the game will be starting at."
        );

        Configdynamic = cfg.Bind(
            "Dynamic",
            "Allow",
            true,
            "This allow/deny if you want to dynamically increase the quota per unrecovered body"
                    
        );
    }
    
}