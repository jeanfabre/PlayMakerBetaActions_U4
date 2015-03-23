//	(c) Jean Fabre, 2011-2015 All rights reserved.

using HutongGames.PlayMaker.Actions;
using HutongGames.PlayMakerEditor;
using UnityEditor;
using UnityEngine;


[CustomActionEditor(typeof(uGuiGroupLayoutUpdateContent))]
public class uGuiGroupLayoutUpdateContentCustomEditor : CustomActionEditor
{

    public override bool OnGUI()
    {
		EditField("gameObject");
		EditField("content");
		EditField("prefab");
		EditField("destroyEvent");
		EditField("updateEvent");
		EditField("everyFrame");

		return GUI.changed;
    }


}
