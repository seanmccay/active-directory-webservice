using System;
using System.Globalization;
using System.Resources;


namespace RemoteWindowsAgent
{
    /// <summary>
    /// These values are copied over from com.avatier.ps.util.ReturnCodes from the
    /// Unix agent.  This is just a subset of values.  Please make sure that any
    /// value here is added there as well.  This is being used by the Unix wrapper
    /// as well as the LDAP w
    /// </summary>
    public class ReturnCodes {


        public const int SUCCESS=0;

        public const int INTERNAL_ERROR=-1;
        public const int AGENT_NOT_INSTALLED=-2;
        public const int NO_SUCH_USER=-3;
        public const int SERVER_WRITE_FAILED=-4;
        public const int ACCESS_DENIED=-5;
        public const int CONNECTION_REFUSED=-6;
        public const int ACCESS_REFUSED=-7;
        public const int BAD_IP=-8;
        public const int UNKNOWN_FAILURE=-9;
        public const int SERVER_TIMED_OUT=-10;
        public const int BAD_USERID=-11;
        public const int BAD_PASSWORD=-12;
        public const int BAD_XML=-13;
        public const int BAD_DATA_SOURCE_NAME=-14;
        public const int BAD_PROVIDER_NAME=-15;
        public const int UNKNOWN_SERVER_TYPE=-16;
        public const int PASSWORD_CHANGE_FAILED=-17;

        //Added for Account Creator
        public const int UNKNOWN_DATA_TYPE=-18;
        public const int REQUIRED_VALUE_MISSING=-19;
        public const int CANT_CREATE_EXISTING_OBJECT=-20;
		public const int NUMERIC_USERID_ALREADY_EXISTS=-21;
		public const int NO_SUCH_GROUP=-22;
		public const int CANT_FIND_AVAILABLE_NUMERIC_ID=-23;  //mostly for Unix
		public const int SOURCE_ACCOUNT_DOES_NOT_EXIST=-24;
		public const int AC_BAD_PASSWORD_ON_CREATE=-25;
		public const int CANT_CREATE_HOME_DIR=-26;
		public const int CANT_SET_OWNER_ON_HOME_DIR=-27;
		public const int CANT_SET_PARMS_ON_HOME_DIR=-28;
        public const int NO_MSG_ERROR=-30;

		//Added for Enroll & DeEnroll
		public const int INVALID_QA_COUNT=-31;
		public const int LICENSE_COUNT_EXCEEDED=-32;
        public const int NOT_ENROLLED = -33;

		public const int USER_CANNOT_CHANGE_PASSWORD=-40;
		public const int ILLEGAL_USERID=-41;
		public const int NO_SYSTEMS_SELECTED=-42;

		public const int ACTION_CANCELLED=-43;
		public const int BAD_SQL=-44;
		public const int MULTIPLE_GROUPS=-45;
        public const int SKIPPED = -46;

		//More for Account Creator
        public const int CANT_CREATE_EXCHANGE_MAILENABLEDUSER = -49;
		public const int CANT_COPY_GROUP_MEMBERSHIPS=-50;
		public const int CANT_CREATE_EXCHANGE_MAILBOX=-51;
		public const int CANT_CREATE_HOME_SHARE=-52;
		public const int TEMPLATE_NOT_FOUND=-53;

		//IE
		public const int NO_ACTIVE_PRIVS_IN_ROLE=-54;
		public const int INVALID_ITEM=-55;
		public const int NO_USERID_AND_NO_CREATION_ON_PRIVILEGE=-56;
		public const int CANT_ADD_UNHEALTHY_ITEM_TO_CART=-57;
		public const int IE_OBJECT_UNHEALTHY=-58;
        public const int IE_ITEM_ALREADY_IN_CART=-59;

		//AC
		public const int NOT_PROCESSED_BLANK_USERID=-60;
		public const int NOT_PROCESSED_BLANK_PREWIN2K=-61;
		public const int NOT_PROCESSED_BLANK_EXCHANGE_DISPLAY_NAME=-62;
		public const int NOT_PROCESSED_BLANK_EXCHANGE_ALIAS=-63;
		public const int NOT_PROCESSED_BLANK_FULL_NAME=-64;
        public const int SICK_ROLE = -65;

		public const int MANAGER_NOT_FOUND = -66;
		public const int CANNOT_MOVE_MAILBOX = -67;
        public const int IE_ITEM_IN_CART_WOULD_EXCEED_BUDGET = -68;
        public const int CANT_ADD_NONREVIEWABLE_ITEM_TO_CART = -69;
        public const int CANT_LYNC_ENABLE_ACCOUNT = -70;
        public const int MAILBOX_NOT_FOUND = -71;

