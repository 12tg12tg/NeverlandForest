using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindowManager : MonoBehaviour
{
	[HideInInspector]
	public GenericWindow[] windows;
	public int currentWindowID;
	public int defaultWindowID;
	public int NextWindowID;
	public GenericWindow GetWindow(int value)
	{
		return windows[value];
	}

	private void CloseAllAndOpen(int value, bool isPopUpOpen) 
	{
		var total = windows.Length;

		if (!isPopUpOpen)
		{
			for (var i = 0; i < total; i++)
			{
				var window = windows[i];
				if (window.gameObject.activeSelf)
					window.Close();
			}
		}
		GetWindow(value ).Open();
	}
	public GenericWindow Open(int value, bool isPopUpOpen = false)
	{
		if (value < 0 || value >= windows.Length)
			return null;

		currentWindowID = value;

		CloseAllAndOpen(currentWindowID, isPopUpOpen);

		return GetWindow(currentWindowID);
	}
	void Start()
	{
		GenericWindow.manager = this;
		Open(defaultWindowID,true);
		Open(NextWindowID, true);
	}
}
