using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CreatableObjectDropdown : MonoBehaviour
{
	[SerializeField, AssetPathData(typeof(GameObject), StoreResourcePath = true)]
	AssetPath[] _assetsToSpawn;

	[SerializeField]
	GameObject _buttonPrefab;

	void Awake()
	{
		_buttonPrefab.SetActive(false);

		for(int i = 0; i < _assetsToSpawn.Length; ++i)
		{
			GameObject newInstance = (GameObject)Instantiate(_buttonPrefab);
			newInstance.SetActive(true);
			newInstance.transform.SetParent(_buttonPrefab.transform.parent, false);

			Text text = newInstance.GetComponentInChildren<Text>();
			text.text = _assetsToSpawn[i].resx;

			Button button = newInstance.GetComponent<Button>();
			int index = i;
			button.onClick.AddListener(() => { SpawnInstance(index); });
		}
	}

	void SpawnInstance(int index)
	{
		Ray ray = Camera.main.ViewportPointToRay(new Vector2(.5f, .5f));
		int roomBoundsMask = (1 << LayerMask.NameToLayer("RoomBounds"));

		RaycastHit hit; Vector3? spawnPosition = null;
		if(Physics.Raycast(ray, out hit, Mathf.Infinity, roomBoundsMask))
		{
			spawnPosition = hit.point;
		}

		index = Mathf.Clamp(index, 0, _assetsToSpawn.Length - 1);
		SpawnObjectCommand command = new SpawnObjectCommand(_assetsToSpawn[index], spawnPosition);
		UndoManager.GetInstance().Do(command);
	}
}
