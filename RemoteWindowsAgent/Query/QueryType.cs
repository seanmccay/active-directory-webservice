using System;

namespace RemoteWindowsAgent
{
	/// <summary>
	/// Summary description for QueryType.
	/// </summary>
	public class QueryType
	{
		public const string USER  = "user";
		public const string GROUP = "group";
		public const string GROUP_USER = "groupuser";
		public const string COMPUTER = "computer";
        //cwa - JD Edwards Connector
        public const string JDE_ADDRESS_NUMBER = "address";
        public const string JDE_USER_PREFERENCES = "userpreferences";

	}
}
