using System;
using System.Collections;

namespace RemoteWindowsAgent
{
	public class QueryClause
	{
		private string AttributeDisplay;
		private string Attribute;
		private string OperatorDisplay;
		private string Operator;
		private string ValueDisplay;
		private string Value;
		private string AttributeType;

		public const string QUERY_BOOLEAN_OP_AND = "AND";
		public const string QUERY_BOOLEAN_OP_OR = "OR";

		public QueryClause(string AttributeDisplay, string Attribute,
			string OperatorDisplay, string Operator,
			string ValueDisplay, string Value, string AttributeType)
		{
			this.AttributeDisplay	= AttributeDisplay;
			this.Attribute			= Attribute;
			this.OperatorDisplay	= OperatorDisplay;
			this.Operator			= Operator;
			this.ValueDisplay		= ValueDisplay;
			this.Value				= Value;
			this.AttributeType		= AttributeType;
		}

		public QueryClause(string attribute, string op, string val, string attributeType)
		{
			this.AttributeDisplay	= attribute;
			this.Attribute			= attribute;
			this.OperatorDisplay	= op;
			this.Operator			= op;
			this.ValueDisplay		= val;
			this.Value				= val;
			this.AttributeType		= attributeType;
		}

		public string GetAttributeDisplay()
		{
			return AttributeDisplay;
		}

		public void SetAttributeDisplay(string strValue)
		{
			AttributeDisplay = strValue;
		}

		public string GetAttribute()
		{
			return Attribute;
		}

		public void SetAttribute(string strValue)
		{
			Attribute = strValue;
		}

		public string GetOperatorDisplay()
		{
			return OperatorDisplay;
		}

		public void SetOperatorDisplay(string strValue)
		{
			OperatorDisplay = strValue;
		}

		public string GetOperator()
		{
			return Operator;
		}

		public void SetOperator(string strValue)
		{
			Operator = strValue;
		}

		public string GetValueDisplay()
		{
			return ValueDisplay;
		}

		public void SetValueDisplay(string strValue)
		{
			ValueDisplay = strValue;
		}

		public string GetValue()
		{
			return Value;
		}

		public void SetValue(string strValue)
		{
			Value = strValue;
		}

		public string GetAttributeType()
		{
			return AttributeType;
		}

		public void SetAttributeType(string strValue)
		{
			AttributeType = strValue;
		}

		public static void PrepareQueryRequest(
			string objectType,
            IList arFieldList,
            IList arQueryList,	
			string strAttribute,	
			int maxrows,			
			bool bPicklist,			
			string strBooleanOp,
            bool OnlyUniversalGroups,  // applies to AD groups only
			ref ReturnValues rvsQual,
			ref ReturnValues rvsAttr
			)
		{
			ReturnValue rvtemp = new ReturnValue(0);

			// Pass in object type
			rvtemp = new ReturnValue(0);
			rvtemp.SetName(Parameter.QUERY_OBJECT_TYPE);
			rvtemp.SetValue(objectType);
			rvsAttr.AddReturnValue(rvtemp);

            if (OnlyUniversalGroups) // applies to AD groups only
            {
                rvtemp = new ReturnValue(0);
                rvtemp.SetName(Parameter.QUERY_GROUP_TYPE);
                rvtemp.SetValue(((int)ADGroupScopeEnum.Universal).ToString());
                rvsAttr.AddReturnValue(rvtemp);
            }

            // Load optional secondary attributes to be returned
			rvtemp = new ReturnValue(0);
			rvtemp.SetName(Parameter.QUERY_FIELDLIST);
			rvtemp.SetList(arFieldList);
			rvsAttr.AddReturnValue(rvtemp);

			rvtemp = new ReturnValue(0);
			rvtemp.SetName(Parameter.QUERY_PRIMARYATTRIBUTE);
			rvtemp.SetValue(strAttribute);
			rvsAttr.AddReturnValue(rvtemp);

			rvtemp = new ReturnValue(0);
			rvtemp.SetName(Parameter.QUERY_FILTERBOOLEANOP);
			rvtemp.SetValue(strBooleanOp);
			rvsAttr.AddReturnValue(rvtemp);

			// Load picklist mode
			rvtemp = new ReturnValue(0);
			rvtemp.SetName(Parameter.QUERY_PICKLISTMODE);
			rvtemp.SetType(ReturnValueTypes.BOOLEAN);
			if (bPicklist) 
			{
				rvtemp.SetValue("true");
			}
			else 
			{
				rvtemp.SetValue("false");
			}
			rvsAttr.AddReturnValue(rvtemp);

			// Load query filter
            IList arFieldName = new ArrayList();
            IList arOperator = new ArrayList();
            IList arValue = new ArrayList();
            IList arFieldType = new ArrayList();

			foreach (QueryClause qi in arQueryList)
			{
				arFieldName.Add(qi.GetAttribute());
				arFieldType.Add(qi.GetAttributeType());
				arOperator.Add(qi.GetOperator());
				arValue.Add(qi.GetValue());
			}

			rvtemp = new ReturnValue(0);
			rvtemp.SetName(Parameter.QUERY_FILTERFIELDLIST);
			rvtemp.SetList(arFieldName);
			rvsQual.AddReturnValue(rvtemp);

			rvtemp = new ReturnValue(0);
			rvtemp.SetName(Parameter.QUERY_FILTERFIELDTYPELIST);
			rvtemp.SetList(arFieldType);
			rvsQual.AddReturnValue(rvtemp);

			rvtemp = new ReturnValue(0);
			rvtemp.SetName(Parameter.QUERY_FILTEROPERATORLIST);
			rvtemp.SetList(arOperator);
			rvsQual.AddReturnValue(rvtemp);

			rvtemp = new ReturnValue(0);
			rvtemp.SetName(Parameter.QUERY_FILTERVALUELIST);
			rvtemp.SetList(arValue);
			rvsQual.AddReturnValue(rvtemp);

            rvtemp = new ReturnValue(0);
            rvtemp.SetName(Parameter.QUERY_MAXROWS);
            rvtemp.SetType(ReturnValueTypes.STRING);
            rvtemp.SetValue(maxrows.ToString());
            rvsQual.AddReturnValue(rvtemp);
        }

		public override bool Equals(object obj)
		{

			if (obj == null || (GetType() != obj.GetType()))
			{
				return false;
			}

			QueryClause o = (QueryClause)obj;
			if (!String.Equals(this.AttributeDisplay, o.AttributeDisplay)) return false;
			if (!String.Equals(this.Attribute, o.Attribute)) return false;
			if (!String.Equals(this.OperatorDisplay, o.OperatorDisplay)) return false;
			if (!String.Equals(this.Operator, o.Operator)) return false;
			if (!String.Equals(this.ValueDisplay, o.ValueDisplay)) return false;
			if (!String.Equals(this.Value, o.Value)) return false;
			if (!String.Equals(this.AttributeType, o.AttributeType)) return false;

			return true;
		}

		public override int GetHashCode()
		{
			int hashCode = 0;
			if (AttributeDisplay != null) hashCode += AttributeDisplay.GetHashCode();
			if (Attribute != null) hashCode += Attribute.GetHashCode();
			if (OperatorDisplay != null) hashCode += OperatorDisplay.GetHashCode();
			if (Operator != null) hashCode += Operator.GetHashCode();
			if (ValueDisplay != null) hashCode += ValueDisplay.GetHashCode();
			if (Value != null) hashCode += Value.GetHashCode();
			if (AttributeType != null) hashCode += AttributeType.GetHashCode();

			return hashCode;
		}

	}

}
