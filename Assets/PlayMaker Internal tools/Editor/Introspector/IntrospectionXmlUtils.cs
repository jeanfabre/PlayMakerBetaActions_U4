using UnityEngine;
using UnityEditor;

using System;
using System.IO;
using System.Xml;
using System.Collections;
using System.Collections.Generic;

using HutongGames.PlayMaker;

namespace HutongGames.PlayMakerEditor
{
	public class IntrospectionXmlUtils  {

		PlayMakerFSM _test;

		static Dictionary<String,String[]> EnumBuffer = new Dictionary<string, String[]>();

		public static void Init()
		{
			EnumBuffer = new Dictionary<string, string[]>();
		}

		public static void StoreEnumReferences(XmlElement parentElement)
		{
			if (EnumBuffer==null || EnumBuffer.Count == 0)
			{
				return;
			}

			XmlElement _enumsElement = IntrospectionXmlProxy.AddElement(parentElement,"FsmEnumReferences");

			foreach(KeyValuePair<string,String[]> _enum in EnumBuffer)
			{
				XmlNode _enumXmlnode =  parentElement.SelectSingleNode("Enum[text()='"+_enum.Key+"']");
				if (_enumXmlnode==null)
				{
					XmlElement _enumElement = IntrospectionXmlProxy.AddElement(_enumsElement,"Enum");
					IntrospectionXmlProxy.AddElement(_enumElement,"Type",_enum.Key);
					 XmlElement _enumValues =	IntrospectionXmlProxy.AddElement(_enumElement,"Values");
					foreach(String _value in _enum.Value)
					{
						IntrospectionXmlProxy.AddElement(_enumValues,"Value",_value);
					}
				}
			}
		}

		public static XmlElement IntrospectPlayMaker(XmlElement parentElement)
		{

			XmlElement _PlayMakerElement = IntrospectionXmlProxy.AddElement(parentElement,"PlayMaker");

			IntrospectionXmlProxy.AddElement(_PlayMakerElement,"Version",  Net.FabreJean.UnityEditor.Utils.GetPlayMakerVersion() );

			// State Colors
		



			// Preferences
			XmlElement _PrefsElement = IntrospectionXmlProxy.AddElement(_PlayMakerElement,"Preferences");

			XmlElement _PrefColorsElement = IntrospectionXmlProxy.AddElement(_PrefsElement,"Colors");

			for(int i=0;i<PlayMakerPrefs.ColorNames.Length;i++)
			{
				string _colorName = PlayMakerPrefs.ColorNames[i];
				if (string.IsNullOrEmpty(_colorName)) continue;

				XmlElement _colorElement = IntrospectionXmlProxy.AddElement(_PrefsElement,"Color");

				IntrospectionXmlProxy.AddElement(_PrefsElement,"Index",i.ToString());
				IntrospectionXmlProxy.AddElement(_PrefsElement,"Name",_colorName);
				IntrospectionXmlProxy.AddElement(_PrefsElement,"Value",PlayMakerPrefs.Colors[i].ToString());
			}

			foreach(KeyValuePair<String,System.Object> _pref in PreferencesLUT)
			{
 				//Debug.Log("Key"+_pref.Key+" "+_pref.Value.GetType().FullName+" "+_pref.Value);

				switch(_pref.Value.GetType().FullName)
				{
					case "System.Boolean":
					IntrospectionXmlProxy.AddElementIfValueDifferent(_PrefsElement,_pref.Key,EditorPrefs.GetBool("PlayMaker."+_pref.Key,(bool)_pref.Value ),(bool)_pref.Value );
					break;
					case "System.Int32":
					IntrospectionXmlProxy.AddElementIfValueDifferent(_PrefsElement,_pref.Key,EditorPrefs.GetInt("PlayMaker."+_pref.Key,(int)_pref.Value ),(int)_pref.Value );
					break;    
					case "System.Single":
					IntrospectionXmlProxy.AddElementIfValueDifferent(_PrefsElement,_pref.Key,EditorPrefs.GetFloat("PlayMaker."+_pref.Key,(float)_pref.Value ),(float)_pref.Value );
					break; 
				case "System.String":

					Debug.Log(EditorPrefs.GetString("PlayMaker."+_pref.Key,(string)_pref.Value )+" ?= "+(string)_pref.Value);

					IntrospectionXmlProxy.AddElementIfValueDifferent(_PrefsElement,_pref.Key,EditorPrefs.GetString("PlayMaker."+_pref.Key,(string)_pref.Value ),(string)_pref.Value );
					break; 
				}

				
			}

			return _PlayMakerElement;
		}



