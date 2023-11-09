using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Artanim;
using Artanim.Location.Network;

public class Suit : MonoBehaviour {

    public Animator suitCaseAnim;
    public Animator nameAnim;
    public Transform footprint;
    public TextTyper alertText;
    private bool shouldAnimate;

	// Use this for initialization
	void Start () {
		
	}

    public void AllowAnimationIfMainPlayer()
    {
        if (NetworkInterface.Instance.IsClient)
        {
            float minDistanceFromPlayer = float.MaxValue;
            RuntimePlayer closestPlayer = GameController.Instance.RuntimePlayers[0];
            foreach (RuntimePlayer player in GameController.Instance.RuntimePlayers)
            {
                float distanceFromPlayer = Vector3.Distance(this.footprint.position, player.AvatarController.GetAvatarRoot().position);
                if (distanceFromPlayer < minDistanceFromPlayer)
                {
                    minDistanceFromPlayer = distanceFromPlayer;
                    closestPlayer = player;
                }

            }
            if (closestPlayer.IsMainPlayer)
                shouldAnimate = true;
        }
    }

    public void HandScannerAnimationTrigger(string trigger)
    {
            nameAnim.SetTrigger(trigger);
    }
    
}
