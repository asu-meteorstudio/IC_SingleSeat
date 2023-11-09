using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Artanim;

public class RegisterOffsets : MonoBehaviour
{
    public AvatarOffset avatarOffset;
    System.Guid playerId;
    public Transform vSeat;
   
    public Transform tempPodPlayer;

    public Transform tempPodCenter;
    private void Awake()
    {
        RegisterOffset();
    }

    //Register avatar offsets to the seat 
    public void RegisterOffset()
    {
        for (int i = 0; i < GameController.Instance.RuntimePlayers.Count; i++)
        {
            //transforms position from local origin to world origin which is the snow mobile
            //transform.TransformVector(snowmobile.transform.position.x, snowmobile.transform.position.y, snowmobile.transform.position.z);
            //transform.TransformVector(snowmobile.transform.rotation.x, snowmobile.transform.rotation.y, snowmobile.transform.rotation.z);

            RuntimePlayer player = GameController.Instance.RuntimePlayers[i];

            print("player" + player + "playerId" + playerId + "offset" + avatarOffset);

            AvatarOffsetController.Instance.RegisterAvatarOffset(player.AvatarController.PlayerId, this.avatarOffset, true, AvatarOffsetController.ESyncMode.Unsynced, true);
            //avatarOffset.transform.localRotation = vSeat.transform.localRotation * Quaternion.Inverse(player.AvatarController.GetAvatarRoot().rotation);// seatMappingMessage.physicalSeatRotation); ;

            //Move temp Pod Player to avatar position
            tempPodPlayer.position = player.AvatarController.GetAvatarRoot().position;
            tempPodPlayer.rotation = player.AvatarController.GetAvatarRoot().rotation;
            
            //Move tempPod Center to pod center
            tempPodCenter.parent = null;
            tempPodCenter.position = avatarOffset.transform.position;
            tempPodCenter.rotation = avatarOffset.transform.rotation;

            //Child Center to Player, and relocate player to seat
            tempPodCenter.parent = tempPodPlayer;
            tempPodPlayer.position = vSeat.transform.position;
            tempPodPlayer.rotation = vSeat.transform.rotation;

            //Set the avatarOffset to the tempPod center
            avatarOffset.transform.position = tempPodCenter.position;
            avatarOffset.transform.rotation = tempPodCenter.rotation;
            //avatarOffset.transform.localPosition = vSeat.transform.localPosition - player.AvatarController.GetAvatarRoot().position;

        }




        //transform.TransformDirection(snowmobile.transform.rotation.x, snowmobile.transform.rotation.y, snowmobile.transform.rotation.z);

    }
}
