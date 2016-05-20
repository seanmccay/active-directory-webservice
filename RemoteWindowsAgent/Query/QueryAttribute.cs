using System;

namespace RemoteWindowsAgent
{
	public class QueryAttribute
	{
		private string DisplayName;
		private string Name;
		private string Type;

		public QueryAttribute(string DisplayName, string Name, string Type)
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
