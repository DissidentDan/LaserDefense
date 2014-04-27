using UnityEngine;
using System.Collections;

public struct UIButtonDesc
{
	public Vector3 ScreenPosition;
	public delegate void ButtonAction ();
	public ButtonAction Action;
	public string Text;
}

public struct MenuDesc
{
	public UIButtonDesc[] Buttons;
}

public class UIManager : MonoBehaviour {

	MenuDesc CurrentMenu;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void OpenMenu(MenuDesc menu)
	{
		CurrentMenu = menu;
	}

	public void OnGUI()
	{
		if (null == CurrentMenu.Buttons)
			return;

		for (int b = 0; b < CurrentMenu.Buttons.Length; b++) {
			Vector3 center = CurrentMenu.Buttons[b].ScreenPosition;
			Rect rect = new Rect(center.x - 200, Camera.main.pixelHeight - center.y - 100, 400, 200);
			if (GUI.Button(rect, CurrentMenu.Buttons[b].Text))
			{
				if (null != CurrentMenu.Buttons[b].Action)
					CurrentMenu.Buttons[b].Action();
				CurrentMenu = new MenuDesc();
				return;
			}
		}
	}
}
