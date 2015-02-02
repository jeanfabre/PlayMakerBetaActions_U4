using System;
using System.Linq;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

public static class ClassFileFinder
{


	public static ClassFileDetails FindClassFile(System.Type t)
	{
		return FindClassFile(t.Name);
	}
	
	public static ClassFileDetails FindClassFile(string className)
	{
		ClassFileDetails details = null; 
		if (details == null)
		{
			//Lookup class name in file names
			classFiles = new List<string>();
			FindAllScriptFiles(Application.dataPath);
			for (int i = 0; i < classFiles.Count; i++)
			{
				if (classFiles[i].Contains(className))
				{
					details = new ClassFileDetails(className,"", classFiles[i], File.GetLastWriteTimeUtc(classFiles[i]));
				}
			}
			//Lookup class name in the class file text 
			if (details == null)
			{
				for (int i = 0; i < classFiles.Count; i++)
				{
					string codeFile = File.ReadAllText(classFiles[i]);
					if (codeFile.Contains("class " + className))
					{
						details = new ClassFileDetails(className,"", classFiles[i], File.GetLastWriteTimeUtc(classFiles[i]));
					}
				}
			}
			if (details == null)
			{
				Debug.LogWarning("Failed to lookup class file for class " + className);
			}
			return details;
		}
		else
		{
			return details;
		}
	}


	public static Dictionary<string,ClassFileDetails> FindClassFiles(List<string> classNames)
	{

		//Lookup class name in file names
		Dictionary<string,ClassFileDetails> classDetailsList = new Dictionary<string,ClassFileDetails>();

		classFiles = new List<string>();
		FindAllScriptFiles(Application.dataPath);

		//Lookup class name in the class file text 
		for (int i = 0; i < classFiles.Count; i++)
		{
			string filePath = classFiles[i];
			string codeFile = File.ReadAllText(filePath);
			foreach(string className in classNames)
			{
				if (codeFile.Contains("partial class " + className+": EnumClassBase") && !classDetailsList.ContainsKey(filePath))
				{

					string nameSpace = "";

					ClassFileDetails _details = new ClassFileDetails(
						className, 
						nameSpace,
						filePath, 
						File.GetLastWriteTimeUtc(filePath)
						);

					_details.projectPath =  filePath.Substring(Application.dataPath.Length+1);

					classDetailsList.Add (filePath,_details);
				}
			}
		}


		return classDetailsList;

	}


	public static Dictionary<string,EnumFileDetails> FindEnumFiles()
	{
		
		//Lookup enums in file names
		Dictionary<string,EnumFileDetails> enumDetailsList = new Dictionary<string,EnumFileDetails>();
		
		classFiles = new List<string>();
		FindAllScriptFiles(Application.dataPath);
		
		//Lookup class name in the class file text 
		for (int i = 0; i < classFiles.Count; i++)
		{
			string filePath = classFiles[i];
			string codeFile = File.ReadAllText(filePath);

			if (codeFile.Contains("["+"PLAYMAKER_ENUM]")) // compose the tag to avoid this file to be found...
			{


				// read all lines, we are going to parse data
				string[] lines = File.ReadAllLines(filePath);

				// safety precaution
				if (lines.Length<10)
				{
					continue;
				}

				string nameSpace = lines[5].Substring(10);
				string enumName	= lines[7].Substring(13);

				EnumFileDetails _details = new EnumFileDetails(
					enumName, 
					nameSpace,
					filePath, 
					File.GetLastWriteTimeUtc(filePath)
					);
				
				_details.projectPath =  filePath.Substring(Application.dataPath.Length+1);
				
				enumDetailsList.Add (filePath,_details);
			}

		}

		return enumDetailsList;
	}


	public static Dictionary<string,ClassFileDetails> GetEnumerableOfType<T>() where T : class
	{

		List<string> _shortList = new List<string>() ;
		foreach (Type type in 
		         Assembly.GetAssembly(
					typeof(T)).GetTypes()
		        	 .Where(myType =>
							myType.IsSubclassOf(typeof(T))
		                    )
			)
		{
			_shortList.Add(type.Name);
		}

		return FindClassFiles(_shortList);
	}



	static List<string> classFiles;
	static void FindAllScriptFiles(string startDir)
	{

		try
		{
			foreach (string file in Directory.GetFiles(startDir))
			{
				if (file.Contains(".cs") || file.Contains(".js"))
					classFiles.Add(file);
			}
			foreach (string dir in Directory.GetDirectories(startDir))
			{
				FindAllScriptFiles(dir);
			}
		}
		catch (System.Exception ex)
		{
			Debug.Log(ex.Message);
		}
	}
		
}

public class ClassFileDetails
{
	public string className { get; set; }
	public string nameSpace { get; set; }
	public string fileName { get; set; }
	public string path { get; set; }
	public string projectPath { get; set; }
	public System.DateTime updateTime { get; set; }

	public override string ToString ()
	{
		return string.Format ("[ClassFileDetails: className={0}, path={1}, updateTime={2}]", className, path, updateTime);
	}

	internal ClassFileDetails()
	{ }
	internal ClassFileDetails(string setClassName,string setNameSpace, string setPath, System.DateTime setUpdateTime)
	{
		className = setClassName;
		nameSpace = setNameSpace;
		path = setPath;
		updateTime = setUpdateTime;

		FileInfo _info = new FileInfo(setPath);
		fileName = _info.Name;

		//DatabaseLink.StoreClassFileDetails(this);
	}
}

public class EnumFileDetails
{
	public string enumName { get; set; }
	public string nameSpace { get; set; }
	public string fileName { get; set; }
	public string path { get; set; }
	public string projectPath { get; set; }
	public System.DateTime updateTime { get; set; }
	
	public override string ToString ()
	{
		return string.Format ("[EnumFileDetails: enumName={0}, path={1}, updateTime={2}]", enumName, path, updateTime);
	}
	
	internal EnumFileDetails()
	{ }
	internal EnumFileDetails(string setEnumName,string setNameSpace, string setPath, System.DateTime setUpdateTime)
	{
		enumName = setEnumName;
		nameSpace = setNameSpace;
		path = setPath;
		updateTime = setUpdateTime;
		
		FileInfo _info = new FileInfo(setPath);
		fileName = _info.Name;
		
		//DatabaseLink.StoreClassFileDetails(this);
	}
}
