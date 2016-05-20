using System;
using System.Collections.Generic;
using System.Text;

namespace RemoteWindowsAgent
{
    /// <summary>
    /// This class represents a value xml node (with a field 
    /// attribute) that is returned in a list of value nodes inside 
    /// a returnValue node. This class stores both the string value
    /// of the node and the value of the field attribute of
    /// the node.
    /// </summary>
    public class ReturnValueField : IReturnValueField
    {
        private string fieldName;
        private string value;

        public ReturnValueField(string value, string fieldName)
        {
            this.value = value;
            this.fieldName = fieldName;
        }

        public string GetFieldName()
        {
            return fieldName;
        }
        public string GetValue() 
        {
            return value;
        }

        public override string ToString()
        {
            return value;
        }

    }
}
