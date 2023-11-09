using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Artanim;

public class RegisterAvatarOffset : MonoBehaviour
{
    public AvatarOffset avatarOffset;
    System.Guid playerId;
    public Transform snowmobile;

    private void Start()
    {
        RegisterOffset();
    }

    //Register avatar offsets to the seat 
    public void RegisterOffset()
    {
        for (int i = 0; i < GameController.Instance.RuntimePlayers.Count; i++)
        {
            //transforms position from local origin to world origin which is the snow mobile
            transform.TransformVector(snowmobile.transform.position.x, snowmobile.transform.position.y, snowmobile.transform.position.z);
            transform.TransformVector(snowmobile.transform.rotation.x, snowmobile.transform.rotation.y, snowmobile.transform.rotation.z);

            RuntimePlayer player = GameController.Instance.RuntimePlayers[i];

            print("player" + player + "playerId" + playerId + "offset" + avatarOffset);

            AvatarOffsetController.Instance.RegisterAvatarOffset(player.AvatarController.PlayerId, this.avatarOffset, true, AvatarOffsetController.ESyncMode.Unsynced, true);
        }


        

        //transform.TransformDirection(snowmobile.transform.rotation.x, snowmobile.transform.rotation.y, snowmobile.transform.rotation.z);

    }
}
