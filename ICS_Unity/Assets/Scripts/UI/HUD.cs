using UnityEngine;
using System.Collections;

public class HUD : Singleton<HUD>
{
	public ColorPicker ColorPicker;
	public LensManager<LensHandle, bool> ShowColorPicker;

	public PanelFileBrowser PanelFileBrowser;

	protected override void Awake()
	{
		base.Awake();

		ColorPicker.gameObject.SetActive(false);
		ShowColorPicker = new LensManager<LensHandle, bool>((requests) => 
		{
			ColorPicker.gameObject.SetActive(requests.Count > 0);
			return requests.Count > 0;
		});

		PanelFileBrowser.gameObject.SetActive(false);
	}

	public void OnClickLoad()
	{
		PanelFileBrowser.gameObject.SetActive(true);
	}

	public void OnClickSave()
	{
		PanelFileBrowser.gameObject.SetActive(true);
	}
}