        public const int OPERATION_NOT_SUPPORTED=-100;
        public const int UNSUPPORTED_ENCRYPTION=-101;
        public const int DATA_UNAVAILABLE=-102;
        public const int REMOTE_SERVER_UNAVAILABLE=-103;
        public const int UNKNOWN_AGENT_FAILURE=-104;
        public const int AGENT_ACCOUNT_MISCONFIGURED=-105;
        public const int AGENT_MISCONFIGURED=-106;
        public const int SERVER_MISCONFIGURED=-107;
        public const int AGENT_INSTALLED_WRONG=-108;
        public const int UNINSTALL_FAILED=-109;
        public const int BLOCKED_ACCOUNT=-110;
        public const int MULTIPLE_ACCOUNTS=-111;
        public const int ACCOUNT_EXPIRED=-112;
        public const int BAD_DICTIONARY=-113;
        public const int ACCOUNT_LOCKED=-114;
        public const int PASSWD_FILE_LOCK_FAILURE=-115;
        public const int CHMOD_FAILURE=-116;
        public const int PASSWD_FILE_FAILED=-117;
        public const int GROUP_OTHER_FILE_FAILED=-118;
        public const int USER_LOGGED_IN=-119;
        public const int UNABLE_DELETE_HOME_DIR=-120;
        public const int ACCOUNT_DISABLED=-121;
        public const int UNABLE_CREATE_HOME_DIR=-122;
		public const int SAP_CHANGE_NOT_ALLOWED=-123;
		public const int UNLOCK_ACCOUNT_NOT_ALLOWED=-124;
		public const int COULD_NOT_ENABLE=-125;
		public const int CAN_NOT_DELETE_PRIMARY_GROUP=-126;
		public const int USERID_PROTECTED=-127;   //Mainframe
		public const int NETAPI_ERROR=-128;   //Mainframe
		public const int PB_SERVICE_DOWN=-129;
		public const int DC_NOT_RESPONDING=-130;
		public const int CIRCULAR_DEPENDENCY_IN_CART=-131;
		public const int ITEM_HAS_MULTIPLE_DEPENDENCIES_IN_CART=-132;
        public const int BAD_HTML = -133;
		public const int SDS_APPLICATION_IN_USE_AND_IS_LOCKED = -134;
		public const int SDS_EMPLOYEE_FUNCTION_INVALID = -135;
		public const int REMOTE_SSH_HOST_NOT_TRUSTED = -136;
		public const int AGENT_ACCOUNT_LACKS_AUTHORITY = -137;
        public const int BAD_DOMAIN = -138;


        //Google Apps
        public const int CANT_REUSE_USERNAME_OF_RECENTLY_DELETED_ACCOUNT = -139;

        //Cart
        public const int TOO_MANY_INSTANCES_OF_PRIVILEGE = -140;
        public const int CART_ALREADY_SUBMITTED = -141;
        public const int PRIVILEGE_NOT_FOUND = -142;
        public const int ROLE_NOT_FOUND = -143;
        public const int MORE_THAN_ONE_PRIV_ON_RESOURCE = -144;

        public const int REQUEST_EXPIRED_BEFORE_FINAL_APPROVAL = -145;
        public const int USER_IS_NOT_ASSIGNED_THIS_PRIVILEGE = -146;
        public const int USER_IS_NOT_ASSIGNED_THIS_ROLE = -147;
        public const int SOD_VIOLATION = -148;

        public const int INVALID_PIN = -150;  //Phone PIN


        public const int UNKNOWN_PASSWORD_SYNTAX=-200;
        public const int BAD_OLD_PW=-201;
        public const int PASSWORD_CHANGE_TOO_SOON=-202;
        public const int PASSWORD_IN_HISTORY=-203;

