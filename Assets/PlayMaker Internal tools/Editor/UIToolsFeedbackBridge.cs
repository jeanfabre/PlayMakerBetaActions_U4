using System;
using UnityEngine;
using UnityEditor;

namespace HutongGames.PlayMakerEditor
{
	public class UIToolsFeedbackBridge
	{
		
		public void LogAction(string message)
		{
			if (ProjectToolsUI.Instance!=null)
			{
				ProjectToolsUI.Instance.SetCurrentAction(message);
			}
			
			Debug.Log("Action: "+message);
		}
		
		public void StartProcedure(string name)
		{
			if (ProjectToolsUI.Instance!=null)
			{
				ProjectToolsUI.Instance.StartProcedure(name);
			}else{
				Debug.Log("-------- missing instance");
			}
			
			Debug.Log("Start Procedure: "+name);
		}
		
		public void EndProcedure(string name)
		{
			if (ProjectToolsUI.Instance!=null)
			{
				ProjectToolsUI.Instance.EndProcedure(name);
			}
			
			Debug.Log("End Procedure: "+name);
		}
	}
}