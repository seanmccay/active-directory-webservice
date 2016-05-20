using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Text;
using System.Security.Principal;

namespace RemoteWindowsAgent
{
    public class WindowsAgent
    {
        #region STATE AND CONSTRUCTOR

        private String securityKey;
        private String domain;
        private String defaultOU;
        private String serviceAccountName;
        private String serviceAccountPassword;
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger("WindowsAgent");
        private bool allowDebug = log.IsDebugEnabled;

        //constructor - load in config items
        public WindowsAgent()
        {
            this.securityKey = AgentConfigurator.ReadSetting("security");
            this.domain = AgentConfigurator.Decrypt(this.securityKey, AgentConfigurator.ReadByteSetting("domain"));
            this.defaultOU = AgentConfigurator.Decrypt(this.securityKey, AgentConfigurator.ReadByteSetting("defaultOU"));
            this.serviceAccountName = AgentConfigurator.Decrypt(this.securityKey, AgentConfigurator.ReadByteSetting("username"));
            this.serviceAccountPassword = AgentConfigurator.Decrypt(this.securityKey, AgentConfigurator.ReadByteSetting("password"));
        }

        #endregion


        #region PASSWORD STATION

        public int getAccountInfo(String user, out List<attribute> attributes)
        {
            try
            {
                if (allowDebug) log.Debug("Getting user object for - " + user);
                UserPrincipalExtended upx = getUser(user, null);                


                //make sure user exists
                if (upx == null)
                {
                    log.Info("Could not find user - " + user + ". Returning failure xml");
                    attributes = null;
                    return ReturnCodes.NO_SUCH_USER;
                }

                if (allowDebug) log.Debug("Found user - " + user + ". Getting attribute values.");
                List<attribute> tempAttributes = new List<attribute>();
                tempAttributes.Add(new attribute { name = "Full Name", dataType = "string", value = upx.FullName });
                DateTime expirationDate = upx.AccountExpirationDate.GetValueOrDefault();
                if (expirationDate.ToShortDateString().Equals("1/1/0001"))
                {
                    tempAttributes.Add(new attribute { name = "Account Expires", dataType = "string", value = "Never" });
                }
                else
                {
                    string value = expirationDate.ToUniversalTime().ToShortDateString() + " " +
                        expirationDate.ToUniversalTime().ToLongTimeString();
                    tempAttributes.Add(new attribute { name = "Account Expires", dataType = "time", value = convertToUnixTimestamp(value) });
                }
                tempAttributes.Add(new attribute { name = "Account Disabled", dataType = "boolean", value = (!upx.Enabled).ToString() });
                tempAttributes.Add(new attribute { name = "Account Locked", dataType = "boolean", value = upx.IsAccountLockedOut().ToString() });
                if(upx.PasswordNeverExpires)
                {
                    tempAttributes.Add(new attribute { name = "Password Expires", dataType = "string", value = "Never" });
                }
                else
                {
                   tempAttributes.Add(new attribute { name = "Password Expires", dataType = "string", value = getPwdExpirationDate(user) });
                }

                DateTime changeDate = upx.LastPasswordSet.GetValueOrDefault().ToUniversalTime();
                if(changeDate.ToShortDateString().Equals("1/1/0001") || String.IsNullOrEmpty(changeDate.ToShortDateString()))
                {
                    tempAttributes.Add(new attribute { name = "Password Age", dataType = "string", value = "Never Set" });
                }
                else
                {
                    TimeSpan pwdAge = DateTime.Now.ToUniversalTime() - changeDate;
                    int days = pwdAge.Days;
                    int hours = pwdAge.Hours;
                    int mins = pwdAge.Minutes;
                    tempAttributes.Add(new attribute { name = "Password Age", dataType = "string", value = days + " Days, " + hours + " Hours, " + mins + " Minutes" });
                }
                tempAttributes.Add(new attribute { name = "Display Name", dataType = "string", value = upx.DisplayName });
                
                DateTime lastLogon = upx.RealLastLogon.GetValueOrDefault();
                if(lastLogon.ToShortDateString().Equals("1/1/0001"))
                {
                    tempAttributes.Add(new attribute { name = "LastLogon", dataType = "string", value = "Never logged on" });
                }
                else
                {
                    string value = lastLogon.ToUniversalTime().ToShortDateString() + " " +
                        lastLogon.ToUniversalTime().ToLongTimeString();
                    tempAttributes.Add(new attribute { name = "LastLogon", dataType = "time", value =  convertToUnixTimestamp(value) });
                }

                DateTime lastLogonTimestamp = upx.LastLogon.GetValueOrDefault();
                if(lastLogonTimestamp.ToShortDateString().Equals("1/1/0001"))
                {
                    tempAttributes.Add(new attribute { name = "LastLogonTimeStamp", dataType = "string", value = "Never logged on" });
                }
                else
                {
                    string value = lastLogonTimestamp.ToUniversalTime().ToShortDateString() + " " +
                        lastLogonTimestamp.ToUniversalTime().ToLongTimeString();
                    tempAttributes.Add(new attribute { name = "LastLogonTimeStamp", dataType = "time", value = convertToUnixTimestamp(value) });
                }                                

                log.Info("Got attribute values: \r\n" + listAttributes(tempAttributes));
                attributes = tempAttributes;
                return ReturnCodes.SUCCESS;
            }
            catch (Exception e)
            {
                log.Error("Caught error while getting information for the user (" + user + ") :" + e.StackTrace , e);
                attributes = null;
                return ReturnCodes.INTERNAL_ERROR;
            }
        }

        public int resetPassword(String user, String password)
        {
            try
            {
                if (allowDebug) log.Debug("Getting user object for - " + user);
                UserPrincipalExtended userPrincipal = getUser(user, null);
                if (userPrincipal == null) return ReturnCodes.NO_SUCH_USER;
                userPrincipal.SetPassword(password);
                userPrincipal.Save();

                // verify change occured
                if (!userPrincipal.VerifyPassword(password)) return ReturnCodes.INTERNAL_ERROR;
                userPrincipal.Dispose();
                return ReturnCodes.SUCCESS;
            }
            catch (Exception e)
            {
                log.Warn("Error caught while resetting password for the user (" + user + ")", e);
                return ReturnCodes.INTERNAL_ERROR;
            }
        }

        public int verifyPassword(String user, String password)
        {
            try
            {
                // check that user exists
                if (allowDebug) log.Debug("Getting user object for - " + user);
                UserPrincipalExtended userPrincipal = getUser(user, null);
                if (userPrincipal == null) return ReturnCodes.NO_SUCH_USER;
                userPrincipal.Dispose();


                // verify credentials
                if (userPrincipal.VerifyPassword(password))
                {
                    log.Info("Verified password for user (" + user + ")");
                    return ReturnCodes.SUCCESS;
                }
                else
                {
                    log.Info("Password was not valid.");
                    return ReturnCodes.INTERNAL_ERROR;
                }
            }
            catch (Exception e)
            {
                log.Warn("Error caught while verifying password for user (" + user + ")", e);
                return ReturnCodes.INTERNAL_ERROR;
            }
        }