        public const int REVERSE_PASSWORD=-204;
        public const int NO_MIN_ALPHA=-205;
        public const int CIRCULAR_PASSWORD=-206;
        public const int TOO_SHORT_PASSWORD=-207;
        public const int NO_THREE_CHAR_DIFF=-208;
        public const int TRIVIAL_WORD=-209;
        public const int TOO_LONG_PASSWORD=-210;
        public const int BAD_CHARS=-211;
        public const int NO_MIN_OTHER=-212;
        public const int MAX_REPEAT=-213;
        public const int PASSWORD_EXPIRED=-214;
        public const int PASSWORD_IN_DICTIONARY=-215;
        public const int ONLY_ADMIN_CHANGE=-216;
        public const int NO_MIN_DIFF=-217;
        public const int ADJUNCT_NUMBERS_PASSWORD=-218;
        public const int ADJUNCT_CHARS_PASSWORD=-219;
        public const int REPEATED_CHARS_PASSWORD=-220;
        public const int NUMBER_REQUIRED_PASSWORD=-221;
        public const int REPEATED_POSITION_PASSWORD=-222;
        public const int ILLEGAL_CHARACTERS_PASSWORD=-223;
        public const int NO_SPECIAL_CHARS_PASSWORD=-224;
		public const int UPPERCASE_REQUIRED_PASSWORD=-225;
		public const int INSUFFICIENT_NUM_OF_NUMERIC_CHARS = -226;  
		public const int MIN_LOWERCASE=-227;
		public const int MIN_UPPERCASE=-228;
		public const int TWO_ALPHA_ONE_NUM_SPECIAL=-229;
	
		public const int INSUFFICIENT_NUM_OF_ALPHA_CHARS = -230;  
		public const int INSUFFICIENT_NUM_OF_SPECIAL_CHARS = -231;  
		public const int INSUFFICIENT_NUM_OF_8BIT_CHARS = -232;  
		public const int INSUFFICIENT_NUM_OF_CATEGORIES = -233;  

		//This weird one is because CMS passwords must be globally unique within CMS
		public const int PASSWORD_ALREADY_IN_USE = -234;
        public const int PASSWORD_NOT_SET_BECAUSE_PRIMARY_CONNECTOR_FAILED = -235;

        // AT 
        public const int CANNOT_DELETE_PRI_ACCOUNT_BECAUSE_OF_PENDING_ACTIONS_FOR_SEC_ACCOUNT = -250;
        public const int CANNOT_DELETE_BECAUSE_PRI_ACCOUNT_IS_IN_DELETE_EXEMPT_GROUP = -251;

        //-300 IVR Return Code Errors
        //I did not put the string versions below because these should not end up on the UI
        public const int SOAP_SYSTEM_ERROR=-300;  //means that the IVR system could not talk to the SOAP system
        public const int NON_UNIQUE_PHONE_NUMBER=-301;
        public const int UNFOUND_PHONE_NUMBER=-302;
        public const int WRONG_PIN=-303;
        public const int PARTIAL_RESET_FAILURE=-304;
        public const int COMPLETE_RESET_FAILURE=-305;
        public const int PARTIAL_UNLOCK_FAILURE=-306;
        public const int COMPLETE_UNLOCK_FAILURE=-307;
        //-308 only occurs on the IVR Server
        //-309 only occurs on the IVR Server

		public const int LOTUS_SYSTEM_ERR_REG_MINPSWDCHARS=-401;                        // ERR_REG_MINPSWDCHARS = PKG_REG+235 //842
		public const int LOTUS_SYSTEM_ERR_REG_NO_PASSWORD=-402;                 // ERR_REG_NO_PASSWORD = PKG_REG + 21 // 8213
		public const int LOTUS_SYSTEM_ERR_SECURE_BADPASSWORD=-403;              // ERR_SECURE_BADPASSWORD = PKG_SECURE+8 // 6408
		public const int LOTUS_SYSTEM_ERR_SECURE_ZERO_LENGTH_PW=-404;           // ERR_SECURE_ZERO_LENGTH_PW = PKG_SECURE+26 // 6426
		public const int LOTUS_SYSTEM_ERR_SECURE_MULTI_PSW_REQUIRED=-405;       // ERR_SECURE_MULTI_PSW_REQUIRED = PKG_SECURE+117 // 6517
		public const int LOTUS_SYSTEM_ERR_SECURE_UNSYNCED_PASSWORD=-406;        // ERR_SECURE_UNSYNCED_PASSWORD = PKG_SECURE+238 // 6638
		public const int LOTUS_SYSTEM_ERR_SECURE_PASSWORD_REUSE=-407;           // ERR_SECURE_PASSWORD_REUSE = PKG_SECURE+249 // 6649
		public const int LOTUS_SYSTEM_ERR_SECURE_PW_MISMATCH=-408;              // ERR_SECURE_PW_MISMATCH = PKG_SECURE+250 // 6650
		public const int LOTUS_SYSTEM_ERR_SECURE_NEW_PW_REPEAT=-409;            // ERR_SECURE_NEW_PW_REPEAT = PKG_SECURE+253 // 6653
		public const int LOTUS_SYSTEM_ERR_SECURE_NEW_PW_IN_FUTURE=-410;         // ERR_SECURE_NEW_PW_IN_FUTURE = PKG_SECURE+254 // 6654
		public const int LOTUS_SYSTEM_ERR_SECURE_CHANGE_SIG_INVALID=-411;               // ERR_SECURE_CHANGE_SIG_INVALID = PKG_SECURE+255 // 6655
		public const int LOTUS_SYSTEM_ERR_SECURE_BAD_LENGTH_PW=-412;            // ERR_SECURE_BAD_LENGTH_PW = PKG_SECURE2+2 // 5602
		public const int LOTUS_SYSTEM_ERR_BSAFE_WRITEPROTECTED=-413;            // ERR_BSAFE_WRITEPROTECTED = PKG_BSAFE+91 // 5979
		public const int LOTUS_SYSTEM_ERR_BSAFE_NULLPARAM=-414;					// ERR_BSAFE_NULLPARAM = PKG_BSAFE+37 // 5925


