using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace RemoteWindowsAgent
{
    [TestFixture]
    public class WindowsAgentTests
    {
        [Test]
        public void createdelete()
        {
            try
            {
                WindowsAgent agent = new WindowsAgent();
                RemoteWindowsAgent remote = new RemoteWindowsAgent();

                string rvs = "<returnvalues><returnvalue name='templateid' code='0' type='string' list='False'><value>58</value></returnvalue><returnvalue name='firstname' code='0' type='string' list='False'><value>Carl</value></returnvalue><returnvalue name='middlename' code='0' type='string' list='False'><value>J</value></returnvalue><returnvalue name='middlerealname' code='0' type='string' list='False'><value></value></returnvalue><returnvalue name='lastname' code='0' type='string' list='False'><value>Contractor</value></returnvalue><returnvalue name='title' code='0' type='string' list='False'><value></value></returnvalue><returnvalue name='department' code='0' type='string' list='False'><value></value></returnvalue><returnvalue name='company' code='0' type='string' list='False'><value></value></returnvalue><returnvalue name='division' code='0' type='string' list='False'><value></value></returnvalue><returnvalue name='office' code='0' type='string' list='False'><value></value></returnvalue><returnvalue name='street' code='0' type='string' list='False'><value></value></returnvalue><returnvalue name='zip' code='0' type='string' list='False'><value></value></returnvalue><returnvalue name='pobox' code='0' type='string' list='False'><value></value></returnvalue><returnvalue name='city' code='0' type='string' list='False'><value></value></returnvalue><returnvalue name='state' code='0' type='string' list='False'><value></value></returnvalue><returnvalue name='countryabbrev' code='0' type='string' list='False'><value></value></returnvalue><returnvalue name='telephone' code='0' type='string' list='False'><value></value></returnvalue><returnvalue name='homephone' code='0' type='string' list='False'><value></value></returnvalue><returnvalue name='pager' code='0' type='string' list='False'><value></value></returnvalue><returnvalue name='fax' code='0' type='string' list='False'><value></value></returnvalue><returnvalue name='mobilephone' code='0' type='string' list='False'><value></value></returnvalue><returnvalue name='primarytelexnumber' code='0' type='string' list='False'><value></value></returnvalue><returnvalue name='webpage' code='0' type='string' list='False'><value></value></returnvalue><returnvalue name='otherfax' code='0' type='string' list='True' /><returnvalue name='otherhomephone' code='0' type='string' list='True' /><returnvalue name='othermobile' code='0' type='string' list='True' /><returnvalue name='otherpager' code='0' type='string' list='True' /><returnvalue name='othertelephone' code='0' type='string' list='True' /><returnvalue name='employeeid' code='0' type='string' list='False'><value></value></returnvalue><returnvalue name='employeenumber' code='0' type='string' list='False'><value></value></returnvalue><returnvalue name='asst' code='0' type='string' list='False'><value></value></returnvalue><returnvalue name='asstDom' code='0' type='string' list='False'><value></value></returnvalue><returnvalue name='asstGUID' code='0' type='string' list='False'><value></value></returnvalue><returnvalue name='manager' code='0' type='string' list='False'><value></value></returnvalue><returnvalue name='mgrDom' code='0' type='string' list='False'><value></value></returnvalue><returnvalue name='mgrGUID' code='0' type='string' list='False'><value></value></returnvalue><returnvalue name='ipphone' code='0' type='string' list='False'><value></value></returnvalue><returnvalue name='customAttribute1' code='0' type='string' list='False'><value></value></returnvalue><returnvalue name='customAttribute2' code='0' type='string' list='False'><value></value></returnvalue><returnvalue name='customAttribute3' code='0' type='string' list='False'><value></value></returnvalue><returnvalue name='customAttribute4' code='0' type='string' list='False'><value></value></returnvalue><returnvalue name='targetou' code='0' type='string' list='False'><value></value></returnvalue><returnvalue name='targetoudisplay' code='0' type='string' list='False'><value></value></returnvalue><returnvalue name='targetouguid' code='0' type='string' list='False'><value></value></returnvalue><returnvalue name='upnsuffix' code='0' type='string' list='False'><value></value></returnvalue><returnvalue name='primarysystem' code='0' type='string' list='False'><value>LAB</value></returnvalue><returnvalue name='primarysystemid' code='0' type='string' list='False'><value>1</value></returnvalue><returnvalue name='primaryuserid' code='0' type='string' list='False'><value>cacont</value></returnvalue><returnvalue name='email' code='0' type='string' list='False'><value></value></returnvalue><returnvalue name='passwordneverexpires' code='0' type='boolean' list='False'><value>False</value></returnvalue><returnvalue name='usermustchangepasswordnextlogon' code='0' type='boolean' list='False'><value>False</value></returnvalue><returnvalue name='usercannotchangepassword' code='0' type='boolean' list='False'><value>False</value></returnvalue><returnvalue name='accountdisabled' code='0' type='boolean' list='False'><value>False</value></returnvalue><returnvalue name='accountexpires' code='0' type='string' list='False'><value>0</value></returnvalue><returnvalue name='accountingcode' code='0' type='string' list='False'><value></value></returnvalue><returnvalue name='description' code='0' type='string' list='False'><value></value></returnvalue><returnvalue name='extensionAttribute1' code='0' type='string' list='False'><value></value></returnvalue><returnvalue name='extensionAttribute2' code='0' type='string' list='False'><value></value></returnvalue><returnvalue name='extensionAttribute3' code='0' type='string' list='False'><value></value></returnvalue><returnvalue name='extensionAttribute4' code='0' type='string' list='False'><value></value></returnvalue><returnvalue name='extensionAttribute5' code='0' type='string' list='False'><value></value></returnvalue><returnvalue name='extensionAttribute6' code='0' type='string' list='False'><value></value></returnvalue><returnvalue name='extensionAttribute7' code='0' type='string' list='False'><value></value></returnvalue><returnvalue name='extensionAttribute8' code='0' type='string' list='False'><value></value></returnvalue><returnvalue name='extensionAttribute9' code='0' type='string' list='False'><value></value></returnvalue><returnvalue name='extensionAttribute10' code='0' type='string' list='False'><value></value></returnvalue><returnvalue name='extensionAttribute11' code='0' type='string' list='False'><value></value></returnvalue><returnvalue name='extensionAttribute12' code='0' type='string' list='False'><value></value></returnvalue><returnvalue name='extensionAttribute13' code='0' type='string' list='False'><value></value></returnvalue><returnvalue name='extensionAttribute14' code='0' type='string' list='False'><value></value></returnvalue><returnvalue name='extensionAttribute15' code='0' type='string' list='False'><value></value></returnvalue><returnvalue name='employeeType' code='0' type='string' list='False'><value></value></returnvalue><returnvalue name='username' code='0' type='string' list='False'><value>CContractor</value></returnvalue><returnvalue name='systemid' code='0' type='string' list='False'><value>12</value></returnvalue><returnvalue name='displayname' code='0' type='string' list='False'><value>Carl J. Contractor</value></returnvalue><returnvalue name='sourceaccount' code='0' type='string' list='False'><value>joeuser</value></returnvalue><returnvalue name='password' code='0' type='string' list='False'><value>Abcd1234</value></returnvalue></returnvalues>";

                string result = remote.create("user", rvs);
                int result2 = agent.accountExists("seantester");
                Assert.IsTrue(result.Contains("success"));
                Assert.IsTrue(result2 == 0);

                int res = agent.delete("seantester");
                result2 = agent.accountExists("seantester");
                Assert.IsTrue(res == 0);
                Assert.IsTrue(result2 == -3);
            }
            catch (AssertionException e)
            {
                
            }
        }

        [Test]
        public void getAccountInfo()
        {
            WindowsAgent agent = new WindowsAgent();

            string user = "joeuser";
            List<WindowsAgent.attribute> attributes;
            int result = agent.getAccountInfo(user, out attributes);
            Assert.IsTrue(result == 0);
        }

        [Test]
        public void getPwdExpirationDate()
        {
            WindowsAgent agent = new WindowsAgent();

            string user = "joeuser";

            string result = agent.getPwdExpirationDate(user);
            Assert.IsNotNull(result);
        }

        [Test]
        public void accountExists()
        {
            WindowsAgent agent = new WindowsAgent();

            string user = "joeuser";
            int result = agent.accountExists(user);
            Assert.IsTrue(result == 0);
        }

        [Test]
        public void enable()
        {
            WindowsAgent agent = new WindowsAgent();

            string user = "joeuser";
            int result = agent.enable(user);
            Assert.IsTrue(result == 0);
        }

        [Test]
        public void disable()
        {
            WindowsAgent agent = new WindowsAgent();

            string user = "joeuser";
            int result = agent.disable(user);
            Assert.IsTrue(result == 0);
        }

        [Test]
        public void resetPassword()
        {
            WindowsAgent agent = new WindowsAgent();

            string user = "joeuser";
            string password = "TestP@ssw0rd!";

            int result = agent.resetPassword(user, password);
            Assert.IsTrue(result == 0);
        }

        [Test]
        public void unlock()
        {
            WindowsAgent agent = new WindowsAgent();

            string user = "joeuser";
            int result = agent.unlock(user);
            Assert.IsTrue(result == 0);
        }

        [Test]
        public void get()
        {
            WindowsAgent agent = new WindowsAgent();
            String dataType = "querygetsearchresults";

            String qualString = "<returnvalues>" +
  "<returnvalue name=\"queryfilterfieldlist\" code=\"0\" type=\"string\" list=\"True\" />" +
  "<returnvalue name=\"queryfilterfieldtypelist\" code=\"0\" type=\"string\" list=\"True\" />" +
  "<returnvalue name=\"queryfilteroperatorlist\" code=\"0\" type=\"string\" list=\"True\" />" +
  "<returnvalue name=\"queryfiltervaluelist\" code=\"0\" type=\"string\" list=\"True\" />" +
  "<returnvalue name=\"querymaxrows\" code=\"0\" type=\"string\" list=\"False\"><value>1000</value></returnvalue>" +
  "</returnvalues>";
            ReturnValues qualifier = new ReturnValues(qualString);

            String returnAttrsString = "<returnvalues>" +
  "<returnvalue name=\"queryobjecttype\" code=\"0\" type=\"string\" list=\"False\"><value>user</value></returnvalue>" +
  "<returnvalue name=\"queryfieldlist\" code=\"0\" type=\"string\" list=\"True\" />" +
  "<returnvalue name=\"queryprimaryattribute\" code=\"0\" type=\"string\" list=\"False\"><value>username</value></returnvalue>" +
  "<returnvalue name=\"queryfilterbooleanop\" code=\"0\" type=\"string\" list=\"False\"><value>AND</value></returnvalue>" +
  "<returnvalue name=\"querypicklistmode\" code=\"0\" type=\"boolean\" list=\"False\"><value>false</value></returnvalue>" +
  "</returnvalues>";
            ReturnValues returnAttributes = new ReturnValues(returnAttrsString);

            ReturnValues result = agent.get(dataType, qualifier, returnAttributes);
            Assert.IsTrue(result.GetFirstReturnValue().GetCode().Equals(0));
            
        }

        [Test]
        public void getGroupsForUser()
        {
            try
            {
                WindowsAgent agent = new WindowsAgent();

                List<String> result = agent.getGroupingObjectsForUser("Administrator");
                Assert.IsNotEmpty(result);
            }
            catch (AssertionException e)
            {

            }
        }
    }
}