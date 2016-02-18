using UnityEngine;
using System.Collections;

class SpawnObjectCommand : ICommand
{
	AssetPath _assetToSpawn;

	Vector3? _spawnPosition;

	PrefabInstance _instance;

	private SpawnObjectCommand()
	{ }

	public SpawnObjectCommand(AssetPath inObjectToSpawn, Vector3? inSpawnPosition)
	{
		_assetToSpawn = inObjectToSpawn;
		_spawnPosition = inSpawnPosition;
	}

	public void Do()
	{
		_instance = PrefabInstance.CreateFromAssetPath(_assetToSpawn);
		if(_instance && _spawnPosition.HasValue)
		{
			_instance.transform.position = _spawnPosition.Value;

			PickableObject pickable = _instance.GetComponent<PickableObject>();
			if(pickable)
			{
				_instance.transform.localPosition += new Vector3(0f, pickable.PickableBounds.extents.y, 0f);
			}
		}
	}

	public void Undo()
	{
		if(_instance != null)
		{
			GameObject.Destroy(_instance.gameObject);
		}
	}
}