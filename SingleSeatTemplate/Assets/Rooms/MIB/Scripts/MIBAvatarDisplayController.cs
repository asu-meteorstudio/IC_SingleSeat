using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Artanim
{
    public class MIBAvatarDisplayController : AvatarDisplayController
    {

        [Tooltip("The avatars visual root used to show/hide the avatars body")]
        public GameObject[] AvatarVisuals;

        public GameObject casualRoot;

        public GameObject[] standaloneVisuals;

        public GameObject[] mibAvatar;

        public GameObject[] zarthanAvatar;

        [Header("AndrogynousFemale ArmsSwap")]
        public bool performArmSwap;
        public GameObject suitArms;
        public GameObject[] casualArmsAndHead;

        public override void InitializePlayer(string initials)
        {

        }

        public void ToggleAvatar(bool state)
        {
            if (mibAvatar != null)
            {
                HideAvatar();
                foreach (GameObject go in mibAvatar)
                    go.SetActive(true);
            }
                
        }

        public void ShowZarthanAvatar()
        {
            HideAvatar();
            AvatarController ac = GetComponent<AvatarController>();
            if (ac == null)
                return;
            if(ac.IsMainPlayer)
            {
                foreach (GameObject go in zarthanAvatar)
                    go.SetActive(true);
            }
        }

        public void TurnOffCasualRoot()
        {
            casualRoot.SetActive(false);
            if(performArmSwap)
            {
                suitArms.SetActive(true);
                foreach(GameObject go in casualArmsAndHead)
                    go.SetActive(false);
            }
        }

        public void SwitchToStandaloneAvatar()
        {
            if(DevelopmentMode.CurrentMode == EDevelopmentMode.Standalone)
            {
                foreach (GameObject go in AvatarVisuals)
                    go.SetActive(false);
                foreach (GameObject go in mibAvatar)
                    go.SetActive(false);
                foreach(GameObject go in standaloneVisuals)
                {
                    go.SetActive(true);
                }
            }
        }

        public override void ShowAvatar()
        {
            if (AvatarVisuals != null && AvatarVisuals.Length > 0 && mibAvatar != null && mibAvatar.Length > 0)
            {
                foreach (var avatarVisual in AvatarVisuals)
                {
                    if (avatarVisual)
                        avatarVisual.SetActive(true);
                }
                foreach (GameObject go in mibAvatar)
                    go.SetActive(true);
                foreach (GameObject go in standaloneVisuals) //TODO - Adrian: Super janky and breaks the standalone prefab functionality at the end. Abusing this to make zarthan portal swap work
                    go.SetActive(true);
            }
            else
            {
                Debug.LogError("Failed to show avatar. No AvatarVisualRoot set.");
            }
        }

        public override void HideAvatar()
        {
            if (AvatarVisuals != null && AvatarVisuals.Length > 0 && mibAvatar != null && mibAvatar.Length > 0)
            {
                foreach (var avatarVisual in AvatarVisuals)
                {
                    if (avatarVisual)
                        avatarVisual.SetActive(false);
                }
                foreach (GameObject go in mibAvatar)
                    go.SetActive(false);
                foreach (GameObject go in standaloneVisuals) //TODO - Adrian: Super janky and breaks the standalone prefab functionality at the end. Abusing this to make zarthan portal swap work
                    go.SetActive(false);
                foreach (GameObject go in zarthanAvatar)
                    go.SetActive(false);
            }
            else
            {
                Debug.LogError("Failed to hide avatar. No AvatarVisualRoot set.");
            }
        }

        public override void ShowHead()
        {
            //Do nothing
        }

        public override void HideHead()
        {
            //DoNothing
        }
    }
}
