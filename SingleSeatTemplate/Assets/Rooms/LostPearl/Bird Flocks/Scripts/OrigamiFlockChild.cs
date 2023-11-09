/**************************************									
	FlockChild v2.3
	Copyright Unluck Software	
 	www.chemicalbliss.com								
***************************************/

using UnityEngine;
using Artanim;
using Artanim.Location.Network;

public class OrigamiFlockChild:FlockChild{
    
    public override Vector3 findWaypoint(){
    	Vector3 t = Vector3.zero;
        t.x = _spawner.spawnSphereSetPos[_spawner.waypointIdx % _spawner.spawnSphereSetPos.Length].x + _spawner._posBuffer.x;
        t.z = _spawner.spawnSphereSetPos[_spawner.waypointIdx % _spawner.spawnSphereSetPos.Length].z + _spawner._posBuffer.z;
        t.y = _spawner.spawnSphereSetPos[_spawner.waypointIdx % _spawner.spawnSphereSetPos.Length].y + _spawner._posBuffer.y;
        _spawner.incrementWaypointIdx();

    //    t.x = Random.Range(-_spawner._spawnSphere, _spawner._spawnSphere) + _spawner._posBuffer.x;
    //    t.z = Random.Range(-_spawner._spawnSphereDepth, _spawner._spawnSphereDepth) + _spawner._posBuffer.z;
    //    t.y = Random.Range(-_spawner._spawnSphereHeight, _spawner._spawnSphereHeight) + _spawner._posBuffer.y;
        return t;
    }
    

}