        public int changePassword(String user, String oldPassword, String password)
        {
            try
            {
                //make sure user exists
                if (allowDebug) log.Debug("Getting user object for - " + user);
                UserPrincipalExtended userPrincipal = getUser(user, null);
                if (userPrincipal == null) return ReturnCodes.NO_SUCH_USER;

                if (allowDebug) log.Debug("Changing password for user (" + user + ")");
                userPrincipal.ChangePassword(oldPassword, password);
                userPrincipal.Save();
                userPrincipal.Dispose();

                if (!userPrincipal.VerifyPassword(password)) return ReturnCodes.INTERNAL_ERROR;

                log.Info("Changed the password for user (" + user + ")");
                return ReturnCodes.SUCCESS;
            }
            catch (Exception e)
            {
                log.Warn("Error caught while changing password for user (" + user + ")", e);
                return ReturnCodes.INTERNAL_ERROR;
            }
        }

        public int unlock(String user)
        {
            try
            {
                if (allowDebug) log.Debug("Getting user object for - " + user);
                UserPrincipal userPrincipal = getUser(user, null);
                int result = UnlockAccount(userPrincipal);
                if (userPrincipal.IsAccountLockedOut()) return ReturnCodes.INTERNAL_ERROR;

                return result;
            }
            catch (Exception e)
            {
                log.Warn("Error caught while unlocking user (" + user + ")", e);
                return ReturnCodes.INTERNAL_ERROR;
            }
        }

        #endregion


        #region ACCOUNT TERMINATOR

        public int disable(String user)
        {
            try
            {
                if (allowDebug) log.Debug("Getting user object for user - " + user);
                UserPrincipal userPrincipal = getUser(user, null);

                //make sure user exists
                if (userPrincipal == null) return ReturnCodes.NO_SUCH_USER;

                if(allowDebug) log.Debug("Attempting disable for user (" + user + ")");
                userPrincipal.Enabled = false;
                userPrincipal.Save();
                if ((Boolean)userPrincipal.Enabled) return ReturnCodes.INTERNAL_ERROR;
                userPrincipal.Dispose();

                log.Info("User disabled");
                return ReturnCodes.SUCCESS;
            }
            catch (Exception e)
            {
                log.Warn("Error caught while disabling user (" + user + ")", e);
                return ReturnCodes.INTERNAL_ERROR;
            }
        }

        public int enable(String user)
        {
            try
            {
                if (allowDebug) log.Debug("Getting user object for user - " + user);
                UserPrincipal userPrincipal = getUser(user, null);

                //make sure user exists
                if (userPrincipal == null) return ReturnCodes.NO_SUCH_USER;

                if (allowDebug) log.Debug("Attempting enable for user (" + user + ")");
                userPrincipal.Enabled = true;
                userPrincipal.Save();
                if (!(Boolean)userPrincipal.Enabled) return ReturnCodes.INTERNAL_ERROR;
                userPrincipal.Dispose();

                log.Info("User enabled");
                return ReturnCodes.SUCCESS;
            }
            catch (Exception e)
            {
                log.Warn("Error caught while enabling user (" + user + ")", e);
                return ReturnCodes.INTERNAL_ERROR;
            }
        }

        public int delete(String user)
        {
            try
            {
                if(allowDebug) log.Debug("Getting user object for - " + user);
                UserPrincipal userPrincipal = getUser(user, null);

                //make sure user exists
                if (userPrincipal == null) return ReturnCodes.NO_SUCH_USER;

                //perform deletion - saving or disposing of the object will throw an error. I guess deleting gets rid of the instance of the object as well.
                if(allowDebug) log.Debug("Attempting to delete user");
                userPrincipal.Delete();

                //try to get user again
                UserPrincipal userPrincipal1 = getUser(user, null);
                if (userPrincipal1 != null)
                {
                    userPrincipal1.Dispose();
                    log.Info("Delete failed");
                    return ReturnCodes.INTERNAL_ERROR;
                }

                log.Info("Deleted user");
                return ReturnCodes.SUCCESS;
            }
            catch (Exception e)
            {
                log.Warn("Error caught while deleting user (" + user + ")", e);
                return ReturnCodes.INTERNAL_ERROR;
            }
        }

        #endregion


        #region ACCOUNT CREATOR        

        public int accountExists(String user)
        {
            try
            {
                if (allowDebug) log.Debug("Getting user object for - " + user);
                UserPrincipal userPrincipal = getUser(user, null);
                if (userPrincipal == null)
                {
                    log.Info("User does not exist");
                    return ReturnCodes.NO_SUCH_USER;
                }
                else
                {
                    log.Info("User exists");
                    return ReturnCodes.SUCCESS;
                }
            }
            catch (Exception e)
            {
                log.Warn("Error caught while trying to find user object for (" + user + ")", e);
                return ReturnCodes.INTERNAL_ERROR;
            }
        }

