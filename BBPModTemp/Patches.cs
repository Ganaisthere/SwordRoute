using HarmonyLib;
using MTM101BaldAPI.AssetTools;
using MTM101BaldAPI.Reflection;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SwordRoute
{
    [HarmonyPatch(typeof(MainGameManager))]
    public class MainGameManagerPatches
    {
        public static bool killedAll = false;
        public static bool allElevatorsFound = false;
        public static Her herFound = null;

        [HarmonyPatch("CreateHappyBaldi")]
        [HarmonyPrefix]
        public static bool CreateHappyBaldiPrefix()
        {
            return false;
        }

        [HarmonyPatch("BeginPlay")]
        [HarmonyPostfix]
        public static void BeginPlayPostfix(MainGameManager __instance)
        {
            List<RandomEvent> events = __instance.Ec.ReflectionGetVariable("events") as List<RandomEvent>;
            events.Clear();

            Singleton<MusicManager>.Instance.StopMidi();
            Singleton<BaseGameManager>.Instance.BeginSpoopMode();
            __instance.Ec.SpawnNPCs();
            __instance.Ec.StartEventTimers();
            //__instance.Ec.GetBaldi().Despawn();

            Singleton<CoreGameManager>.Instance.musicMan.pitchModifier = 1f;

            RoomController office = null;
            foreach (RoomController room in __instance.Ec.rooms)
            {
                if (room.category == RoomCategory.Office)
                {
                    office = room;
                    break;
                }
            }
            if (office != null)
            {
                __instance.Ec.GetBaldi().Entity.Teleport(office.RandomEntitySafeCellNoGarbage().TileTransform.position);
            }

            Singleton<CoreGameManager>.Instance.musicMan.FlushQueue(true);
            if (Singleton<CoreGameManager>.Instance.sceneObject.levelTitle == "F1" || Singleton<CoreGameManager>.Instance.sceneObject.levelTitle == "F2")
            {
                Singleton<CoreGameManager>.Instance.musicMan.QueueAudio(BasePlugin.AssetMan.Get<SoundObject>("Sea"), true);
            }
            else if (Singleton<CoreGameManager>.Instance.sceneObject.levelTitle == "F3")
            {
                Singleton<CoreGameManager>.Instance.musicMan.QueueAudio(BasePlugin.AssetMan.Get<SoundObject>("GLACEIR"), true);
            }
            else if (Singleton<CoreGameManager>.Instance.sceneObject.levelTitle == "F4")
            {
                Singleton<CoreGameManager>.Instance.musicMan.QueueAudio(BasePlugin.AssetMan.Get<SoundObject>("BITROOTS"), true);
            }
            Singleton<CoreGameManager>.Instance.musicMan.SetLoop(true);

            killedAll = false;
            allElevatorsFound = false;
            herFound = null;
            Other.TwoLifeNPCs.Clear();

            if (Singleton<CoreGameManager>.Instance.sceneObject.levelTitle == "F2")
            {
                __instance.StartCoroutine(JustDoIt(__instance));
            }
            else if (Singleton<CoreGameManager>.Instance.sceneObject.levelTitle == "F3")
            {
                __instance.StartCoroutine(KnownSins(__instance));
            }
            else //if (Singleton<CoreGameManager>.Instance.sceneObject.levelTitle == "F4")
            {
                GameObject text_Obj = new GameObject("Text");
                text_Obj.transform.SetParent(Singleton<CoreGameManager>.Instance.GetHud(0).Canvas().transform);
                TMP_Text text = text_Obj.AddComponent<TextMeshProUGUI>();
                text.rectTransform.anchoredPosition3D = new Vector3(0f, 0f, 0f);
                text.rectTransform.sizeDelta = new Vector2(420f, 360f);
                text.rectTransform.localScale = Vector3.one;
                text.color = Color.white;
                text.alignment = TextAlignmentOptions.Center;
                text.text = "Demo End\n(Other content\nhas not been made yet)";
            }
        }

        public static IEnumerator KnownSins(MainGameManager mainGameManager)
        {
            CoreGameManager coreGameManager = Singleton<CoreGameManager>.Instance;
            HudManager hudManager = coreGameManager.GetHud(0);

            coreGameManager.audMan.PlaySingle(BasePlugin.AssetMan.Get<SoundObject>("EventRing"));

            float timer = 0f;
            while (timer < 3f)
            {
                if (mainGameManager == null)
                {
                    yield break;
                }
                timer += Time.deltaTime;
                yield return null;
            }

            GameObject black_Obj = new GameObject("Black");
            black_Obj.transform.SetParent(hudManager.Canvas().transform);
            RawImage black = black_Obj.AddComponent<RawImage>();
            black.rectTransform.anchoredPosition3D = new Vector3(0f, 0f, 0f);
            black.rectTransform.anchorMax = new Vector2(1f, 0f);
            black.rectTransform.anchorMin = new Vector2(0f, 0f);
            black.rectTransform.sizeDelta = new Vector2(420f, 200f);
            black.rectTransform.localScale = Vector3.one;
            black.rectTransform.SetAsLastSibling();
            black.color = new Color(0f, 0f, 0f, 0.5f);

            GameObject text_Obj = new GameObject("Text");
            text_Obj.transform.SetParent(black_Obj.transform);
            TMP_Text text = text_Obj.AddComponent<TextMeshProUGUI>();
            text.rectTransform.anchoredPosition3D = new Vector3(0f, 50f, 0f);
            text.rectTransform.sizeDelta = new Vector2(420f, 360f);
            text.rectTransform.localScale = Vector3.one;
            text.color = Color.white;
            text.text = LocalizationManager.Instance.GetLocalizedText("Hud_KnownSins");
            text.alignment = TextAlignmentOptions.Center;

            foreach (NPC npc in mainGameManager.Ec.Npcs)
            {
                if (npc.Character != Character.Chalkles && !coreGameManager.GetPlayer(0).ec.map.arrowTargets.Contains(npc.Entity))
                {
                    if (npc.TryGetComponent(out AudioManager audioMan))
                    {
                        Color subtitleColor = (Color)audioMan.ReflectionGetVariable("subtitleColor");
                        coreGameManager.GetPlayer(0).ec.map.AddArrow(npc.Entity, new Color(subtitleColor.r, subtitleColor.g, subtitleColor.b, 1f));
                    }
                    else
                    {
                        coreGameManager.GetPlayer(0).ec.map.AddArrow(npc.Entity, new Color(0.5f, 0.5f, 0.5f, 1f));
                    }
                }
            }

            timer = 0f;
            while (timer < 5f)
            {
                if (mainGameManager == null)
                {
                    yield break;
                }
                timer += Time.deltaTime;
                yield return null;
            }

            Object.Destroy(text_Obj);
            Object.Destroy(black_Obj);

            yield break;
        }

        public static IEnumerator JustDoIt(MainGameManager mainGameManager)
        {
            float timer = 0f;
            while (timer < 2f)
            {
                if (mainGameManager == null)
                {
                    yield break;
                }
                timer += Time.deltaTime;
                yield return null;
            }

            CoreGameManager coreGameManager = Singleton<CoreGameManager>.Instance;
            HudManager hudManager = coreGameManager.GetHud(0);

            coreGameManager.musicMan.FlushQueue(true);

            GameObject black_Obj = new GameObject("Black");
            black_Obj.transform.SetParent(hudManager.Canvas().transform);
            RawImage black = black_Obj.AddComponent<RawImage>();
            black.rectTransform.anchoredPosition3D = new Vector3(0f, 0f, 0f);
            black.rectTransform.anchorMax = new Vector2(1f, 1f);
            black.rectTransform.anchorMin = new Vector2(0f, 0f);
            black.rectTransform.localScale = Vector3.one;
            black.rectTransform.SetAsLastSibling();
            black.color = Color.black;

            SoundObject sound = BasePlugin.AssetMan.Get<SoundObject>("snd_board_sword_metal");
            if (sound != null)
            {
                coreGameManager.audMan.PlaySingle(sound);
            }

            coreGameManager.disablePause = true;
            coreGameManager.GetPlayer(0).plm.Entity.SetFrozen(true);
            coreGameManager.GetCamera(0).SetControllable(false);
            Singleton<BaseGameManager>.Instance.Ec.PauseEnvironment(true);

            timer = 0f;
            while (timer < 2f)
            {
                if (mainGameManager == null)
                {
                    yield break;
                }
                timer += Time.deltaTime;
                yield return null;
            }

            GameObject text_Obj = new GameObject("Text");
            text_Obj.transform.SetParent(black_Obj.transform);
            TMP_Text text = text_Obj.AddComponent<TextMeshProUGUI>();
            text.rectTransform.anchoredPosition3D = new Vector3(0f, 0f, 0f);
            text.rectTransform.sizeDelta = new Vector2(420f, 360f);
            text.rectTransform.localScale = Vector3.one;
            text.color = Color.white;
            text.text = LocalizationManager.Instance.GetLocalizedText("Hud_Proceed");
            text.font = AssetFinder.FindOfTypeWithName<TMP_FontAsset>("COMIC_36_Pro", false);
            text.fontSize = 36f;
            text.alignment = TextAlignmentOptions.Center;

            sound = BasePlugin.AssetMan.Get<SoundObject>("snd_link_secret_bad");
            if (sound != null)
            {
                coreGameManager.audMan.PlaySingle(sound);
            }

            timer = 0f;
            while (timer < 3f)
            {
                if (mainGameManager == null)
                {
                    yield break;
                }
                timer += Time.deltaTime;
                yield return null;
            }

            coreGameManager.disablePause = false;
            coreGameManager.GetPlayer(0).plm.Entity.SetFrozen(false);
            coreGameManager.GetCamera(0).SetControllable(true);
            Singleton<BaseGameManager>.Instance.Ec.PauseEnvironment(false);

            Object.Destroy(text.gameObject);
            Object.Destroy(black.gameObject);

            foreach (Activity activity in mainGameManager.Ec.activities)
            {
                if (!activity.IsCompleted)
                {
                    activity.Completed(0, false);
                }
                if (!activity.NotebookCollected)
                {
                    Notebook notebook = activity.ReflectionGetVariable("notebook") as Notebook;
                    notebook.Clicked(0);
                }
            }

            coreGameManager.musicMan.FlushQueue(true);
            coreGameManager.musicMan.QueueAudio(BasePlugin.AssetMan.Get<SoundObject>("Sea"), true);
            coreGameManager.musicMan.SetLoop(true);

            yield break;
        }

        [HarmonyPatch("AllNotebooks")]
        [HarmonyPrefix]
        public static bool AllNotebooksPrefix(MainGameManager __instance)
        {
            __instance.ReflectionSetVariable("allNotebooksFound", true);

            CoreGameManager coreGameManager = Singleton<CoreGameManager>.Instance;

            if (coreGameManager.sceneObject.levelTitle == "F1" || coreGameManager.sceneObject.levelTitle == "F2")
            {
                RoomController office = null;
                foreach (RoomController room in __instance.Ec.rooms)
                {
                    if (room.category == RoomCategory.Office)
                    {
                        office = room;
                        break;
                    }
                }
                if (office != null)
                {
                    Cell randomCell = office.RandomEntitySafeCellNoGarbage();
                    __instance.Ec.CreateItem(office, BasePlugin.AssetMan.Get<ItemObject>("ITM_Sword"), new Vector2(randomCell.TileTransform.position.x, randomCell.TileTransform.position.z));
                    __instance.StartCoroutine(StartKill(coreGameManager, __instance));
                    return false;
                }
            }
            else if (coreGameManager.sceneObject.levelTitle == "F3")
            {
                __instance.StartCoroutine(FindHer(coreGameManager, __instance));
            }

            return true;
        }

        public static IEnumerator FindHer(CoreGameManager coreGameManager, BaseGameManager baseGameManager)
        {
            baseGameManager.Ec.SpawnNPC(BasePlugin.AssetMan.Get<Her>("Her"), baseGameManager.Ec.RandomCell(false, false, true).position);

            while (herFound == null)
            {
                if (coreGameManager == null || baseGameManager == null)
                {
                    yield break;
                }
                coreGameManager.GetHud(0).UpdateNotebookText(0, "Find Her", false);
                yield return null;
            }
            coreGameManager.musicMan.FlushQueue(true);

            HudManager hudManager = coreGameManager.GetHud(0);

            GameObject black_Obj = new GameObject("Black");
            black_Obj.transform.SetParent(hudManager.Canvas().transform);
            RawImage black = black_Obj.AddComponent<RawImage>();
            black.rectTransform.anchoredPosition3D = new Vector3(0f, 0f, 0f);
            black.rectTransform.anchorMax = new Vector2(1f, 1f);
            black.rectTransform.anchorMin = new Vector2(0f, 0f);
            black.rectTransform.localScale = Vector3.one;
            black.rectTransform.SetAsLastSibling();
            black.color = Color.black;

            coreGameManager.disablePause = true;
            coreGameManager.GetPlayer(0).plm.Entity.SetFrozen(true);
            coreGameManager.GetCamera(0).SetControllable(false);
            baseGameManager.Ec.PauseEnvironment(true);

            yield return new WaitForSeconds(0.5f);

            coreGameManager.audMan.PlaySingle(BasePlugin.AssetMan.Get<SoundObject>("snd_board_sword" + Random.Range(1, 3).ToString()));
            yield return new WaitForSeconds(0.1f);
            coreGameManager.audMan.PlaySingle(BasePlugin.AssetMan.Get<SoundObject>("snd_board_playerhurt"));
            yield return new WaitForSeconds(0.5f);

            coreGameManager.audMan.PlaySingle(BasePlugin.AssetMan.Get<SoundObject>("snd_board_sword" + Random.Range(1, 3).ToString()));
            yield return new WaitForSeconds(0.1f);
            coreGameManager.audMan.PlaySingle(BasePlugin.AssetMan.Get<SoundObject>("snd_board_playerhurt"));
            yield return new WaitForSeconds(0.5f);

            coreGameManager.audMan.PlaySingle(BasePlugin.AssetMan.Get<SoundObject>("snd_board_sword" + Random.Range(1, 3).ToString()));
            yield return new WaitForSeconds(0.1f);
            coreGameManager.audMan.PlaySingle(BasePlugin.AssetMan.Get<SoundObject>("snd_board_playerhurt"));
            yield return new WaitForSeconds(1f);

            coreGameManager.audMan.PlaySingle(BasePlugin.AssetMan.Get<SoundObject>("snd_link_secret_bad"));
            yield return new WaitForSeconds(2f);

            coreGameManager.disablePause = false;
            coreGameManager.GetPlayer(0).plm.Entity.SetFrozen(false);
            coreGameManager.GetCamera(0).SetControllable(true);
            baseGameManager.Ec.PauseEnvironment(false);
            Object.Destroy(black.gameObject);

            coreGameManager.musicMan.FlushQueue(true);
            coreGameManager.musicMan.QueueAudio(BasePlugin.AssetMan.Get<SoundObject>("GLACEIR"), true);
            coreGameManager.musicMan.SetLoop(true);
            coreGameManager.musicMan.pitchModifier = 0.9f;

            int NPCTotalNeedKilled = baseGameManager.Ec.Npcs.Count;
            foreach (NPC npc in baseGameManager.Ec.Npcs)
            {
                if (npc.Character == Character.Chalkles || npc.GetType() == typeof(Her) || npc.GetType() == typeof(SnowAttack))
                {
                    NPCTotalNeedKilled -= 1;
                }
                else if (!coreGameManager.GetPlayer(0).ec.map.arrowTargets.Contains(npc.Entity))
                {
                    coreGameManager.GetPlayer(0).ec.map.AddArrow(npc.Entity, new Color(1f, 0f, 0f, 1f));
                }
            }

            int killed;
            int npcLeft;
            while (true)
            {
                if (coreGameManager == null)
                {
                    yield break;
                }

                npcLeft = baseGameManager.Ec.Npcs.Count;
                foreach (NPC npc in baseGameManager.Ec.Npcs)
                {
                    if (npc.Character == Character.Chalkles || npc.TryGetComponent<Her>(out _) || npc.TryGetComponent<SnowAttack>(out _))
                    {
                        npcLeft -= 1;
                    }
                }

                killed = NPCTotalNeedKilled - npcLeft;
                coreGameManager.GetHud(0).UpdateNotebookText(0, killed.ToString() + "/" + NPCTotalNeedKilled.ToString() + " Killed", false);
                if (killed >= NPCTotalNeedKilled)
                {
                    break;
                }
                yield return null;
            }
            killedAll = true;

            coreGameManager.musicMan.FlushQueue(true);

            coreGameManager.disablePause = true;
            coreGameManager.GetPlayer(0).plm.Entity.SetFrozen(true);
            baseGameManager.Ec.PauseEnvironment(true);

            yield return new WaitForSeconds(2f);

            coreGameManager.audMan.PlaySingle(BasePlugin.AssetMan.Get<SoundObject>("snd_board_escaped"));

            black_Obj = new GameObject("Black");
            black_Obj.transform.SetParent(hudManager.Canvas().transform);
            black = black_Obj.AddComponent<RawImage>();
            black.rectTransform.anchoredPosition3D = new Vector3(0f, 0f, 0f);
            black.rectTransform.anchorMax = new Vector2(1f, 1f);
            black.rectTransform.anchorMin = new Vector2(0f, 0f);
            black.rectTransform.localScale = Vector3.one;
            black.rectTransform.SetAsFirstSibling();
            black.color = new Color(0f, 0f, 0f, 0f);

            float timer = 0f;
            while (timer < 3f)
            {
                if (coreGameManager == null)
                {
                    yield break;
                }
                else
                {
                    yield return null;
                }
                if (timer > 2f)
                {
                    black.color = new Color(0f, 0f, 0f, 0.666f);
                }
                else if (timer > 1f)
                {
                    black.color = new Color(0f, 0f, 0f, 0.333f);
                }
                else
                {
                    black.color = new Color(0f, 0f, 0f, 0f);
                }
                timer += Time.deltaTime;
            }
            black.color = Color.black;

            yield return new WaitForSeconds(2f);

            coreGameManager.musicMan.FlushQueue(true);
            coreGameManager.musicMan.QueueAudio(BasePlugin.AssetMan.Get<SoundObject>("NORTHERNLIGHT"), true);
            coreGameManager.musicMan.SetLoop(true);
            coreGameManager.musicMan.pitchModifier = 0.5f;

            GameObject text_Obj = new GameObject("Text");
            text_Obj.transform.SetParent(hudManager.Canvas().transform);
            TMP_Text text = text_Obj.AddComponent<TextMeshProUGUI>();
            text.rectTransform.anchoredPosition3D = new Vector3(0f, 0f, 0f);
            text.rectTransform.sizeDelta = new Vector2(420f, 360f);
            text.rectTransform.localScale = Vector3.one;
            text.color = Color.white;
            text.alignment = TextAlignmentOptions.Center;

            coreGameManager.audMan.PlaySingle(BasePlugin.AssetMan.Get<SoundObject>("snd_board_text_main_end"));
            text.text = LocalizationManager.Instance.GetLocalizedText("IDK_F3Text0");
            while (Other.AnykeyDown)
            {
                if (coreGameManager == null)
                {
                    yield break;
                }
                else
                {
                    yield return null;
                }
            }
            timer = 0f;
            while (!Other.AnykeyDown && timer < 5f)
            {
                if (coreGameManager == null)
                {
                    yield break;
                }
                else
                {
                    yield return null;
                }
                timer += Time.deltaTime;
            }

            coreGameManager.audMan.PlaySingle(BasePlugin.AssetMan.Get<SoundObject>("snd_board_text_main_end"));
            text.text = LocalizationManager.Instance.GetLocalizedText("IDK_F3Text1");
            while (Other.AnykeyDown)
            {
                if (coreGameManager == null)
                {
                    yield break;
                }
                else
                {
                    yield return null;
                }
            }
            timer = 0f;
            while (!Other.AnykeyDown && timer < 5f)
            {
                if (coreGameManager == null)
                {
                    yield break;
                }
                else
                {
                    yield return null;
                }
                timer += Time.deltaTime;
            }

            coreGameManager.audMan.PlaySingle(BasePlugin.AssetMan.Get<SoundObject>("snd_board_text_main_end"));
            text.text = LocalizationManager.Instance.GetLocalizedText("IDK_F3Text2");
            while (Other.AnykeyDown)
            {
                if (coreGameManager == null)
                {
                    yield break;
                }
                else
                {
                    yield return null;
                }
            }
            timer = 0f;
            while (!Other.AnykeyDown && timer < 5f)
            {
                if (coreGameManager == null)
                {
                    yield break;
                }
                else
                {
                    yield return null;
                }
                timer += Time.deltaTime;
            }

            coreGameManager.audMan.PlaySingle(BasePlugin.AssetMan.Get<SoundObject>("snd_board_text_main_end"));
            text.text = LocalizationManager.Instance.GetLocalizedText("IDK_F3Text3");
            while (Other.AnykeyDown)
            {
                if (coreGameManager == null)
                {
                    yield break;
                }
                else
                {
                    yield return null;
                }
            }
            timer = 0f;
            while (!Other.AnykeyDown && timer < 5f)
            {
                if (coreGameManager == null)
                {
                    yield break;
                }
                else
                {
                    yield return null;
                }
                timer += Time.deltaTime;
            }

            coreGameManager.audMan.PlaySingle(BasePlugin.AssetMan.Get<SoundObject>("snd_board_text_main_end"));
            text.text = LocalizationManager.Instance.GetLocalizedText("IDK_F3Text4");
            while (Other.AnykeyDown)
            {
                if (coreGameManager == null)
                {
                    yield break;
                }
                else
                {
                    yield return null;
                }
            }
            timer = 0f;
            while (!Other.AnykeyDown && timer < 5f)
            {
                if (coreGameManager == null)
                {
                    yield break;
                }
                else
                {
                    yield return null;
                }
                timer += Time.deltaTime;
            }

            coreGameManager.audMan.PlaySingle(BasePlugin.AssetMan.Get<SoundObject>("snd_board_text_main_end"));
            text.text = LocalizationManager.Instance.GetLocalizedText("IDK_F3Text5");
            while (Other.AnykeyDown)
            {
                if (coreGameManager == null)
                {
                    yield break;
                }
                else
                {
                    yield return null;
                }
            }
            timer = 0f;
            while (!Other.AnykeyDown && timer < 5f)
            {
                if (coreGameManager == null)
                {
                    yield break;
                }
                else
                {
                    yield return null;
                }
                timer += Time.deltaTime;
            }

            Object.Destroy(black_Obj);
            coreGameManager.musicMan.FlushQueue(true);
            coreGameManager.musicMan.pitchModifier = 1f;

            string IDK_F3Text5 = LocalizationManager.Instance.GetLocalizedText("IDK_F3Text6");
            string fullText = "";
            for (int i = 0; i < IDK_F3Text5.Length; i++)
            {
                fullText += IDK_F3Text5.Substring(i, 1);
                text.text = fullText;
                timer = 0f;
                while (timer < 0.5f)
                {
                    if (coreGameManager == null)
                    {
                        yield break;
                    }
                    else
                    {
                        yield return null;
                    }
                    timer += Time.deltaTime;
                }
            }
            text.text = LocalizationManager.Instance.GetLocalizedText("IDK_F3Text6");

            yield return new WaitForSeconds(2f);

            text.text = LocalizationManager.Instance.GetLocalizedText("IDK_F3Text7");
            foreach (NPC npc in baseGameManager.Ec.Npcs)
            {
                if (npc.GetType() == typeof(Her))
                {
                    coreGameManager.audMan.PlaySingle(BasePlugin.AssetMan.Get<SoundObject>("snd_board_kill"));
                    npc.Despawn();
                    break;
                }
            }
            yield return new WaitForSeconds(2f);

            text.text = LocalizationManager.Instance.GetLocalizedText("IDK_F3Text8");
            coreGameManager.audMan.PlaySingle(BasePlugin.AssetMan.Get<SoundObject>("snd_board_kill"));
            yield return new WaitForSeconds(0.5f);

            coreGameManager.musicMan.FlushQueue(true);
            coreGameManager.musicMan.QueueAudio(AssetFinder.FindOfTypeWithName<SoundObject>("CFT_Loop", true), true);
            coreGameManager.musicMan.SetLoop(true);
            coreGameManager.musicMan.pitchModifier = 1f;

            Object.Destroy(text_Obj);

            GameObject Static_Obj = new GameObject("Static");
            Static_Obj.transform.SetParent(hudManager.Canvas().transform);
            Image Static = Static_Obj.AddComponent<Image>();
            Static.rectTransform.anchoredPosition3D = new Vector3(0f, 0f, 0f);
            Static.rectTransform.anchorMax = new Vector2(1f, 1f);
            Static.rectTransform.anchorMin = new Vector2(0f, 0f);
            Static.rectTransform.localScale = Vector3.one;
            Static.rectTransform.SetAsLastSibling();
            Static.sprite = BasePlugin.AssetMan.Get<Sprite>("Static0");

            timer = 0f;
            while (timer < 5f)
            {
                if (coreGameManager == null)
                {
                    yield break;
                }
                else
                {
                    yield return null;
                }
                Static.sprite = BasePlugin.AssetMan.Get<Sprite>("Static" + Random.Range(0, 2).ToString());
                timer += Time.deltaTime;
            }

            Object.Destroy(Static_Obj);

            Shader.SetGlobalColor("_SkyboxColor", Color.black);
            coreGameManager.GetCamera(0).StopRendering(true);
            coreGameManager.GetCamera(0).SetControllable(false);
            coreGameManager.musicMan.FlushQueue(true);

            timer = 0f;
            while (timer < 3f)
            {
                if (coreGameManager == null)
                {
                    yield break;
                }
                timer += Time.unscaledDeltaTime;
                yield return null;
            }

            baseGameManager.LoadNextLevel();

            yield break;
        }

        public static IEnumerator StartKill(CoreGameManager coreGameManager, BaseGameManager baseGameManager)
        {
            bool hasSword = false;
            while (!hasSword)
            {
                if (coreGameManager == null || baseGameManager == null)
                {
                    yield break;
                }
                else
                {
                    yield return null;
                    coreGameManager.GetHud(0).UpdateNotebookText(0, "Office", false);
                }
                foreach (ItemObject item in coreGameManager.GetPlayer(0).itm.items)
                {
                    if (item == BasePlugin.AssetMan.Get<ItemObject>("ITM_Sword"))
                    {
                        hasSword = true;
                        break;
                    }
                }
            }

            coreGameManager.musicMan.FlushQueue(true);

            coreGameManager.disablePause = true;
            coreGameManager.GetPlayer(0).plm.Entity.SetFrozen(true);
            coreGameManager.GetCamera(0).SetControllable(false);
            baseGameManager.Ec.PauseEnvironment(true);

            HudManager hudManager = coreGameManager.GetHud(0);

            GameObject black_Obj = new GameObject("Black");
            black_Obj.transform.SetParent(hudManager.Canvas().transform);
            RawImage black = black_Obj.AddComponent<RawImage>();
            black.rectTransform.anchoredPosition3D = new Vector3(0f, 0f, 0f);
            black.rectTransform.anchorMax = new Vector2(1f, 1f);
            black.rectTransform.anchorMin = new Vector2(0f, 0f);
            black.rectTransform.localScale = Vector3.one;
            black.rectTransform.SetAsLastSibling();
            black.color = Color.black;

            SoundObject sound = BasePlugin.AssetMan.Get<SoundObject>("snd_link_secret_bad");
            if (sound != null)
            {
                coreGameManager.audMan.PlaySingle(sound);
            }

            float timer = 0f;
            while (timer < 3f)
            {
                if (coreGameManager == null)
                {
                    yield break;
                }
                timer += Time.unscaledDeltaTime;
                yield return null;
            }

            coreGameManager.disablePause = false;
            coreGameManager.GetPlayer(0).plm.Entity.SetFrozen(false);
            coreGameManager.GetCamera(0).SetControllable(true);
            baseGameManager.Ec.PauseEnvironment(false);
            Object.Destroy(black.gameObject);

            coreGameManager.musicMan.FlushQueue(true);
            coreGameManager.musicMan.QueueAudio(BasePlugin.AssetMan.Get<SoundObject>("SWORD"), true);
            coreGameManager.musicMan.SetLoop(true);
            if (coreGameManager.sceneObject.levelTitle == "F2")
            {
                coreGameManager.musicMan.pitchModifier = 0.875f;
            }

            /*Baldi[] baldi = AssetFinder.FindAllOfType<Baldi>(true);
            if (baldi.Length > 0)
            {
                baldi[0].ReflectionSetVariable("tutorialMode", false);
                baldi[0].ReflectionSetVariable("smoothMove", false);
                Singleton<BaseGameManager>.Instance.Ec.SpawnNPC(baldi[0], Singleton<BaseGameManager>.Instance.Ec.CellFromPosition(Singleton<BaseGameManager>.Instance.Ec.spawnPoint).position);
                Singleton<BaseGameManager>.Instance.AngerBaldi(Singleton<BaseGameManager>.Instance.Ec.notebookTotal);
            }*/

            int NPCTotalNeedKilled = baseGameManager.Ec.Npcs.Count;
            foreach (NPC npc in baseGameManager.Ec.Npcs)
            {
                if (npc.Character == Character.Chalkles || npc.TryGetComponent<Her>(out _) || npc.TryGetComponent<SnowAttack>(out _) || npc.TryGetComponent<Ice>(out _))
                {
                    NPCTotalNeedKilled -= 1;
                }
                else if (!coreGameManager.GetPlayer(0).ec.map.arrowTargets.Contains(npc.Entity))
                {
                    coreGameManager.GetPlayer(0).ec.map.AddArrow(npc.Entity, new Color(1f, 0f, 0f, 1f));
                }
            }

            int killed;
            int npcLeft;
            while (true)
            {
                if (coreGameManager == null)
                {
                    yield break;
                }

                npcLeft = baseGameManager.Ec.Npcs.Count;
                foreach (NPC npc in baseGameManager.Ec.Npcs)
                {
                    if (npc.Character == Character.Chalkles || npc.TryGetComponent<Her>(out _) || npc.TryGetComponent<SnowAttack>(out _) || npc.TryGetComponent<Ice>(out _))
                    {
                        npcLeft -= 1;
                    }
                }

                killed = NPCTotalNeedKilled - npcLeft;
                coreGameManager.GetHud(0).UpdateNotebookText(0, killed.ToString() + "/" + NPCTotalNeedKilled.ToString() + " Killed", false);
                if (killed >= NPCTotalNeedKilled)
                {
                    break;
                }
                yield return null;
            }
            killedAll = true;

            coreGameManager.musicMan.pitchModifier = 1f;
            coreGameManager.musicMan.FlushQueue(true);
            if (coreGameManager.sceneObject.levelTitle == "F2")
            {
                coreGameManager.musicMan.QueueAudio(BasePlugin.AssetMan.Get<SoundObject>("NORTHERNLIGHT"), true);
            }
            else
            {
                coreGameManager.musicMan.QueueAudio(BasePlugin.AssetMan.Get<SoundObject>("Sea"), true);
            }
            coreGameManager.musicMan.SetLoop(true);

            if (coreGameManager.sceneObject.levelTitle == "F2")
            {
                baseGameManager.Ec.ElevatorManager.SetTotalOutOfOrderElevators(99);
            }
            else
            {
                baseGameManager.Ec.ElevatorManager.SetTotalOutOfOrderElevators(baseGameManager.Ec.Elevators.Count - 1);
            }
            baseGameManager.Ec.ElevatorManager.SetAllElevators(ElevatorState.OpenForExit);
            coreGameManager.GetHud(0).UpdateNotebookText(0, killed.ToString() + "/" + NPCTotalNeedKilled.ToString() + " Killed", false);

            if (coreGameManager.sceneObject.levelTitle == "F1")
            {
                RoomController office = null;
                foreach (RoomController room in baseGameManager.Ec.rooms)
                {
                    if (room.category == RoomCategory.Office)
                    {
                        office = room;
                        break;
                    }
                }
                if (office != null)
                {
                    Cell randomCell = office.RandomEntitySafeCellNoGarbage();
                    baseGameManager.Ec.CreateItem(office, BasePlugin.AssetMan.Get<ItemObject>("ITM_Key0"), new Vector2(randomCell.TileTransform.position.x, randomCell.TileTransform.position.z));
                }

                bool hasKey0 = false;
                while (!hasKey0)
                {
                    if (coreGameManager == null || baseGameManager == null)
                    {
                        yield break;
                    }
                    else
                    {
                        yield return null;
                    }
                    foreach (ItemObject item in coreGameManager.GetPlayer(0).itm.items)
                    {
                        if (item == BasePlugin.AssetMan.Get<ItemObject>("ITM_Key0"))
                        {
                            hasKey0 = true;
                            break;
                        }
                    }
                }
                for (int i = 0; i < coreGameManager.GetPlayer(0).itm.items.Length; i++)
                {
                    if (coreGameManager.GetPlayer(0).itm.items[i] == BasePlugin.AssetMan.Get<ItemObject>("ITM_Sword"))
                    {
                        coreGameManager.GetPlayer(0).itm.RemoveItem(i);
                        break;
                    }
                }

                coreGameManager.musicMan.FlushQueue(true);

                coreGameManager.disablePause = true;
                coreGameManager.GetPlayer(0).plm.Entity.SetFrozen(true);
                coreGameManager.GetCamera(0).SetControllable(false);
                baseGameManager.Ec.PauseEnvironment(true);

                black_Obj = new GameObject("Black");
                black_Obj.transform.SetParent(hudManager.Canvas().transform);
                black = black_Obj.AddComponent<RawImage>();
                black.rectTransform.anchoredPosition3D = new Vector3(0f, 0f, 0f);
                black.rectTransform.anchorMax = new Vector2(1f, 0f);
                black.rectTransform.anchorMin = new Vector2(0f, 0f);
                black.rectTransform.sizeDelta = new Vector2(420f, 200f);
                black.rectTransform.localScale = Vector3.one;
                black.rectTransform.SetAsLastSibling();
                black.color = new Color(0f, 0f, 0f, 0.5f);

                GameObject text_Obj = new GameObject("Text");
                text_Obj.transform.SetParent(black_Obj.transform);
                TMP_Text text = text_Obj.AddComponent<TextMeshProUGUI>();
                text.rectTransform.anchoredPosition3D = new Vector3(0f, 50f, 0f);
                text.rectTransform.sizeDelta = new Vector2(420f, 360f);
                text.rectTransform.localScale = Vector3.one;
                text.color = Color.white;
                //text.font = AssetFinder.FindOfTypeWithName<TMP_FontAsset>("COMIC_36_Pro", false);
                text.text = LocalizationManager.Instance.GetLocalizedText("Hud_YouFoundKey0");
                //text.fontSize = 36f;
                text.alignment = TextAlignmentOptions.Center;

                sound = BasePlugin.AssetMan.Get<SoundObject>("snd_link_get_key");
                if (sound != null)
                {
                    coreGameManager.audMan.PlaySingle(sound);
                }

                timer = 0f;
                while (timer < 8.5f)
                {
                    if (coreGameManager == null)
                    {
                        yield break;
                    }
                    timer += Time.unscaledDeltaTime;
                    yield return null;
                }

                Shader.SetGlobalColor("_SkyboxColor", Color.black);
                coreGameManager.GetCamera(0).StopRendering(true);

                Object.Destroy(text.gameObject);
                Object.Destroy(black.gameObject);

                timer = 0f;
                while (timer < 3f)
                {
                    if (coreGameManager == null)
                    {
                        yield break;
                    }
                    timer += Time.unscaledDeltaTime;
                    yield return null;
                }

                baseGameManager.LoadNextLevel();
            }
            else if (coreGameManager.sceneObject.levelTitle == "F2")
            {
                List<Elevator> brokenElevators = baseGameManager.Ec.ElevatorManager.ReflectionGetVariable("brokenElevators") as List<Elevator>;
                while (brokenElevators.Count == 0)
                {
                    if (coreGameManager == null)
                    {
                        yield break;
                    }
                    yield return null;
                }
                baseGameManager.StartCoroutine(RedLight(baseGameManager));

                while (brokenElevators.Count < baseGameManager.Ec.Elevators.Count)
                {
                    if (coreGameManager == null)
                    {
                        yield break;
                    }
                    yield return null;
                }
                allElevatorsFound = true;
                baseGameManager.Ec.ElevatorManager.SetTotalOutOfOrderElevators(0);

                bool hasKey0 = false;
                foreach (ItemObject item in coreGameManager.GetPlayer(0).itm.items)
                {
                    if (item == BasePlugin.AssetMan.Get<ItemObject>("ITM_Key0"))
                    {
                        hasKey0 = true;
                        break;
                    }
                }
                foreach (ItemObject item in coreGameManager.currentLockerItems)
                {
                    if (item == BasePlugin.AssetMan.Get<ItemObject>("ITM_Key0"))
                    {
                        hasKey0 = true;
                        break;
                    }
                }
                if (hasKey0)
                {
                    coreGameManager.GetHud(0).UpdateNotebookText(0, "I NEED KEY", false);
                }
                else
                {
                    coreGameManager.GetHud(0).UpdateNotebookText(0, "YOU FORGET\nSOME THING", false);
                }
            }

            yield break;
        }

        public static IEnumerator RedLight(BaseGameManager baseGameManager)
        {
            float timer;
            for (float i = 0f; i < 11; i++)
            {
                timer = 0f;
                while (timer < 1.5f)
                {
                    if (baseGameManager == null)
                    {
                        yield break;
                    }
                    timer += Time.deltaTime;
                    yield return null;
                }
                SetAllCellsLight(1f, 1f - i / 10f, 1f - i / 10f);
            }
            yield break;
        }

        public static void SetAllCellsLight(float r, float g, float b)
        {
            if (Singleton<BaseGameManager>.Instance == null)
            {
                return;
            }
            foreach (Cell cell in Singleton<BaseGameManager>.Instance.Ec.cells)
            {
                cell.lightColor = new Color(r, g, b, 1f);
                Singleton<BaseGameManager>.Instance.Ec.UpdateLightingAtCell(cell);
            }
            Color skyboxColor = new Color(r, g, b, 1f);
            Shader.SetGlobalColor("_SkyboxColor", skyboxColor);
        }

        public static void SetAllCellsLight(float r, float g, float b, BaseGameManager baseGameManager)
        {
            if (baseGameManager == null)
            {
                return;
            }
            foreach (Cell cell in baseGameManager.Ec.cells)
            {
                cell.lightColor = new Color(r, g, b, 1f);
                baseGameManager.Ec.UpdateLightingAtCell(cell);
            }
            Color skyboxColor = new Color(r, g, b, 1f);
            Shader.SetGlobalColor("_SkyboxColor", skyboxColor);
        }
    }

    [HarmonyPatch(typeof(Gum))]
    public class GumPatches
    {
        [HarmonyPatch("EntityTriggerExit")]
        [HarmonyPrefix]
        public static bool EntityTriggerExitPrefix(Gum __instance)
        {
            if (__instance.beans == null)
            {
                return false;
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(CoreGameManager))]
    public class CoreGameManagerPatches
    {
        [HarmonyPatch("EndGame")]
        [HarmonyPrefix]
        public static void EndGamePrefix(CoreGameManager __instance)
        {
            __instance.musicMan.FlushQueue(true);
        }
    }

    [HarmonyPatch(typeof(ElevatorScreen))]
    public class ElevatorScreenPatches
    {
        [HarmonyPatch("Start")]
        [HarmonyPostfix]
        public static void StartPostfix()
        {
            CoreGameManager coreGameManager = Singleton<CoreGameManager>.Instance;
            if (coreGameManager != null)
            {
                coreGameManager.musicMan.FlushQueue(true);
            }
        }

        [HarmonyPatch("StartGame")]
        [HarmonyPostfix]
        public static void StartGamePostfix()
        {
            CoreGameManager coreGameManager = Singleton<CoreGameManager>.Instance;
            BaseGameManager baseGameManager = Singleton<BaseGameManager>.Instance;
            if (coreGameManager != null)
            {
                if (baseGameManager != null)
                {
                    if (baseGameManager.GameMode == GameMode.HideAndSeek)
                    {
                        if (coreGameManager.sceneObject.levelTitle == "F3")
                        {
                            MainGameManagerPatches.SetAllCellsLight(0.1f, 0.1f, 0.2f, baseGameManager);
                        }
                        if (coreGameManager.sceneObject.levelTitle == "F4")
                        {
                            MainGameManagerPatches.SetAllCellsLight(0.1f, 0f, 0.1f, baseGameManager);
                        }
                    }
                }
            }
        }

        [HarmonyPatch("Update")]
        [HarmonyPostfix]
        public static void UpdatePostfix()
        {
            if (Singleton<MusicManager>.Instance.MidiPlaying)
            {
                Singleton<MusicManager>.Instance.StopMidi();
            }
        }
    }

    [HarmonyPatch(typeof(Pickup))]
    public class PickupPatches
    {
        [HarmonyPatch("Clicked")]
        [HarmonyPrefix]
        public static bool ClickedPrefix(int player, Pickup __instance)
        {
            ItemManager itemManager = Singleton<CoreGameManager>.Instance.GetPlayer(player).itm;
            if (__instance.item == BasePlugin.AssetMan.Get<ItemObject>("ITM_Key0") && Singleton<CoreGameManager>.Instance.sceneObject.levelTitle == "F1")
            {
                for (int i = 0; i < itemManager.items.Length; i++)
                {
                    if (itemManager.items[i] == BasePlugin.AssetMan.Get<ItemObject>("ITM_Sword"))
                    {
                        itemManager.RemoveItem(i);
                    }
                }
                return true;
            }
            if (itemManager.items[itemManager.selectedItem] == BasePlugin.AssetMan.Get<ItemObject>("ITM_Sword"))
            {
                return false;
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(NpcState))]
    public class NpcStatePatches
    {
        [HarmonyPatch("OnStateTriggerEnter")]
        [HarmonyPrefix]
        public static bool OnStateTriggerEnterPrefix(Entity otherEntity, Collider other, bool validCollision, NpcState __instance)
        {
            return TryToKillPlayer(other, validCollision, __instance.Npc);
        }

        [HarmonyPatch("OnStateTriggerStay")]
        [HarmonyPrefix]
        public static bool OnStateTriggerStayPrefix(Entity otherEntity, Collider other, bool validCollision, NpcState __instance)
        {
            return TryToKillPlayer(other, validCollision, __instance.Npc);
        }

        public static bool TryToKillPlayer(Collider other, bool validCollision, NPC npc)
        {
            if (Singleton<CoreGameManager>.Instance.sceneObject.levelTitle != "F3" || IsSafe(npc))
            {
                return true;
            }
            if (validCollision && other.CompareTag("Player"))
            {
                Other.NpcKillPlayer(npc);
            }
            return false;
        }

        public static bool IsSafe(NPC npc)
        {
            bool isSafe = false;

            bool isStudent = npc.TryGetComponent<Student>(out _);
            bool isPlaytime = npc.TryGetComponent<Playtime>(out _);
            bool isBully = npc.TryGetComponent<Bully>(out _);
            bool isBeans = npc.TryGetComponent<Beans>(out _);
            bool isFirstPrize = npc.TryGetComponent<FirstPrize>(out _);
            bool isArtsAndCrafters = npc.TryGetComponent<ArtsAndCrafters>(out _);
            bool isCumulo = npc.TryGetComponent<Cumulo>(out _);
            bool isBaldi = npc.TryGetComponent<Baldi>(out _);
            bool isPrincipal = npc.TryGetComponent<Principal>(out _);
            bool isTest = npc.TryGetComponent<LookAtGuy>(out _);
            bool isHer = npc.TryGetComponent<Her>(out _);
            bool isSnowAttack = npc.TryGetComponent<SnowAttack>(out _);
            bool isIce = npc.TryGetComponent<Ice>(out _);
            if (isHer || isSnowAttack || isIce || isStudent || isPlaytime || isBully || isBeans || isFirstPrize || isArtsAndCrafters || isCumulo || isBaldi || isPrincipal || isTest)
            {
                isSafe = true;
            }

            bool isGottaSweep = npc.TryGetComponent<GottaSweep>(out _);
            bool isNoLateTeacher = npc.TryGetComponent<NoLateTeacher>(out _);
            if (BasePlugin.Instance.ConfigEasyMode.Value && (isGottaSweep || isNoLateTeacher))
            {
                isSafe = true;
            }

            return isSafe;
        }
    }
}
