using System;
using System.Collections;
using System.Xml;
using System.Collections.Generic;

namespace RemoteWindowsAgent
{
    [Serializable]
    public enum ReturnValueTypes {
        STRING,
        DATE,
        BOOLEAN,
        TIME
    }

	//The remaining strings are internal usage
    [Serializable]
	public class ReturnValue
	{
        int code;
        String name;
        String color;
        ReturnValueTypes type;
        String val;
        String text;
        private bool isList=false;
        private IList originalList = null;
        private List<String> stringList = null;

        public ReturnValue(XmlNode node)
        {
            Parse(node);
        }

        public ReturnValue(int code) {
            this.code=code;
        }

		public ReturnValue(String name, String val)
			:this(0, name, val)
		{
		}

        public ReturnValue(int code, String name, String val)
        {
            this.code = code;
            this.val = val;
            this.name = name;
            type = ReturnValueTypes.STRING;
        }

        private String GetAttribute(XmlAttributeCollection attrs, 
        String name) {

            if (attrs == null) return null;

            XmlNode tmpNode = attrs.GetNamedItem(name);
            if (tmpNode==null) return null;
            return tmpNode.Value;
        }


		public void SetCode(int code) 
		{
			this.code = code;
		}

        public void SetName(String name) 
		{
            this.name = name;
        }

        public void SetText(String text) {
            this.text = text;
        }

        public void SetType(ReturnValueTypes type) {
            this.type = type;
        }

        public void SetValue(String val) {
            this.val = val;
        }
        
        public int GetCode() {
            return code;
        }

        public ReturnValueTypes GetValueType() {
            return type;
        }

        public String GetStringType() {
            switch (type) {
                case ReturnValueTypes.BOOLEAN: 
                    return "boolean";
                case ReturnValueTypes.DATE: 
                    return "date";                
                case ReturnValueTypes.STRING: 
                    return "string";
                case ReturnValueTypes.TIME: 
                    return "time";
                default: 
                    return "string";
            }
        }

        public String GetName() {
            return name;
        }

        public String GetStringValue() {
            return val;
        }

        public String GetText() {
            return text;
        }

        public int GetUnixTime() {
            return Int32.Parse(val);
        }

		public int GetIntValue() 
		{
			return Int32.Parse(val);
		}

        //Note: can throw parsing exceptions
        public DateTime GetDateValue() {
            int unixTime = Int32.Parse(val);
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0);
            return origin + new TimeSpan(unixTime*TimeSpan.TicksPerSecond);
        }

        //Note: can throw parsing exceptions
        public DateTime GetTimeValue() {
            return GetDateValue();
        }
        

        /// <summary>
        /// Return a list of items in the form of a list of string objects.
        /// The original objects that were in the list are not returned, but
        /// string versions of the objects are returned. To get a list of the
        /// original objects, see GetOriginalList()
        /// </summary>
        /// <returns></returns>
        public List<String> GetList() {
            return stringList;
        }

        /// <summary>
        /// Return the original list of items that was passed into the object.
        /// For a list of the string verions of the objects, see GetList().
        /// </summary>
        /// <returns></returns>
        public IList GetOriginalList()
        {
            return originalList;
        }

        public void SetList(IList list) {
            if (list != null)
            {
                isList = true;
                this.originalList = list;

                stringList = new List<String>();
                foreach (object o in originalList)
                {
                    stringList.Add(o.ToString());
                }
            }
        }

        public bool IsList() {
            return isList;
        }

        public void SetDate(DateTime dt)
        {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0);
			TimeSpan ts = dt.Subtract(origin);
            long seconds = ts.Ticks/TimeSpan.TicksPerSecond;
            val = seconds.ToString();
        }

        public String toXML()
        {
            try
            {
                // Code to demonstrate creating of XmlDocument programmatically
                XmlDocument xmlDom = new XmlDocument();
                xmlDom.AppendChild(xmlDom.CreateElement("", "returnvalues", ""));
                XmlElement xmlRoot = xmlDom.DocumentElement;
                XmlElement e = ReturnValues.buildReturnValueElement(this, xmlDom);
                xmlRoot.AppendChild(e);
                return xmlDom.InnerXml;

            }
            catch (Exception)
            {
                return "";
            }
        }

        private void Parse(XmlNode node)
        {
            XmlAttributeCollection attrs = node.Attributes;
            String codeStr = GetAttribute(attrs, "code");

            if (codeStr == null) throw new XmlException(Resources.CODE_IS_MISSING, new Exception());
            code = Int32.Parse(codeStr);
            if (code == ReturnCodes.INTERNAL_ERROR) code = ReturnCodes.UNKNOWN_AGENT_FAILURE;

            name = GetAttribute(attrs, "name");
            String typeStr = GetAttribute(attrs, "type");
            if (typeStr == null)
            {
                type = ReturnValueTypes.STRING;
            }
            else if (typeStr.Equals("date"))
            {
                type = ReturnValueTypes.DATE;
            }
            else if (typeStr.Equals("time"))
            {
                type = ReturnValueTypes.TIME;
            }
            else if (typeStr.Equals("boolean"))
            {
                type = ReturnValueTypes.BOOLEAN;
            }
            else
            {
                type = ReturnValueTypes.STRING;
            }

            color = GetAttribute(attrs, "color");

            String listNode = GetAttribute(attrs, "list");
            if (listNode != null)
            {
                isList = String.Equals(listNode.ToLower(), "true");
            }

            XmlNodeList values = node.SelectNodes("value");
            if (!isList)
            {
                XmlNode valNode = values.Item(0);
                if (valNode != null)
                {
                    val = valNode.InnerText;
                }
            }
            else
            {
                originalList = new ArrayList();
                for (int j = 0; j < values.Count; j++)
                {
                    XmlNode valueNode = values.Item(j);
                    if (valueNode != null)
                    {
                        if (String.Equals(valueNode.Name.ToLower(), "value"))
                        {
                            XmlAttributeCollection valueAttrs = valueNode.Attributes;
                            String fieldName = GetAttribute(valueAttrs, "field");
                            if (fieldName != null)
                            {
                                IReturnValueField rvf =
                                    new ReturnValueField(valueNode.InnerText, fieldName);
                                // add an rvf to the list
                                originalList.Add(rvf);
                            }
                            else
                            {
                                // add a plain string to the list
                                originalList.Add(valueNode.InnerText);
                            }
                        }
                    }
                }

                // Initializes stringList
                SetList(originalList);
            }

            /*
            NodeList nodes = node.getChildNodes();
            if (!isList) 
            {
                value = this.getFirstValue(nodes,"value");
            } 
            else 
            {
                list = new IList();
                for (int j = 0; j < nodes.getLength(); j++) 
                {
                    Node valNode = nodes.item(j);
                    if (valNode != null) 
                    {
                        if (valNode.getNodeName().equalsIgnoreCase("value")) 
                        {
                            Node textNode = valNode.getFirstChild();
                            list.add(textNode.getNodeValue());
                        }
                    }
                }
            }
            */
            XmlNodeList texts = node.SelectNodes("text");
            XmlNode textNode = texts.Item(0);
            if (textNode != null)
            {
                text = textNode.InnerText;
            }
        }
	}
}
