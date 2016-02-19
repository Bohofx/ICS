using UnityEngine;
using System.Collections;

// A little class for leaving notes on GameObjects. Destroys self during the real game.

public class MonoBehaviourNote : MonoBehaviour
{
	[SerializeField]
	public bool unlocked
	{ get; set;}

	[SerializeField]
	string _text;

	public string text
	{ get { return _text; } set { _text = value; } }

	void Awake()
	{
		if(!Debug.isDebugBuild)
			Destroy(this);
	}
}