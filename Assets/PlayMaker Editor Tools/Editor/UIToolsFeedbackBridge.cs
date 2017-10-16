using System;
using UnityEngine;
using UnityEditor;

namespace HutongGames.PlayMakerEditor
{
	public class UIToolsFeedbackBridge
	{
		
		public void LogAction(string message,bool forwardToUnityLog = false)
		{
			if (ProjectToolsUI.Instance!=null)
			{
				ProjectToolsUI.Instance.SetCurrentAction(message);
			}

			if (forwardToUnityLog)
			{
				Debug.Log("Action: "+message);
			}
		}
		
		public void StartProcedure(string name,bool forwardToUnityLog = false)
		{
			if (ProjectToolsUI.Instance!=null)
			{
				ProjectToolsUI.Instance.StartProcedure(name);
			}else{
			//	Debug.Log("-------- missing instance");
			}

			if (forwardToUnityLog)
			{
				Debug.Log ("Start Procedure: " + name);
			}
		}
		
		public void EndProcedure(string name,bool forwardToUnityLog = false)
		{
			if (ProjectToolsUI.Instance!=null)
			{
				ProjectToolsUI.Instance.EndProcedure(name);
			}
			if (forwardToUnityLog) 
			{
				Debug.Log ("End Procedure: " + name);
			}
		}
	}
}