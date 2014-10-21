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
	[Tooltip("Get the size of this RectTransform relative to the distances between the anchors. this is the 'Width' and 'Height' values in the RectTransform inspector.")]
	public class RectTransformGetSizeDelta : FsmStateActionAdvanced
	{
		[RequiredField]
		[CheckForComponent(typeof(RectTransform))]
		[Tooltip("The GameObject target.")]
		public FsmOwnerDefault gameObject;
		
		[Tooltip("The sizeDelta")]
		[UIHint(UIHint.Variable)]
		public FsmVector2 sizeDelta;
		
		[Tooltip("The x component of the sizeDelta, the width.")]
		[UIHint(UIHint.Variable)]
		public FsmFloat width;
		
		[Tooltip("The y component of the sizeDelta, the height")]
		[UIHint(UIHint.Variable)]
		public FsmFloat height;
		
		
		RectTransform _rt;
		
		public override void Reset()
		{
			base.Reset();
			gameObject = null;
			sizeDelta = null;
			width = null;
			height = null;
			
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
			
			if (!sizeDelta.IsNone) sizeDelta.Value = _rt.sizeDelta;
			if (!width.IsNone) width.Value = _rt.sizeDelta.x;
			if (!height.IsNone) height.Value = _rt.sizeDelta.y;
		}
	}
}