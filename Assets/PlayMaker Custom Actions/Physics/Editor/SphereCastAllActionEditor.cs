using HutongGames.PlayMaker.Actions;
using HutongGames.PlayMakerEditor;
using UnityEditor;
using UnityEngine;
using System.Collections;

namespace HutongGames.PlayMakerEditor
{
	[CustomActionEditor(typeof (SphereCastAll))]
	public class SphereCastAllActionEditor : CustomActionEditor
	{
		public override bool OnGUI()
		{
			return DrawDefaultInspector();
		}
		
		public override void OnSceneGUI()
		{
			SphereCastAll _action = (SphereCastAll) target;

			if (_action.debug.Value && _action.UpdateSphereCastAll())
			{

				//var handleSize = HandleUtility.GetHandleSize(goPosition);
				//var arrowSize = handleSize*0.2f;
				//var distance = (lookAtPosition - goPosition).magnitude;
				
				//var goTarget = lookAtAction.targetObject.Value;
				
				Vector3 _center = _action.Start;
				Vector3 _end = _action.End;
				Vector3 _direction = _action.direction.Value;
				_direction.Normalize();
				Vector3 _from = Vector3.right;
				float _angle = 360f;
				float _radius = _action.radius.Value;





				Vector3 _local_up = Vector3.Cross(_direction,Vector3.right);
				_local_up.Normalize();
				_local_up *= _radius;

				Vector3 _local_right = Vector3.Cross(_direction,_local_up);
				_local_right.Normalize();
				_local_right *= _radius;

				/*
				Handles.color = Color.blue;
				Handles.DrawLine(_center,_end);
	
				Handles.color = Color.green;
				Handles.DrawLine(_center,_center+_local_up);
				Handles.color = Color.red;
				Handles.DrawLine(_center,_center+_local_right);
				*/


				Handles.color = _action.debugColor.Value;

				// draw both start and end circles
				Handles.DrawWireArc(_center,_direction,_local_right,_angle,_radius);
				Handles.DrawWireArc(_end,_direction,_local_right,_angle,_radius);
				// draw the outer connecting lines between star and end
				Handles.DrawLine(_center+_local_right,_end+_local_right);
				Handles.DrawLine(_center-_local_right,_end-_local_right);
				Handles.DrawLine(_center+_local_up,_end+_local_up);
				Handles.DrawLine(_center-_local_up,_end-_local_up);

				// draw start half sphere
				Handles.DrawWireArc(_center,_local_right,_local_up,180f,_radius);
				Handles.DrawWireArc(_center,_local_up,_local_right,-180f,_radius);
				// draw end half sphere
				Handles.DrawWireArc(_end,_local_right,_local_up,-180f,_radius);
				Handles.DrawWireArc(_end,_local_up,_local_right,180f,_radius);

					/*
				// Lookat vector
				
				//Handles.DrawLine(goPosition, lookAtPosition);


				Handles.ConeCap(0, goPosition + lookAtVector.normalized * (distance - arrowSize * 0.7f)  , lookAtRotation, arrowSize); // fudge factor to position cap correctly
				
				// Arc between vectors
				
				Handles.color = new Color(1, 1, 1, 0.2f);
				Handles.DrawSolidArc(goPosition, lookAtNormal, goTransform.forward, lookAtAngle, handleSize);
				
				// Show vertical offset
				
				if (lookAtAction.keepVertical.Value)
				{
					Handles.DrawLine(lookAtPosition, lookAtAction.GetLookAtPositionWithVertical());
				}
				*/

				if (GUI.changed)
				{
					FsmEditor.EditingActions();
					FsmEditor.Repaint(true);
				}
			}


		}
	}
}
