// (c) Copyright HutongGames, LLC 2010-2015. All rights reserved.
/*--- __ECO__ __ACTION__ __BETA__
EcoMetaStart
{
"script dependancies":[
						"Assets/PlayMaker Custom Actions/__internal/FsmStateActionAdvanced.cs"
					]
}
EcoMetaEnd
---*/
using UnityEngine;
using System;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("RectTransform")]
	[Tooltip("Get the corners of the calculated rectangle in world space or local space of its Transform.")]
	public class RectTransformGetCorners : FsmStateActionAdvanced
	{
		[RequiredField]
#if UNITY_4_6 
		[CheckForComponent(typeof(RectTransform))] 
#endif
		[Tooltip("The GameObject target.")]
		public FsmOwnerDefault gameObject;

		public FsmBool localspace;

		[Tooltip("The BottomLeft Corner")]
		[UIHint(UIHint.Variable)]
		public FsmVector3 bottomLeft;

		[Tooltip("The TopLeft Corner")]
		[UIHint(UIHint.Variable)]
		public FsmVector3 topLeft;
		
		[Tooltip("The TopRight Corner")]
		[UIHint(UIHint.Variable)]
		public FsmVector3 topRight;

		[Tooltip("The BottomRight Corner")]
		[UIHint(UIHint.Variable)]
		public FsmVector3 bottomRight;

		[Tooltip("All Corners")]
		[UIHint(UIHint.Variable)]
		[ArrayEditor(VariableType.Vector3)]
		public FsmArray allCorners;


#if UNITY_4_6
		RectTransform _rt;
#endif
		Vector3[] corners = new Vector3[4];

		public override void Reset()
		{
			base.Reset();
			gameObject = null;
			localspace = null;
			topLeft = null;
			topRight = null;
			bottomLeft = null;
			bottomRight = null;
			allCorners = null;
		}
		
		public override void OnEnter()
		{
			GameObject go = Fsm.GetOwnerDefaultTarget(gameObject);
			if (go != null)
			{
#if UNITY_4_6
				_rt = go.GetComponent<RectTransform>();
#endif
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
			#if UNITY_4_6
			if (localspace.Value)
			{
				_rt.GetLocalCorners(corners);
			}else{
				_rt.GetWorldCorners(corners);
			}
			if (!bottomLeft.IsNone) bottomLeft.Value = corners[0];
			if (!topLeft.IsNone) topLeft.Value = corners[1];
			if (!topRight.IsNone) topRight.Value = corners[2];
			if (!bottomRight.IsNone) bottomRight.Value = corners[3];

			if (!allCorners.IsNone) allCorners.Values = Array.ConvertAll(corners, item => (object)item);
			#endif
		}

	}

}