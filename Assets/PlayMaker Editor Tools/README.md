PlayMaker Editor tools
================

## Licensing
This package is released under LGPL license: [http://opensource.org/licenses/LGPL-3.0](http://opensource.org/licenses/LGPL-3.0)


## Description
This is a set of Editor tools for PlayMaker that you will find under the PlayMaker Menu: Addons/tools/

### Introspection

Introspection will scan the whole project and produce an Xml file feature all PlayMaker usage, Fsm, actions and values, as well as Unity project information and code related to PlayMaker usages ( like enums).

 Right now there is no interface to use this data but the xml file is produced ( at the root of the project ../PlayMakerIntrospection.xml
 
### Turn Off all DebugFlow

This tool will scan all prefabs and scenes in your Build and turn off all Debugflow flags in Fsm, so that they don't affect performances, so as you go trhough a debug session and turn on DebugDFlow in many Fsm, you don't have to worry about forgetting to turn them back off, just use this tool. It will inform in the console about what fsm was affected.
 
 
