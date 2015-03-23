// (c) Copyright HutongGames, LLC 2010-2015. All rights reserved.
/*--- __ECO__ __ACTION__ __BETA__ ---*/

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.StateMachine)]
	[Tooltip("Gets properties on the last event that caused a state change. \n" +
		"Keys are using enum to remove guess work and potential issues with string based keys.\n" +
		"Use Set Event Enum Properties to define these values when sending events")]
	public class GetEventEnumProperties : FsmStateAction
	{

		[CompoundArray("Event Properties", "Key", "Data")]
		[UIHint(UIHint.Variable)]
		public FsmEnum[] keys;
		[UIHint(UIHint.Variable)]
		public FsmVar[] datas;

		[Tooltip("EVent Fire if a key is not found in the event properties")]
		public FsmEvent notFoundEvent;


		public override void Reset()
		{
			keys = new FsmEnum[1];
			datas = new FsmVar[1];
			notFoundEvent = null;
		}

		public override void OnEnter()
		{
		
			try{
				if (SetEventEnumProperties.properties == null)
				{
					throw new System.ArgumentException("no properties");
				}
				
			
				for (int i = 0; i < keys.Length; i++) 
				{	
					if (SetEventEnumProperties.properties.ContainsKey(keys[i].Value))
					{
						PlayMakerUtils.ApplyValueToFsmVar(this.Fsm,datas[i],SetEventEnumProperties.properties[keys[i].Value]);
					}else{
						Fsm.Event(notFoundEvent);
					}
				}
				
			}catch(Exception e)
			{
				Fsm.Event(notFoundEvent);
				Debug.Log("no properties found "+e);
			}
			
			Finish();
		}
	}
}