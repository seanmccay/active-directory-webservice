using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Net;
using System.Web;
using System.Web.Services;

namespace RemoteWindowsAgent
{
    /// <summary>
    /// Built by: Sean McCay
    /// 
    /// This is a replacement for the older windows agent.  I ended up using ASMX because of complications with WCF. 
    /// The UtilDev Web Server Pro had issues when using bindings other than basicHttp, so we wouldn't have been able to
    /// implement security as is required. It has been noted that WCF is supposed to be the replacement for ASMX services.
    /// It's doubtful that Microsoft will depricate ASMX services, so it should work just fine.
    /// 
    /// NOTES:
    /// - There are a couple of concerns with verifyPassword(). First, it may return true for old passwords. Second, if the account is 
    /// flagged for reset at next logon, or if the password expired it will, or should, return false.
    /// 
    /// </summary>
    [WebService(Namespace = "http://avatier.com/ISelfService")]
    [WebServiceBinding(ConformsTo = WsiProfiles.None, Name="RemoteWindowsAgent")]
    [System.ComponentModel.ToolboxItem(false)]

    public class RemoteWindowsAgent : System.Web.Services.WebService
    {
        #region STATE AND CONSTRUCTOR

        private String allowedIPs;
        private String sharedKey;
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger("RemoteWindowsAgent");
        private bool allowDebug = log.IsDebugEnabled;
        public WindowsAgent windowsAgent;

        //default constructor
        public RemoteWindowsAgent()
        {
            this.windowsAgent = new WindowsAgent();
            this.allowedIPs = AgentConfigurator.Decrypt(windowsAgent.GetSecurityKey(), AgentConfigurator.ReadByteSetting("allowedIPs"));
            this.sharedKey = AgentConfigurator.Decrypt(windowsAgent.GetSecurityKey(), AgentConfigurator.ReadByteSetting("sharedKey"));
        }


        #endregion


        #region GENERAL AIMS FUNCTIONS

        [WebMethod]
        public String getSystemStatus()
        {
            if (!isExpectedIP() || !isValidSharedKey())
            {
                log.Warn("This request failed authentication. Refusing access.");
                return getGenericResult(ReturnCodes.ACCESS_REFUSED, "Request failed auth. Acess Refused.", "false");
            }
            else
            {
                return getGenericResult(ReturnCodes.SUCCESS, "Success", "true");
            }
        }

        [WebMethod]
        public String getAgentIdentity()
        {
            if (!isExpectedIP() || !isValidSharedKey())
            {
                log.Warn("This request failed authentication. Refusing access.");
                return getGenericResult(ReturnCodes.ACCESS_REFUSED, "Request failed auth. Acess Refused.", "");
            }
            return getGenericResult(ReturnCodes.SUCCESS, "Connector/Agent Name", "Remote Windows Agent");
        }

        [WebMethod]
        public String getAgentVersion()
        {
            if (!isExpectedIP() || !isValidSharedKey())
            {
                log.Warn("This request failed authentication. Refusing access.");
                return getGenericResult(ReturnCodes.ACCESS_REFUSED, "Request failed auth. Acess Refused.", "");
            }
            return getGenericResult(ReturnCodes.SUCCESS, "Agent Version", "1.0");
        }

        [WebMethod]
        public String getAPIVersion()
        {
            if (!isExpectedIP() || !isValidSharedKey())
            {
                log.Warn("This request failed authentication. Refusing access.");
                return getGenericResult(ReturnCodes.ACCESS_REFUSED, "Request failed auth. Acess Refused.", "");
            }
            return getGenericResult(ReturnCodes.SUCCESS, "API Verison", "1");
        }

        #endregion 


        #region PASSWORD STATION

