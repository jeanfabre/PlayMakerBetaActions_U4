// (c) Copyright HutongGames, LLC 2010-2015. All rights reserved.
/*--- __ECO__ __ACTION__ __BETA__ ---*/

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;


namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.StateMachine)]
	[Tooltip("Sets Event Data before sending an event.\n" +
	         "Keys are using enum to remove guess work and potential issues with string based keys.\n" +
	         "To Get the Event Data, use Get Event Enum Properties action.")]
	public class SetEventEnumProperties : FsmStateAction
	{
		[CompoundArray("Event Properties", "Key", "Data")]
		[UIHint(UIHint.Variable)]
		public FsmEnum[] keys;
		public FsmVar[] datas;
		
		public static Dictionary<object,object> properties;

		public override void Reset()
		{
			keys = new FsmEnum[1];
			datas = new FsmVar[1];
		}

		public override void OnEnter()
		{
			properties = new Dictionary<object, object>();
			
			for (int i = 0; i < keys.Length; i++) 
			{
				properties[keys[i].Value] = PlayMakerUtils.GetValueFromFsmVar(this.Fsm,datas[i]);
			}
			
			Finish();
		}
	}
}