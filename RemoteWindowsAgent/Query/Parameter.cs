using System;

namespace RemoteWindowsAgent
{
	public class Parameter
	{
		public const string DC						= "dc";
        public const string AIMS_CONNECTOR_ID       = "aimsconnectorid";
        public const string ADMIN_USER              = "adminuser";
		public const string ADMIN_PASSWORD			= "adminpassword";
        public const string GROUP_DOMAIN = "groupdomain";
        public const string GROUP_NAME              = "groupname";
		public const string GROUP_SID				= "groupsid";
		public const string MEMBER_NAME				= "membername";
		public const string QUERY_DISPLAYNAMELIST	= "querydisplaynamelist";
		public const string QUERY_FIELDLIST			= "queryfieldlist";
		public const string QUERY_FILTER			= "queryfilter";
		public const string QUERY_FILTERBOOLEANOP	= "queryfilterbooleanop";
		public const string QUERY_FILTERFIELDLIST	= "queryfilterfieldlist";
		public const string QUERY_FILTERFIELDTYPELIST= "queryfilterfieldtypelist";
		public const string QUERY_FILTEROPERATORLIST= "queryfilteroperatorlist";
		public const string QUERY_FILTER_STRING		= "queryfilterstring";
		public const string QUERY_FILTERVALUELIST	= "queryfiltervaluelist";
        public const string QUERY_RESULT_SIZE_LIMIT = "queryresultsizelimit";
        public const string QUERY_NAMELIST = "querynamelist";
		public const string QUERY_OPERATORLIST		= "queryoperatorlist";
		public const string QUERY_OBJECT_TYPE		= "queryobjecttype";
        public const string QUERY_GROUP_TYPE        = "querygrouptype";
        public const string QUERY_PICKLISTMODE = "querypicklistmode";
		public const string QUERY_PRIMARYATTRIBUTE	= "queryprimaryattribute";
		public const string QUERY_PROVIDER			= "queryprovider";
		public const string QUERY_TYPELIST			= "querytypelist";
		public const string QUERY_VALUELIST			= "queryvaluelist";
        public const string QUERY_MAXROWS           = "querymaxrows";
        public const string REFRESH_CACHE           = "refreshcache";
        public const string REGISTRY_COMPUTERNAME = "registrycomputername";
		public const string REGISTRY_FULLKEYNAME	= "registryfullkeyname";
		public const string REGISTRY_ATTR_NAME		= "registryattrname";
		public const string REGISTRY_ATTR_VALUE		= "registryattrvalue";
        public const string AD_DOMAIN               = "addomain";
		public const string AD_USER					= "aduser";
        public const string AD_USER_DN              = "aduserdn";
        public const string AD_USER_DISPLAY_NAME = "aduserdisplayname";
        public const string AD_PROPERTY_NAME        = "adpropertyname";
		public const string AD_PROPERTY_INDEX		= "adpropertyindex";
		public const string AD_PROPERTY_VALUE		= "adpropertyvalue";
        public const string AD_SERVER               = "adserver";
        public const string EMAIL_ADDRESS           = "emailaddress";
		public const string USER_SID				= "usersid";
        public const string OBJECT_GUID = "objectguid";
		public const string FILE_PATH				= "filepath";
		public const string SOURCE                  = "source";
		public const string DEST                    = "dest";
		public const string ACCESS_MASK				= "accessmask";
        public const string OU                      = "ou";
        public const string OU_CANONICAL_NAME       = "ou_canonical_name";
        public const string SINGLE_LEVEL = "singlelevel";
        public const string EMPLOYEEID_LIST         = "employeeidlist";
		public const string EMPLOYEE_NUMBER_LIST    = "employeeNumberList";
		public const string SAMACCOUNT_NAME_LIST    = "samAccountNameList";
        public const string USER_LIST_TO_ADD        = "userlisttoadd";
        public const string USER_LIST_TO_REMOVE     = "userlisttoremove";
        public const string GROUP_OBJECT            = "GroupObject";
        public const string EXCHANGE2010_CLIENT_ACCESS_SERVER = "Exchange2010ClientAccessServer";
        public const string EXCHANGE2010_AUTH_USER = "Exchange2010AuthUser";
        public const string EXCHANGE2010_AUTH_PASSWORD = "Exchange2010AuthPassword";
        public const string EXCHANGE2013_CLIENT_ACCESS_SERVER = "Exchange2013ClientAccessServer";
        public const string EXCHANGE2013_AUTH_USER = "Exchange2013AuthUser";
        public const string EXCHANGE2013_AUTH_PASSWORD = "Exchange2013AuthPassword";
        public const string COMPUTER_NAME = "computername";
        public const string ENABLED                 = "enabled";
        public const string DESCRIPTION             = "description";
        public const string INCLUDE_NESTED_USERS    = "includenestedusers";
        public const string LYNC2010_SERVER         = "Lync2010Server";
        public const string LYNC2010_AUTH_USER      = "Lync2010AuthUser";
        public const string LYNC2010_AUTH_PASSWORD  = "Lync2010AuthPassword";
        public const string LYNC2010_REG_POOL       = "Lync2010RegPool";
        public const string LYNC2010_CONF_POLICY    = "Lync2010ConfPolicy";
        public const string LYNC2010_TELEPHONY_OPTION = "Lync2010TelephonyOption";
        public const string LYNC2010_LINEURI        = "Lync2010LineUri";
        
