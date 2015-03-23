// (c) Copyright HutongGames, LLC 2010-2015. All rights reserved.
/*--- __ECO__ __ACTION__ __BETA__
EcoMetaStart
{
"script dependancies":[
						"Assets/PlayMaker Custom Actions/uGui/Editor/uGuiGroupLayoutUpdateContentCustomEditor.cs"
					]
}
EcoMetaEnd
---*/

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("uGui")]
	[Tooltip("Manage GameObjects within a GroupLayout to match FsmArray content")]
	public class uGuiGroupLayoutUpdateContent : FsmStateAction
	{
		// I use this to avoid the error that destroyEvent and updateEvent are not used in this fsm
		public FsmEventTarget eventTarget;

		[RequiredField]
		[CheckForComponent(typeof(UnityEngine.UI.LayoutGroup))]
		[Tooltip("The GameObject with the layout group ui component.")]
		public FsmOwnerDefault gameObject;

		[RequiredField]
		[UIHint(UIHint.Variable)]
		[ObjectType(typeof(string))]
		[Tooltip("the array of strings representing items to display. This is just a reference system.")]
		public FsmArray content;

		[RequiredField]
		[Tooltip("The prefab to use to create new items")]
		public FsmGameObject prefab;

		[Tooltip("If defined, the item will receive this event when it's no longer in content array. The Gameobject will be responsible for destroying itself. If not defined, will destroy the gameobject automatically")]
		public FsmEvent destroyEvent;

		[Tooltip("If defined, all items will receive this event when the content array doesn't match the group List")]
		public FsmEvent updateEvent;

		[Tooltip("Check and manage list every frame, keeping the content array and group list in synch.")]
		public bool everyFrame;

		Transform _t;
		//List<Transform> _list;
		Dictionary<string,Transform> _lut;

		public override void Reset()
		{
			eventTarget = new FsmEventTarget();
			eventTarget.target = FsmEventTarget.EventTarget.BroadcastAll;

			content = null;
			prefab = null;
			destroyEvent = null;
			updateEvent = null;
			everyFrame = false;
		}
		
		public override void OnEnter()
		{

			// set up the listing and lookups.
			GameObject _go = Fsm.GetOwnerDefaultTarget(gameObject);
			if (_go!=null)
			{
				_t = _go.GetComponent<Transform>();
				//_list = new List<Transform>();
				_lut = new Dictionary<string, Transform>();

				foreach (Transform _child in _t)
				{
					_lut[_child.name] = _child;
				//	_list.Add(_child);
				}


			}
		
			Check();
			
			if (!everyFrame)
			{
				Finish();
			}
		}
		
		public override void OnUpdate()
		{
			Check();
		}
		
		
		void Check()
		{
			if (_t==null)
			{
				return;
			}

			if (prefab.Value==null)
			{
				return;
			}

			int i=0;
			foreach(string item in content.Values)
			{

				// create new item if needed
				if (!_lut.ContainsKey(item))
				{
					GameObject _newItem = (GameObject)GameObject.Instantiate(prefab.Value);
					_newItem.name = item;
					_newItem.GetComponent<Transform>().SetParent(_t,false);
					_lut[item] = _newItem.GetComponent<Transform>();
				}

				// force sibling index
				_lut[item].SetSiblingIndex(i);

				if (updateEvent!=null)
				{
					Fsm.EventData.IntData = i;
					Fsm.EventData.StringData = item;
					PlayMakerUtils.SendEventToGameObject((PlayMakerFSM)this.Fsm.Owner,_lut[item].gameObject,updateEvent.Name);
				}
				i++;
			}

			// delete items not needed anymore.
			foreach(Transform t in _t)
			{
				if (!content.Values.Contains(t.name))
				{
					GameObject.Destroy(t.gameObject);
					_lut.Remove(t.name);
				}
			}
		}
	}
}