        [WebMethod]
        public String getAccountInfo(String user)
        {
            log.Info("User = " + user);
            // Check request IP and paramters for validity
            if (!isExpectedIP() || !isValidSharedKey())
            {
                log.Warn("This request failed authentication. Refusing access.");
                return getGenericResult(ReturnCodes.ACCESS_REFUSED, "Request failed auth. Acess Refused.", "");
            }
            if (String.IsNullOrEmpty(user))
            {
                log.Warn("User parameter was empty, returning failure.");
                return getGenericResult(ReturnCodes.INTERNAL_ERROR, "One or more parameters were null", "");
            }
            List<WindowsAgent.attribute> attributeValues;
            int result = windowsAgent.GetAccountInfo(user, out attributeValues);

            switch (result)
            {
                case ReturnCodes.INTERNAL_ERROR:
                    log.Warn("Failure.");
                    return getGenericResult(ReturnCodes.INTERNAL_ERROR, "Check log file for information", "");
                case ReturnCodes.NO_SUCH_USER:
                    log.Warn("Failure. No such user.");
                    return getGenericResult(ReturnCodes.NO_SUCH_USER, "", "");
                default:
                    log.Info("Successfully returned account info.");
                    return buildAttributesResult(ReturnCodes.SUCCESS, attributeValues);
            }
        }

        [WebMethod]
        public String resetPassword(String user, String password) 
        {
            // Check request IP and paramters for validity
            if (!isExpectedIP() || !isValidSharedKey())
            {
                log.Warn("This request failed authentication. Refusing access.");
                return getGenericResult(ReturnCodes.ACCESS_REFUSED, "Request failed auth. Acess Refused.", "");
            }
            if (String.IsNullOrEmpty(user) || String.IsNullOrEmpty(password)) return getGenericResult(ReturnCodes.INTERNAL_ERROR, "One or more parameters were null", "");
            log.Info("User = " + user);

            int result = windowsAgent.ResetPassword(user, password);
            
            switch (result)
            {
                case ReturnCodes.INTERNAL_ERROR:
                    log.Warn("Failure.");
                    return getGenericResult(ReturnCodes.INTERNAL_ERROR, "Check log file for information", "");
                case ReturnCodes.NO_SUCH_USER:
                    log.Warn("Failure. No such user.");
                    return getGenericResult(ReturnCodes.NO_SUCH_USER, "", "");
                default:
                    log.Info("Password reset successful.");
                    return getGenericResult(ReturnCodes.SUCCESS, "", "");
            }
        }

        [WebMethod]
        public String verifyPassword(String user, String password)
        {
            // Check request IP and paramters for validity
            if (!isExpectedIP() || !isValidSharedKey())
            {
                log.Warn("This request failed authentication. Refusing access.");
                return getGenericResult(ReturnCodes.ACCESS_REFUSED, "Request failed auth. Acess Refused.", "");
            }
            if (String.IsNullOrEmpty(user) || String.IsNullOrEmpty(password)) return getGenericResult(ReturnCodes.INTERNAL_ERROR, "One or more parameters were null", "");
            log.Info("User = " + user);

            int result = windowsAgent.VerifyPassword(user, password);

            switch (result)
            {
                case ReturnCodes.INTERNAL_ERROR:
                    return getGenericResult(ReturnCodes.INTERNAL_ERROR, "Check log file for information", "false");
                case ReturnCodes.NO_SUCH_USER:
                    return getGenericResult(ReturnCodes.NO_SUCH_USER, "", "false");
                default:
                    return getGenericResult(ReturnCodes.SUCCESS, "", "true");
            }
        }

        [WebMethod]
        public String changePassword(String user, String oldPassword, String password)
        {
            // Check request IP and paramters for validity
            if (!isExpectedIP() || !isValidSharedKey())
            {
                log.Warn("This request failed authentication. Refusing access.");
                return getGenericResult(ReturnCodes.ACCESS_REFUSED, "Request failed auth. Acess Refused.", "");
            }
            if (String.IsNullOrEmpty(user) || String.IsNullOrEmpty(oldPassword) || String.IsNullOrEmpty(password)) return getGenericResult(ReturnCodes.INTERNAL_ERROR, "One or more parameters were null", "");
            log.Info("User = " + user);

            int result = windowsAgent.ChangePassword(user, oldPassword, password);

            switch (result)
            {
                case ReturnCodes.INTERNAL_ERROR:
                    return getGenericResult(ReturnCodes.INTERNAL_ERROR, "Check log file for information", "");
                case ReturnCodes.NO_SUCH_USER:
                    return getGenericResult(ReturnCodes.NO_SUCH_USER, "", "");
                default:
                    return getGenericResult(ReturnCodes.SUCCESS, "", "");
            }
        }

