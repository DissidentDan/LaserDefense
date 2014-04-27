using UnityEngine;
using System.Collections;

public class Enemy : InteractsWithLaser {

	// Use this for initialization
	void Start () 
	{

	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}

	public override LaserReaction OnLaserHit(Laser laser, bool bPreview) {
		return new LaserReaction(ELaserResponse.Stop, 0);
	}
	public void HandlePathComplete()
	{
		Debug.Log("Enemy has completed path");
		//lose a life
	}
}
