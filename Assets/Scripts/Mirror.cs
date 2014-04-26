using UnityEngine;
using System.Collections;

public class Mirror : InteractsWithLaser {

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public override LaserReaction OnLaserHit(Laser laser, bool bPreview) {
		return new LaserReaction(ELaserResponse.Bounce, 0);
	}
}