        [WebMethod]
        public String unlock(String user)
        {
            // Check request IP and paramters for validity
            if (!isExpectedIP() || !isValidSharedKey())
            {
                log.Warn("This request failed authentication. Refusing access.");
                return getGenericResult(ReturnCodes.ACCESS_REFUSED, "Request failed auth. Acess Refused.", "");
            }
            if (String.IsNullOrEmpty(user)) return getGenericResult(ReturnCodes.INTERNAL_ERROR, "User parameter was null", "");
            log.Info("User = " + user);

            int result = windowsAgent.Unlock(user);

            switch (result)
            {
                case ReturnCodes.ACCESS_DENIED:
                    log.Warn("Failure. Access denied.");
                    return getGenericResult(ReturnCodes.ACCESS_DENIED, "", "");
                case ReturnCodes.NO_SUCH_USER:
                    log.Warn("Failure. No such user.");
                    return getGenericResult(ReturnCodes.NO_SUCH_USER, "", "");
                case ReturnCodes.INTERNAL_ERROR:
                    log.Warn("Failure. Check log for RemoteWindowsAgent.WindowsAgent.UnlockAccount()");
                    return getGenericResult(ReturnCodes.INTERNAL_ERROR, "", "");

                default:
                    log.Info("Unlock successful. Exiting.");
                    return getGenericResult(ReturnCodes.SUCCESS, "", "");
            }
        }

        #endregion


        #region ACCOUNT TERMINATOR

        [WebMethod]
        public String disableUser(String user)
        {
            // Check request IP and paramters for validity
            if (!isExpectedIP() || !isValidSharedKey())
            {
                log.Warn("This request failed authentication. Refusing access.");
                return getGenericResult(ReturnCodes.ACCESS_REFUSED, "Request failed auth. Acess Refused.", "");
            }
            if (String.IsNullOrEmpty(user)) return getGenericResult(ReturnCodes.INTERNAL_ERROR, "User parameter was null", "");
            log.Info("User = " + user);

            int result = windowsAgent.Disable(user);

            switch (result)
            {
                case ReturnCodes.NO_SUCH_USER:
                    return getGenericResult(ReturnCodes.NO_SUCH_USER, "", "");
                case ReturnCodes.INTERNAL_ERROR:
                    return getGenericResult(ReturnCodes.INTERNAL_ERROR, "", "");

                default:
                    return getGenericResult(ReturnCodes.SUCCESS, "", "");
            }
        }

        [WebMethod]
        public String enableUser(String user)
        {
            // Check request IP and paramters for validity
            if (!isExpectedIP() || !isValidSharedKey())
            {
                log.Warn("This request failed authentication. Refusing access.");
                return getGenericResult(ReturnCodes.ACCESS_REFUSED, "Request failed auth. Acess Refused.", "");
            }
            if (String.IsNullOrEmpty(user)) return getGenericResult(ReturnCodes.INTERNAL_ERROR, "User parameter was null", "");
            log.Info("User = " + user);

            int result = windowsAgent.Enable(user);

            switch (result)
            {
                case ReturnCodes.NO_SUCH_USER:
                    return getGenericResult(ReturnCodes.NO_SUCH_USER, "", "");
                case ReturnCodes.INTERNAL_ERROR:
                    return getGenericResult(ReturnCodes.INTERNAL_ERROR, "", "");

                default:
                    return getGenericResult(ReturnCodes.SUCCESS, "", "");
            }
        }

