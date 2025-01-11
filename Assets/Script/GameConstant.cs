
public enum SceneType
{
    Home,
    Game
}

public enum PlayerType
{
    Red,
    Blue
}
public enum PlayMode
{
    Normal,
    Boss
} 
public enum Weather
{
    None,
    Rain,
    Sunny,
    Storm
}
public enum BoosterType
{
    GoldPotion = 0,
    Magnet = 1,
    BonusCoin = 2,
    FreeLive = 3,
    TrailFire = 4,
    TrailIce = 5,
    TrailRainbow = 6
}

public enum WeaponName
{
    Sword,
    WindySword,
    BattleAxe,
    Bow,
    DanteSword,
    KratosAxe,
    DeathScythe,
    DeathComboSword,
    DeathWindySword,
    DeathBattleAxe,
    DeathBow,
    DeathHammer
}

public enum PopupType
{
    Skin,
    DailyReward,
    LuckySpin,
    Subscription,
    RemoveAds,
    UnlockSkin,
    Treasure,
    WatchAds,
    LeaderBoard,
    Chest,
    Shop,
    BoostItems,
    Pause,
    Revive,
    ReviveRedBlue,
    ReviveStick,
    Update,
    BoosterStore,
    UniqueSkinStore,
    InfoUser,
    LuckyChest,
    ModeSelection,
    HeartStore,
    ShopIngame,
    BossCollection,
    SkinSelection
}

public enum PetSkill{
    Default = 0,
    Magnet = 1,
    Heart = 2
}

public enum QuitGameReason
{
    Back,
    Fail,
    Win,
    Restart,
    Skip
}

public enum Season
{
    Spring = 0,
    Summer = 1,
    Autumn = 2,
    Winter = 3,
    Ramadan = 4,
    Castle = 5,
    Universe = 6,
    Library = 7,
    Mushroom = 8,
    Lava = 9,
    Underground = 10,
    Mountain = 11,
    Sky = 12,
    RedBlue = 13,
    StickFight = 14,
    Gold = 15,
    Stone = 16,
    Forest = 17,
    Shadowland = 18
}

public enum CrackingChestRewardType
{
    Coin,
    Heart,
    HealPotionMini,
    HealPotionLarge
}

public enum MoveDirection
{
    None = 0,
    Right = 1,
    Left = -1
}

public enum MovingPlatformType
{
    PingPong,
    Elevator,
    Loop
}

public enum InputTutorialType
{
    Hold,
    Click
}

public enum InputTutorialTarget
{
    Fight,
    Jump,
    Left,
    Right,
    Dash
}

public static class GameConstant
{
    public const string MESSAGE_NO_ADS = "No Video ads are available at the moment";

    public static readonly byte[] ENCRYPT_KEY = {0x6e, 0x68, 0x75, 0x76, 0x67, 0x74, 0x66, 0x62, 0x68, 0x74, 0x66, 0x67, 0x62, 0x68, 0x74, 0x64};    

    public const string DEFAULT_SKIN = "5";
    public const string DEFAULT_WEAPON = "w7";
    public const string DEFAULT_EQUIPED_WEAPON = "DeathScythe";
    public const int BONUS_HEART_ADS = 1;
    public const int BONUS_COIN_ADS = 500;
    public const int WEAPPON_STRAIN_READY_ANIMATION_TRACK_INDEX = 1;

    public const string URL_POLICY = "https://sites.google.com/view/mgif-studio/";
    public const string URL_TERM = "https://sites.google.com/view/mgif-studio/tos";
    public const string URL_FACEBOOK_PAGE = "https://www.facebook.com/ballvgame";

    //sound
    public const string AUDIO_CLICK = "click"; 
}

public static class PlayerAnim
{
    public const string IDLE_IN_GAME = "1_idle_ingame";
    public const string IDLE_IN_HOME = "1_idle_inhome";
    public const string HURT = "2_hurt";
    public const string LAUGH = "3_laugh";
    public const string SAD = "4_sad";
    public const string SKIN_BUY = "5_skinbuy";
    public const string REVIVE = "6_revive";
    public const string GET_ADS_1 = "7_getads1";
    public const string GET_ADS_2 = "8_getads2";
    public const string LIKE = "9_like";
    public const string COIN_EYES = "10_coineyes";
    public const string HEART_EYES = "11_hearteyes";
    public const string WOW = "12_wow";
    public const string GLASS_1 = "13_glass";
    public const string GLASS_2 = "14_glass2";
    public const string GLASS_3 = "15_glass3";
    public const string GLASS_4 = "16_glass4";
    public const string GLASS_5 = "17_glass5";
    public const string RIGHT_HAND = "tayphai";
    public const string LEFT_HAND = "taytrai";
}