        public int create(Dictionary<String,String> attributePairs)
        {
            try
            {               
                string username = attributePairs["username"];
                log.Info("User to create = " + username);

                string targetOU;

                if (attributePairs.ContainsKey("targetou"))
                {
                    targetOU = attributePairs["targetou"];
                    attributePairs.Remove("targetou");
                }
                else
                {
                    targetOU = defaultOU;
                }

                log.Info("Target OU for create = " + targetOU);

                //check to see if user already exists.
                if (allowDebug) log.Debug("Checking if user already exists");
                UserPrincipal userCheck = getUser(username, targetOU);
                if (userCheck != null)
                {
                    log.Warn("User already exists.");
                    userCheck.Dispose();
                    return ReturnCodes.INTERNAL_ERROR;
                }
               
                //check for source account
                bool sourceProvided;
                string sourceAccount = attributePairs["sourceaccount"];
                sourceProvided = String.IsNullOrEmpty(sourceAccount) ? false : true;
                
                //create the new user principal object
                if (allowDebug) log.Debug("Building new UserPrincipal object for - " + username);
                UserPrincipalExtended user = new UserPrincipalExtended(getContext(targetOU));        
                
                if (sourceProvided)
                {
                    UserPrincipalExtended source = getUser(sourceAccount, targetOU);
                    if (allowDebug) log.Debug("Looking for source account - " + sourceAccount);
                    if (source == null) return ReturnCodes.INTERNAL_ERROR;

                    log.Info("Source account to copy from = " + sourceAccount);

                    //copy from source UserPrincipal
                    if (allowDebug) log.Debug("Copying source account properties to User Principal");
                    user.SamAccountName = username;
                    user.SetPassword(attributePairs["password"]);
                    user.AccountExpirationDate = source.AccountExpirationDate;
                    user.Enabled = source.Enabled;
                    user.HomeDirectory = source.HomeDirectory;
                    user.HomeDrive = source.HomeDrive;
                    user.PasswordNeverExpires = source.PasswordNeverExpires;
                    user.PasswordNotRequired = source.PasswordNotRequired;
                    user.PermittedLogonTimes = source.PermittedLogonTimes;
                    user.UserCannotChangePassword = source.UserCannotChangePassword;
                    user.Save();

                    //copy group memberships
                    List<String> sourceGroups = source.MemberOf;
                    foreach (String group in sourceGroups)
                    {
                        if (allowDebug) log.Debug("Copying user to source group: " + group);
                        user.AddToGroup(group);
                    }

                    //start assigning given values from attributePairs. Will overwrite source properties.
                    if (allowDebug) log.Debug("Assigning given values to UserPrincipal");
                    assignAttributes(user, attributePairs);

                    //check to expire password
                    if (attributePairs["usermustchangepasswordnextlogon"].ToLower().Equals("true"))
                    {
                        if (allowDebug) log.Debug("User flagged for password change at next logon.");
                        user.ExpirePasswordNow();
                        user.Save();
                    }

                    //save and dispose
                    user.Save();

                    source.Dispose();
                    user.Dispose();

                    log.Info("User creation from template user successful.");
                    return ReturnCodes.SUCCESS;
                }
                else
                {
                    if (allowDebug) log.Debug("Assigning given values to UserPrincipal");
                    assignAttributes(user, attributePairs);

                    //check to expire password
                    if (attributePairs["usermustchangepasswordnextlogon"].ToLower().Equals("true"))
                    {
                        if (allowDebug) log.Debug("User flagged for password change at next logon.");
                        user.ExpirePasswordNow();
                        user.Save();
                    }

                    //create directory entry now that UserPrincipal is persisted
                    DirectoryEntry userEntry = (DirectoryEntry)user.GetUnderlyingObject();

                    foreach (KeyValuePair<string, string> attribute in attributePairs)
                    {
                        if (userEntry.Properties.Contains(attribute.Key))
                        {
                            if (allowDebug) log.Debug("Property: " + attribute.Key + " given value: " + attribute.Value);
                            userEntry.Properties[attribute.Key].Value = attribute.Value;
                        }
                    }

                    //save directoryentry object
                    userEntry.CommitChanges();

                    //dispose of objects
                    user.Dispose();
                    userEntry.Dispose();

                    log.Info("User creation successful");
                    return ReturnCodes.SUCCESS;
                }
            }
            catch (Exception e)
            {
                log.Warn("Error while creating user", e);
                return ReturnCodes.INTERNAL_ERROR;
            }
        }

        #endregion


        #region GROUPING OBJECT METHODS

        public int doesGroupingObjectExist(String groupingObjectKey)
        {
            try
            {
                using (var ctx = new PrincipalContext(ContextType.Domain, domain))
                {
                    var group = GroupPrincipal.FindByIdentity(ctx, groupingObjectKey);
                    if (group == null) return ReturnCodes.NO_SUCH_GROUP;

                    return ReturnCodes.SUCCESS;
                }
            }
            catch (Exception e)
            {
                log.Error("Error caught while checking for group: ", e);
                return ReturnCodes.INTERNAL_ERROR;
            }
        }

        public int addUserToGroupingObject(String user, String groupingObjectKey)
        {
            try
            {
                int exists = accountExists(user);
                if (exists == ReturnCodes.NO_SUCH_USER)
                {
                    log.Warn("User does not exist.");
                    return ReturnCodes.NO_SUCH_USER;
                }
                if (allowDebug) log.Debug("Adding user (" + user + ") to group (" + groupingObjectKey + ").");

                UserPrincipalExtended userPrincipal = getUser(user, defaultOU);
                int result = userPrincipal.AddToGroup(groupingObjectKey);

                return result;
            }
            catch (Exception e)
            {
                log.Error("Error caught while adding user to group: ", e);
                return ReturnCodes.INTERNAL_ERROR;
            }
        }
        
        public int removeUserFromGroupingObject(String user, String groupingObjectKey)
        {
            try
            {
                int exists = accountExists(user);
                if (exists == ReturnCodes.NO_SUCH_USER)
                {
                    log.Warn("User does not exist.");
                    return ReturnCodes.NO_SUCH_USER;
                }
                if (allowDebug) log.Debug("Removing user (" + user + ") from group (" + groupingObjectKey + ").");

                UserPrincipalExtended userPrincipal = getUser(user, defaultOU);
                int result = userPrincipal.RemoveFromGroup(groupingObjectKey);

                return result;
            }
            catch (Exception e)
            {
                log.Error("Error caught while adding user to group: ", e);
                return ReturnCodes.INTERNAL_ERROR;
            }
        }

        public List<String> getGroupingObjectsForUser(String user)
        {
            try
            {
                UserPrincipalExtended userPrincipal = getUser(user, defaultOU);
                List<string> groups = userPrincipal.MemberOf;
                if (allowDebug) log.Debug("Building list of groups...");
                for (int i = 0; i < groups.Count; i++)
                {
                    int comma = groups[i].IndexOf(',') - 3;
                    groups[i] = groups[i].Substring(3, comma);
                }

                int rid = userPrincipal.PrimaryGroupRID;
                switch (rid)
                {
                    case -1:
                        break;
                    case 500:
                        groups.Insert(0, "Administrator");
                        break;
                    case 501:
                        groups.Insert(0, "Guest");
                        break;
                    case 512:
                        groups.Insert(0, "Domain Admins");
                        break;
                    case 513:
                        groups.Insert(0, "Domain Users");
                        break;
                    case 514:
                        groups.Insert(0, "Domain Guests");
                        break;
                    default:
                        PrincipalContext ctx = new PrincipalContext(ContextType.Domain, domain);
                        DirectoryEntry root = new DirectoryEntry("LDAP://" + domain);
                        byte[] sidBytes = (byte[])root.Properties["objectSid"].Value;
                        var sid = new SecurityIdentifier(sidBytes, 0);
                        GroupPrincipal gp = GroupPrincipal.FindByIdentity(ctx, IdentityType.Sid, sid.ToString() + "-" + rid);
                        groups.Insert(0, gp.Name);
                        break;
                }

                return groups;
            }
            catch (Exception e)
            {
                log.Error("Error caught while getting groups for user: ", e);
                return null;
            }
        }

        public List<String> getGroupMembers(String groupKey)
        {
            PrincipalContext ctx = new PrincipalContext(ContextType.Domain, this.domain);
            GroupPrincipal group = GroupPrincipal.FindByIdentity(ctx, groupKey);
            if (group == null)
            {
                log.Warn("Group was not found.");
                return null;
            }
            List<String> members = new List<String>();
            if (allowDebug) log.Debug("Building list of members...");
            foreach (Principal user in group.GetMembers())
            {
                members.Add(user.SamAccountName);
            }

            return members;
        }

        #endregion


        #region GET

