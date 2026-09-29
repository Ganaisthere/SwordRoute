using System.Collections.Generic;
using MTM101BaldAPI.AssetTools;
using NilLib;
using UnityEngine;

namespace SwordRoute
{
    public class Other
    {
        public static List<NPC> TwoLifeNPCs = new List<NPC>();
        public static List<WeightedSoundObject> DefaultDeathSounds = new List<WeightedSoundObject>();
        public static NPC Her = null;
        public static bool AnykeyDown => Input.anyKeyDown || Singleton<InputManager>.Instance.GetDigitalInput("MouseSubmit", onDown: true) || Singleton<InputManager>.Instance.GetDigitalInput("Pause", onDown: true) || Singleton<InputManager>.Instance.AnyButton(onDown: true);

        public static void NpcKillPlayer(NPC npc)
        {
            if (DefaultDeathSounds.Count <= 0)
            {
                SoundObject soundObject = AssetFinder.FindOfTypeWithName<SoundObject>("Lose_Buzz", true);
                soundObject.color = Color.white;

                DefaultDeathSounds.Add(new WeightedSoundObject { selection = soundObject, weight = 100 });

                soundObject = AssetFinder.FindOfTypeWithName<SoundObject>("Lose_Corruption", true);
                DefaultDeathSounds.Add(new WeightedSoundObject { selection = soundObject, weight = 100 });

                soundObject = AssetFinder.FindOfTypeWithName<SoundObject>("Lose_Corruption2", true);
                DefaultDeathSounds.Add(new WeightedSoundObject { selection = soundObject, weight = 100 });

                soundObject = AssetFinder.FindOfTypeWithName<SoundObject>("Lose_Creepy", true);
                DefaultDeathSounds.Add(new WeightedSoundObject { selection = soundObject, weight = 100 });

                soundObject = AssetFinder.FindOfTypeWithName<SoundObject>("Lose_GlassBoom", true);
                DefaultDeathSounds.Add(new WeightedSoundObject { selection = soundObject, weight = 100 });

                soundObject = AssetFinder.FindOfTypeWithName<SoundObject>("Lose_NO", true);
                DefaultDeathSounds.Add(new WeightedSoundObject { selection = soundObject, weight = 100 });

                soundObject = AssetFinder.FindOfTypeWithName<SoundObject>("Lose_What", true);
                DefaultDeathSounds.Add(new WeightedSoundObject { selection = soundObject, weight = 100 });
            }

            UsefulHelpers.KillPlayer(npc, DefaultDeathSounds.ToArray());
        }
    }
}