		public const int LOTUS_NOTES_NOT_INSTALLED_OR_CONFIGURED	= -421;
		public const int LOTUS_DOMINO_NOT_INSTALLED_OR_CONFIGURED	= -422;
		public const int DOMINO_SERVER_IS_NOT_RUNNING				= -423;
		public const int SYSTEM_MAY_NOT_BE_UP_AND_RUNNING			= -424;
		public const int NOTES_DCOM_CLIENT_MACHINE_UNAVAILABLE		= -425;
		public const int NOTES_EXTENSION_MANAGER_SETUP_FAILED		= -426;
		public const int NOTES_DATABASE_CONNECTION_FAILED_EX_PASSWORD_WRONG	= -427;

		public const int NO_SUCH_OU=-430;

        public const int GROUP_IS_NOT_SECURITY_TYPE = -431;
        public const int GROUP_NOT_FOUND = -432;

        public const int SMS_CODE_HAS_EXPIRED= -433;
        public const int SMS_CODE_DOES_NOT_MATCH = -434;

        public const int QUESTION_CHALLENGE_HAS_EXPIRED = -435;
        public const int QUESTION_CHALLENGE_INVALID_QUESTION_SEQ= -436;
        public const int QUESTION_CHALLENGE_INVALID_ANSWER_SEQ= -437;

        public const int SMS_USER_HAS_NO_MOBILE_NUMBER = -438;

		public const int VMS_NO_LICENSE=-450;                                   //VMS does not have enough user licenses


		//-500 RSA Admin Return Code Errors
		public const int RSAADMIN_API_NO_TOKEN		= -500;
		public const int RSAADMIN_API_MULTI_TOKENS	= -501;

        // -550 Added for orphan handling
        public const int BAD_PARAMETER_VALUE        = -550;
        public const int USER_DB_UNAVAILABLE        = -551;
        public const int USER_DB_EXPIRED            = -552;

        public const int NO_EXPIRATION_DATE=101;
        public const int NO_LAST_PWD_CHANGE_DATE=102;
        public const int NO_PASSWORD_ALLOW_CHANGE_TIME_SET=103;
        public const int NO_PREVIOUS_SIGNON_DATE=104;
        public const int SUCCESS_GRANTING_NEEDED = 105;
        public const int SUBMITTED_ASYNCHRONOUSLY = 106;
        public const int SUCCESS_RECEIPT_NEEDED = 107;
        public const int GRANTOR_REJECTED = 108;
        public const int RECIPIENT_REJECTED = 109;
        public const int GRANTOR_INACTIVITY_EXPIRED = 110;
        //300 IVR Return Codes - successes
        //I did not put the string versions below because these should not end up on the UI
        //public const int VALID_PHONE_NUMBER=300;  //should this be zero?
        //public const int VALID_PIN=301;
        //public const int RESET_SUCCESS=302;
        //public const int UNLOCK_SUCCESS=303;

