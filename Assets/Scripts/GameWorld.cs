using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameWorld : MonoBehaviour {

	public List<Mirror> Mirrors { get; set; }
	public List<InteractsWithLaser> DamageableObjects { get; set; }

	public GameObject MirrorPrefab;

	// Use this for initialization
	void Start () {
		Mirrors = new List<Mirror>();
		DamageableObjects = new List<InteractsWithLaser> ();

		FindAllOfType<Mirror> (Mirrors);
		FindAllOfType<InteractsWithLaser> (DamageableObjects);
	}

	void FindAllOfType<T>(List<T> outList) where T : Component
	{
		outList.Clear ();
		object[] sceneObjects = GameObject.FindObjectsOfType (typeof(T));
		foreach (T found in sceneObjects) {
			outList.Add (found);
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
