using UnityEngine;
using System.Collections;

public class HUD : Singleton<HUD>
{
	public ColorPicker ColorPicker;
	public LensManager<LensHandle, bool> ShowColorPicker;

	[SerializeField]
	PanelFileBrowser _panelFileBrowser;

	protected override void Awake()
	{
		base.Awake();

		ColorPicker.gameObject.SetActive(false);
		ShowColorPicker = new LensManager<LensHandle, bool>((requests) => 
		{
			ColorPicker.gameObject.SetActive(requests.Count > 0);
			return requests.Count > 0;
		});

		_panelFileBrowser.onFinishBrowsing.AddListener(OnFinishBrowsing);
		_panelFileBrowser.gameObject.SetActive(false);
	}

	public void OnClickLoad()
	{
		_panelFileBrowser.Mode = PanelFileBrowser.FileBrowserMode.Open;
		_panelFileBrowser.gameObject.SetActive(true);
	}

	public void OnClickSave()
	{
		_panelFileBrowser.Mode = PanelFileBrowser.FileBrowserMode.Save;
		_panelFileBrowser.gameObject.SetActive(true);
	}

	public void OnClickNew()
	{
		Environment.GetInstance().ClearScene();
	}

	void OnFinishBrowsing(string inTargetPath)
	{
		if(!string.IsNullOrEmpty(inTargetPath))
		{
			if(_panelFileBrowser.Mode == PanelFileBrowser.FileBrowserMode.Open)
			{
				Environment.GetInstance().DeserializeScene(inTargetPath);
			}
			else if(_panelFileBrowser.Mode == PanelFileBrowser.FileBrowserMode.Save)
			{
				Environment.GetInstance().SerializeScene(inTargetPath);
			}
		}
	}
}
