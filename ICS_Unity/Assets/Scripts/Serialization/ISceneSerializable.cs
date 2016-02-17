using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SimpleJSON;

interface ISceneSerializable
{
	void Serialize(JSONNode inNode);

	void Deserialize(JSONNode inNode);
}