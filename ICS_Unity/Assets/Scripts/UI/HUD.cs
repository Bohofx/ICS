using UnityEngine;
using System.Collections;

public class HUD : Singleton<HUD>
{
	[SerializeField]
	public ColorPicker ColorPicker;

	public LensManager<LensHandle, bool> ShowColorPicker;

	void Awake()
	{
		ShowColorPicker = new LensManager<LensHandle, bool>((requests) => 
		{
			ColorPicker.gameObject.SetActive(requests.Count > 0);
			return requests.Count > 0;
		});
		ColorPicker.gameObject.SetActive(false);
	}
}