        public ReturnValues get(String dataType, ReturnValues qualifier, ReturnValues returnAttributes)
        {
            try
            {
                ReturnValue rv = null;
                ReturnValues rvsResult = new ReturnValues();

                Hashtable qualTable = qualifier.GetHashtable();
                Hashtable attrTable = returnAttributes.GetHashtable();

                string objectType = (string)attrTable[Parameter.QUERY_OBJECT_TYPE];
                if (objectType == null)
                {
                    objectType = QueryType.USER;
                }

                string strCommand = dataType;

                // return query search results based on what user specifies in lookup/query tool UI
                #region get query search results
                log.Info("Getting search results for " + objectType);
                if (strCommand == Command.QUERY_GETSEARCHRESULTS)
                {
                    #region querytype user
                    if (objectType == QueryType.USER)
                    {
                        int nRetVal = 0;
                        List<lookupUser> foundUsers = new List<lookupUser>();
                        List<String> samAccountNameList = new List<String>();
                        List<String> fullNameList = new List<String>();

                        // Look up users here
                        this.GetAllUsers(ref foundUsers, ref nRetVal);

                        if (nRetVal < 0)
                        {
                            return ReturnValues.GetSimpleReturnValues(ReturnCodes.UNKNOWN_FAILURE, Resources.NO_USERS_FOUND);
                        }
                        else
                        {

                            if (foundUsers.Count > 0)
                            {
                                List<String> tempSamAccountNameList = new List<String>();
                                List<String> tempFullNameList = new List<String>();

                                foreach (lookupUser user in foundUsers)
                                {
                                    tempSamAccountNameList.Add(user.samAccountName);
                                    tempFullNameList.Add(user.fullName);
                                }

                                // by default boolean operator is OR.
                                string strBooleanOp = (string)attrTable[Parameter.QUERY_FILTERBOOLEANOP];
                                if (strBooleanOp == null)
                                    strBooleanOp = QueryClause.QUERY_BOOLEAN_OP_OR;

                                //currently supporting only OR operator
                                /*if (strBooleanOp == QueryClause.QUERY_BOOLEAN_OP_AND)
                                    return ReturnValues.GetSimpleReturnValues(ReturnCodes.UNKNOWN_DATA_TYPE, rm.GetString(Resources.OPERATION_NOT_SUPPORTED));*/

                                // strBooleanOp = QueryClause.QUERY_BOOLEAN_OP_OR;

                                IList arFilterFieldName = (IList)qualTable[Parameter.QUERY_FILTERFIELDLIST];

                                // all are strings so not taking into account.
                                // IList arFilterFieldType = (IList)qualTable[Parameter.QUERY_FILTERFIELDTYPELIST];

                                IList arFilterOperator = (IList)qualTable[Parameter.QUERY_FILTEROPERATORLIST];
                                // FilterOperator can have following values "contains", "startswith", "endswith", "isexactly", "isnot"

                                IList arFilterValue = (IList)qualTable[Parameter.QUERY_FILTERVALUELIST];

                                if (arFilterFieldName.Count == 0)
                                {
                                    samAccountNameList = tempSamAccountNameList;
                                    fullNameList = tempFullNameList;
                                }
                                else
                                {
                                    // Currently all list are ORed
                                    FilterUsersList(tempSamAccountNameList, tempFullNameList, foundUsers, arFilterFieldName, arFilterOperator, arFilterValue, strBooleanOp, ref samAccountNameList, ref fullNameList);
                                }
                            }

                            rv = new ReturnValue(0);
                            rv.SetName("username");
                            rv.SetList(samAccountNameList);
                            rvsResult.AddReturnValue(rv);

                            rv = new ReturnValue(0);
                            rv.SetName("displayName");
                            rv.SetList(fullNameList);
                            rvsResult.AddReturnValue(rv);

                        }
                    }

                    #endregion

                    #region querytype group
                    if (objectType == QueryType.GROUP)
                    {
                        int nRetVal = 0;
                        string strGroups = "";
                        string strDescriptions = "";
                        string strTypes = "";
                        string strReserved = "";
                        List<String> GroupsList = new List<String>();
                        List<String> DescriptionsList = new List<String>();

                        this.GetAllGroups(ref strGroups, ref strDescriptions, ref strTypes, ref strReserved, ref nRetVal);

                        if (nRetVal < 0)
                        {
                            return ReturnValues.GetSimpleReturnValues(ReturnCodes.NO_MSG_ERROR, Resources.NO_SUCH_GROUP);
                        }
                        else
                        {

                            if (strGroups.Length > 0)
                            {
                                string[] indices;
                                indices = strGroups.Split(new char[] { ',' });
                                for (int i = 0; i < indices.Length; i++)
                                {
                                    GroupsList.Add(indices[i].ToString());
                                }

                                indices = strDescriptions.Split(new char[] { ',' });
                                for (int i = 0; i < indices.Length; i++)
                                {
                                    DescriptionsList.Add(indices[i].ToString());
                                }


                            }

                            rv = new ReturnValue(0);
                            rv.SetName("cn");
                            rv.SetList(GroupsList);
                            rvsResult.AddReturnValue(rv);


                            rv = new ReturnValue(0);
                            rv.SetName("description");
                            rv.SetList(DescriptionsList);
                            rvsResult.AddReturnValue(rv);

                        }
                    }
                    #endregion

                    #region querytype computer
                    if (objectType == QueryType.COMPUTER)
                    {
                        int nRetVal = 0;
                        string strComputers = "";
                        string strDistinguishedName = "";
                        string strReserved1 = "";
                        string strReserved2 = "";
                        List<String> ComputersList = new List<String>();
                        List<String> DistinguishedNameList = new List<String>();

                        // Look up users here
                        this.GetAllComputers(ref strComputers, ref strDistinguishedName, ref strReserved1, ref strReserved2, ref nRetVal);

                        if (nRetVal < 0)
                        {
                            return ReturnValues.GetSimpleReturnValues(ReturnCodes.UNKNOWN_FAILURE, Resources.NO_USERS_FOUND);
                        }
                        else
                        {

                            if (strComputers.Length > 0)
                            {
                                List<String> tempComputersList = new List<String>();
                                List<String> tempDistinguishedNameList = new List<String>();

                                string[] indices;
                                indices = strComputers.Split(new char[] { ',' });
                                for (int i = 0; i < indices.Length; i++)
                                {
                                    tempComputersList.Add(indices[i].ToString());
                                }

                                indices = strDistinguishedName.Split(new char[] { ',' });
                                for (int i = 0; i < indices.Length; i++)
                                {
                                    tempDistinguishedNameList.Add(indices[i].ToString());
                                }

                                // by default boolean operator is OR.
                                string strBooleanOp = (string)attrTable[Parameter.QUERY_FILTERBOOLEANOP];
                                if (strBooleanOp == null)
                                    strBooleanOp = QueryClause.QUERY_BOOLEAN_OP_OR;

                                //currently supporting only OR operator
                                /*if (strBooleanOp == QueryClause.QUERY_BOOLEAN_OP_AND)
                                    return ReturnValues.GetSimpleReturnValues(ReturnCodes.UNKNOWN_DATA_TYPE, rm.GetString(Resources.OPERATION_NOT_SUPPORTED));*/

                                // strBooleanOp = QueryClause.QUERY_BOOLEAN_OP_OR;

                                IList arFilterFieldName = (IList)qualTable[Parameter.QUERY_FILTERFIELDLIST];

                                // all are strings so not taking into account.
                                // IList arFilterFieldType = (IList)qualTable[Parameter.QUERY_FILTERFIELDTYPELIST];

                                IList arFilterOperator = (IList)qualTable[Parameter.QUERY_FILTEROPERATORLIST];
                                // FilterOperator can have following values "contains", "startswith", "endswith", "isexactly", "isnot"

                                IList arFilterValue = (IList)qualTable[Parameter.QUERY_FILTERVALUELIST];

                                if (arFilterFieldName.Count == 0)
                                {
                                    ComputersList = tempComputersList;
                                    DistinguishedNameList = tempDistinguishedNameList;
                                }
                                else
                                {
                                    // Currently all list are ORed
                                    //FilterUsersList(tempComputersList, tempDistinguishedNameList, arFilterFieldName, arFilterOperator, arFilterValue, strBooleanOp, ref ComputersList, ref DistinguishedNameList);
                                }
                            }

                            rv = new ReturnValue(0);
                            rv.SetName("displayname");
                            rv.SetList(ComputersList);
                            rvsResult.AddReturnValue(rv);

                            rv = new ReturnValue(0);
                            rv.SetName("distinguishedName");
                            rv.SetList(DistinguishedNameList);
                            rvsResult.AddReturnValue(rv);

                        }
                    }
                    #endregion

                    #region querytype group_user
                    if (objectType == QueryType.COMPUTER)
                    {
                        int nRetVal = 0;
                        string strGroupUsers = "";
                        string strDistinguishedName = "";
                        string strReserved1 = "";
                        string strReserved2 = "";
                        List<String> groupUsersList = new List<String>();
                        List<String> DistinguishedNameList = new List<String>();

                        // Look up users here
                        this.GetAllGroupUsers(ref strGroupUsers, ref strDistinguishedName, ref strReserved1, ref strReserved2, ref nRetVal);

                        if (nRetVal < 0)
                        {
                            return ReturnValues.GetSimpleReturnValues(ReturnCodes.UNKNOWN_FAILURE, Resources.NO_USERS_FOUND);
                        }
                        else
                        {

                            if (strGroupUsers.Length > 0)
                            {
                                List<String> tempGroupUsersList = new List<String>();
                                List<String> tempDistinguishedNameList = new List<String>();

                                string[] indices;
                                indices = strGroupUsers.Split(new char[] { ',' });
                                for (int i = 0; i < indices.Length; i++)
                                {
                                    tempGroupUsersList.Add(indices[i].ToString());
                                }

                                indices = strDistinguishedName.Split(new char[] { ',' });
                                for (int i = 0; i < indices.Length; i++)
                                {
                                    tempDistinguishedNameList.Add(indices[i].ToString());
                                }

                                // by default boolean operator is OR.
                                string strBooleanOp = (string)attrTable[Parameter.QUERY_FILTERBOOLEANOP];
                                if (strBooleanOp == null)
                                    strBooleanOp = QueryClause.QUERY_BOOLEAN_OP_OR;

                                //currently supporting only OR operator
                                /*if (strBooleanOp == QueryClause.QUERY_BOOLEAN_OP_AND)
                                    return ReturnValues.GetSimpleReturnValues(ReturnCodes.UNKNOWN_DATA_TYPE, rm.GetString(Resources.OPERATION_NOT_SUPPORTED));*/

                                // strBooleanOp = QueryClause.QUERY_BOOLEAN_OP_OR;

                                IList arFilterFieldName = (IList)qualTable[Parameter.QUERY_FILTERFIELDLIST];

                                // all are strings so not taking into account.
                                // IList arFilterFieldType = (IList)qualTable[Parameter.QUERY_FILTERFIELDTYPELIST];

                                IList arFilterOperator = (IList)qualTable[Parameter.QUERY_FILTEROPERATORLIST];
                                // FilterOperator can have following values "contains", "startswith", "endswith", "isexactly", "isnot"

                                IList arFilterValue = (IList)qualTable[Parameter.QUERY_FILTERVALUELIST];

                                if (arFilterFieldName.Count == 0)
                                {
                                    groupUsersList = tempGroupUsersList;
                                    DistinguishedNameList = tempDistinguishedNameList;
                                }
                                else
                                {
                                    // Currently all list are ORed
                                    //FilterUsersList(tempGroupUsersList, tempDistinguishedNameList, arFilterFieldName, arFilterOperator, arFilterValue, strBooleanOp, ref groupUsersList, ref DistinguishedNameList);
                                }
                            }

                            rv = new ReturnValue(0);
                            rv.SetName("username");
                            rv.SetList(groupUsersList);
                            rvsResult.AddReturnValue(rv);

                            rv = new ReturnValue(0);
                            rv.SetName("distinguishedName");
                            rv.SetList(DistinguishedNameList);
                            rvsResult.AddReturnValue(rv);

                        }
                    }
                    #endregion

                    return rvsResult;
                }
                #endregion

                return ReturnValues.GetSimpleReturnValues(ReturnCodes.OPERATION_NOT_SUPPORTED, Resources.NOT_YET_SUPPORTED);
            }
            catch (Exception e)
            {
                log.Error("Error while performing get: ", e);
                return ReturnValues.GetSimpleReturnValues(ReturnCodes.INTERNAL_ERROR, e.Message);
            }
        }