        [WebMethod] 
        public String delete(String dataType, String attributes)
        {
            // Check request IP and paramters for validity
            if (!isExpectedIP() || !isValidSharedKey())
            {
                log.Warn("This request failed authentication. Refusing access.");
                return getGenericResult(ReturnCodes.ACCESS_REFUSED, "Request failed auth. Acess Refused.", "");
            }
            if (String.IsNullOrEmpty(dataType) || String.IsNullOrEmpty(attributes)) return getGenericResult(ReturnCodes.INTERNAL_ERROR, "One or more parameters was null", "");
            if (dataType != "user") return getGenericResult(ReturnCodes.INTERNAL_ERROR, "data type was not \'user\'", "");
            if (allowDebug) log.Debug("Pulling username out of attributes xml - " + attributes);

            XDocument xml = XDocument.Parse(attributes);
            IEnumerable<XElement> user = (from e in xml.Descendants("returnvalue")
                                                where e.Attribute("name").Value.Equals("username")
                                                select e.Element("value"));

            string username = user.ElementAt(0).Value;
            log.Info("User = " + username);

            int result = windowsAgent.Delete(username);

            switch (result)
            {
                case ReturnCodes.NO_SUCH_USER:
                    return getGenericResult(ReturnCodes.NO_SUCH_USER, "", "");
                case ReturnCodes.INTERNAL_ERROR:
                    return getGenericResult(ReturnCodes.INTERNAL_ERROR, "", "");

                default:
                    return getGenericResult(ReturnCodes.SUCCESS, "", "");
            }
        }

        #endregion


        #region ACCOUNT CREATOR
            
        [WebMethod]
        public String doesAccountExist(String user)
        {
            // Check request IP and paramters for validity
            if (!isExpectedIP() || !isValidSharedKey())
            {
                log.Warn("This request failed authentication. Refusing access.");
                return getGenericResult(ReturnCodes.ACCESS_REFUSED, "Request failed auth. Acess Refused.", "");
            }
            if (String.IsNullOrEmpty(user)) return getGenericResult(ReturnCodes.INTERNAL_ERROR, "User parameter was null", "");
            log.Info("User = " + user);

            int result = windowsAgent.AccountExists(user);

            switch (result)
            {
                case ReturnCodes.NO_SUCH_USER:
                    return getGenericResult(ReturnCodes.NO_SUCH_USER, "", "");
                case ReturnCodes.INTERNAL_ERROR:
                    return getGenericResult(ReturnCodes.INTERNAL_ERROR, "", "");
                default:
                    return getGenericResult(ReturnCodes.SUCCESS, "", "");
            }
        }         

        [WebMethod]
        public String getCreateSteps(String dataType, String qualifier, String returnAttrs)
        {
            try
            {
                // Check request IP and paramters for validity
                if (!isExpectedIP() || !isValidSharedKey())
                {
                    log.Warn("This request failed authentication. Refusing access.");
                    return getGenericResult(ReturnCodes.ACCESS_REFUSED, "Request failed auth. Acess Refused.", "");
                }
                if (dataType != "user" || String.IsNullOrEmpty(returnAttrs)) return getGenericResult(ReturnCodes.INTERNAL_ERROR, "One or more parameters were null", "");

                XElement xml = new XElement("returnvalues",
                    new XElement("returnvalue",
                        new XAttribute("code", "0"),
                        new XAttribute("list", "true"),
                        new XAttribute("name", "createsteplist"),
                            new XElement("value", "CREATEUSERACCOUNT"), 
                            new XElement("value", "COPYGROUPMEMBERSHIP")
                            //new XElement("value", "CREATEHOMEDIR"),
                           // new XElement("value", "CREATEHOMESHARE"),
                            //new XElement("value", "CREATEEXCHANGEMAILBOX")
                    )
                );

                String result = xml.ToString();

                return result;
            }
            catch (Exception e)
            {
                return getGenericResult(ReturnCodes.INTERNAL_ERROR, e.Message + e.StackTrace, "");
            }
        }