public static class DataKey
{
    public const string MAP_LEVEL = "map-level";
    public const string WORLD_LEVEL = "world-level";
    public const string WORLD_POINT = "world-point-";
    public const string RAMADAN_POINT = "ramadan-point-";
    public const string RED_BLUE_POINT = "red-blue-point";
    public const string STICK_POINT = "stick-point";
    public const string CURRENT_SKIN = "current-skin";
    public const string CURRENT_WEAPON = "current-weapon";
    public const string UNLOCK_SKINS = "unlock-skins";
    public const string UNLOCK_WEAPONS = "unlock-weapons";
    public const string EQUIPED_WEAPONS = "equiqed-weapons";
    public const string USER_COIN = "user-coin";
    public const string USER_HEART = "user-heart";
    public const string USER_STAR = "user-star";
    public const string SUBSCRIPTION = "subscription";
    public const string REMOVE_ADS = "remove-ads";
    public const string MUSIC = "music-setting";
    public const string SOUND = "sound-setting";
    public const string DAILY_REWARD_COUNT = "daily-rewards-count";
    public const string DAILY_REWARD_TIME = "daily-rewards-time";
    public const string CHEST_START_TIME = "chest-start-time";
    public const string CHEST_COUNTDOWN_TIME = "chest-countdown-time";
    public const string IDLE_CHEST_INDEX = "idle-chest-index";
    public const string SURVIVAL_BUTTON_CLICKED = "survival-clicked";
    public const string SOLD_OUT_BOOSTER = "sold-out-booster";
    public const string PREFIX_BALL_SKILL = "ball-skill-";
    public const string GOT_UNIQUE_SKINS = "got-unique-skins";
    public const string PLAYED_CUTSCENE = "played-cutscene";
    public const string MODE_PLAYED = "mode-played";
    public const string MODE_INTRODUCED = "mode-introduced";
    public const string SOLD_OUT_HEART = "sold-out-heart";
    public const string FIRST_BOSS_KILLED = "first-boss-killed";
    public const string KEEP_SKIN_START_TIME = "keep-skin-start-time";
    public const string KEEP_SKIN_COUNTDOWN_TIME = "keep-skin-countdown-time";
    public const string AVAILABLE_SKIN_TO_COLLECT = "available-skin-to-collect";
    public const string AVAILABLE_WEAPON_TO_COLLECT = "available-weapon-to-collect";
    public const string AVAILABLE_COMBO_ADS_WATCHED = "available-combo-ads-watched";
    public const string BOSS_COLLECTION = "boss-collection";
    public const string NEW_BOSS_UNLOCK = "new-boss-unlock";
    public const string INTRODUCED_BUTTON_KEEP_COMBO = "introduced-button-keep-combo";
    public const string COMBO_ADS_WATCHED_PREFIX = "combo-";
}

public static class MapConstant
{
    public const int COIN_RATIO = 5;
    public const float MONSTER_MOVE_RATIO_HIT = 0f;
    public const float IMMORTAL_TIME = 0.5f;
    public const float IMMORTAL_TIME_RED_BLUE = 1f;
    public const int TIME_RIVIVE = 10;
    public const int MAX_HEALTH = 3;
    public const int MAX_TIME_PLAY = 300; // sec
}

public static class LeaderBoardConstant
{
    public const float TIME_RESPONE_TOP_USER = 5f;
    public const int DEFAULT_POINT_STAR = 500;
    public const int DEFAULT_LEVEL_SHOW_LEADER_BOARD = 1;
}

public static class GameTag
{
    public const string PLAYER = "Player";
    public const string MONSTER = "monster";
    public const string COIN = "coin";
    public const string STAR = "star";
    public const string GROUND = "ground";
    public const string OBJECT_BOX = "object-box";
    public const string WALL = "wall";
    public const string MONSTER_MOVE_GROUND = "monster-move-ground";
    public const string MAGNET = "magnet";
    public const string CLOUD = "cloud";
    public const string ELEVATOR = "elevator";
    public const string WEAPON = "weapon";
    public const string ONE_WAY = "one-way";
    public const string FLAX = "flax";
}