        private void GetQueryFilterForOR(IList arFilterFieldName, IList arFilterOperator, IList arFilterValue, ref string strRegexUserName)
        {
            strRegexUserName = "";

            for (int filterCount = 0; filterCount < arFilterFieldName.Count; filterCount++)
            {
                if (arFilterFieldName[filterCount].ToString().ToLower() == "username")
                {
                    if (arFilterOperator[filterCount].ToString().ToLower() == "contains")
                        strRegexUserName += "|(" + arFilterValue[filterCount] + ")";
                    else if (arFilterOperator[filterCount].ToString().ToLower() == "startswith")
                        strRegexUserName += "|(^(" + arFilterValue[filterCount] + "))";
                    else if (arFilterOperator[filterCount].ToString().ToLower() == "endswith")
                        strRegexUserName += "|((" + arFilterValue[filterCount] + ")$)";
                }
            }

            if (strRegexUserName.Length > 0) // remove starting |
                strRegexUserName = strRegexUserName.Substring(1);


        }

        private void GetQueryFilterForAND(IList arFilterFieldName, IList arFilterOperator, IList arFilterValue, ref List<string> strRegexUsersList, ref List<string> strRegexDistinguishedNameList)
        {
            for (int filterCount = 0; filterCount < arFilterFieldName.Count; filterCount++)
            {
                if (arFilterFieldName[filterCount].ToString().ToLower() == "username")
                {
                    if (arFilterOperator[filterCount].ToString().ToLower() == "contains")
                        strRegexUsersList.Add("(" + arFilterValue[filterCount] + ")");
                    else if (arFilterOperator[filterCount].ToString().ToLower() == "startswith")
                        strRegexUsersList.Add("(^(" + arFilterValue[filterCount] + "))");
                    else if (arFilterOperator[filterCount].ToString().ToLower() == "endswith")
                        strRegexUsersList.Add("((" + arFilterValue[filterCount] + ")$)");
                }
            }

            // FilterOperator can have following values "contains", "startswith", "endswith", "isexactly", "isnot"
            // ((^(123))|(^(12))|(ContainsManisha)|(ContainsDadge)|((a)$))
            for (int filterCount = 0; filterCount < arFilterFieldName.Count; filterCount++)
            {
                if (arFilterFieldName[filterCount].ToString().ToLower() == "distinguishedname")
                {
                    if (arFilterOperator[filterCount].ToString().ToLower() == "contains")
                        strRegexDistinguishedNameList.Add("(" + arFilterValue[filterCount] + ")");
                    else if (arFilterOperator[filterCount].ToString().ToLower() == "startswith")
                        strRegexDistinguishedNameList.Add("(^(" + arFilterValue[filterCount] + "))");
                    else if (arFilterOperator[filterCount].ToString().ToLower() == "endswith")
                        strRegexDistinguishedNameList.Add("((" + arFilterValue[filterCount] + ")$)");
                }
            }


        }

