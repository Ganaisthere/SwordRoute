using System.Collections;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using UnityEngine;

namespace SwordRoute
{
    public class Her : NPC
    {
        public IEnumerator enumerator = null;
        public bool attack = false;
        public bool running = false;

        public override void Initialize()
        {
            base.Initialize();
            enumerator = null;
            attack = false;
            running = false;
            spriteRenderer[0].sprite = BasePlugin.AssetMan.Get<Sprite>("Her_0");
            behaviorStateMachine.ChangeState(new Her_WaitForPlayer(this));
            navigator.SetSpeed(16f);
        }
    }

    public class Her_StateBase : NpcState
    {
        public Her her;
        public Her_StateBase(Her me) : base(me)
        {
            her = me;
        }
    }

    public class Her_WaitForPlayer : Her_StateBase
    {

        public Her_WaitForPlayer(Her me) : base(me)
        {
            her = me;
        }

        public override void Enter()
        {
            base.Enter();
            her.attack = false;
            ChangeNavigationState(new NavigationState_WanderRandom(npc, 63));
        }

        public override void PlayerInSight(PlayerManager player)
        {
            base.PlayerInSight(player);
            if (!her.running)
            {
                her.running = true;
                her.enumerator = SeePlayerAndRun(player);
                npc.StartCoroutine(her.enumerator);
            }
        }

        public override void Update()
        {
            base.Update();
            CoreGameManager coreGameManager = Singleton<CoreGameManager>.Instance;
            if (!coreGameManager.GetPlayer(0).ec.map.arrowTargets.Contains(npc.Entity))
            {
                coreGameManager.GetPlayer(0).ec.map.AddArrow(npc.Entity, new Color(1f, 0f, 1f, 1f));
            }
        }

        public IEnumerator SeePlayerAndRun(PlayerManager player)
        {
            her.running = true;
            Singleton<CoreGameManager>.Instance.audMan.PlaySingle(BasePlugin.AssetMan.Get<SoundObject>("snd_board_sword_metal"));
            ChangeNavigationState(new NavigationState_DoNothing(npc, 63));
            npc.spriteRenderer[0].sprite = BasePlugin.AssetMan.Get<Sprite>("Her_1");

            yield return new WaitForSeconds(1f);

            Singleton<CoreGameManager>.Instance.audMan.PlaySingle(BasePlugin.AssetMan.Get<SoundObject>("snd_board_escaped"));
            ChangeNavigationState(new NavigationState_WanderFlee(npc, 63, player.DijkstraMap));
            npc.spriteRenderer[0].sprite = BasePlugin.AssetMan.Get<Sprite>("Her_0");

            yield break;
        }
    }

    public class Her_FollowPlayer : Her_StateBase
    {
        protected PlayerManager player;

        public Her_FollowPlayer(Her me, PlayerManager playerManager) : base(me)
        {
            her = me;
            player = playerManager;
        }

        public override void Enter()
        {
            base.Enter();
            her.attack = false;
            if (her.enumerator != null)
            {
                npc.StopCoroutine(her.enumerator);
            }
            npc.spriteRenderer[0].sprite = BasePlugin.AssetMan.Get<Sprite>("Her_2");
        }

        public override void Update()
        {
            base.Update();
            float dist = Vector3.Distance(npc.transform.position, player.transform.position);
            if (dist > 150f)
            {
                her.Entity.Teleport(player.transform.position);
            }
            if (dist < 5f)
            {
                ChangeNavigationState(new NavigationState_DoNothing(npc, 63));
            }
            if (dist > 10f)
            {
                ChangeNavigationState(new NavigationState_TargetPosition(npc, 63, player.transform.position));
            }
            if (her.attack)
            {
                her.attack = false;
                her.behaviorStateMachine.ChangeState(new Her_Attack(her, player));
            }
        }
    }

    public class Her_Attack : Her_StateBase
    {
        protected PlayerManager player;
        public Her_Attack(Her me, PlayerManager playerManager) : base(me)
        {
            her = me;
            player = playerManager;
        }

        public override void Enter()
        {
            base.Enter();
            her.attack = false;
            npc.spriteRenderer[0].sprite = BasePlugin.AssetMan.Get<Sprite>("Her_3");
            npc.StartCoroutine(Attack(player));
        }

        public IEnumerator Attack(PlayerManager playerManager)
        {
            Singleton<CoreGameManager>.Instance.audMan.PlaySingle(BasePlugin.AssetMan.Get<SoundObject>("snd_board_damage"));

            yield return new WaitForSeconds(0.5f);

            playerManager.ec.SpawnNPC(BasePlugin.AssetMan.Get<SnowAttack>("SnowAttack"), playerManager.ec.CellFromPosition(npc.transform.position).position);

            Singleton<CoreGameManager>.Instance.audMan.PlaySingle(BasePlugin.AssetMan.Get<SoundObject>("snd_board_sword3"));

            her.behaviorStateMachine.ChangeState(new Her_FollowPlayer(her, player));
        }
    }
}
