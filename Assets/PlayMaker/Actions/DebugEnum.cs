// (c) Copyright HutongGames, LLC 2010-2013. All rights reserved.

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Debug)]
	[Tooltip("Logs the value of an Enum Variable in the PlayMaker Log Window.")]
	public class DebugEnum : FsmStateAction
	{
        [Tooltip("Info, Warning, or Error.")]
        public LogLevel logLevel;
		
        [UIHint(UIHint.Variable)]
        [Tooltip("Prints the value of an Enum Variable in the PlayMaker log window.")]
		public FsmEnum enumVariable;

		public override void Reset()
		{
			logLevel = LogLevel.Info;
			enumVariable = null;
		}

		public override void OnEnter()
		{
			var text = "None";

			if (!enumVariable.IsNone)
			{
				text = enumVariable.Name + ": " + enumVariable.Value;
			}
			
			ActionHelpers.DebugLog(Fsm, logLevel, text);
						
			Finish();
		}
	}
}