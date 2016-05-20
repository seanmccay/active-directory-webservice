using System;

namespace RemoteWindowsAgent
{
	public class QueryOperator
	{
		public const string OPERATOR_LESSTHANOREQUALTO		= "lessthanorequalto";
		public const string OPERATOR_GREATERTHANOREQUALTO	= "greaterthanorequalto";
		public const string OPERATOR_CONTAINS		= "contains";
		public const string OPERATOR_STARTSWITH		= "startswith";
		public const string OPERATOR_ENDSWITH		= "endswith";
		public const string OPERATOR_ISEXACTLY		= "isexactly";
		public const string OPERATOR_ISNOT			= "isnot";
		public const string OPERATOR_PRESENT		= "present";
		public const string OPERATOR_NOTPRESENT		= "notpresent";

		private string DisplayName;
		private string Name;
		private string Type;


		public QueryOperator(string DisplayName, string Name, string Type)
		{
			this.DisplayName = DisplayName;
			this.Name = Name;
			this.Type = Type;
		}

		public string GetDisplayName()
		{
			return DisplayName;
		}

		public void SetDisplayName(string strValue)
		{
			DisplayName = strValue;
		}

		public string GetName()
		{
			return Name;
		}

		public void SetName(string strValue)
		{
			Name = strValue;
		}

		public string GetQueryType()
		{
			return Type;
		}

	}
}
