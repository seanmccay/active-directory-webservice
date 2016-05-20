using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Linq;
using System.Text.RegularExpressions;

namespace RemoteWindowsAgent
{
	//remaining strings are internal use only - no I18n needed
    [Serializable]
    public class ReturnValues {
        List<ReturnValue> values = new List<ReturnValue>();

		private static Object lockObj = new Object();
        private static XmlReaderSettings settings = null;

		public ReturnValues(ReturnValue rv) 
		{
			values.Add(rv);
		}

        public ReturnValues() {
        }

        public ReturnValues(String xml)
        {
            parse(xml);
        }

		public ReturnValues Clone()
		{
			ReturnValues ReturnValuesCopy = new ReturnValues();
			ReturnValuesCopy.values = new List<ReturnValue>();
			
			List<ReturnValue>.Enumerator ienum = values.GetEnumerator();
			while (ienum.MoveNext()) 
				ReturnValuesCopy.values.Add(ienum.Current);

			return ReturnValuesCopy;
		}

        public static ReturnValues GetSimpleSuccessReturnValues(String val) {
            ReturnValues rvs = new ReturnValues();
            ReturnValue rv = new ReturnValue(ReturnCodes.SUCCESS);
            rv.SetValue(val);
            rvs.AddReturnValue(rv);
            return rvs;          
        }
        
        public static ReturnValues GetSimpleReturnValues(int code, String text) {
            ReturnValues rvs = new ReturnValues();
            ReturnValue rv = new ReturnValue(code);
            rv.SetText(text);
            rvs.AddReturnValue(rv);
            return rvs;
        }

        public static ReturnValues GetSimpleReturnValues(int code, String text, String value) {
            ReturnValues rvs = new ReturnValues();
            ReturnValue rv = new ReturnValue(code);
            rv.SetText(text);
            rv.SetValue(value);
            rvs.AddReturnValue(rv);
            return rvs;
        }
        
        public void AddReturnValue(ReturnValue rv) {
            values.Add(rv);
        }

		//Does an Add if the value does not exist
		public void ReplaceReturnValue(ReturnValue rv) 
		{
			String name = rv.GetName();
			bool done = false;
			IEnumerator ienum = this.values.GetEnumerator();
			while (ienum.MoveNext()) 
			{
				ReturnValue rv2 = (ReturnValue)ienum.Current;
				if (name==rv2.GetName()) 
				{
					rv2.SetValue(rv.GetStringValue());
					rv2.SetText(rv.GetText());
					rv2.SetCode(rv.GetCode());
					done = true;
					break;
				}
			}
			if (!done) 
			{
				AddReturnValue(rv);
			}
		}

		//returns null if not found
		public ReturnValue GetReturnValue(String name) 
		{
			IEnumerator ienum = this.values.GetEnumerator();
			while (ienum.MoveNext()) 
			{
				ReturnValue rv2 = (ReturnValue)ienum.Current;
				if (name==rv2.GetName()) 
				{
					return rv2;
				}
			}
			return null;
		}

        public void Validate(
            object sender,
            ValidationEventArgs e
            ) {
        }

        //convenience method
        public bool IsSuccessful() {

			//bool allowDebug = LogFile.AllowDebug;

			if (values.Count == 0)
			{
				//if (allowDebug) LogFile.Debug("ReturnValues.IsSuccessful(): Returning true (by default) for an empty ReturnValues object");
				return true;
			}
			
			IEnumerator e = values.GetEnumerator();
            while (e.MoveNext()) {
                ReturnValue rv = (ReturnValue)(e.Current);
                int code = rv.GetCode();
                if (code<0) return false;
            }
            return true;
        }


		/// <summary>
		/// Returns true if all errors (if there are any) in the object 
		/// are retryable.
		/// </summary>
		/// <param name="rvs"></param>
		/// <returns>false if there is at least one error that is not
		/// retryable. Returns true otherwise.</returns>
		public bool AllErrorsAreRetryable()
		{
			// look at each rv in rvs. If anything is NOT retryable, return false
			List<ReturnValue> returnValues = GetAllReturnValues();
			foreach (ReturnValue returnValue in returnValues)
			{
				int code = returnValue.GetCode();
				if (ReturnCodes.IsError(code))
				{
					// we found an error. See if it is retryable.
					if (!ReturnCodes.IsRetryableError(code))
					{
						return false;
					}
				}
			}

			// If we made it here, then there are no permanent errors
			return true;
		}

