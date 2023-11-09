using UnityEngine;
using System.Collections.Generic;
using Artanim;

[ExecuteInEditMode]
public class SphericalMaskController : MonoBehaviour
{
    [Header("Main Properties")]
    //public float radius = 0.5f;
    public float softness = 0.5f;

    public List<Material> sphericalMaskMaterials = new List<Material>();

    [Header("Debug Properties")]
    public bool debugMode;

    public Transform leftHandTransform;
    public Transform rightHandTransform;

    public Transform leftFootTransform;
    public Transform rightFootTransform;

    public Transform headTransform;
    public Transform hipsTransform;

    int globalMaskLeftHandPositionID;
    int globalMaskRightHandPositionID;

    int globalMaskLeftFootPositionID;
    int globalMaskRightFootPositionID;

    int globalMaskHeadPositionID;
    int globalMaskHipsPositionID;

    int globalMaskRadiusID;
    int globalMaskSoftnessID;

    GameController gameController => GameController.Instance;

    Transform avatarHips;

    void Awake()
    {
        globalMaskLeftHandPositionID = Shader.PropertyToID("_GLOBALMaskLeftHandPosition");
        globalMaskRightHandPositionID = Shader.PropertyToID("_GLOBALMaskRightHandPosition");

        globalMaskLeftFootPositionID = Shader.PropertyToID("_GLOBALMaskLeftFootPosition");
        globalMaskRightFootPositionID = Shader.PropertyToID("_GLOBALMaskRightFootPosition");

        globalMaskHeadPositionID = Shader.PropertyToID("_GLOBALMaskHeadPosition");
        globalMaskHipsPositionID = Shader.PropertyToID("_GLOBALMaskHipsPosition");

        globalMaskRadiusID = Shader.PropertyToID("_GLOBALMaskRadius");
        globalMaskSoftnessID = Shader.PropertyToID("_GLOBALMaskSoftness");
    }

    void LateUpdate()
    {
        if (debugMode)
        {
            if (sphericalMaskMaterials != null && leftHandTransform != null && rightHandTransform != null && headTransform != null)
            {
                UpdateValues(leftHandTransform.position, rightHandTransform.position, leftFootTransform.position, rightFootTransform.position, headTransform.position, hipsTransform.position);

                /*
                Shader.SetGlobalVector(globalMaskLeftPositionID, leftTransform.position);
                Shader.SetGlobalVector(globalMaskRightPositionID, rightTransform.position);
                Shader.SetGlobalVector(globalMaskHeadPositionID, headTransform.position);
                Shader.SetGlobalFloat(globalMaskRadiusID, radius);
                Shader.SetGlobalFloat(globalMaskSoftnessID, softness);
                */
            }
        }
        else if(Application.isPlaying)
        {
            if (sphericalMaskMaterials != null && gameController != null && gameController.CurrentPlayer != null && gameController.CurrentPlayer.AvatarController != null)
            {
                if(avatarHips == null)
                { avatarHips = gameController.CurrentPlayer.AvatarController.AvatarAnimator.GetBoneTransform(HumanBodyBones.Hips); }

                UpdateValues(gameController.CurrentPlayer.AvatarController.GetAvatarBodyPart(Artanim.Location.Messages.EAvatarBodyPart.LeftHand).transform.position, gameController.CurrentPlayer.AvatarController.GetAvatarBodyPart(Artanim.Location.Messages.EAvatarBodyPart.RightHand).transform.position,
                    gameController.CurrentPlayer.AvatarController.GetAvatarBodyPart(Artanim.Location.Messages.EAvatarBodyPart.LeftFoot).transform.position, gameController.CurrentPlayer.AvatarController.GetAvatarBodyPart(Artanim.Location.Messages.EAvatarBodyPart.RightFoot).transform.position,
                    gameController.CurrentPlayer.AvatarController.GetAvatarBodyPart(Artanim.Location.Messages.EAvatarBodyPart.Head).transform.position, avatarHips.transform.position);
            }
        }
    }

    public void UpdateValues(Vector3 leftHandPos, Vector3 rightHandPos, Vector3 leftFootPos, Vector3 rightFootPos, Vector3 headPos, Vector3 hipsPos)
    {
        foreach(var mat in sphericalMaskMaterials)
        {
            mat.SetVector(globalMaskLeftHandPositionID, leftHandPos);
            mat.SetVector(globalMaskRightHandPositionID, rightHandPos);

            mat.SetVector(globalMaskLeftFootPositionID, leftFootPos);
            mat.SetVector(globalMaskRightFootPositionID, rightFootPos);

            mat.SetVector(globalMaskHeadPositionID, headPos);
            mat.SetVector(globalMaskHipsPositionID, hipsPos);

            //mat.SetFloat(globalMaskRadiusID, radius);
            mat.SetFloat(globalMaskSoftnessID, softness);
        }
    }

    public void ResetValues()
    {
        UpdateValues(Vector3.zero, Vector3.zero, Vector3.zero, Vector3.zero, Vector3.zero, Vector3.zero);
    }
}
