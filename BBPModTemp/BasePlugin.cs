using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using MTM101BaldAPI;
using MTM101BaldAPI.AssetTools;
using MTM101BaldAPI.ObjectCreation;
using MTM101BaldAPI.Registers;
using MTM101BaldAPI.SaveSystem;
using System.Collections;
using System.IO;
using UnityEngine;

namespace SwordRoute
{
    [BepInPlugin("ganaisthere.plus.swordroute", "Sword Route", "0.1.0.0")]
    [BepInDependency("mtm101.rulerp.bbplus.baldidevapi")]
    [BepInDependency("Nil.Library")]

    public class BasePlugin : BaseUnityPlugin
    {
        public static BasePlugin Instance { get; private set; }
        public static Harmony harmony = new Harmony("ganaisthere.plus.swordroute");
        public static AssetManager AssetMan = new AssetManager();

        //----------------------------------------

        public ConfigEntry<bool> ConfigEasyMode;

        public void Awake()
        {
            ConfigEasyMode = Config.Bind
            (
                "General",
                "Easy Mode",
                false,
                "If true, Mrs Pomp and Gotta Sweep can not kill you in F3."
            );

            Instance = this;
            harmony.PatchAllConditionals();
            ModdedSaveGame.AddSaveHandler(Info);
            LoadingEvents.RegisterOnAssetsLoaded(Info, LoadAssets(), LoadingEventOrder.Start);
            GeneratorManagement.Register(this, GenerationModType.Addend, ChangeGenerator);
            AddLocalization("Subtitles_English.json");
        }

        internal void ChangeGenerator(string floorName, int floorNumber, SceneObject sceneObject)
        {
            CustomLevelObject[] customLevelObjects = CustomLevelObjectExtensions.GetCustomLevelObjects(sceneObject);
            if (floorName == "F3")
            {
                foreach (CustomLevelObject customLevelObject in customLevelObjects)
                {
                    foreach (RoomGroup roomGroup in customLevelObject.roomGroup)
                    {
                        if (roomGroup.name == "Class")
                        {
                            roomGroup.minRooms = 7;
                            roomGroup.maxRooms = 7;
                        }
                    }
                    //customLevelObject.MarkAsNeverUnload();
                }
            }
        }

