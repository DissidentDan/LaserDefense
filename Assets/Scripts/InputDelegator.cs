using UnityEngine;
using System.Collections;

public enum EMouseEventType
{
	OnMouseDown,
	OnMouseUp,
	OnMouseClick
}

public class InputDelegator : MonoBehaviour {
	const int UICollisionMask = (1 << 8);

	bool bMouseDown = false;
	float MouseDownStartTime = 0;

	const float MouseClickMaxDuration = 0.5f;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		bool wasDown = bMouseDown;
		bool isDown = Input.GetMouseButton (0);
		bMouseDown = isDown;
		if (isDown && !wasDown) {
			MouseDownStartTime = Time.time;
			DelegateMouseEvent(EMouseEventType.OnMouseDown);
			return;
		}
		if (!isDown && wasDown) {
			float downDuration = Time.time - MouseDownStartTime;
			if (downDuration <= MouseClickMaxDuration)
				DelegateMouseEvent(EMouseEventType.OnMouseClick);
			DelegateMouseEvent(EMouseEventType.OnMouseUp);
		}
	}

	GameObject GetObjectAtMouseLocation() {
		Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
		
		GameObject closest = null;
		float smallestDistance = 10000000;
		
		RaycastHit[] hits = Physics.RaycastAll (ray, 1000, UICollisionMask);
		foreach (RaycastHit hit in hits) {
			Vector3 objCenter_ScreenSpace = Camera.main.WorldToScreenPoint(hit.collider.transform.position);
			float dist = (objCenter_ScreenSpace - Input.mousePosition).magnitude;
			if (dist < smallestDistance)
			{
				smallestDistance = dist;
				closest = hit.collider.gameObject;
			}
		}

		if (null == closest)
			return null;

		return closest.transform.parent.gameObject;
	}

	void DelegateMouseEvent(EMouseEventType mouseEvent) {
		GameObject closest = GetObjectAtMouseLocation ();

		if (null == closest)
			return;

		string eventName = mouseEvent.ToString ();
		closest.SendMessage (eventName, Input.mousePosition, SendMessageOptions.DontRequireReceiver);
	}
}