        [WebMethod]
        public String create(String dataType, String attributes)
        {
            //Check request IP and paramters for validity
            if (!isExpectedIP() || !isValidSharedKey())
            {
                log.Warn("This request failed authentication. Refusing access.");
                return getGenericResult(ReturnCodes.ACCESS_REFUSED, "Request failed auth. Acess Refused.", "");
            }
            if (dataType != "user" || String.IsNullOrEmpty(attributes)) return getGenericResult(ReturnCodes.INTERNAL_ERROR, "One or more parameters were null", "");

            if (allowDebug) log.Debug("Beginning parsing of attributes xml - " + attributes);

            //pull attributes and their values out of retvals
            XDocument rvs = XDocument.Parse(attributes);            

            IEnumerable<String> attributeNames = from x in rvs.Descendants("returnvalue")
                                                 where x.HasElements && !x.Element("value").IsEmpty 
                                                 select x.Attribute("name").Value;

            String[] attNms = attributeNames.ToArray();

            IEnumerable<String> attributeValues = from x in rvs.Descendants("value")
                                                  where !x.IsEmpty
                                                  select x.Value;
                                                  
            String[] attVls = attributeValues.ToArray();

            Dictionary<String, String> attributePairs = new Dictionary<String, String>();
            for (int i = 0; i < attNms.Length; i++)
            {
                attributePairs.Add(attNms[i], attVls[i]);
            }

            int result = windowsAgent.Create(attributePairs);

            switch (result)
            {
                case ReturnCodes.INTERNAL_ERROR:
                    return getGenericResult(ReturnCodes.INTERNAL_ERROR, "Check log file for information", "");
                case ReturnCodes.NO_SUCH_USER:
                    return getGenericResult(ReturnCodes.NO_SUCH_USER, "Source user not found", "");

                default:
                    return getGenericResult(ReturnCodes.SUCCESS, "", "");
            }
        }

        [WebMethod]
        public String get(String dataType, String qualifier, String returnAttributes)
        {
            if (!isExpectedIP() || !isValidSharedKey())
            {
                log.Warn("This request failed authentication. Refusing access.");
                return getGenericResult(ReturnCodes.ACCESS_REFUSED, "Request failed auth. Acess Refused.", "");
            }

            if (allowDebug) log.Debug("dataType: " + dataType + "\r\n qualifier: " + qualifier + "\r\n returnAttributes: " + returnAttributes);

            if (String.IsNullOrEmpty(dataType) ||
                String.IsNullOrEmpty(qualifier) ||
                String.IsNullOrEmpty(returnAttributes)){return getGenericResult(ReturnCodes.INTERNAL_ERROR, "One or more parameters were null", "");}
            
            ReturnValues qual = new ReturnValues(qualifier);
            ReturnValues attrs = new ReturnValues(returnAttributes);

            ReturnValues result = windowsAgent.get(dataType, qual, attrs);
            string resultString = result.toXML();
            return resultString;
        }

        #endregion


        #region GROUPING OBJECT METHODS

        [WebMethod]
        public String DoesGroupingObjectExist(String type, String groupingObjectKey)
        {
            //check params and IP
            if (!isExpectedIP() || !isValidSharedKey())
            {
                log.Warn("This request failed authentication. Refusing access.");
                return getGenericResult(ReturnCodes.ACCESS_REFUSED, "Request failed auth. Acess Refused.", "");
            }
            if (String.IsNullOrEmpty(type) || type != "group") return getGenericResult(ReturnCodes.INTERNAL_ERROR, "Type parameter was not group or was empty.", "");
            if (String.IsNullOrEmpty(groupingObjectKey)) return getGenericResult(ReturnCodes.INTERNAL_ERROR, "GroupingObjectKey was null", "");

            log.Info("Looking for grouping object.");
            int result = windowsAgent.DoesGroupingObjectExist(groupingObjectKey);

            switch (result)
            {
                case ReturnCodes.NO_SUCH_GROUP:
                    log.Info("Group (" + groupingObjectKey + ") does not exist.");
                    return getGenericResult(ReturnCodes.NO_SUCH_GROUP, "Group (" + groupingObjectKey + ") does not exist.", "false");
                case ReturnCodes.INTERNAL_ERROR:
                    return getGenericResult(ReturnCodes.INTERNAL_ERROR, "Check agent log for more info.", "");
                default:
                    log.Info("Group (" + groupingObjectKey + ") exists.");
                    return getGenericResult(ReturnCodes.SUCCESS, "Group (" + groupingObjectKey + ") exists.", "true");
            }
        }