        private void FilterUsersList(List<string> inUsersList, List<string> inDistinguishedNameList, List<lookupUser> foundUsers, IList arFilterFieldName, IList arFilterOperator, IList arFilterValue, string strBooleanOp, ref List<string> refUsersList, ref List<string> refDistinguishedNameList)
        {
            if (allowDebug) log.Debug("Filtering list.");
            if (strBooleanOp == QueryClause.QUERY_BOOLEAN_OP_AND)
            {
                List<string> strRegexUserList = new List<string>();
                List<string> strRegexDistinguishedNameList = new List<string>();
                GetQueryFilterForAND(arFilterFieldName, arFilterOperator, arFilterValue, ref strRegexUserList, ref strRegexDistinguishedNameList);
                for (int userCount = 0; userCount < foundUsers.Count; userCount++)
                {
                    int countForUserList = 0;
                    int countForDN = 0;
                    // Check username list
                    for (countForUserList = 0; countForUserList < strRegexUserList.Count; countForUserList++)
                    {
                        Regex regexUsername = new Regex(strRegexUserList[countForUserList], RegexOptions.IgnoreCase);
                        if (!regexUsername.IsMatch(inUsersList[userCount]))
                            break;
                    }

                    // check dnName list 
                    if (countForUserList == strRegexUserList.Count)
                    {
                        for (countForDN = 0; countForDN < strRegexDistinguishedNameList.Count; countForDN++)
                        {
                            Regex regexDNName = new Regex(strRegexDistinguishedNameList[countForDN], RegexOptions.IgnoreCase);
                            if (!regexDNName.IsMatch(inDistinguishedNameList[userCount]))
                                break;
                        }
                    }

                    if ((countForDN == strRegexDistinguishedNameList.Count) && (countForUserList == strRegexUserList.Count))
                    {
                        refUsersList.Add(inUsersList[userCount]);
                        refDistinguishedNameList.Add(inDistinguishedNameList[userCount]);
                    }
                }

            }
            else //if (strBooleanOp == QueryClause.QUERY_BOOLEAN_OP_OR)
            {
                string strRegexUserName = "";


                GetQueryFilterForOR(arFilterFieldName, arFilterOperator, arFilterValue, ref strRegexUserName);

                Regex regexUserName = new Regex(strRegexUserName, RegexOptions.IgnoreCase);


                for (int userCount = 0; userCount < foundUsers.Count; userCount++)
                {
                    if (((strRegexUserName.Length > 0) && (regexUserName.IsMatch(inUsersList[userCount]))))
                    {
                        refUsersList.Add(inUsersList[userCount]);

                    }
                }
            }
        }

        private void GetAllUsers(ref List<lookupUser> foundUsers, ref int nRetVal)
        {
            if (allowDebug) log.Debug("Getting all users");
            PrincipalContext ctx = new PrincipalContext(ContextType.Domain, domain, domain + "\\" + serviceAccountName, serviceAccountPassword);
            UserPrincipal qbeUser = new UserPrincipal(ctx);
            qbeUser.DisplayName = "*";
            qbeUser.SamAccountName = "*";

            PrincipalSearcher srch = new PrincipalSearcher(qbeUser);

            foreach (var found in srch.FindAll())
            {
                foundUsers.Add(new lookupUser { samAccountName = found.SamAccountName, fullName = found.Name });
            }
            if (foundUsers.Count == 0) nRetVal = -1;
        }

        private void GetAllGroups(ref String strGroups, ref String strDescription, ref String strType, ref String strReserved2, ref int nRetVal)
        {
            if (allowDebug) log.Debug("Getting all groups");
            PrincipalContext ctx = new PrincipalContext(ContextType.Domain, domain, domain + "\\" + serviceAccountName, serviceAccountPassword);
            GroupPrincipal qbeGroup = new GroupPrincipal(ctx);
            qbeGroup.DisplayName = "*";
            qbeGroup.Description = "*";

            // create your principal searcher passing in the QBE principal    
            PrincipalSearcher srch = new PrincipalSearcher(qbeGroup);

            int groupCount = 0;

            // find all matches
            foreach (var found in srch.FindAll())
            {
                strGroups += found.DisplayName + ",";
                strDescription += found.Description + ",";
                groupCount += 1;
            }

            strGroups = strGroups.Substring(0, strGroups.Length - 1);
            strDescription = strDescription.Substring(0, strGroups.Length - 1);
            nRetVal = groupCount;
        }

        private void GetAllComputers(ref String strComputers, ref String strDistinguishedName, ref String strType, ref String strReserved2, ref int nRetVal)
        {

        }

        private void GetAllGroupUsers(ref String strGroupUsers, ref String strDescription, ref String strReserved1, ref String strReserved2, ref int nRetVal)
        {

        }

        #endregion


        #region SUPPORTING FUNCITONS        

        public int UnlockAccount(UserPrincipal user)
        {
            if (user == null)
            {
                if (allowDebug) log.Debug("User (" + user + ") does not exist in the given context");
                return ReturnCodes.NO_SUCH_USER;
            }

            try
            {
                if (allowDebug) log.Debug("Attempting unlock on user (" + user + ")");
                user.UnlockAccount();
                user.Save();
                return ReturnCodes.SUCCESS;
            }
            catch (PrincipalOperationException e)
            {
                log.Error("Error on unlock: ", e);
                return ReturnCodes.ACCESS_DENIED;
            }
            catch (Exception e)
            {
                log.Error("Error on unlock: " + e.Message);
                return ReturnCodes.INTERNAL_ERROR;
            }
        }