        public bool IsSuccessfulOrNotSupported()
        {
            IEnumerator e = values.GetEnumerator();
            while (e.MoveNext())
            {
                ReturnValue rv = (ReturnValue)(e.Current);
                int code = rv.GetCode();
				if ((code < 0) && (code != ReturnCodes.OPERATION_NOT_SUPPORTED))
				{
					return false;
				}
            }
            return true;
        }

		public bool IsFirstValueSuccessful() 
		{
			if (values.Count==0) return true;

			ReturnValue rv = (ReturnValue)values[0];
			return rv.GetCode()>=0;
		}

        //warnings in this case are considered non-success
        public bool IsStrictSuccessful() {
            IEnumerator e = values.GetEnumerator();
            while (e.MoveNext()) {
                ReturnValue rv = (ReturnValue)(e.Current);
                int code = rv.GetCode();
                if (code!=0) return false;
            }
            return true;
        }

        public bool IsNoSuchUser() {
            IEnumerator e = values.GetEnumerator();
            while (e.MoveNext()) {
                ReturnValue rv = (ReturnValue)(e.Current);
                if (rv.GetCode()!=ReturnCodes.NO_SUCH_USER) return false;
            }
            return true;
        }
        
        public String GetSuccessString() {
            IEnumerator e = values.GetEnumerator();
            while (e.MoveNext()) {
                ReturnValue rv = (ReturnValue)(e.Current);
                int code = rv.GetCode();
                if (code==0) {
                    return rv.GetStringValue();
                }
            }    
            return null;
        }

        //An enumeration of ReturnValue objects
        public List<ReturnValue>.Enumerator GetEnumerator() {
            return values.GetEnumerator();
        }
 
        public static XmlElement buildReturnValueElement(ReturnValue rv, XmlDocument doc) {
            XmlElement e = doc.CreateElement("","returnvalue","");
            String name = rv.GetName();
            if (name!=null) {
                e.SetAttribute("name","",name);
            }

            String code = rv.GetCode().ToString();
            e.SetAttribute("code","",code);

            //some fields such as color are ignored because we don't need to send to the agent

            String type = rv.GetStringType();
            if (type!=null) {
                e.SetAttribute("type","",type);
            }

            bool isList = rv.IsList();
            e.SetAttribute("list","",isList.ToString());

            if (isList) {
                IList list = rv.GetOriginalList();
                IEnumerator ienum = list.GetEnumerator();
                while (ienum.MoveNext()) {
                    String val = ienum.Current.ToString();

					XmlElement valueElement = doc.CreateElement("", "value", "");
					if (ienum.Current is ReturnValueField)
					{
						valueElement.SetAttribute("field",
							((ReturnValueField) ienum.Current).GetFieldName());
					}
                    
                    valueElement.AppendChild(doc.CreateTextNode(val));
                    e.AppendChild(valueElement);
                }
            }
            else {
                String val2 = rv.GetStringValue();
                if (val2!=null) {
                    XmlElement valueElement = doc.CreateElement("","value","");
                    valueElement.AppendChild(doc.CreateTextNode(val2));
                    e.AppendChild(valueElement);
                }
            }

            String text = rv.GetText();
            if (text!=null) {
                XmlElement textElement = doc.CreateElement("","text","");
                textElement.AppendChild(doc.CreateTextNode(text));
                e.AppendChild(textElement);
            }

            return e;

        }

