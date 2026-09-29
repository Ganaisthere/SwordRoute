using MTM101BaldAPI.Reflection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SwordRoute
{
    public class SnowAttack : NPC
    {
        public override void Initialize()
        {
            base.Initialize();
            spriteRenderer[0].sprite = BasePlugin.AssetMan.Get<Sprite>("Snow");
            this.Entity.SetFrozen(true);
            behaviorStateMachine.ChangeState(new SnowAttack_Attack(this));
        }
    }

    public class SnowAttack_StateBase : NpcState
    {
        public SnowAttack snowAttack;
        public SnowAttack_StateBase(SnowAttack me) : base(me)
        {
            snowAttack = me;
        }
    }

    public class SnowAttack_Attack : SnowAttack_StateBase
    {
        public SnowAttack_Attack(SnowAttack me) : base(me)
        {
            snowAttack = me;
        }

        public override void Enter()
        {
            base.Enter();
            ChangeNavigationState(new NavigationState_Disabled(npc));
            snowAttack.StartCoroutine(Action());
        }

        public IEnumerator Action()
        {
            Vector3 forward = Singleton<CoreGameManager>.Instance.GetCamera(0).transform.forward;
            float timer = 0f;
            while (timer < 3f)
            {
                npc.transform.position += forward * 40f * Time.deltaTime;
                timer += Time.deltaTime;
                yield return null;
            }
            npc.Despawn();
            yield break;
        }

        public override void OnStateTriggerStay(Entity otherEntity, Collider other, bool validCollision)
        {
            base.OnStateTriggerStay(otherEntity, other, validCollision);
            if (other.CompareTag("Player") || !NeedFrozen(other))
            {
                return;
            }
            if (other.TryGetComponent(out NPC otherNpc))
            {
                npc.ec.SpawnNPC(BasePlugin.AssetMan.Get<Ice>("Ice"), npc.ec.CellFromPosition(otherNpc.transform.position).position);

                if (otherNpc.Character == Character.Playtime)
                {
                    Playtime playtime = otherNpc.GetComponent<Playtime>();
                    if (playtime != null)
                    {
                        Jumprope currentJumprope = playtime.ReflectionGetVariable("currentJumprope") as Jumprope;
                        if (Singleton<CoreGameManager>.Instance.GetPlayer(0).jumpropes.Count > 0 && currentJumprope != null)
                        {
                            List<Jumprope> getJumprope = new List<Jumprope>();
                            foreach (Jumprope jumprope in Singleton<CoreGameManager>.Instance.GetPlayer(0).jumpropes)
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
                else if (otherNpc.Character == Character.Beans)
                {
                    Beans beans = otherNpc.GetComponent<Beans>();
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
                else if (otherNpc.Character == Character.Cumulo)
                {
                    Cumulo cumulo = otherNpc.GetComponent<Cumulo>();
                    if (cumulo != null)
                    {
                        Cell _currentEndCell = cumulo.ReflectionGetVariable("_currentEndCell") as Cell;
                        if (_currentEndCell != null)
                        {
                            cumulo.StopBlowing();
                        }
                    }
                }

                Singleton<CoreGameManager>.Instance.audMan.PlaySingle(BasePlugin.AssetMan.Get<SoundObject>("snd_icespell"));

                otherNpc.Despawn();
            }
        }

        public bool NeedFrozen(Collider other)
        {
            bool needFrozen = true;

            bool isHer = other.TryGetComponent<Her>(out _);
            bool isSnowAttack = other.TryGetComponent<SnowAttack>(out _);
            bool isIce = other.TryGetComponent<Ice>(out _);
            bool isChalkFace = other.TryGetComponent<ChalkFace>(out _);

            if (isHer || isSnowAttack || isIce || isChalkFace)
            {
                needFrozen = false;
            }

            return needFrozen;
        }
    }
}
