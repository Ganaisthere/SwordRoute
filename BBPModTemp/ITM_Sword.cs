using MTM101BaldAPI.Reflection;
using Rewired.UI.ControlMapper;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SwordRoute
{
    public class ITM_Sword : Item
    {
        public override bool Use(PlayerManager pm)
        {
            ITM_Sword[] iTM_Swords = FindObjectsOfType<ITM_Sword>();
            if (iTM_Swords.Length <= 1)
            {
                StartCoroutine(Action(pm.ec, pm));
            }
            else
            {
                Object.Destroy(gameObject);
            }
            return false;
        }

        public IEnumerator Action(EnvironmentController Ec, PlayerManager pm)
        {
            if (Singleton<CoreGameManager>.Instance == null)
            {
                Destroy(this);
                yield break;
            }
            Singleton<CoreGameManager>.Instance.audMan.PlaySingle(BasePlugin.AssetMan.Get<SoundObject>("snd_board_sword" + Random.Range(1, 3).ToString()));
            pm.RuleBreak("Bullying", 0.8f, 0.25f);

            if (MainGameManagerPatches.herFound != null)
            {
                MainGameManagerPatches.herFound.attack = true;
            }

            HudManager hudManager = Singleton<CoreGameManager>.Instance.GetHud(pm.playerNumber);

            GameObject sweepMask_Obj = new GameObject("SweepMask");
            sweepMask_Obj.transform.SetParent(hudManager.Canvas().transform);
            RawImage sweepMask = sweepMask_Obj.AddComponent<RawImage>();
            sweepMask.rectTransform.sizeDelta = new Vector2(160f, 64f);
            sweepMask.rectTransform.anchorMax = new Vector2(0.5f, 0f);
            sweepMask.rectTransform.anchorMin = new Vector2(0.5f, 0f);
            sweepMask.rectTransform.anchoredPosition3D = new Vector3(192f, 64f, 0f);
            sweepMask.rectTransform.localScale = Vector3.one;
            sweepMask.rectTransform.SetAsFirstSibling();
            Mask mask = sweepMask_Obj.AddComponent<Mask>();
            mask.showMaskGraphic = false;

            GameObject sweep_Obj = new GameObject("Sweep");
            sweep_Obj.transform.SetParent(sweepMask_Obj.transform);
            Image sweep = sweep_Obj.AddComponent<Image>();
            sweep.rectTransform.sizeDelta = new Vector2(192f, 64f);
            sweep.rectTransform.anchoredPosition3D = new Vector3(-192f, 0f, 0f);
            sweep.rectTransform.localScale = Vector3.one;
            sweep.sprite = BasePlugin.AssetMan.Get<Sprite>("Sweep");
            sweep.maskable = true;

            Window[] windows = Object.FindObjectsOfType<Window>();
            foreach (Window window in windows)
            {
                float dist = Vector3.Distance(pm.plm.Entity.transform.position + Singleton<CoreGameManager>.Instance.GetCamera(pm.playerNumber).transform.forward * 10f, window.transform.position);
                if (dist < 10f)
                {
                    if (Singleton<CoreGameManager>.Instance.sceneObject.levelTitle == "F1")
                    {
                        Singleton<CoreGameManager>.Instance.audMan.PlaySingle(BasePlugin.AssetMan.Get<SoundObject>("snd_board_sword_metal"));
                    }
                    else
                    {
                        window.Break(true);
                    }
                    break;
                }
            }

            bool sweeped = false;
            float timer = 0f;
            while (timer < 0.27f)
            {
                if (Ec == null)
                {
                    yield break;
                }

                if (timer < 0.1f && !sweeped)
                {
                    List<NPC> killNPCs = new List<NPC>();
                    foreach (NPC npc in pm.ec.Npcs)
                    {
                        float dist = Vector3.Distance(pm.plm.Entity.transform.position + Singleton<CoreGameManager>.Instance.GetCamera(pm.playerNumber).transform.forward * 10f, npc.Entity.transform.position);
                        if (dist < 5f && npc.spriteRenderer[0].enabled)
                        {
                            killNPCs.Add(npc);
                        }
                    }

                    Gum[] gums = Object.FindObjectsOfType<Gum>();
                    foreach (Gum gum in gums)
                    {
                        float dist = Vector3.Distance(pm.plm.Entity.transform.position + Singleton<CoreGameManager>.Instance.GetCamera(pm.playerNumber).transform.forward * 10f, gum.transform.position);
                        if (dist < 5f)
                        {
                            bool flying = (bool)gum.ReflectionGetVariable("flying");
                            if (flying)
                            {
                                gum.beans.HitPlayer();
                                gum.beans.GumHit(gum, false);
                                Object.Destroy(gum.gameObject);
                            }
                        }
                    }

                    if (killNPCs.Count > 0)
                    {
                        sweeped = true;

                        while (killNPCs.Count > 0)
                        {
                            if (killNPCs[0].Character == Character.Playtime)
                            {
                                Playtime playtime = killNPCs[0].GetComponent<Playtime>();
                                if (playtime != null)
                                {
                                    Jumprope currentJumprope = playtime.ReflectionGetVariable("currentJumprope") as Jumprope;
                                    if (pm.jumpropes.Count > 0 && currentJumprope != null)
                                    {
                                        List<Jumprope> getJumprope = new List<Jumprope>();
                                        foreach (Jumprope jumprope in pm.jumpropes)
                                        {
                                            if (jumprope == currentJumprope)
                                            {
                                                getJumprope.Add(jumprope);
                                            }
                                        }
                                        if (getJumprope.Count > 0)
                                        {
                                            while (getJumprope.Count > 0)
                                            {
                                                getJumprope[0].End(false);
                                                getJumprope.RemoveAt(0);
                                            }
                                        }
                                    }
                                }
                            }
                            else if (killNPCs[0].Character == Character.Beans)
                            {
                                Beans beans = killNPCs[0].GetComponent<Beans>();
                                if (beans != null)
                                {
                                    if (beans.gum != null)
                                    {
                                        bool flying = (bool)beans.gum.ReflectionGetVariable("flying");
                                        if (flying)
                                        {
                                            Object.Destroy(beans.gum.gameObject);
                                        }
                                    }
                                }
                            }
                            else if (killNPCs[0].Character == Character.Cumulo)
                            {
                                Cumulo cumulo = killNPCs[0].GetComponent<Cumulo>();
                                if (cumulo != null)
                                {
                                    Cell _currentEndCell = cumulo.ReflectionGetVariable("_currentEndCell") as Cell;
                                    if (_currentEndCell != null)
                                    {
                                        cumulo.StopBlowing();
                                    }
                                }
                            }

                            if (killNPCs[0].Character != Character.Chalkles)
                            {
                                if (killNPCs[0].GetType() == typeof(Her))
                                {
                                    if (MainGameManagerPatches.herFound != null)
                                    {
                                        Singleton<CoreGameManager>.Instance.audMan.PlaySingle(BasePlugin.AssetMan.Get<SoundObject>("snd_board_damage"));
                                        killNPCs[0].Entity.AddForce(new Force((killNPCs[0].Entity.transform.position - pm.plm.Entity.transform.position).normalized, 100f, -140f));
                                    }
                                    else
                                    {
                                        Her her = killNPCs[0].GetComponent<Her>();
                                        MainGameManagerPatches.herFound = her;
                                        her.behaviorStateMachine.ChangeState(new Her_FollowPlayer(her, pm));
                                    }
                                }
                                else if (Singleton<CoreGameManager>.Instance.sceneObject.levelTitle == "F1" || killNPCs[0].GetType() == typeof(Ice))
                                {
                                    if (killNPCs[0].GetType() == typeof(Ice))
                                    {
                                        Singleton<CoreGameManager>.Instance.audMan.PlaySingle(BasePlugin.AssetMan.Get<SoundObject>("snd_glassbreak"));
                                    }
                                    else
                                    {
                                        Singleton<CoreGameManager>.Instance.audMan.PlaySingle(BasePlugin.AssetMan.Get<SoundObject>("snd_board_kill"));
                                    }
                                    killNPCs[0].Despawn();
                                }
                                else if (Singleton<CoreGameManager>.Instance.sceneObject.levelTitle == "F2")
                                {
                                    if (Other.TwoLifeNPCs.Contains(killNPCs[0]))
                                    {
                                        Other.TwoLifeNPCs.Remove(killNPCs[0]);
                                        Singleton<CoreGameManager>.Instance.audMan.PlaySingle(BasePlugin.AssetMan.Get<SoundObject>("snd_board_kill"));
                                        killNPCs[0].Despawn();
                                    }
                                    else if (!Other.TwoLifeNPCs.Contains(killNPCs[0]))
                                    {
                                        Other.TwoLifeNPCs.Add(killNPCs[0]);
                                        Singleton<CoreGameManager>.Instance.audMan.PlaySingle(BasePlugin.AssetMan.Get<SoundObject>("snd_board_damage"));
                                        killNPCs[0].Entity.AddForce(new Force((killNPCs[0].Entity.transform.position - pm.plm.Entity.transform.position).normalized, 100f, -140f));
                                    }
                                }
                                else
                                {
                                    Singleton<CoreGameManager>.Instance.audMan.PlaySingle(BasePlugin.AssetMan.Get<SoundObject>("snd_board_sword_metal"));
                                    killNPCs[0].Entity.AddForce(new Force((killNPCs[0].Entity.transform.position - pm.plm.Entity.transform.position).normalized, 100f, -140f));
                                }
                            }
                            killNPCs.RemoveAt(0);
                        }
                    }
                }

                sweepMask.rectTransform.anchoredPosition3D = new Vector3(192f - 384f * (timer / 0.27f), 64f, 0f);
                sweep.rectTransform.anchoredPosition3D = new Vector3(-192f + 384f * (timer / 0.27f), 0f, 0f);

                timer += Time.deltaTime;
                yield return null;
            }

            Object.Destroy(sweep_Obj);
            Object.Destroy(sweepMask_Obj);
            Object.Destroy(gameObject);

            yield break;
        }
    }
}
