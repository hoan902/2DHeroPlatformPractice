
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Serialization;

public class MainModel
{
    public static bool configLoaded = false;
    public static bool paused;
    public static bool readyToCheckInternet;
    public static bool isTablet = false;

    //user info
    public static string currentSkin;
    public static string currentWeapon;
    public static string trySkin = "";
    public static string tryWeapon = "";
    public static string availableSkinToUnlock = "";
    public static string availableWeaponToUnlock = "";
    public static int totalCoin = 0;
    public static int totalHeart = 0;
    public static int totalStar = 0;
    public static bool subscription = false;
    public static int availableComboAdsWatched = 0;
    public static bool buttonKeepSkinIntroduced = false;
    public static List<string> unlockedSkins = new List<string>();
    public static List<string> unlockedWeapons = new List<string>();
    public static List<string> equipedWeapons = new List<string>();
    public static List<string> bossCollection = new List<string>();
    public static string newBossUnlock = "";

    public static GameInfo gameInfo;
    public static LevelResult levelResult;
    //
    public static bool inCutscene;
    public static bool hasBonusLevelCoin = false;
    public static bool skinComboJustShowed = false;
    
    private static bool m_removeAds = false;
    public static bool removeAds
    {
        get
        {
            return m_removeAds || subscription;
        }
        set
        {
            m_removeAds = value;
        }
    }

    public static string CurrentSkin
    {
        get
        {
            return trySkin != "" ? trySkin : currentSkin;
        }
    }

    public static string CurrentWeapon
    {
        get
        {
            return tryWeapon != "" ? tryWeapon : currentWeapon;
        }
    }

    public static void LoadConfig(string store) 
    {
        configLoaded = true;
        LoadUserInfo();
        SaveAllInfo();
    }

    public static void InitGameInfo(int level, int world, PlayMode playMode)
    {
        gameInfo = new GameInfo(0, level, world, playMode);
        levelResult = new LevelResult();
        levelResult.bonusHeart = 0;
        levelResult.skin = "";
        levelResult.weapon = "";
        levelResult.playMode = playMode;
        trySkin = "";
        tryWeapon = "";
    }

    public static void ResetSavePoint()
    {
        gameInfo.health = MapConstant.MAX_HEALTH;
    }

    public static void LoadUserInfo()
    {
        //skins
        currentSkin = PlayerPrefs.GetString(DataKey.CURRENT_SKIN, GameConstant.DEFAULT_SKIN);
        unlockedSkins = PlayerPrefs.GetString(DataKey.UNLOCK_SKINS, GameConstant.DEFAULT_SKIN).Split(',').ToList();
        currentWeapon = PlayerPrefs.GetString(DataKey.CURRENT_WEAPON, GameConstant.DEFAULT_WEAPON);
        unlockedWeapons = PlayerPrefs.GetString(DataKey.UNLOCK_WEAPONS, GameConstant.DEFAULT_WEAPON).Split(',').ToList();
        equipedWeapons = PlayerPrefs.GetString(DataKey.EQUIPED_WEAPONS, GameConstant.DEFAULT_EQUIPED_WEAPON).Split(',').ToList();
        availableSkinToUnlock = PlayerPrefs.GetString(DataKey.AVAILABLE_SKIN_TO_COLLECT, "");
        availableWeaponToUnlock = PlayerPrefs.GetString(DataKey.AVAILABLE_WEAPON_TO_COLLECT, "");
        availableComboAdsWatched = PlayerPrefs.GetInt(DataKey.AVAILABLE_COMBO_ADS_WATCHED, 0);
        string bossData = PlayerPrefs.GetString(DataKey.BOSS_COLLECTION, "");
        if (string.IsNullOrEmpty(bossData))
            bossCollection = new List<string>();
        else
            bossCollection = bossData.Split(',').ToList();
        newBossUnlock = PlayerPrefs.GetString(DataKey.NEW_BOSS_UNLOCK, "");
        //other
        totalHeart = PlayerPrefs.GetInt(DataKey.USER_HEART, 3);
        totalCoin = PlayerPrefs.GetInt(DataKey.USER_COIN, 0);
        totalStar = PlayerPrefs.GetInt(DataKey.USER_STAR, 0);
        // totalRamadanStar = PlayerPrefs.GetInt(DataKey.USER_RAMADAN_STAR, 0);
        subscription = PlayerPrefs.GetInt(DataKey.SUBSCRIPTION, 0) == 1;
        m_removeAds = PlayerPrefs.GetInt(DataKey.REMOVE_ADS, 0) == 1;
        buttonKeepSkinIntroduced = PlayerPrefs.GetInt(DataKey.INTRODUCED_BUTTON_KEEP_COMBO, 0) == 1 ? true : false;
    }

