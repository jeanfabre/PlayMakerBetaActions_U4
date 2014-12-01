// (c) Copyright HutongGames, LLC 2010-2015. All rights reserved.

using UnityEngine;
using System.Collections;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.String)]
	[Tooltip("Automatically types into a string.")]
	public class AutoTypeString : FsmStateAction
	{
		[RequiredField]
		[Tooltip("The string with the entire message to type out.")]
		public FsmString baseString;

		[RequiredField]
		[Tooltip("The target result string (can be the same as the base string).")]
		public FsmString resultString;

		[Tooltip("The time between letters appearing.")]
		public FsmFloat pauseTime;

		[Tooltip("Send this event when finished typing.")]
		public FsmEvent finishEvent;



		[UIHint(UIHint.Description)]
		public string d1 = "     Optional Sounds Section:";

		[Tooltip("Check this to play sounds while typing.")]
		public bool useSounds;

		[ObjectType(typeof(AudioClip))]
		[Tooltip("The sound to play for each letter typed.")]
		public FsmObject typingSound;

		[Tooltip("The GameObject with an AudioSource component.")]
		public FsmOwnerDefault audioSourceObj;
		
		string message;
		private AudioSource audioSource;
		private AudioClip sound;

		public override void Reset()
		{
			baseString = null;
			resultString = null;
			pauseTime = 0.2f;
			useSounds = false;
			typingSound = null;
			finishEvent = null;
		}

		public override void OnEnter()
		{
			// sort out the sounds
			if (useSounds)
			{
				// find the audio source
				var go = Fsm.GetOwnerDefaultTarget(audioSourceObj);
				if (go != null)
				{
					audioSource = go.GetComponent<AudioSource>();
					if (audioSource == null)
					{
						Debug.Log ("AudioSource component not found.");
						useSounds = false;
					}

					sound = typingSound.Value as AudioClip;
					if (sound == null)
					{
						Debug.Log ("AudioClip not found.");
						useSounds = false;
					}
				}

				else 
				{
					Debug.Log ("AudioSource Object not found.");
					useSounds = false;
				}
			}

			// clone the base string to a local value.
			message = baseString.Value;

			// clear the target string.
			resultString.Value = "";

			DoTyping ();
		}

		public static IEnumerable DoTyping()
		{
			foreach (char letter in message.ToCharArray()) {
				resultString.Value += letter;
				if (useSounds)
					audioSource.PlayOneShot (sound);
				yield return 0;
				yield return new WaitForSeconds (pauseTime);
			}
		}

		public void DoFinish()
		{
			Fsm.Event(finishEvent);
		}
	}
}









