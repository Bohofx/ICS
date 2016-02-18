using UnityEngine;
using System.Collections;
using SimpleJSON;

public abstract class SerializeComponentBase : MonoBehaviour
{
	PrefabInstance _instanceParent = null;
	protected PrefabInstance instanceParent
	{
		get
		{
			if(_instanceParent == null)
			{
				_instanceParent = gameObject.GetComponentInParent<PrefabInstance>();
				if(_instanceParent == null)
				{
					Debug.Assert(instanceParent != null, "SerializeComponentBase has no parent PrefabInstance component.", this);
				}
			}
			return _instanceParent;
		}
	}

	#region Lifespan

	void Start()
	{
		if(instanceParent)
		{
			var pickableObject = instanceParent.PickableObject;
			pickableObject.onSelectionStateChanged.AddListener((state) => { OnSelectionStateChanged(state); });
		}
	}

	#endregion Lifespan

	#region Selection State

	protected virtual void OnSelectionStateChanged(bool inState)
	{ }

	#endregion

	#region Serialization

	public virtual void Serialize(JSONNode inNode)
	{
		inNode["Path"] = UtilitiesGameObject.GetHierarchyPath(this, instanceParent != null ? instanceParent.transform : null);
		inNode["Type"] = this.GetType().ToString();
	}

	public virtual void Deserialize(JSONNode inNode)
	{

	}

	#endregion Serialization
}