    public static void SaveAllInfo()
    {
        //skin
        PlayerPrefs.SetString(DataKey.CURRENT_SKIN, currentSkin);
        PlayerPrefs.SetString(DataKey.CURRENT_WEAPON, currentWeapon);
        PlayerPrefs.SetString(DataKey.UNLOCK_SKINS, string.Join(",", unlockedSkins));
        PlayerPrefs.SetString(DataKey.UNLOCK_WEAPONS, string.Join(",", unlockedWeapons));
        PlayerPrefs.SetString(DataKey.EQUIPED_WEAPONS, string.Join(",", equipedWeapons));
        PlayerPrefs.SetString(DataKey.AVAILABLE_SKIN_TO_COLLECT, availableSkinToUnlock);
        PlayerPrefs.SetString(DataKey.AVAILABLE_WEAPON_TO_COLLECT, availableWeaponToUnlock);
        PlayerPrefs.SetInt(DataKey.AVAILABLE_COMBO_ADS_WATCHED, availableComboAdsWatched);
        PlayerPrefs.SetString(DataKey.BOSS_COLLECTION, string.Join(",",bossCollection));
        PlayerPrefs.SetString(DataKey.NEW_BOSS_UNLOCK, newBossUnlock);
        //
        PlayerPrefs.SetInt(DataKey.USER_HEART, totalHeart);
        PlayerPrefs.SetInt(DataKey.USER_COIN, totalCoin);
        PlayerPrefs.SetInt(DataKey.USER_STAR, totalStar);
        PlayerPrefs.SetInt(DataKey.SUBSCRIPTION, subscription ? 1 : 0);
        PlayerPrefs.SetInt(DataKey.REMOVE_ADS, m_removeAds ? 1 : 0);
        PlayerPrefs.SetInt(DataKey.INTRODUCED_BUTTON_KEEP_COMBO, buttonKeepSkinIntroduced ? 1 : 0);
        //
        PlayerPrefs.Save();
    }

    public static void BuySkin(string skin)
    {
        if (unlockedSkins.Contains(skin))
            return;
        unlockedSkins.Add(skin);
        SaveAllInfo();
    }

    public static void BuyWeapon(string weaponSkinName)
    {
        if (unlockedWeapons.Contains(weaponSkinName))
            return;
        unlockedWeapons.Add(weaponSkinName);
        SaveAllInfo();
    }
    public static void Subscribe(bool active)
    {
        subscription = active;
        SaveAllInfo();
    }
    public static void RemoveAds()
    {
        m_removeAds = true;
        SaveAllInfo();
    }
    public static void UpdateHeart(int heart)
    {
        totalHeart += heart;
        SaveAllInfo();
    }
    public static void UpdateTotalCoin(int coin)
    {
        totalCoin += coin;
        SaveAllInfo();        
    }
    public static void UpdateTotalStar(int star)
    {
        totalStar += star;
        SaveAllInfo();
    }
    public static bool IsSkinUnlock(string skin)
    {
        // return unlockSkins.Contains(skin);
        return false;
    }
    public static void SetCurrentSkin(string skin)
    {
        currentSkin = skin;
        trySkin = "";
        SaveAllInfo();
    }
    public static void SetCurrentWeapon(string weaponSkinName)
    {
        currentWeapon = weaponSkinName;
        tryWeapon = "";
        SaveAllInfo();
    }
    public static void SaveEquipWeapons(WeaponName weaponName)
    {
        if (equipedWeapons.Contains(weaponName.ToString()))
            return;
        equipedWeapons.Add(weaponName.ToString());
        SaveAllInfo();
    }

    public static void SetAvailableSkinToCollect(string skin)
    {
        availableSkinToUnlock = skin;
        SaveAllInfo();
    }
    public static void SetAvailableWeaponToCollect(string weapon)
    {
        availableWeaponToUnlock = weapon;
        SaveAllInfo();
    }
    public static void ResetAdsWatchedToCollectCombo()
    {
        availableComboAdsWatched = 0;
        ResetKeepSkinCountdown();
    }
    public static void ResetKeepSkinCountdown()
    {
        PlayerPrefs.DeleteKey(DataKey.KEEP_SKIN_COUNTDOWN_TIME);
        PlayerPrefs.DeleteKey(DataKey.KEEP_SKIN_START_TIME);
        PlayerPrefs.Save();
    }
    public static void SetAdsWatchedToCollectCombo(int value)
    {
        availableComboAdsWatched = value;
        SaveAllInfo();
    }

    public static void UpdateBossCollection(string bossName)
    {
        if(bossCollection.Contains(bossName))
            return;
        newBossUnlock = bossName;
        bossCollection.Add(bossName);
        SaveAllInfo();
    }

