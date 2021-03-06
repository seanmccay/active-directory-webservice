using System;

namespace RemoteWindowsAgent
{
	public class Command
	{
		public const string ADD_USER_TO_GLOBALGROUP_AD = "add_user_to_globalgroup_ad";
        public const string ADD_USER_TO_GROUP_AD = "add_user_to_group_ad";
        public const string DOES_GROUPING_OBJECT_EXIST = "doesgroupingobjectexist";
		public const string REMOVE_USER_FROM_GLOBALGROUP_AD = "remove_user_from_globalgroup_ad";
		public const string QUERY_GETATTRIBUTES		= "querygetattributes";
		public const string QUERY_GETFILTER			= "querygetfilter";
		public const string QUERY_GETOPERATORS		= "querygetoperators";
		public const string QUERY_GETSEARCHRESULTS	= "querygetsearchresults";
		public const string QUERY_IS_SUPPORTED		= "queryissupported";
		public const string CHECK_FOR_AIMSEXEMPT_MEMBERSHIP = "checkforaimsexemptmembership";
		public const string VERIFY_PASSWORD_LOCAL_WINDOWS = "verifypasswordlocalwindows";
		public const string LASTLOGON_IS_SUPPORTED= "lastlogonissupported";
		public const string LIST_WEB_PATHS_FOR_AIMS = "list_web_paths_for_aims";
		public const string GET_LASTLOPON_SYNC_INTERVAL= "getlastlogonsyncinterval";
		public const string READ_REGISTRY_HKLM= "readregistrylocalmachine";
		public const string WRITE_REGISTRY_HKLM= "writeregistrylocalmachine";
		public const string READ_AD_ATTRIBUTE = "readadattribute";
		public const string WRITE_AD_ATTRIBUTE = "writeadattribute";
		public const string CLEAR_AD_ATTRIBUTE = "clearadattribute";
		public const string GET_USER_BASED_ON_EMAIL_ADDRESS= "getuserbasedonemailaddress";
		public const string GET_USER_BASED_ON_EMPLOYEEID= "getuserbasedonemployeeid";
		public const string GET_USER_BASED_ON_EMPLOYEENUMBER= "getuserbasedonemployeenumber";
		public const string GET_USER_BASED_ON_FIRSTMIDDLELASTNAME= "getuserbasedonfirstmiddlelastname";
		public const string GET_USER_BASED_ON_USERLOGON="getuserbasedonuserlogon";
		public const string GET_USER_BASED_ON_USERID="getuserbasedonuserid";
		public const string GET_USER_BASED_ON_FULLNAME="getuserbasedonfullname";
		public const string GET_SMTP_SUFFIX= "getsmtpsuffix";
		public const string ENUM_USERS_GROUP_MEMBERSHIPS= "enumusersgroupmemberships";
		public const string COPY_FILE = "CopyFile";
		public const string GET_USER_TOKENGROUPS_LIST="getusertokengrouplist";
		public const string CHECK_USER_PERMISSIONS = "checkuserpermissions";
		public const string CHECK_EXCHG_MGMT_TOOLS = "checkexchgmgmttools";
		public const string CHECK_FOR_GROUP_MEMBERSHIP = "checkforgroupmembership";
		public const string GET_UPN_LIST = "getupnlist";
		public const string UPDATE_GROUP_MEMBERS_TABLE = "update_group_members_table";
		public const string UPDATE_USERS_GROUP_TABLE = "update_users_group_table";
		public const string CHANGE_OU = "changeOU";
        public const string GET_ORPHAN_USERLIST = "getorphanuserlist";
        public const string GET_ORPHAN_SINGLE_USER = "getorphansingleuser";
        public const string CHECK_EXCHG2007_SHELL = "checkexchg2007shell";
        public const string CHECK_EXCHG2010_CONFIG_CONNECT = "checkexchg2010configconnect";
        public const string CHECK_EXCHG2010_FULL_CONNECT = "checkexchg2010fullconnect";
        public const string CHECK_EXCHG2013_CONFIG_CONNECT = "checkexchg2013configconnect";
        public const string CHECK_EXCHG2013_FULL_CONNECT = "checkexchg2013fullconnect";
        public const string CHECK_LYNC2010_FULL_CONNECT = "checklync2010fullconnect";
        public const string CHECK_LYNC2010_CONFIG_CONNECT = "checklync2010configconnect";
        public const string GET_LYNC_2010_ACCOUNT_PROPERTIES = "getlync2010accountproperties";
        public const string GET_LYNC2010_REGISTRAR_POOLS = "getlync2010registrarpools";
        public const string GET_LYNC2010_CONF_POLICIES = "getlync2010confpolicies";
        public const string ENUM_USERS_IN_GROUP = "enumusersingroup";
        public const string ENUM_FOREIGNUSERS_IN_GROUP = "enumforeignusersingroup";
        public const string ENUM_DIRECT_REPORTS = "enumdirectreports";
        public const string ENUM_USERS_IN_DOMAIN_USERS = "enumusersindomainusers";
        public const string ENUM_ALL_USERS = "enumallusers";
        public const string ENUM_USERS_WITH_EMPLOYEEIDS = "enumuserswithemployeeids";
		public const string ENUM_USERS_WITH_EMPLOYEE_NUMBERS = "enumUsersWithEmployeeNumbers";
		public const string ENUM_USERS_WITH_SAMACCOUNT_NAME = "enumUsersWithSamAccountName";
        public const string ENUM_USERS_WITH_SPECIFIC_GUIDS = "enumuserswithspecificguids";
        public const string ENUM_USERS_WITH_ATTRIBUTE_VALUE = "enumuserswithattributevalue";
        public const string GET_GROUPS_IN_OU = "getgroupsinou";
        public const string GET_GROUP_TYPE = "getgrouptype";
        public const string DOES_GROUP_CONTAIN_IMMEDIATE_USER = "doesgroupcontainimmediateuser";
        public const string GET_CURRENT_AND_TRUSTED_DOMAINS = "getCurrentAndTrustedDomains";
        public const string CHECK_PASSWORD = "CheckPassword";
        public const string GET_GROUPS_THAT_CONTAIN_GROUP = "getgroupsthatcontaingroup";
        public const string GET_ALL_GROUPS = "get_all_groups";
        public const string GET_TOP_LEVEL_GROUP_MEMBERS = "get_top_level_group_members";
        public const string GET_MEMBER_SERVERS = "get_member_servers";
        public const string GET_MEMBER_SERVERS_MATCHING_PATTERN = "get_member_servers_matching_pattern";
        public const string GET_SHARE_LIST_FROM_SERVER = "get_share_list_from_server";
        public const string IS_OU_VALID = "is_ou_valid";
        public const string GET_OU_LIST = "get_ou_list";
        public const string GET_OU_CANONICAL_NAME = "get_ou_canonical_name";
        public const string GET_EXCHANGE_2010_MAILBOXCOUNT_FOR_EACH_MAILBOXDB = "get_exchange_2010_mailboxcount_for_each_mailboxdb";
        public const string GET_EXCHANGE_2010_MAILBOXCOUNT_FOR_SPECIFIC_MAILBOXDB = "get_exchange_2010_mailboxcount_for_specific_mailboxdb";
        public const string GET_EXCHANGE_2013_MAILBOXCOUNT_FOR_EACH_MAILBOXDB = "get_exchange_2013_mailboxcount_for_each_mailboxdb";
        public const string GET_EXCHANGE_2013_MAILBOXCOUNT_FOR_SPECIFIC_MAILBOXDB = "get_exchange_2013_mailboxcount_for_specific_mailboxdb";
        public const string CREATE_COMPUTER_ACCOUNT = "create_comptuer_account";
        public const string PROVISION_USAF_USER = "provision_usaf_user";
        public const string DEPROVISION_USAF_USER = "deprovision_usaf_user";
		public const string TRANSFER_USAF_USER = "transfer_usaf_user";
        public const string EXCHANGE2007_MAILENABLE_UNIVERSAL_GROUP = "exchange2007_mailenable_universal_group";
        public const string EXCHANGE2010_MAILENABLE_UNIVERSAL_GROUP = "exchange2010_mailenable_universal_group";
        public const string EXCHANGE2013_MAILENABLE_UNIVERSAL_GROUP = "exchange2013_mailenable_universal_group";

    }
}