        [WebMethod]
        public String GetGroupingObjectsForUser(String type, String user)
        {
            //check params and IP
            if (!isExpectedIP() || !isValidSharedKey())
            {
                log.Warn("This request failed authentication. Refusing access.");
                return getGenericResult(ReturnCodes.ACCESS_REFUSED, "Request failed auth. Acess Refused.", "");
            }
            if (String.IsNullOrEmpty(user)) return getGenericResult(ReturnCodes.INTERNAL_ERROR, "User parameter was null", "");
            log.Info("User = " + user);
            if (String.IsNullOrEmpty(type) || type != "group") return getGenericResult(ReturnCodes.INTERNAL_ERROR, "Type parameter was not group or was empty.", "");

            int exists = windowsAgent.AccountExists(user);
            if (exists == ReturnCodes.NO_SUCH_USER)
            {
                log.Warn("User was not found.");
                return getGenericResult(ReturnCodes.NO_SUCH_USER, "", "");
            }
            List<String> result = windowsAgent.GetGroupingObjectsForUser(user);           

            return buildListResult(result);
        }

        [WebMethod]
        public String AddUserToGroupingObject(String type, String groupingObjectKey, String user)
        {
            //check params and IP
            if (!isExpectedIP() || !isValidSharedKey())
            {
                log.Warn("This request failed authentication. Refusing access.");
                return getGenericResult(ReturnCodes.ACCESS_REFUSED, "Request failed auth. Acess Refused.", "");
            }
            if (String.IsNullOrEmpty(user)) return getGenericResult(ReturnCodes.INTERNAL_ERROR, "User parameter was null", "");
            log.Info("User = " + user);
            if (String.IsNullOrEmpty(type) || type != "group") return getGenericResult(ReturnCodes.INTERNAL_ERROR, "Type parameter was not group or was empty.", "");
            if (String.IsNullOrEmpty(groupingObjectKey)) return getGenericResult(ReturnCodes.INTERNAL_ERROR, "GroupingObjectKey was null", "");

            int result = windowsAgent.AddUserToGroupingObject(user, groupingObjectKey);

            switch (result)
            {
                case ReturnCodes.INTERNAL_ERROR:
                    return getGenericResult(ReturnCodes.INTERNAL_ERROR, "Check agent log for more info", "");
                case ReturnCodes.NO_SUCH_GROUP:
                    log.Warn("Group was not found.");
                    return getGenericResult(ReturnCodes.NO_SUCH_GROUP, "Group was not found", "");
                case ReturnCodes.NO_SUCH_USER:
                    log.Warn("User was not found");
                    return getGenericResult(ReturnCodes.NO_SUCH_USER, "User was not found", "");
                default:
                    log.Info("Added user (" + user + ") to group (" + groupingObjectKey + ").");
                    return getGenericResult(ReturnCodes.SUCCESS, "Added user to grouping object", "");
            }
        }

        [WebMethod]
        public String RemoveUserFromGroupingObject(String type, String groupingObjectKey, String user)
        {
            //check params and IP
            if (!isExpectedIP() || !isValidSharedKey())
            {
                log.Warn("This request failed authentication. Refusing access.");
                return getGenericResult(ReturnCodes.ACCESS_REFUSED, "Request failed auth. Acess Refused.", "");
            }
            if (String.IsNullOrEmpty(user)) return getGenericResult(ReturnCodes.INTERNAL_ERROR, "User parameter was null", "");
            log.Info("User = " + user);
            if (String.IsNullOrEmpty(type) || type != "group") return getGenericResult(ReturnCodes.INTERNAL_ERROR, "Type parameter was not group or was empty.", "");
            if (String.IsNullOrEmpty(groupingObjectKey)) return getGenericResult(ReturnCodes.INTERNAL_ERROR, "GroupingObjectKey was null", "");

            int result = windowsAgent.RemoveUserFromGroupingObject(user, groupingObjectKey);

            switch(result)
            {
                case ReturnCodes.NO_SUCH_GROUP:
                    log.Warn("Group was not found");
                    return getGenericResult(ReturnCodes.NO_SUCH_GROUP, "Group was not found.", "");
                case ReturnCodes.NO_SUCH_USER:
                    log.Warn("User was not found");
                    return getGenericResult(ReturnCodes.NO_SUCH_USER, "User was not found.", "");
                case ReturnCodes.INTERNAL_ERROR:
                    log.Error("Error caught while removing user from group");
                    return getGenericResult(ReturnCodes.INTERNAL_ERROR, "Check agent log for more info", "");
                default:
                    log.Info("Removed user (" + user + ") from group (" + groupingObjectKey + ").");
                    return getGenericResult(ReturnCodes.SUCCESS, "Removed user from group", "");
            }
        }

