// (c) copyright Hutong Games, LLC 2010-2012. All rights reserved.

using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Debug)]
    [Tooltip("Print the value of an FSM Variable in the PlayMaker Log Window.")]
    public class DebugFsmVariable : FsmStateAction
    {
        [Tooltip("Info, Warning, or Error.")]
        public LogLevel logLevel;

        [HideTypeFilter]
        [UIHint(UIHint.Variable)]
        [Tooltip("Variable to print to the PlayMaker log window.")]
        public FsmVar variable;

        public override void Reset()
        {
            logLevel = LogLevel.Info;
            variable = null;
        }

        public override void OnEnter()
        {
            ActionHelpers.DebugLog(Fsm, logLevel, variable.DebugString());

            Finish();
        }
    }
}
