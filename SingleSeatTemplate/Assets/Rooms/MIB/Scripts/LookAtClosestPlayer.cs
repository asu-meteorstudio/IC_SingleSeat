using UnityEngine;
using Artanim;
using System.Linq;
using RootMotion.FinalIK;

public class LookAtClosestPlayer : MonoBehaviour
{
    public LookAtIK LookAtIK;

    public float Weight = 1f;

    public void EnableLookAt()
    {
        if (GameController.Instance == null)
            return;

        var playerAvatars = GameController.Instance.RuntimePlayers.Select(rt => rt.AvatarController);
        LookAtIK.solver.target = LookAtIK.transform.FindClosestComponent(playerAvatars).HeadBone;
        LookAtIK.solver.IKPositionWeight = Weight;
    }

    public void DisableLookAt()
    {
        LookAtIK.solver.IKPositionWeight = 0f;
    }
}