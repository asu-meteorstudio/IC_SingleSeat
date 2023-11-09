using Artanim.Location.Network;
using Artanim.Location.Messages;
using Artanim.Location.SharedData;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Artanim.Location.Data;

namespace Artanim
{
	public class ProfessorTrigger : AvatarTrigger
	{
		public bool triggerOnlyIfProfessor = true;

		private bool isProfessor(AvatarController avatarController)
		{
			/*var playerComponent = SharedDataUtils.FindLocationComponent<LocationComponentWithSession>(avatarController.PlayerId);
			if (playerComponent != null)
			{
				if (playerComponent.PodId == "PROFESSOR")
					return true;
			}*/

			return avatarController.Player.Avatar.Contains("prof");
		}

		void OnTriggerEnter(Collider other)
		{
			var avatar = other.GetComponentInParent<AvatarController>();
			if (avatar)
			{
				var bodyPart = other.GetComponent<AvatarBodyPart>();
				if (bodyPart)
				{
					// Only send a trigger if we are the professor, or if we are in standalone mode or if we don't want the trigger to work only for PROFESSOR
					if (isProfessor(avatar) || DevelopmentMode.CurrentMode == EDevelopmentMode.Standalone || !triggerOnlyIfProfessor)
					{ 
						//Send network message
						SendTriggerEvent(bodyPart.BodyPart, avatar.PlayerId, true);
					}
				}
			}
		}

		void OnTriggerExit(Collider other)
		{
			var avatar = other.GetComponentInParent<AvatarController>();

			if (avatar)
			{
				var bodyPart = other.GetComponent<AvatarBodyPart>();
				if (bodyPart)
				{
					// Only send a trigger if we are the professor, or if we are in standalone mode
					if (isProfessor(avatar) || DevelopmentMode.CurrentMode == EDevelopmentMode.Standalone || !triggerOnlyIfProfessor)
					{
						//Send network message
						SendTriggerEvent(bodyPart.BodyPart, avatar.PlayerId, false);
					}
				}
			}
		}
	}
}