        private IEnumerator LoadAssets()
        {
            yield return 1225;
            yield return "Loading Assets...";

            AddTexture2DAndToSprite("Her_0.png", "Textures", 15f);
            AddTexture2DAndToSprite("Her_1.png", "Textures", 15f);
            AddTexture2DAndToSprite("Her_2.png", "Textures", 15f);
            AddTexture2DAndToSprite("Her_3.png", "Textures", 15f);
            AddTexture2DAndToSprite("Ice.png", "Textures", 15f);
            AddTexture2DAndToSprite("Snow.png", "Textures", 15f);
            AddTexture2DAndToSprite("SwordIcon_Large.png", "Textures");
            AddTexture2DAndToSprite("SwordIcon_Small.png", "Textures");
            AddTexture2DAndToSprite("Sweep.png", "Textures");
            AddTexture2DAndToSprite("Key0Icon_Large.png", "Textures");
            AddTexture2DAndToSprite("Key0Icon_Small.png", "Textures");
            AddTexture2DAndToSprite("Static0.png", "Textures");
            AddTexture2DAndToSprite("Static1.png", "Textures");
            AddTexture2DAndToSprite("Static2.png", "Textures");

            AddSoundObject("Sea.ogg", "SoundObjects/Musics");
            AddSoundObject("SWORD.ogg", "SoundObjects/Musics");
            AddSoundObject("NORTHERNLIGHT.ogg", "SoundObjects/Musics");
            AddSoundObject("GLACEIR.ogg", "SoundObjects/Musics");
            AddSoundObject("BITROOTS.ogg", "SoundObjects/Musics");
            AddSoundObject("ERAM.ogg", "SoundObjects/Musics");

            AddSoundObject("EventRing.ogg", "SoundObjects/Effects");
            AddSoundObject("snd_board_damage.ogg", "SoundObjects/Effects");
            AddSoundObject("snd_board_escaped.ogg", "SoundObjects/Effects");
            AddSoundObject("snd_board_kill.ogg", "SoundObjects/Effects");
            AddSoundObject("snd_board_playerhurt.ogg", "SoundObjects/Effects");
            AddSoundObject("snd_board_sword_metal.ogg", "SoundObjects/Effects");
            AddSoundObject("snd_board_sword1.ogg", "SoundObjects/Effects");
            AddSoundObject("snd_board_sword2.ogg", "SoundObjects/Effects");
            AddSoundObject("snd_board_sword3.ogg", "SoundObjects/Effects");
            AddSoundObject("snd_board_torch_low.ogg", "SoundObjects/Effects");
            AddSoundObject("snd_link_get_key.ogg", "SoundObjects/Effects");
            AddSoundObject("snd_link_secret_bad.ogg", "SoundObjects/Effects");
            AddSoundObject("snd_glassbreak.wav", "SoundObjects/Effects");
            AddSoundObject("snd_icespell.ogg", "SoundObjects/Effects");
            AddSoundObject("snd_snowgrave.ogg", "SoundObjects/Effects");
            AddSoundObject("snd_board_text_main_end.ogg", "SoundObjects/Effects");

            yield return "Adding ItemObjects...";

            ItemObject iTM_Sword = new ItemBuilder(Info)
                .SetNameAndDescription("Itm_Sword", "Desc_Sword")
                .SetSprites(AssetMan.Get<Sprite>("SwordIcon_Small"), AssetMan.Get<Sprite>("SwordIcon_Large"))
                .SetEnum("ITM_Sword")
                .SetShopPrice(12251225)
                .SetGeneratorCost(12251225)
                .SetItemComponent<ITM_Sword>()
                .SetMeta(ItemFlags.MultipleUse, new string[] { "ITM_Sword" })
                //.SetPickupSound(AssetMan.Get<SoundObject>("snd_link_secret_bad"))
                .Build();
            AssetMan.Add("ITM_Sword", iTM_Sword);

            ItemObject iTM_Key0 = new ItemBuilder(Info)
                .SetNameAndDescription("ITM_Key0", "Desc_Key0")
                .SetSprites(AssetMan.Get<Sprite>("Key0Icon_Small"), AssetMan.Get<Sprite>("Key0Icon_Large"))
                .SetEnum("ITM_Key0")
                .SetShopPrice(12251225)
                .SetGeneratorCost(12251225)
                .SetItemComponent<ITM_Key0>()
                .SetMeta(ItemFlags.MultipleUse, new string[] { "ITM_Key0" })
                .Build();
            AssetMan.Add("ITM_Key0", iTM_Key0);

            yield return "Add NPCs...";

            Her her = new NPCBuilder<Her>(Info)
                .SetName("Her")
                .SetEnum("Her")
                .IgnorePlayerOnSpawn()
                .SetAudioTimescaleType(TimeScaleType.Player)
                .AddMetaFlag(NPCFlags.StandardNoCollide)
                .AddLooker()
                .SetWanderEnterRooms()
                .Build();
            her.spriteRenderer[0].sprite = AssetMan.Get<Sprite>("Her_0");
            AssetMan.Add("Her", her);

            SnowAttack snowAttack = new NPCBuilder<SnowAttack>(Info)
                .SetName("SnowAttack")
                .SetEnum("SnowAttack")
                .IgnorePlayerOnSpawn()
                .SetAudioTimescaleType(TimeScaleType.Environment)
                .AddMetaFlag(NPCFlags.StandardNoCollide)
                .SetAirborne()
                .Build();
            snowAttack.spriteRenderer[0].sprite = AssetMan.Get<Sprite>("Snow");
            AssetMan.Add("SnowAttack", snowAttack);

            Ice ice = new NPCBuilder<Ice>(Info)
                .SetName("Ice")
                .SetEnum("Ice")
                .IgnorePlayerOnSpawn()
                .SetAudioTimescaleType(TimeScaleType.Npc)
                .AddMetaFlag(NPCFlags.StandardNoCollide)
                .SetAirborne()
                .Build();
            ice.spriteRenderer[0].sprite = AssetMan.Get<Sprite>("Ice");
            AssetMan.Add("Ice", ice);

            yield break;
        }

