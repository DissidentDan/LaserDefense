using UnityEngine;
using System.Collections;

public enum EMouseEventType
{
	OnMouseDown,
	OnMouseUp,
	OnMouseClick,
	OnMouseDrag
}

public class InputDelegator : MonoBehaviour {
	const int UICollisionMask = (1 << 8);

	bool bMouseDown = false;
	float MouseDownStartTime = 0;
	Vector3 MouseDownStartPosition;
	GameObject DragObject;

	const float MouseClickMaxDuration = 0.5f;
	const float MouseDragMinimumPixels = 20;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		bool wasDown = bMouseDown;
		bool isDown = Input.GetMouseButton (0);
		bMouseDown = isDown;

		GameObject touchObject = GetObjectAtMouseLocation ();

		if (Input.GetMouseButton(1))
		{
			Debug.Log("right button");
		}

		if (isDown)
		{
			if (wasDown)
			{
				if (null == DragObject)
				{
					float dist = (Input.mousePosition - MouseDownStartPosition).magnitude;
					if (dist > MouseDragMinimumPixels)
					{
						DragObject = touchObject;
					}
				}
			}
			else
			{
				MouseDownStartTime = Time.time;
				MouseDownStartPosition = Input.mousePosition;
				DelegateMouseEvent(touchObject, EMouseEventType.OnMouseDown, Input.mousePosition);
				return;
			}
		}
		else
		{
			if (wasDown)
			{
				float downDuration = Time.time - MouseDownStartTime;
				if (downDuration <= MouseClickMaxDuration)
					DelegateMouseEvent(touchObject, EMouseEventType.OnMouseClick, Input.mousePosition);
				DelegateMouseEvent(touchObject, EMouseEventType.OnMouseUp, Input.mousePosition);
			}

			if (null != DragObject && DragObject != touchObject)
			{
				DelegateMouseEvent(DragObject, EMouseEventType.OnMouseUp, Input.mousePosition);
			}
			DragObject = null;
		}

		if (null != DragObject)
		{
			DelegateMouseEvent(DragObject, EMouseEventType.OnMouseDrag, Input.mousePosition - MouseDownStartPosition);
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

	void DelegateMouseEvent(GameObject target, EMouseEventType mouseEvent, Vector3 parameter) {
		if (null == target)
			return;

		string eventName = mouseEvent.ToString ();
		target.SendMessage (eventName, parameter, SendMessageOptions.DontRequireReceiver);
	}
}
