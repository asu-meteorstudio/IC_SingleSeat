using Artanim;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneControllButton : ServerSideBehaviour
{
	public enum ESceneAction { Next, Prev }

	public ESceneAction SceneAction;
	
	void Start()
	{

	}

	public void DoActivate()
	{
		switch (SceneAction)
		{
			case ESceneAction.Next:
				GameController.Instance.LoadNextScene();
				break;
			case ESceneAction.Prev:
				GameController.Instance.LoadPrevScene();
				break;
		}
	}

	public void LoadScene(string sceneName)
    {
		GameController.Instance.LoadGameScene(sceneName, Artanim.Location.Messages.Transition.FadeBlack, Artanim.Location.Messages.ELoadSequence.LoadFirst);
    }
}
