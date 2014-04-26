using UnityEngine;
using System.Collections;

public class CamMovement : MonoBehaviour {

	public float FlySpeed = 10;
	public GameObject ViewTarget;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		float moveAmount = Time.deltaTime * FlySpeed;
		if (Input.GetKey(KeyCode.W))
			transform.position += transform.forward * moveAmount;
		if (Input.GetKey(KeyCode.S))
			transform.position -= transform.forward * moveAmount;
		if (Input.GetKey(KeyCode.D))
			transform.position += transform.right * moveAmount;
		if (Input.GetKey(KeyCode.A))
			transform.position -= transform.right * moveAmount;
		if (Input.GetKey(KeyCode.E))
			transform.position += transform.up * moveAmount;
		if (Input.GetKey(KeyCode.Q))
			transform.position -= transform.up * moveAmount;

		if (null != ViewTarget)
			transform.LookAt (ViewTarget.transform.position);
	}
}
