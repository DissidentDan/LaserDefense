using UnityEngine;
using System.Collections;

public enum ELaserResponse
{
	Continue,
	Stop,
	Bounce
}

public struct LaserReaction
{
	public ELaserResponse ResponseType;
	public float Parameter;

	public LaserReaction(ELaserResponse response, float param) {
		ResponseType = response;
		Parameter = param;
	}
}

public class InteractsWithLaser : MonoBehaviour {
	public int Health = 0;
	GameWorld World = null;

	// Use this for initialization
	void Start () {
		World = (GameWorld)FindObjectOfType (typeof(GameWorld));
		World.DamageableObjects.Add (this);
	}

	void Die()
	{
		World.DamageableObjects.Remove (this);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public virtual LaserReaction OnLaserHit(Laser laser, bool bPreview) {
		if (!bPreview && Health > 0) {
			Health--;
			if (Health <= 0)
				Die ();
		}
		return new LaserReaction (ELaserResponse.Continue, 0);
	}
}