        public String toXML() {
            try {
                // Code to demonstrate creating of XmlDocument programmatically
                XmlDocument xmlDom = new XmlDocument(  );
                xmlDom.AppendChild(xmlDom.CreateElement("", "returnvalues", ""));
                XmlElement xmlRoot = xmlDom.DocumentElement;

                IEnumerator ienum = values.GetEnumerator();
                while (ienum.MoveNext()) {
                    ReturnValue rv = (ReturnValue)(ienum.Current);
                    XmlElement e = buildReturnValueElement(rv,xmlDom);
                    xmlRoot.AppendChild(e);
                }
    
                return xmlDom.InnerXml;

            }
            catch (Exception) {
                return null;
            }
        }

        //Not sure this is going to meet our needs.  It is a port from the Java side.
        //the Java side could tell all fields as strings, but I'm not sure that applies here.
        public Hashtable GetHashtable() {
            Hashtable table = new Hashtable();
            IEnumerator ienum = values.GetEnumerator();
            while (ienum.MoveNext()) {
                ReturnValue rv = (ReturnValue)(ienum.Current);
                String name = rv.GetName();
                ReturnValueTypes type = rv.GetValueType();
                bool isList = rv.IsList();
                if (!isList) {
                    table[name]=rv.GetStringValue();
                }
                else {
                    table[name]=rv.GetList();
                }
            }

            return table;
        }

		public Hashtable GetReturnValueHashtable() 
		{
			Hashtable table = new Hashtable();
			IEnumerator ienum = values.GetEnumerator();
			while (ienum.MoveNext()) 
			{
				ReturnValue rv = (ReturnValue)(ienum.Current);
				String name = rv.GetName();
				if (name!=null)    //This was thrown in due to crashes occuring when Solaris was timing out.
				{
					table[name]=rv;
				}
			}

			return table;
		}

		public void LoadFromReturnValueHashtable(Hashtable ht) 
		{
			IDictionaryEnumerator ienum = ht.GetEnumerator();
			while (ienum.MoveNext()) 
			{
				ReturnValue rv = (ReturnValue)(ienum.Value);
				this.ReplaceReturnValue(rv);
			}
		}

		public void LoadFromHashtable(Hashtable ht) 
		{
			IDictionaryEnumerator ienum = ht.GetEnumerator();
			while (ienum.MoveNext()) 
			{
				ReturnValue rv = new ReturnValue(0);
				rv.SetName((String)(ienum.Key));
				Object val = ienum.Value;
				if (val is IList) 
				{
					rv.SetList((IList)val);
				}
				else 
				{
					rv.SetValue((String)val);
				}
				this.ReplaceReturnValue(rv);
			}
		}

		public bool IsEmpty() 
		{
			return (values==null) || (values.Count==0);
		}

		public void AddReturnValues(ReturnValues rvs) 
		{
			values.AddRange(rvs.values);
		}

		public ReturnValue GetFirstReturnValue() 
		{
			 List<ReturnValue>.Enumerator rvsEnum = values.GetEnumerator();
            if (rvsEnum.MoveNext())
            {

                return rvsEnum.Current;
            }
            return null;
		}

		public List<ReturnValue> GetAllReturnValues()
		{
			if (values == null)
			{
				return new List<ReturnValue>();
			}
			else
			{
				return values;
			}
		}

		public void SetAllNames(String name) 
		{
			IEnumerator e = values.GetEnumerator();
			while (e.MoveNext()) 
			{
				ReturnValue rv = (ReturnValue)(e.Current);
				rv.SetName(name);
			}
		}

        private void parse(String xml)
        {

            try
            {
                XmlReader r = XmlReader.Create(new StringReader(xml));

                XmlDocument doc = new XmlDocument();

                doc.Load(r);

                //XmlDocument doc = new XmlDocument();
                //doc.LoadXml(xml);
                XmlElement root = doc.DocumentElement;
                XmlNodeList nodes = doc.GetElementsByTagName("returnvalues");
                XmlNode node = nodes.Item(0);
                XmlNodeList rvs = node.SelectNodes("returnvalue");
                if (rvs == null)
                {
                    
                }
                for (int i = 0; i < rvs.Count; i++)
                {
                    XmlNode child = rvs.Item(i);
                    ReturnValue srv = new ReturnValue(child);
                    values.Add(srv);

                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
                throw new XmlException(e.Message, e);
            }

        }
 
	}
}