    public static void ClearNotiBossUnlock(string bossName)
    {
        if(bossName != newBossUnlock)
            return;
        newBossUnlock = "";
        SaveAllInfo();
    }

    public static void IntroduceButtonKeepSkin()
    {
        buttonKeepSkinIntroduced = true;
        SaveAllInfo();
    }
}

public class GameInfo
{
    public int level;
    public PlayMode playMode;
    public int world;
    public int health = 0;
    public int levelCoin = 0;    
    public List<string> keys;
    public Vector2? savePoint;
    public int treasureKeys = 0;
    public float startImmortalTime;
    public string levelPath;//will set when load level

    public bool immortal => (Time.time - startImmortalTime) < (MapConstant.IMMORTAL_TIME);

    public GameInfo(int treasureKey, int level, int world, PlayMode playMode = PlayMode.Normal)
    {
        this.level = level;
        this.world = world;
        this.playMode = playMode;
        health = MapConstant.MAX_HEALTH;
        levelCoin = 0;
        keys = new List<string>();
        savePoint = null;
        treasureKeys = treasureKey;
    }

    public bool HasKey(string key)
    {
        return keys.Contains(key);
    }

    public void AddKey(string key)
    {
        keys.Add(key);
    }

    public void RemoveKey(string key)
    {
        keys.Remove(key);
    }
}

public class LevelResult
{
    public PlayMode playMode;
    public bool isReplay;
    public bool isComplete;
    public int mapLevel;
    public int worldLevel;
    public int fakeLevelIndex;
    public int oldCoin;
    public int coin;
    public int maxPoint;
    public int point;
    public int heart;
    public int bonusHeart;//fo pet heart
    public string skin;
    public string weapon;

    public int remainPoint
    {
        get
        {
            return maxPoint - point;
        }
    }
}

[Serializable]
public class BoostDamageItem
{
    [SerializeField] public int price;
    [SerializeField] public int ratio;
    [SerializeField] public bool active;
}

[Serializable]
public class BoostHealthItem
{
    [SerializeField] public int price;
    [SerializeField] public int ratio;
    [SerializeField] public bool active;
}

[Serializable]
public class StoreConfig
{
    public List<PremiumSkinConfig> premium_skins;
    public List<CoinSkinConfig> coin_skins;
    public List<RescueSkinConfig> rescue_skins;
    public List<TreasureSkin> treasure_skins;
    public List<UniqueSkinDefine> unique_skins;
    public List<PackageConfig> package;

    public RescueSkinConfig GetRescueSkin(string skin)
    {
        foreach(RescueSkinConfig cf in rescue_skins)
        {
            if (cf.skin == skin)
                return cf;
        }
        return null;
    }

    public CoinSkinConfig GetCoinSkin(string skin)
    {
        foreach (CoinSkinConfig cf in coin_skins)
        {
            if (cf.skin == skin)
                return cf;
        }
        return null;
    }
}

[Serializable]
public class PremiumSkinConfig
{
    public string skin;
}

[Serializable]
public class PackageConfig
{
    public int id;
    public string name;
    public string description;
}

[Serializable]
public class CoinSkinConfig
{
    public int price;
    public string skin;
}

[Serializable]
public class RescueSkinConfig
{
    public string skin;
    public int level;
}
[Serializable]
public class TreasureSkin
{
    public string skin;
    public int level;
}
[Serializable]
public class UniqueSkinDefine
{
    public string skin;
}

[Serializable]
public class HistoryLevel
{
    public int level;
    public int highScore = 0;
    public int stars = 0;

    public HistoryLevel(int level)
    {
        this.level = level;
        highScore = 0;
        stars = 0;
    }

    public (int, int) GetHistory
    {
        get => (highScore, stars);
    }
}

[Serializable]
public class LeaderBoardScore
{
    public int coins;
    public int monsters;
    public int stars;
    public int times;

    public LeaderBoardScore()
    {
        coins = 0;
        monsters = 0;
        stars = 0;
        times = MapConstant.MAX_TIME_PLAY;
    }

    public int DisplayScore
    {
        get => (coins + monsters) * MapConstant.COIN_RATIO + (stars * LeaderBoardConstant.DEFAULT_POINT_STAR);
    }

    public int TotalScore
    {
        get
        {
            return (coins + monsters) * MapConstant.COIN_RATIO + (stars * LeaderBoardConstant.DEFAULT_POINT_STAR) + times;
        }
    }
}

[System.Serializable]
public class DamageDealerInfo
{
    public int damage;
    public bool critical;
    [FormerlySerializedAs("attackerTransform")] public Transform attacker;
}

[System.Serializable]
public class PlayerFightInfor
{
    public DamageDealerInfo damageDealerInfo;
    public int targetFightIndex;
}