		public const string MULTIPLE_LOOKUP_MATCH_POLICY = "multipleLookupMatchPolicy";

        public const string EXTENSIONATTRIBUTE1     = "extensionattribute1";
        public const string EXTENSIONATTRIBUTE2     = "extensionattribute2";
        public const string EXTENSIONATTRIBUTE3     = "extensionattribute3";
        public const string EXTENSIONATTRIBUTE4     = "extensionattribute4";
        public const string EXTENSIONATTRIBUTE5     = "extensionattribute5";
        public const string EXTENSIONATTRIBUTE6     = "extensionattribute6";
        public const string EXTENSIONATTRIBUTE7     = "extensionattribute7";
        public const string EXTENSIONATTRIBUTE8     = "extensionattribute8";
        public const string EXTENSIONATTRIBUTE9     = "extensionattribute9";
        public const string EXTENSIONATTRIBUTE10    = "extensionattribute10";
        public const string EXTENSIONATTRIBUTE11    = "extensionattribute11";
        public const string EXTENSIONATTRIBUTE12    = "extensionattribute12";
        public const string EXTENSIONATTRIBUTE13    = "extensionattribute13";
        public const string EXTENSIONATTRIBUTE14    = "extensionattribute14";
        public const string EXTENSIONATTRIBUTE15    = "extensionattribute15";

        public const string AF_BASE_NAME            = "afbasename";
        public const string AF_BASE_UNIT            = "afbaseunit";
        public const string AF_BASE_OU              = "afbaseou";
        public const string AF_COMPUTER_LOCATION    = "afcomputerlocation";
        public const string AF_DOD_BRANCH           = "afdodbranch";
        public const string AF_DOD_MAJCOM           = "afdodmajcom";
        public const string AF_MAILBOX_CATEGORY     = "afmailboxcategory";
        public const string AF_PROV_ACCOUNT_TYPE = "afprovaccounttype";
        public const string AF_REQUEST_SIPRNET_ACCCOUNT = "afrequesetsiprnetaccount";
        public const string AF_USER_OFFICE_SYMBOL = "afuserofficesymbol";
        public const string AF_USER_POSITION_TITLE  = "afuserpositiontitle";
        public const string AF_USER_RANK            = "afuserrank";
        public const string AF_USER_DSN_NUMBER = "afuserdsnnumber";
        public const string AF_USER_FAX_NUMBER      = "afuserfaxnumber";
        public const string AF_USER_IPPHONE_NUMBER  = "afuseripphonenumber";
        public const string AF_USER_COMMERCIAL_NUMBER = "afusercommercialnumber";
        public const string AF_USER_EDI_PI          = "afuseredipi";
        public const string AF_SOURCE_OU            = "afsourceou";
        public const string AF_SOURCE_BASE_OU       = "afsourcebaseou";
        public const string AF_TARGET_OU            = "aftargetou";
    }

}