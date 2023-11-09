using Artanim.Location.Messages;
using Artanim.Location.Network;
using Artanim.Location.SharedData;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Artanim.Location.Data;

public class CreateSessionTest : MonoBehaviour
{

	void Start ()
    {
        if(NetworkInterface.Instance.IsClient)
            StartCoroutine(CreateSession());
	}
	
	
    private IEnumerator CreateSession()
    {
        //Find first available server...
        Guid serverGuid = Guid.Empty;
        yield return FindServer(guid => { serverGuid = guid; });

        //Ask server to create session
        NetworkInterface.Instance.SendMessage(new PrepareNewSession
        {
            RecipientId = serverGuid,
        });

        //Find created session
        Guid sessionId = Guid.Empty;
        yield return FindFirstSession(guid => { sessionId = guid; });

        //Find skeleton
        Guid skeletonId = Guid.Empty;
        yield return FindFirstSkeleton(guid => { skeletonId = guid; });

        //Wait until we're ready for session
        yield return new WaitUntil(() => SharedDataUtils.GetMyComponent<ExperienceClient>().Status == ELocationComponentStatus.ReadyForSession);

        //Add player to session
        NetworkInterface.Instance.SendMessage(new RequestPlayerJoinSession
        {
            SessionId = sessionId,
            Player = new Player
            {
                ComponentId = SharedDataUtils.MySharedId,
                SkeletonId = skeletonId,
                Avatar = "Lisa",
                CalibrationMode = ECalibrationMode.Normal,
            },
        } );

        //Wait until preparing session
        yield return new WaitUntil(() => SharedDataUtils.GetMyComponent<ExperienceClient>().Status == ELocationComponentStatus.PreparingSession);

        //Calibrate
        NetworkInterface.Instance.SendMessage(new RecalibratePlayer
        {
            ExperienceClientId = SharedDataUtils.MySharedId,
        });
    }

    private IEnumerator FindServer(Action<Guid> callback)
    {
        ExperienceServer server = null;
        do
        {
            server = SharedDataUtils.Components.OfType<ExperienceServer>().FirstOrDefault();
            yield return null;
        } while (server == null);

        callback.Invoke(server.SharedId);
    }

    private IEnumerator FindFirstSession(Action<Guid> callback)
    {
        Session session = null;
        do
        {
            session = SharedDataUtils.Sessions.FirstOrDefault();
            yield return null;
        } while (session == null);

        callback.Invoke(session.SharedId);
    }

    private IEnumerator FindFirstSkeleton(Action<Guid> callback)
    {
        SkeletonConfig skeleton = null;
        do
        {
            skeleton = SharedDataUtils.SkeletonConfigs.FirstOrDefault();
            yield return null;
        } while (skeleton == null);

        callback.Invoke(skeleton.SharedId);
    }
}
