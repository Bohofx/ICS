using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Events;
using System.IO;
using System.Linq;

public class PanelFileBrowser : MonoBehaviour
{
	public enum FileBrowserMode
	{
		Open,
		Save,
	}

	public FileBrowserMode Mode
	{ get; set; }

	[SerializeField]
	Button _buttonTemplate;

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

	[SerializeField]
	InputField _inputFieldFileName;

	[SerializeField]
	Button _buttonOpenOrSave;

	DirectoryInfo _activeDirectory;

	public class OnFinishBrowsingEvent : UnityEvent<string>
	{ }

	public OnFinishBrowsingEvent onFinishBrowsing = new OnFinishBrowsingEvent();
	
	Button _selectedButton;

	void Awake()
	{
		_inputFieldFileName.onValueChanged.AddListener(OnFilenameInputFieldValueChanged);
	}

	void OnFilenameInputFieldValueChanged(string inNewValue)
	{
		if(_activeDirectory != null)
		{
			_buttonOpenOrSave.enabled = IsNameValid();
		}
	}

	bool IsNameValid()
	{
		char[] invalidCharacters = Path.GetInvalidFileNameChars();
		bool charactersValid = GetTargetFilename().IndexOfAny(invalidCharacters) < 0;
		bool doesntExist = !File.Exists(GetTargetPath());
		return charactersValid && doesntExist;
	}

	string GetTargetPath()
	{
		string targetPath = string.Empty;
		if(Mode == FileBrowserMode.Save)
		{
			if(_activeDirectory != null)
			{
				targetPath = Path.Combine(_activeDirectory.FullName, GetTargetFilename());
				if(!Path.HasExtension(targetPath))
				{
					targetPath = Path.ChangeExtension(targetPath, ".dat");
				}
			}
		}
		else
		{
			if(_activeDirectory != null && _selectedButton != null)
			{
				targetPath = Path.Combine(_activeDirectory.FullName, _selectedButton.name);
			}
		}

		return targetPath;
	}

	string GetTargetFilename()
	{
		return _inputFieldFileName.text;
	}

	void OnEnable()
	{
		_inputFieldFileName.gameObject.SetActive(Mode == PanelFileBrowser.FileBrowserMode.Save);

		Text text = _buttonOpenOrSave.GetComponentInChildren<Text>(true);
		if(text)
			text.text = Mode == FileBrowserMode.Save ? "Save" : "Open";
		
		string[] logicalDrives = Directory.GetLogicalDrives();
		RebufferContents(_scrollListContentsDrives);
		if(logicalDrives != null)
		{
			foreach(string logicalDrive in logicalDrives)
			{
				DirectoryInfo driveInfo = new DirectoryInfo(logicalDrive);
				CreateButtonDirectory(driveInfo.FullName, _scrollListContentsDrives, driveInfo);
			}
		}

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
				CreateButtonDirectory(directoryInfo.Name, _scrollListContentsCurrentDirectory, directoryInfo);
			}

			var fileInfos = _activeDirectory.GetFiles().OrderBy(fileInfo => fileInfo.Name);
			foreach(FileInfo fileInfo in fileInfos)
			{
				CreateButtonFile(fileInfo.Name, _scrollListContentsCurrentDirectory);
			}
		}
	}

	void RebufferContents(GameObject inContentsContainer)
	{
		while(inContentsContainer.transform.childCount > 0)
		{
			inContentsContainer.transform.GetChild(0).SetParent(_bufferedButtonTransform);
		}
	}

	void CreateButtonDirectory(string inText, GameObject inParent, DirectoryInfo inDirectoryInfo)
	{
		Button button = DequeueButton(inText, inParent, _spriteDirectory);
		if(button)
		{
			button.onClick.RemoveAllListeners();
			if(inDirectoryInfo != null)
			{
				button.onClick.AddListener(() => ChangeDirectory(inDirectoryInfo));
			}
		}
	}

	void CreateButtonFile(string inText, GameObject inParent)
	{
		Button button = DequeueButton(inText, inParent, _spriteFile);
		if(button)
		{
			button.onClick.RemoveAllListeners();
			button.onClick.AddListener(() => _selectedButton = button);
		}
	}

	Button DequeueButton(string inText, GameObject inParent, Sprite inSprite)
	{
		Button button = null;

		if(_bufferedButtonTransform.childCount > 0)
		{
			Transform childTransform = _bufferedButtonTransform.GetChild(_bufferedButtonTransform.childCount - 1);
			button = childTransform.GetComponentInChildren<Button>(true);
		}
		else
		{
			button = GameObject.Instantiate<Button>(_buttonTemplate);
		}

		if(button)
		{
			Image image = null;
			Text text = null;

			button.name = inText;
			button.transform.SetParent(inParent.transform, false);

			text = button.GetComponentInChildren<Text>(true);
			if(text)
			{
				text.text = inText;
			}

			Transform fileTypeImageTransform = button.transform.Find("FileTypeImage");
			if(fileTypeImageTransform)
			{
				image = fileTypeImageTransform.GetComponentInChildren<Image>(true);
				if(image)
				{
					image.sprite = inSprite;
				}
			}
		}
		return button;
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

	public void OnClickHome()
	{
		ChangeDirectory(new DirectoryInfo(Application.dataPath));
	}

	public void OnClickOpenOrSave()
	{
		onFinishBrowsing.Invoke(GetTargetPath());
		gameObject.SetActive(false);
	}

	public void OnClickCancel()
	{
		onFinishBrowsing.Invoke(string.Empty);
		gameObject.SetActive(false);
	}
}
