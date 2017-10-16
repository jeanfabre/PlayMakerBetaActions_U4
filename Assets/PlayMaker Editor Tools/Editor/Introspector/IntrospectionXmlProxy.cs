using UnityEngine;
using System;
using System.IO;
using System.Xml;

using System.Collections;

namespace HutongGames.PlayMakerEditor
{
	public class IntrospectionXmlProxy  {


		#region STATIC INTERFACE

		static IntrospectionXmlProxy _instance;
		public static IntrospectionXmlProxy instance
		{
			get{ 
				if (_instance==null)
				{
					_instance = new IntrospectionXmlProxy();
				}
				return _instance;
			}
		}


		public static XmlDocument XmlDocument;

		public static XmlElement XmlRoot;




		public static IEnumerator Init()
		{
			XmlDocument = null;
			XmlDocument = new XmlDocument();

			XmlRoot= XmlDocument.CreateElement( string.Empty, "root", string.Empty );
			XmlDocument.AppendChild( XmlRoot );

			IntrospectionXmlUtils.Init();
			yield return null;
		}

		public static string SaveInFile()
		{
			// get the project folder path;
			string _projectPath = Application.dataPath.Substring(0,Application.dataPath.Length-6);
			Debug.Log(_projectPath);

			string _filePath = _projectPath+"PlayMakerIntrospection.xml";

			//File.WriteAllText(_filePath,XmlNodeToString(XmlDocument.FirstChild));
			XmlDocument.Save(_filePath);

			return _projectPath;
		}

		public static XmlElement AddCdataElementIfNotEmpty(XmlElement parent,string name,string cdataText="")
		{
			if (string.IsNullOrEmpty(cdataText))
			{
				return null;
			}

			XmlCDataSection cdata  = parent.OwnerDocument.CreateCDataSection(cdataText);
			XmlElement _element =  XmlDocument.CreateElement(name);
			_element.InnerText = cdata.OuterXml;
			parent.AppendChild(_element);

			return _element;
		}


		public static XmlElement AddElementIfNotEmpty(XmlElement parent,string name,bool variable)
		{
			if (!variable)
			{
				return null;
			}

			XmlElement _element =  XmlDocument.CreateElement(name);
			_element.InnerText = "true";
			parent.AppendChild(_element);
			
			return _element;
		}

		public static XmlElement AddElementIfValueDifferent(XmlElement parent,string name,bool variable,bool defaultValue)
		{
			if (variable == defaultValue)
			{
				return null;
			}
			
			XmlElement _element =  XmlDocument.CreateElement(name);
			_element.InnerText = variable?"true":"false";
			parent.AppendChild(_element);
			
			return _element;
		}

		public static XmlElement AddElementIfValueDifferent(XmlElement parent,string name,int variable,int defaultValue)
		{
			if (variable == defaultValue)
			{
				return null;
			}
			
			XmlElement _element =  XmlDocument.CreateElement(name);
			_element.InnerText = variable.ToString();
			parent.AppendChild(_element);
			
			return _element;
		}

		public static XmlElement AddElementIfValueDifferent(XmlElement parent,string name,string variable,string defaultValue)
		{
			if (string.Equals(variable,defaultValue))
			{
				return null;
			}
			
			XmlElement _element =  XmlDocument.CreateElement(name);
			_element.InnerText = variable.ToString();
			parent.AppendChild(_element);
			
			return _element;
		}

		public static XmlElement AddElementIfValueDifferent(XmlElement parent,string name,float variable,float defaultValue)
		{
			if (variable==defaultValue)
			{
				return null;
			}
			
			XmlElement _element =  XmlDocument.CreateElement(name);
			_element.InnerText = variable.ToString();
			parent.AppendChild(_element);
			
			return _element;
		}

		public static XmlElement AddElementIfNotEmpty(XmlElement parent,string name,string innerText = "")
		{
			if (string.IsNullOrEmpty(innerText))
			{
				return null;
			}

			XmlElement _element =  XmlDocument.CreateElement(name);
			_element.InnerText = innerText;
			parent.AppendChild(_element);
			
			return _element;
		}


		public static XmlElement AddElement(XmlElement parent,string name,string innerText = "")
		{
			XmlElement _element =  XmlDocument.CreateElement(name);
			_element.InnerText = innerText;
			parent.AppendChild(_element);

			return _element;
		}


		#endregion

		#region PUBLIC INTERFACE


		#endregion PUBLIC INTERFACE


		#region STATIC TOOLS

		public static string XmlNodeToString(XmlNode node)
		{
			return XmlNodeToString(node, 2);
		}
		
		public static string XmlNodeToString(XmlNode node, int indentation)
		{
			if (node==null)
			{
				return "-- NULL --";
			}
			using (var sw = new StringWriter())
			{
				using (var xw = new XmlTextWriter(sw))
				{
					xw.Formatting = Formatting.Indented;
					xw.Indentation = indentation;
					node.WriteTo(xw);
				}
				return sw.ToString();
			}
		}

		#endregion TOOLS

	}
}