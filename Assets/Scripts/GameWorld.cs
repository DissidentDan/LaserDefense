using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameWorld : MonoBehaviour {

	public List<Mirror> Mirrors = new List<Mirror>();
	public List<InteractsWithLaser> DamageableObjects = new List<InteractsWithLaser>();

	// Use this for initialization
	void Start () {
		FindAllOfType<Mirror> (Mirrors);
	}

	void FindAllOfType<T>(List<T> outList) where T : Component
	{
		outList.Clear ();
		object[] sceneObjects = GameObject.FindObjectsOfType (typeof(T));
		foreach (GameObject obj in sceneObjects) {
			T found = obj.GetComponent<T>();
			if (null != found)
				outList.Add (found);
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
