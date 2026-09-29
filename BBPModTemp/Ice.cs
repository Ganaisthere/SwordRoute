using UnityEngine;

namespace SwordRoute
{
    public class Ice : NPC
    {
        public override void Initialize()
        {
            base.Initialize();
            spriteRenderer[0].sprite = BasePlugin.AssetMan.Get<Sprite>("Ice");
            behaviorStateMachine.ChangeState(new Ice_Disabled(this));
        }
    }

    public class Ice_StateBase : NpcState
    {
        public Ice ice;
        public Ice_StateBase(Ice me) : base(me)
        {
            ice = me;
        }
    }

    public class Ice_Disabled : Ice_StateBase
    {
        public Ice_Disabled(Ice me) : base(me)
        {
            ice = me;
        }

        public override void Enter()
        {
            base.Enter();
            ChangeNavigationState(new NavigationState_Disabled(npc));
        }
    }
}
