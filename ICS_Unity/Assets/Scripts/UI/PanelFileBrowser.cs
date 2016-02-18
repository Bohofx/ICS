using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Events;
using System.IO;
using System.Linq;

public class PanelFileBrowser : MonoBehaviour
{
	[SerializeField]
	public Button ButtonTemplate;

	[SerializeField]
	Transform _bufferedButtonTransform;

	[SerializeField]
	GameObject _scrollListContentsDrives;

	[SerializeField]
	GameObject _scrollListContentsCurrentDirectory;

	[SerializeField]
	Text _currentDirectory;

	[SerializeField]
	Sprite _spriteDirectory;

	[SerializeField]
	Sprite _spriteFile;

	DirectoryInfo _activeDirectory;
	
	string[] _logicalDrives;

	void OnEnable()
	{
		_logicalDrives = Directory.GetLogicalDrives();
		InitializeDrives();
		ChangeDirectory(new DirectoryInfo(Application.dataPath));
	}

	void OnDisable()
	{
		RebufferContents(_scrollListContentsCurrentDirectory);
		RebufferContents(_scrollListContentsDrives);
	}

	void InitializeCurrentDirectory()
	{
		RebufferContents(_scrollListContentsCurrentDirectory);
		if(_activeDirectory != null)
		{
			var directoryInfos = _activeDirectory.GetDirectories().OrderBy(directoryInfo => directoryInfo.Name);
			foreach(DirectoryInfo directoryInfo in directoryInfos)
			{
				CreateButtonForFileOrDirectory(directoryInfo.Name, _scrollListContentsCurrentDirectory, true, () => ChangeDirectory(directoryInfo));
			}

			var fileInfos = _activeDirectory.GetFiles().OrderBy(fileInfo => fileInfo.Name);
			foreach(FileInfo fileInfo in fileInfos)
			{
				CreateButtonForFileOrDirectory(fileInfo.Name, _scrollListContentsCurrentDirectory, false, null);
			}
		}
	}

	void InitializeDrives()
	{
		RebufferContents(_scrollListContentsDrives);
		if(_logicalDrives != null)
		{
			foreach(string logicalDrive in _logicalDrives)
			{
				DirectoryInfo driveInfo = new DirectoryInfo(logicalDrive);
				CreateButtonForFileOrDirectory(driveInfo.FullName, _scrollListContentsDrives, true, () => ChangeDirectory(driveInfo));
			}
		}
	}

	void RebufferContents(GameObject inContentsContainer)
	{
		foreach(Transform child in inContentsContainer.transform)
		{
			child.transform.SetParent(_bufferedButtonTransform);
		}
	}

	GameObject CreateButtonForFileOrDirectory(string inText, GameObject inParent, bool isDirectory, UnityAction inAction)
	{
		Button button = null;
		Image image = null;
		Text text = null;

		if(_bufferedButtonTransform.childCount > 0)
		{
			Transform childTransform = _bufferedButtonTransform.GetChild(_bufferedButtonTransform.childCount - 1);
			button = childTransform.GetComponentInChildren<Button>(true);
		}
		else
		{
			button = GameObject.Instantiate<Button>(ButtonTemplate);
		}

		if(button)
		{
			button.name = inText;
			button.transform.SetParent(inParent.transform, false);
			
			button.onClick.RemoveAllListeners();
			if(inAction != null)
			{
				button.onClick.AddListener(inAction);
			}

			text = button.GetComponentInChildren<Text>(true);
			if(text)
			{
				text.text = inText;
			}

			image = button.GetComponentInChildren<Image>(true);
			if(image)
			{
				image.sprite = isDirectory ? _spriteDirectory : _spriteFile;
			}
		}

		return button.gameObject;
	}

	void ChangeDirectory(DirectoryInfo inDirectoryInfo)
	{
		if(inDirectoryInfo != null)
		{
			_activeDirectory = inDirectoryInfo;
			_currentDirectory.text = _activeDirectory.FullName;
			InitializeCurrentDirectory();
		}
	}

	public void OnClickDirectoryUp()
	{
		if(_activeDirectory != null && _activeDirectory.Parent != null)
		{
			ChangeDirectory(_activeDirectory.Parent);
		}
	}

	public void OnClickOpenOrSave()
	{
		gameObject.SetActive(false);
	}

	public void OnClickCancel()
	{
		gameObject.SetActive(false);
	}
}