        [WebMethod]
        public String getGroupMembers(String group)
        {
            //check params and IP
            if (!isExpectedIP() || !isValidSharedKey())
            {
                log.Warn("This request failed authentication. Refusing access.");
                return getGenericResult(ReturnCodes.ACCESS_REFUSED, "Request failed auth. Acess Refused.", "");
            }
            if (String.IsNullOrEmpty(group)) return getGenericResult(ReturnCodes.INTERNAL_ERROR, "GroupingObjectKey was null", "");

            List<String> members = windowsAgent.GetGroupMembers(group);
            if (members == null)
            {
                return getGenericResult(ReturnCodes.NO_SUCH_GROUP, "Group could not be found.", "");
            }
            return buildListResult(members);
        }        

        #endregion


        #region SUPPORTING FUNCTIONS

        public String getGenericResult(int code, String debug, String value) 
        {
            XElement xml = new XElement("returnvalues",
                new XElement("returnvalue",
                    new XAttribute("code", code),
                        new XElement("debug", debug),
                        new XElement("value", value)
                )
            );

            String result = xml.ToString();

            return result;
        }

        public String buildAttributesResult(int code, List<WindowsAgent.attribute> attributes)
        {
            try
            {
                XDocument xml = new XDocument();
                XElement root = new XElement("returnvalues");

                foreach (WindowsAgent.attribute attribute in attributes)
                {
                    XElement retval = new XElement("returnvalue",
                        new XAttribute("name", attribute.name),
                        new XAttribute("type", attribute.dataType),
                        new XAttribute("code", code),
                            new XElement("value", attribute.value)
                    );

                    root.Add(retval);                   
                }                

                xml.AddFirst(root);
                String result = xml.ToString();

                return result;
            }
            catch (Exception e)
            {
                return getGenericResult(ReturnCodes.INTERNAL_ERROR, "XML failed to build, " + e.Message + e.StackTrace + e.InnerException, "");
            }
        }

        public String buildListResult(List<String> list)
        {
            try
            {
                XDocument xml = new XDocument();
                XElement rvs = new XElement("returnvalues");
                XElement rv = new XElement("returnvalue",
                       new XAttribute("code", 0),
                       new XAttribute("list", "true"),
                       new XAttribute("name", "groupMemberships"));
                

                foreach (String item in list)
                {
                    XElement value = new XElement("value", item);
                    rv.Add(value);
                }

                rvs.AddFirst(rv);
                xml.AddFirst(rvs);
                String result = xml.ToString();

                return result;
            }
            catch (Exception e)
            {
                return getGenericResult(ReturnCodes.INTERNAL_ERROR, "XML failed to build, " + e.Message + e.StackTrace + e.InnerException, "");
            }
        }

        public bool isExpectedIP()
        {            
            if (String.IsNullOrEmpty(this.allowedIPs)) return false;

            IPAddress requestIP = IPAddress.Parse(HttpContext.Current.Request.UserHostAddress);
            if (allowDebug) log.Debug("Recieving request from " + requestIP);
            String allowedIPs = this.allowedIPs.Trim();
            String[] ipArray = allowedIPs.Split(new Char[] { ',' });
            for (int i = 0; i < ipArray.Length; i++)
            {
                if (ipArray[i] == requestIP.ToString()) return true;
             
                IPAddress[] addresses = Dns.GetHostAddresses(ipArray[i]);

                for (int j = 0; j < addresses.Length; j++)
                {
                    if (requestIP.Equals(addresses[j])) return true;
                }
            }

            log.Warn("Request sent from unexpected IP address: " + requestIP.ToString());
            return false;
        }

        public bool isValidSharedKey()
        {
            string requestKey = HttpContext.Current.Request.Headers.Get("Authorization");
            if (allowDebug) log.Debug("Checking request's authentication key.");
            if(requestKey.Equals(sharedKey))
            {
                log.Debug("Key is valid.");
                return true;
            }
            else
            {
                log.Warn("Authentication sent with request was invalid. Key sent: " + requestKey);
                return false;
            }
        }

        #endregion
    }
}