		public static void IntrospectPlayMakerFSM(PlayMakerFSM fsm,XmlElement parentElement)
		{
			XmlElement _fsmElement = IntrospectionXmlProxy.AddElement(parentElement,"Fsm");

		

			IntrospectionXmlUtils.IntrospectFsmComponent(fsm.Fsm,_fsmElement);

			IntrospectFsmTransitions(fsm.FsmGlobalTransitions,"GlobalTransitions",_fsmElement);

			if (fsm.UsesTemplate)
			{
				IntrospectionXmlProxy.AddElement(_fsmElement,"Template",fsm.FsmTemplate.name);

			}
			IntrospectionXmlProxy.AddElement(_fsmElement,"StartState",fsm.Fsm.StartState);

			IntrospectionXmlUtils.IntrospectFsmStates(fsm.Fsm,_fsmElement);
			
			IntrospectionXmlUtils.IntrospectFsmVariables(FsmVariable.GetFsmVariableList(fsm),_fsmElement);

		}


		public static void IntrospectFsmComponent(Fsm fsm,XmlElement parentElement)
		{
			XmlElement _componentElement = IntrospectionXmlProxy.AddElement(parentElement,"Component");
			
			IntrospectionXmlProxy.AddElement(_componentElement,"Path",fsm.GameObject.transform.GetPath());
			
			IntrospectionXmlProxy.AddElement(_componentElement,"GameObjectName",fsm.GameObjectName);
			IntrospectionXmlProxy.AddElement(_componentElement,"Name",fsm.Name);
			
			IntrospectionXmlProxy.AddElement(_componentElement,"DataVersion",fsm.DataVersion.ToString());
			
			IntrospectionXmlProxy.AddCdataElementIfNotEmpty(_componentElement,"Description",fsm.Description);
			IntrospectionXmlProxy.AddElementIfNotEmpty(_componentElement,"DocUrl",fsm.DocUrl);
			IntrospectionXmlProxy.AddElementIfNotEmpty(_componentElement,"Watermark",fsm.Watermark);


		}


		public static void IntrospectFsmStates(Fsm fsm,XmlElement parentElement)
		{
	
			XmlElement _statesElement = IntrospectionXmlProxy.AddElement(parentElement,"States");

			foreach(FsmState _state in fsm.States)
			{
				XmlElement _stateElement = IntrospectionXmlProxy.AddElement(_statesElement,"State");

				IntrospectionXmlProxy.AddElement(_stateElement,"Name",_state.Name);
				if (_state.ColorIndex!=0)
				{
					IntrospectionXmlProxy.AddElement(_stateElement,"ColorIndex",_state.ColorIndex.ToString());
				}

				IntrospectionXmlProxy.AddCdataElementIfNotEmpty(_stateElement,"Description",_state.Description);

				if (_state.IsBreakpoint)
				{
					IntrospectionXmlProxy.AddElement(_stateElement,"IsBreakpoint","true");
				}

				if (string.Equals(fsm.StartState,_state.Name))
				{
					IntrospectionXmlProxy.AddElement(_stateElement,"IsStartState","true");
				}

				if (_state.IsSequence)
				{
					IntrospectionXmlProxy.AddElement(_stateElement,"IsSequence","true");
				}

				IntrospectionXmlProxy.AddElement(_stateElement,"Position",_state.Position.ToString());

				// Actions

				IntrospectFsmStateActions(_state,_stateElement);

				// Transitions
				IntrospectFsmTransitions(_state.Transitions,"Transitions",_stateElement);

			}
		}

