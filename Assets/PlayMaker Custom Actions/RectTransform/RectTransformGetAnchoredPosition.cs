﻿// (c) Copyright HutongGames, LLC 2010-2014. All rights reserved.
/*--- __ECO__ __ACTION__
EcoMetaStart
{
"script dependancies":[
						"Assets/PlayMaker Custom Actions/__internal/FsmStateActionAdvanced.cs"
					]
}
EcoMetaEnd
---*/
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("RectTransform")]
	[Tooltip("Get the position of the pivot of this RectTransform relative to the anchor reference point.")]
	public class RectTransformGetAnchoredPosition : FsmStateActionAdvanced
	{
		[RequiredField]
		[CheckForComponent(typeof(RectTransform))]
		[Tooltip("The GameObject target.")]
		public FsmOwnerDefault gameObject;
		
		[Tooltip("The anchored Position")]
		[UIHint(UIHint.Variable)]
		public FsmVector2 position;

		[Tooltip("The x component of the anchored Position")]
		[UIHint(UIHint.Variable)]
		public FsmFloat x;

		[Tooltip("The y component of the anchored Position")]
		[UIHint(UIHint.Variable)]
		public FsmFloat y;
		
	
		RectTransform _rt;
		
		public override void Reset()
		{
			base.Reset();
			gameObject = null;
			position = null;
			x = null;
			y = null;
			
		}
		
		public override void OnEnter()
		{
			GameObject go = Fsm.GetOwnerDefaultTarget(gameObject);
			if (go != null)
			{
				_rt = go.GetComponent<RectTransform>();
			}
			
			DoGetValues();
			
			if (!everyFrame)
			{
				Finish();
			}		
		}
		
		public override void OnActionUpdate()
		{
			DoGetValues();
		}
		
		void DoGetValues()
		{

			if (!position.IsNone) position.Value = _rt.anchoredPosition;
			if (!x.IsNone) x.Value = _rt.anchoredPosition.x;
			if (!y.IsNone) y.Value = _rt.anchoredPosition.y;
		}
	}
}