        public UserPrincipalExtended getUser(string user, string targetOU)
        {
            PrincipalContext ctx = getContext(targetOU);
            UserPrincipalExtended userPrincipal = UserPrincipalExtended.FindByIdentity(ctx, IdentityType.SamAccountName, user);

            return userPrincipal;
        }

        public PrincipalContext getContext(string targetOU)
        {
            String container = String.IsNullOrEmpty(targetOU) ? defaultOU : targetOU;
            PrincipalContext ctx = new PrincipalContext(ContextType.Domain, domain, container, ContextOptions.SimpleBind, domain + "\\" + serviceAccountName, serviceAccountPassword);

            return ctx;
        }

        public void assignAttributes(UserPrincipal user, Dictionary<String, String> attributePairs)
        {
            //use reflection to iterate through UserPrincipal properties and assign values
            PropertyInfo[] userProperties = typeof(UserPrincipal).GetProperties();

            foreach (KeyValuePair<String, String> attribute in attributePairs)
            {
                if(String.IsNullOrEmpty(attribute.Value))
                {
                    continue;
                }
                if (attribute.Key.ToLower().Equals("password"))
                {
                    if (allowDebug) log.Debug("Property: Password given value: " + attribute.Value);
                    user.SetPassword(attribute.Value);
                }

                foreach (PropertyInfo property in userProperties)
                {
                    if (property.Name.ToLower().Equals("enabled") && attribute.Key.ToLower().Equals("accountdisabled"))
                    {
                        bool enabled = attribute.Value.ToLower().Equals("true") ? false : true;
                        if (allowDebug) log.Debug("Property: " + property.Name + " given value: " + enabled.ToString());
                        property.SetValue(user, enabled, null);
                        break;
                    }
                    if (property.Name.ToLower().Equals("samaccountname") && attribute.Key.ToLower().Equals("username"))
                    {
                        if (allowDebug) log.Debug("Property: " + property.Name + " given value: " + attribute.Value);
                        property.SetValue(user, attribute.Value, null);
                        break;
                    }

                    //if (property.Name.ToLower().Equals("passwordneverexpires") && attribute.Key.ToLower().Equals("passwordneverexpires"))
                    //{
                    //    if (allowDebug) log.Debug("Property: " + property.Name + " given value: " + attribute.Value);
                    //    bool pwdNeverExpires = attribute.Value.ToLower().Equals("true") ? true : false;
                    //    property.SetValue(user, pwdNeverExpires, null);
                    //    break;
                    //}

                    //if (property.Name.ToLower().Equals("accountexiprationdate") && attribute.Key.ToLower().Equals("accountexpirationdate"))
                    //{
                    //    if (allowDebug) log.Debug("Property: " + property.Name + " given value: " + attribute.Value);
                    //    DateTime expirationDate = DateTime.Parse(attribute.Value);
                    //    property.SetValue(user, expirationDate, null);
                    //    break;
                    //}                    

                    //if (property.Name.ToLower().Equals("usercannotchangepassword") && attribute.Key.ToLower().Equals("usercannotchangepassword"))
                    //{
                    //    if (allowDebug) log.Debug("Property: " + property.Name + " given value: " + attribute.Value);
                    //    property.SetValue(user, Boolean.Parse(attribute.Value), null);
                    //}

                    if (property.Name.ToLower().Equals(attribute.Key.ToLower()))
                    {
                        if (allowDebug) log.Debug("Property: " + property.Name + " given value: " + attribute.Value);

                        if (property.PropertyType == typeof(bool))
                        {
                            property.SetValue(user, Boolean.Parse(attribute.Value), null);
                            break;
                        }
                        else if(property.PropertyType == typeof(DateTime))
                        {
                            property.SetValue(user, DateTime.Parse(attribute.Value), null);
                        }
                        else
                        {
                            property.SetValue(user, attribute.Value, null);
                            break;
                        }
                        
                        
                    }
                }
            }

            user.Save();
        }

        public string listAttributes(List<attribute> attributes)
        {
            string result = "";
            foreach (attribute attribute in attributes)
            {                      
                result += attribute.name + "=" + attribute.value + "\r\n";
            }

            return result;
        }

        public string convertToUnixTimestamp(string date)
        {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            DateTime datetime = DateTime.Parse(date);
            TimeSpan diff = datetime.ToUniversalTime() - origin;
            return Math.Floor(diff.TotalSeconds).ToString();
        }

        public string getSecurityKey()
        {
            return AgentConfigurator.ReadSetting("security");
        }

        public string getPwdExpirationDate(string user)
        {
            using (var userEntry = new DirectoryEntry("WinNT://" + domain + '/' + user + ",user"))
            {
                return userEntry.InvokeGet("PasswordExpirationDate").ToString();
            }
        }

        private byte[] CreatePrimaryGroupSID(byte[] userSid, int primaryGroupID)
        {
            //convert the int into a byte array
            byte[] rid = BitConverter.GetBytes(primaryGroupID);

            //place the bytes into the user's SID byte array
            //overwriting them as necessary
            for (int i = 0; i < rid.Length; i++)
            {
                userSid.SetValue(rid[i], new long[] { userSid.Length - (rid.Length - i) });
            }

            return userSid;
        }

        private string BuildOctetString(byte[] bytes)
        {
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < bytes.Length; i++)
            {
                sb.Append(bytes[i].ToString("X2"));
            }
            return sb.ToString();
        }

        #endregion


        #region HELPER CLASSES AND STRUCTURES
        public struct attribute
        {
            public String name;
            public String dataType;
            public String value;
        }

        public struct lookupUser
        {
            public String samAccountName;
            public String fullName;
        }

        /// <summary>
        /// some properties must be retrieved by getting extended properties
        /// this is unfortunately a protected method and only accessible
        /// by using our own derived user from UserPrincipal
        /// </summary>
        [DirectoryRdnPrefix("CN")]
        [DirectoryObjectClass("user")]
        public class UserPrincipalExtended : UserPrincipal
        {
            public UserPrincipalExtended(PrincipalContext context) : base(context) { }

            public UserPrincipalExtended(PrincipalContext
                context,
                string samAccountName,
                string password,
                bool enabled)
                : base(context, samAccountName, password, enabled)
            { }

            public static new UserPrincipalExtended FindByIdentity(PrincipalContext context,
                                                           string identityValue)
            {
                return (UserPrincipalExtended)FindByIdentityWithType(context,
                                                             typeof(UserPrincipalExtended),
                                                             identityValue);
            }

            public static new UserPrincipalExtended FindByIdentity(PrincipalContext context,
                                                           IdentityType identityType,
                                                           string identityValue)
            {
                return (UserPrincipalExtended)FindByIdentityWithType(context,
                                                             typeof(UserPrincipalExtended),
                                                             identityType,
                                                             identityValue);
            }