		public static void IntrospectFsmTransitions(FsmTransition[] fsmTransitions,string listName,XmlElement parentElement)
		{

			if (fsmTransitions==null || fsmTransitions.Length==0)
			{
				return;
			}

			XmlElement _ListElement = IntrospectionXmlProxy.AddElement(parentElement,listName);

			foreach(FsmTransition _transition in fsmTransitions)
			{
				XmlElement _transitionElement = IntrospectionXmlProxy.AddElement(_ListElement,"Transition");
				
				IntrospectionXmlProxy.AddElement(_transitionElement,"EventName",_transition.EventName);
				IntrospectionXmlProxy.AddElementIfNotEmpty(_transitionElement,"ToState",_transition.ToState);
				
				if (_transition.ColorIndex!=0)
				{
					IntrospectionXmlProxy.AddElement(_transitionElement,"ColorIndex",_transition.ColorIndex.ToString());
				}
				if (_transition.LinkConstraint != FsmTransition.CustomLinkConstraint.None)
				{
					IntrospectionXmlProxy.AddElement(_transitionElement,"LinkConstraint",_transition.LinkConstraint.ToString());
				}
				if (_transition.LinkStyle != FsmTransition.CustomLinkStyle.Default)
				{
					IntrospectionXmlProxy.AddElement(_transitionElement,"LinkStyle",_transition.LinkStyle.ToString());
				}
			}
		}


		public static void IntrospectFsmStateActions(FsmState state,XmlElement parentElement)
		{
			if (state.Actions.Length==0)
			{
				return;
			}

			XmlElement _actionsElement = IntrospectionXmlProxy.AddElement(parentElement,"Actions");
			
			foreach(FsmStateAction _action in state.Actions)
			{

				XmlElement _actionElement = IntrospectionXmlProxy.AddElement(_actionsElement,"Action");

				IntrospectionXmlProxy.AddElementIfNotEmpty(_actionElement,"Name",_action.Name);

				if (!_action.IsAutoNamed)
				{
					IntrospectionXmlProxy.AddElement(_actionElement,"IsAutoNamed","False");
				}

				if (!_action.Active)
				{
					IntrospectionXmlProxy.AddElement(_actionElement,"Active","False");
				}
			}

		}


		public static void IntrospectFsmVariables(List<FsmVariable> fsmVariablesList,XmlElement parentElement)
		{

			XmlElement _variablesElement = IntrospectionXmlProxy.AddElement(parentElement,"Variables");

			foreach(FsmVariable _variable in fsmVariablesList )
			{

				XmlElement _variableElement = IntrospectionXmlProxy.AddElement(_variablesElement,"Variable");
				
				IntrospectionXmlProxy.AddElement(_variableElement,"Name",_variable.Name);
				IntrospectionXmlProxy.AddElementIfNotEmpty(_variableElement,"Category",_variable.GetCategory());


				IntrospectionXmlProxy.AddElementIfValueDifferent (_variableElement,"NetworkSynch",_variable.NetworkSync,false);
				IntrospectionXmlProxy.AddElementIfValueDifferent(_variableElement,"ShowInInspector",_variable.ShowInInspector,false);
				IntrospectionXmlProxy.AddCdataElementIfNotEmpty(_variableElement,"Tooltip",_variable.Tooltip);

				IntrospectionXmlProxy.AddElement(_variableElement,"VariableType",_variable.Type.ToString());

				NamedVariable _namedVar = _variable.NamedVar;

				switch(_namedVar.VariableType)
				{
				case VariableType.Array:

					FsmArray _fsmArray = (FsmArray)_namedVar;

					IntrospectionXmlProxy.AddElement(_variableElement,"ElementType",_fsmArray.ElementType.ToString());
					IntrospectionXmlProxy.AddElement(_variableElement,"Length",_fsmArray.Length.ToString());

					if (_fsmArray.Length>0)
					{
						XmlElement _values = IntrospectionXmlProxy.AddElement(_variableElement,"Values");
						foreach(object _obj in _fsmArray.Values)
						{
							IntrospectionXmlProxy.AddElement(_values,"Value",_obj.ToString());
						}
					}

					break;
				case VariableType.Enum:
					
					FsmEnum _fsmEnum = (FsmEnum)_namedVar;
				
					if (_fsmEnum.EnumType != typeof(None))
					{
						if (!EnumBuffer.ContainsKey(_fsmEnum.EnumName))
						{
							EnumBuffer[_fsmEnum.EnumName] = Enum.GetNames(_fsmEnum.EnumType);
						}

						IntrospectionXmlProxy.AddElement(_variableElement,"EnumType",_fsmEnum.EnumType.ToString());
						IntrospectionXmlProxy.AddElement(_variableElement,"EnumName",_fsmEnum.EnumName);

						if (_fsmEnum.Value!=null)
						{
							IntrospectionXmlProxy.AddElement(_variableElement,"Value",_fsmEnum.Value.ToString());
						}
					}
					break;

				default:
					if (_namedVar.RawValue !=null)
					{
						IntrospectionXmlProxy.AddElement(_variableElement,"Value",_namedVar.RawValue.ToString());
					}
					break;
				}
			}
		}