		// N O T E :
		// The GERuleExecutionHistory class defines four private constants 
		// (FAILURE_RANGE_LOW_END, FAILURE_RANGE_HIGH_END, COMPLETED_WITH_ERRORS_RANGE_LOW_END,
		// and COMPLETED_WITH_ERRORS_RANGE_HIGH_END) the MUST correspond to the ranges
		// used below. If you modify these ranges, you MUST modify the constants (or redesign
		// the code that uses them)
        // Group Enforcer rule execution failure (600-629)
        public const int GE_RESULT_GENERAL_FAILURE = -600;
        public const int GE_RESULT_RULE_IS_DISABLED = -601;
        public const int GE_RESULT_EVAL_SOURCE_NOT_FOUND = -602;
        public const int GE_RESULT_EVAL_SOURCE_NOT_ENABLED = -603;
        public const int GE_RESULT_TARGET_GROUP_NOT_FOUND = -604;
        public const int GE_RESULT_TARGET_GROUP_UPDATE_FAILURE = -605;
        public const int GE_RESULT_EVAL_SOURCE_HAD_EXCEPTION = -606;
		public const int GE_RESULT_RULE_IS_ALREADY_EXECUTING = -607;
        public const int GE_RESULT_FAILED_TO_UPDATE_RULE = -608;

        // GE rule execution partial failure - completed with errors (630-649)
        public const int GE_RESULT_EXTERNAL_ID_NOT_FOUND_CONNECTOR = -630;
        public const int GE_RESULT_EXTERNAL_ID_FOUND_MORE_THAN_ONCE_IN_CONNECTOR = -631;


        // Exchange 2007
        public const int ERROR_CREATING_EXCHANGE2007_MAILBOX = -700;
        public const int COULD_NOT_DETERMINE_STATUS_OF_EXCHANGE2007_MAILSTORE = -701;
        public const int COULD_NOT_EXCHANGE2007_MAILENABLE_EXISTING_UNIVERSAL_GROUP = -702;

        // Exchange 2010 
        public const int ERROR_CREATING_EXCHANGE2010_MAILBOX = -800;
        public const int COULD_NOT_DETERMINE_STATUS_OF_EXCHANGE2010_MAILBOXDATABASE = -801;
        public const int COULD_NOT_DETERMINE_MAILBOXDATABASES_FOR_EXCHANGE2010_DAG = -802;
        public const int COULD_NOT_CREATE_NEW_EXCHANGE2010_MAILENABLED_DISTRIBUTION_GROUP = -803;
        public const int COULD_NOT_EXCHANGE2010_MAILENABLE_EXISTING_UNIVERSAL_GROUP = -804;

        // Exchange 2013 
        public const int ERROR_CREATING_EXCHANGE2013_MAILBOX = -820;
        public const int COULD_NOT_DETERMINE_STATUS_OF_EXCHANGE2013_MAILBOXDATABASE = -821;
        public const int COULD_NOT_DETERMINE_MAILBOXDATABASES_FOR_EXCHANGE2013_DAG = -822;
        public const int COULD_NOT_CREATE_NEW_EXCHANGE2013_MAILENABLED_DISTRIBUTION_GROUP = -823;
        public const int COULD_NOT_EXCHANGE2013_MAILENABLE_EXISTING_UNIVERSAL_GROUP = -824;

        // USAF
        public const int AF_COULD_NOT_FIND_EXCHANGE_GEOREFCODE_FOR_BASE = -900;
        public const int AF_COULD_NOT_FIND_EXCHANGE2007_MAILSTORE_FOR_BASE = -901;
        public const int AF_COULD_NOT_FIND_EXCHANGE2010_MAILBOXDB_FOR_BASE = -902;
        public const int AF_COULD_NOT_FIND_BASE_REFERENCE_ENTRY = -903;
        public const int AF_COULD_NOT_UPDATE_MAILBOX_MOVE_TRIGGER_AD_USER_ATTRIBUTE = -904;
        public const int AF_BASE_NOT_CONFIGURED_FOR_MAILBOX_CREATION = -905;

		//Rename
		public const int EXCHANGE_RENAME_FAILED = -1100;

		/// <summary>
		/// Returns true if a given return code is an error and is
		/// retryable. Returns false if the return code is not an error
		/// or is a permanent error. This method is intended to be 
		/// typically called just after a call to IsError() returns 
		/// true.
		/// </summary>
		/// <param name="returnCode"></param>
		/// <returns></returns>
		public static bool IsRetryableError(int returnCode)
		{
			bool isRetryable =
				(returnCode == REMOTE_SERVER_UNAVAILABLE) ||
				(returnCode == SERVER_TIMED_OUT) ||
				(returnCode == CONNECTION_REFUSED);

			return isRetryable;
		}

		/// <summary>
		/// Returns true if a returnCode indicates an error
		/// </summary>
		/// <param name="returnCode"></param>
		/// <returns></returns>
		public static bool IsError(int returnCode)
		{
			bool isError = (returnCode < 0);
			return isError;
		}

    }
}
