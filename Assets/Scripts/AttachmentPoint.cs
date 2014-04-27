using UnityEngine;
using System.Collections;

public class AttachmentPoint : MonoBehaviour {

	GameWorld World;
	UIManager UIMgr;
	MenuDesc PlacementMenu;
	Mirror MyMirror;

	// Use this for initialization
	void Start () {
		World = (GameWorld)FindObjectOfType (typeof(GameWorld));
		UIMgr = FindObjectOfType<UIManager> ();
		CreatePlacementMenu ();
	}

	// Update is called once per frame
	void Update () {
	
	}

	void CreatePlacementMenu() {
		PlacementMenu = new MenuDesc ();
		PlacementMenu.Buttons = new UIButtonDesc[1];
		PlacementMenu.Buttons [0].Text = "Place Mirror";
		PlacementMenu.Buttons [0].Action = PlaceMirror;
	}

	public void OnMouseClick(Vector3 mouseScreenPos) {
		if (null != MyMirror)
			return;

		if (null == PlacementMenu.Buttons)
			return;

		PlacementMenu.Buttons [0].ScreenPosition = Camera.main.WorldToScreenPoint (transform.position);
		UIMgr.OpenMenu (PlacementMenu);
	}

	public void PlaceMirror()
	{
		GameObject mirrorObj = (GameObject)GameObject.Instantiate (World.MirrorPrefab);
		MyMirror = mirrorObj.GetComponent<Mirror> ();

		MyMirror.transform.position = transform.position;
		MyMirror.transform.rotation = transform.rotation;

		MyMirror.transform.parent = transform;
	}
}
