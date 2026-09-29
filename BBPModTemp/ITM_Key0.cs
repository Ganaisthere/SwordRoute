using UnityEngine;

namespace SwordRoute
{
    public class ITM_Key0 : Item
    {
        public override bool Use(PlayerManager pm)
        {
            if (MainGameManagerPatches.allElevatorsFound)
            {
                if (Singleton<CoreGameManager>.Instance.sceneObject.levelTitle == "F2")
                {
                    Elevator elevatorUnlock = null;
                    foreach (Elevator elevator in pm.ec.Elevators)
                    {
                        float dist = Vector3.Distance(pm.plm.Entity.transform.position + Singleton<CoreGameManager>.Instance.GetCamera(pm.playerNumber).transform.forward * 10f, elevator.transform.position);
                        if (dist < 30f)
                        {
                            elevatorUnlock = elevator;
                            break;
                        }
                    }
                    if (elevatorUnlock != null)
                    {
                        elevatorUnlock.SetState(ElevatorState.OpenForExit);
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