            public int AddToGroup(string groupKey)
            {
                using (PrincipalContext ctx = this.Context)
                {
                    GroupPrincipal group = GroupPrincipal.FindByIdentity(ctx, IdentityType.Name, groupKey);
                    if (group != null)
                    {
                        group.Members.Add(ctx, IdentityType.SamAccountName, this.SamAccountName);
                        group.Save();
                        return ReturnCodes.SUCCESS;
                    }
                    else
                    {
                        return ReturnCodes.NO_SUCH_GROUP;
                    }
                }
            }

            public int RemoveFromGroup(string groupKey)
            {
                using (PrincipalContext ctx = this.Context)
                {
                    GroupPrincipal group = GroupPrincipal.FindByIdentity(ctx, IdentityType.Name, groupKey);
                    if (group != null)
                    {
                        group.Members.Remove(ctx, IdentityType.SamAccountName, this.SamAccountName);
                        group.Save();
                        return ReturnCodes.SUCCESS;
                    }
                    else
                    {
                        return ReturnCodes.NO_SUCH_GROUP;
                    }
                }
            }

            public bool VerifyPassword(string password)
            {
                using (PrincipalContext ctx = this.Context)
                {
                    return ctx.ValidateCredentials(this.SamAccountName, password);
                }
            }           


            #region custom attributes
            [DirectoryProperty("RealLastLogon")]
            public DateTime? RealLastLogon
            {
                get
                {
                    if (ExtensionGet("lastLogon").Length > 0)
                    {
                        var lastLogonDate = ExtensionGet("lastLogon")[0];
                        var lastLogonDateType = lastLogonDate.GetType();

                        var highPart = (Int32)lastLogonDateType.InvokeMember("HighPart",
                            BindingFlags.GetProperty, null, lastLogonDate, null);
                        var lowPart = (Int32)lastLogonDateType.InvokeMember("LowPart",
                            BindingFlags.GetProperty | BindingFlags.Public, null, lastLogonDate, null);

                        var longDate = ((Int64)highPart << 32 | (UInt32)lowPart);

                        return longDate > 0 ? (DateTime?)DateTime.FromFileTime(longDate) : null;
                    }

                    return null;
                }
            }

            [DirectoryProperty("initials")]
            public string Initials
            {
                get
                {
                    if (ExtensionGet("initials").Length != 1)
                        return null;


                    return (string)ExtensionGet("initials")[0] + ".";


                }
            }

            [DirectoryProperty("FullName")]
            public string FullName
            {
                get
                {
                    if (String.IsNullOrEmpty(ExtensionGet("cn")[0].ToString()))
                        return null;

                    return (string)ExtensionGet("cn")[0];
                }
            }

            [DirectoryProperty("memberOf")]
            public List<String> MemberOf
            {
                get
                {
                    if (String.IsNullOrEmpty(ExtensionGet("memberOf")[0].ToString()))
                        return null;

                    List<String> result = new List<String>();
                    var groups = ExtensionGet("memberOf").Select(obj => obj.ToString());
                    foreach (String group in groups)
                    {
                        result.Add(group);
                    }

                    return result;
                }
            }

            [DirectoryProperty("primaryGroupId")]
            public int PrimaryGroupRID
            {
                get
                {
                    if (String.IsNullOrEmpty(ExtensionGet("primaryGroupID")[0].ToString()))
                        return -1;

                    var primary = (int)ExtensionGet("primaryGroupID")[0];
                    return primary;
                }
            }

           [DirectoryProperty("objectSid")]
           public byte[] ObjectSID
            {
                get
                {
                    if (String.IsNullOrEmpty(ExtensionGet("objectSid")[0].ToString()))
                        return null;

                    var sid = (byte[])ExtensionGet("objectSid")[0];
                    return sid;
                }
            }
            #endregion
        }        

        #endregion



        //This class is one solution I found that would look up domain password policy info. It turned out to be totally unneccessary but 
        //I will just leave it here.

        //public class PasswordExpires
        //{
        //    DirectoryEntry _domain;
        //    TimeSpan _passwordAge = TimeSpan.MinValue;


        //    const int UF_DONT_EXPIRE_PASSWD = 0x10000;


        //    public PasswordExpires(string domain)
        //    {
        //        DirectoryEntry root = new DirectoryEntry("LDAP://" + domain);
        //        root.AuthenticationType = AuthenticationTypes.Secure;


        //        _domain = new DirectoryEntry("LDAP://" + root.Properties["defaultNamingContext"][0].ToString());
        //        _domain.AuthenticationType = AuthenticationTypes.Secure;
        //    }


        //    public TimeSpan PasswordAge
        //    {
        //        get
        //        {
        //            if (_passwordAge == TimeSpan.MinValue)
        //            {
        //                long ldate = LongFromLargeInteger(_domain.Properties["maxPwdAge"][0]);
        //                _passwordAge = TimeSpan.FromTicks(ldate);
        //            }


        //            return _passwordAge;
        //        }
        //    }


        //    public TimeSpan WhenExpires(string username)
        //    {
        //        DirectorySearcher ds = new DirectorySearcher(_domain);
        //        ds.Filter = String.Format("(&(objectClass=user)(objectCategory=person)(sAMAccountName={0}))", username);


        //        SearchResult sr = FindOne(ds);


        //        DirectoryEntry user = sr.GetDirectoryEntry();


        //        int flags = (int)user.Properties["userAccountControl"].Value;


        //        if (Convert.ToBoolean(flags & UF_DONT_EXPIRE_PASSWD))
        //        {
        //            return TimeSpan.MaxValue; //password never expires
        //        }


        //        //get when they last set their password
        //        DateTime pwdLastSet = DateTime.FromFileTime(LongFromLargeInteger(user.Properties["pwdLastSet"].Value));


        //        // return pwdLastSet.Add(PasswordAge).Subtract(DateTime.Now);


        //        if (pwdLastSet.Subtract(PasswordAge).CompareTo(DateTime.Now) > 0)
        //        {
        //            return pwdLastSet.Subtract(PasswordAge).Subtract(DateTime.Now);
        //        }
        //        else
        //            return TimeSpan.MinValue;  //already expired
        //    }


        //    private long LongFromLargeInteger(object largeInteger)
        //    {
        //        System.Type type = largeInteger.GetType();
        //        int highPart = (int)type.InvokeMember("HighPart", BindingFlags.GetProperty, null, largeInteger, null);
        //        int lowPart = (int)type.InvokeMember("LowPart", BindingFlags.GetProperty, null, largeInteger, null);


        //        return (long)highPart << 32 | (uint)lowPart;
        //    }


        //    private SearchResult FindOne(DirectorySearcher searcher)
        //    {
        //        SearchResult sr = null;


        //        SearchResultCollection src = searcher.FindAll();


        //        if (src.Count > 0)
        //        {
        //            sr = src[0];
        //        }
        //        src.Dispose();


        //        return sr;
        //    }
        //}
    }
}