		static Dictionary<String,System.Object> PreferencesLUT = new Dictionary<String, System.Object>()
		{
			{"MaximizeFileCompatibility", true},
			{"DrawAssetThumbnail", true},
			{"DrawLinksBehindStates", true},
			{"DimFinishedActions", true},
			{"AutoRefreshFsmInfo", true},
			{"ConfirmEditingPrefabInstances", true},
			{"ShowStateLoopCounts", false},
			{"DrawFrameAroundGraph", false},
			{"GraphViewShowMinimap", true},
			{"GraphViewMinimapSize", 300},
			{"GraphViewZoomSpeed", 0.01f},
			{"MouseWheelScrollsGraphView", false},
			{"ScreenshotsPath", "PlayMaker/Screenshots"},
			{"DebugVariables", false},
			{"ConsoleActionReportSortOptionIndex", 1},
			{"LogPauseOnSelect", true},
			{"LogShowSentBy", true},
			{"LogShowTimecode", false},
			{"ShowHints", true},
			{"CloseActionBrowserOnEnter", false},
			{"DisableEditorWhenPlaying", false},
			{"DisableInspectorWhenPlaying", false},
			{"DisableToolWindowsWhenPlaying", true},
			{"DisableActionBrowerWhenPlaying", false},
			{"DisableEventBrowserWhenPlaying", false},
			{"ColorScheme", 0},
			{"EnableRealtimeErrorChecker", true},
			{"CheckForRequiredComponent", true},
			{"CheckForRequiredField", true},
			{"CheckForEventNotUsed", true},
			{"CheckForTransitionMissingEvent", true},
			{"CheckForTransitionMissingTarget", true},
			{"CheckForDuplicateTransitionEvent", true},
			{"CheckForMouseEventErrors", true},
			{"CheckForCollisionEventErrors", true},
			{"CheckForPrefabRestrictions", true},
			{"CheckForObsoleteActions", true},
			{"CheckForMissingActions", true},
			{"CheckForNetworkSetupErrors", true},
			{"AutoPanZone", 100},
			{"EdgeScrollSpeed", 1},
			{"HideObsoleteActions", true},
			{"ColorLinks", false},
			{"DisableErrorCheckerWhenPlaying", true},
			{"EnableLogging", true},
			{"SnapGridSize", 16},
			{"SnapToGrid", false},
			{"ShowScrollBars", true},
			{"ShowWatermark", true},
			{"StateMaxWidth", 400},
			{"AddPrefabLabel", true},
			{"AutoLoadPrefabs", true},
			{"LoadAllPrefabs", true},
			{"UnloadPrefabs", true},
			{"StartStateName", "State 1"},
			{"NewStateName", Strings.FsmEditorSettings_Default_State_Name},
			{"AutoSelectGameObject", true},
			{"SelectStateOnActivated", true},
			{"GotoBreakpoint", true},
			{"GameStateIconSize", 32},
			{"FrameSelectedState", false},
			{"SyncLogSelection", true},
			{"BreakpointsEnabled", true},
			{"MirrorDebugLog", false},
			{"LockGraphView", false},
			{"GraphLinkStyle", 0},
			{"ShowFsmDescriptionInGraphView", true},
			{"ShowCommentsInGraphView", true},
			{"ShowStateLabelsInGameView", true},
			{"DrawPlaymakerGizmos", true},
			{"DrawPlaymakerGizmoInHierarchy", true},
			{"ShowEditWhileRunningWarning", true},
			{"ShowStateDescription", true},
			{"DebugActionParameters", false},
			{"HideUnusedEvents", false},
			{"ShowActionPreview", true},
			{"SelectedActionCategory", 0},
			{"SelectFSMInGameView", true},
			{"DebugLookAtColor", FsmEditorUtility.PackColorIntoInt(Color.gray)},
			{"DebugRaycastColor", FsmEditorUtility.PackColorIntoInt(Color.gray)},
			{"HideUnusedParams", false},
			{"AutoAddPlayMakerGUI", true},
			{"DimUnusedParameters", false},
			{"SelectNewVariables", true},
			{"FsmBrowserShowFullPath", true},
			{"EnableDebugFlow", true},
			{"EnableTransitionEffects", true},
			{"AutoRefreshActionUsage", true},
		};
	}
}