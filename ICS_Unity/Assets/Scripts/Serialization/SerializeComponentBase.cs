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
				_instanceParent = gameObject.GetComponentInParent<PrefabInstance>();
			return _instanceParent;
		}
	}

	public virtual void Serialize(JSONNode inNode)
	{
		Debug.Assert(instanceParent != null, "SerializeComponentBase has no parent PrefabInstance component.", this);
		inNode["Path"] = UtilitiesGameObject.GetHierarchyPath(this, instanceParent != null ? instanceParent.transform : null);
		inNode["Type"] = this.GetType().ToString();
	}

	public virtual void Deserialize(JSONNode inNode)
	{

	}
}