        private void AddTexture2DAndToSprite(string fileNameWithExtension, string chlidPath, float pixelsPerUnit = 50f)
        {
            string filePath = Path.Combine(AssetLoader.GetModPath(this), chlidPath, fileNameWithExtension);
            if (File.Exists(filePath))
            {
                Texture2D texture2D = AssetLoader.TextureFromFile(filePath);
                Sprite sprite = AssetLoader.SpriteFromTexture2D(texture2D, pixelsPerUnit);
                AssetMan.Add(Path.GetFileNameWithoutExtension(filePath), texture2D);
                AssetMan.Add(Path.GetFileNameWithoutExtension(filePath), sprite);
            }
            else
            {
                LogStatic("File Not Exists: " + filePath);
            }
        }
        private void AddAudioClip(string fileNameWithExtension, string chlidPath)
        {
            string filePath = Path.Combine(AssetLoader.GetModPath(this), chlidPath, fileNameWithExtension);
            if (File.Exists(filePath))
            {
                AudioClip audioClip = AssetLoader.AudioClipFromFile(filePath);
                AssetMan.Add(Path.GetFileNameWithoutExtension(filePath), audioClip);
            }
            else
            {
                LogStatic("File Not Exists: " + filePath);
            }
        }
        private void AddSoundObject(string fileNameWithExtension, string chlidPath)
        {
            string filePath = Path.Combine(AssetLoader.GetModPath(this), chlidPath, fileNameWithExtension);
            if (File.Exists(filePath))
            {
                AudioClip audioClip = AssetLoader.AudioClipFromFile(filePath);
                SoundObject soundObject = ObjectCreators.CreateSoundObject(audioClip, "Nothing", SoundType.Music, Color.white, 0f);
                soundObject.name = Path.GetFileNameWithoutExtension(filePath);
                AssetMan.Add(Path.GetFileNameWithoutExtension(filePath), soundObject);
            }
            else
            {
                LogStatic("File Not Exists: " + filePath);
            }
        }
        private void AddMidi(string fileNameWithExtension, string chlidPath)
        {
            string filePath = Path.Combine(AssetLoader.GetModPath(this), chlidPath, fileNameWithExtension);
            if (File.Exists(filePath))
            {
                AssetLoader.MidiFromFile(filePath, Path.GetFileNameWithoutExtension(filePath));
            }
            else
            {
                LogStatic("File Not Exists: " + filePath);
            }
        }
        private void AddLocalization(string fileNameWithExtension, string chlidPath = null)
        {
            string filePath;
            if (chlidPath == null)
            {
                filePath = Path.Combine(AssetLoader.GetModPath(this), fileNameWithExtension);
            }
            else
            {
                filePath = Path.Combine(AssetLoader.GetModPath(this), chlidPath, fileNameWithExtension);
            }
            if (File.Exists(filePath))
            {
                AssetLoader.LocalizationFromFile(filePath, Language.English);
            }
            else
            {
                LogStatic("File Not Exists: " + filePath);
            }
        }

        internal static void LogStatic(object data, int level = 0)
        {
            if (level == 1)
            {
                Instance.Logger.LogWarning(data);
            }
            else if (level == 2)
            {
                Instance.Logger.LogError(data);
            }
            else
            {
                Instance.Logger.LogInfo(data);
            }
        }

        internal void Log(object data, int level = 0)
        {
            if (level == 1)
            {
                Logger.LogWarning(data);
            }
            else if (level == 2)
            {
                Logger.LogError(data);
            }
            else
            {
                Logger.LogInfo(data);
            }
        }
    }
}
