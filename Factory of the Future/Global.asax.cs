using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Security;
using System.Web.SessionState;

namespace Factory_of_the_Future
{
    public class Global : HttpApplication
    {
        public static List<ADUser> _sessions = new List<ADUser>();
        private readonly static object padlock = new object();
        protected void Application_Start()
        {
            try
            {
                GlobalConfiguration.Configure(WebApiConfig.Register);
                //BundleConfig.RegisterBundles(BundleTable.Bundles);
            }
            catch (Exception ex)
            {
                new ErrorLogger().ExceptionLog(ex);
            }
        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {
            //var authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];
            //if (authCookie != null)
            //{
            //    var authTicket = FormsAuthentication.Decrypt(authCookie.Value);
            //    var principal = Context.Cache[authTicket.Name] as IPrincipal;
            //    if (!authTicket.Expired && principal != null)
            //    {
            //        Context.User = principal;
            //    }
            //}
            var request = HttpContext.Current.Request;
            string ipAddress = request.ServerVariables["REMOTE_ADDR"];
            var authHeader = request.Headers["APIAuthorization"];
            if (string.IsNullOrEmpty(authHeader))
            {
                authHeader = request.Headers["Authorization"];
                if (!string.IsNullOrEmpty(authHeader) && !authHeader.EndsWith("Bearer /"))
                {
                    authHeader = null;
                }
            }
            string credentials = string.Empty;
            if (authHeader != null)
            {
                var encoding = Encoding.GetEncoding("iso-8859-1");
                if (authHeader.EndsWith("/") )
                {
                    var identity = new GenericIdentity("APIUser");
                    SetPrincipal(new GenericPrincipal(identity, null));
                }
                else
                {
                    credentials = encoding.GetString(Convert.FromBase64String(authHeader));
                }
                int indexofbasic = credentials.IndexOf(' ');
                if (indexofbasic == 5 && credentials.Substring(0, indexofbasic) == "basic")
                {
                    var authHeaderVal = AuthenticationHeaderValue.Parse(credentials);
                    // RFC 2617 sec 1.2, "scheme" name is case-insensitive
                    if (authHeaderVal.Scheme.Equals("basic", StringComparison.OrdinalIgnoreCase) && authHeaderVal.Parameter != null)
                    {
                        AuthenticateUser(authHeaderVal.Parameter);
                    }
                }
            }
            else
            {
              ipAddress = request.ServerVariables["REMOTE_ADDR"];

            }
            if (Regex.IsMatch(request.Path, "(RFID)$", RegexOptions.IgnoreCase))
            {
                var identity = new GenericIdentity("APIUser");
                SetPrincipal(new GenericPrincipal(identity, null));
            }
        }

        private static void SetPrincipal(IPrincipal principal)
        {
            Thread.CurrentPrincipal = principal;
            if (HttpContext.Current != null)
            {
                HttpContext.Current.User = principal;
            }
        }

        private void AuthenticateUser(string credentials)
        {
            try
            {
                if (credentials == (AppParameters.AppSettings.ContainsKey("API_KEY") ? AppParameters.AppSettings.Property("API_KEY").Value.ToString() : ""))
                {
                    var encoding = Encoding.GetEncoding("iso-8859-1");
                    credentials = encoding.GetString(Convert.FromBase64String(credentials));

                    int separator = credentials.IndexOf(':');
                    string name = credentials.Substring(0, separator);
                    var identity = new GenericIdentity(name);
                    SetPrincipal(new GenericPrincipal(identity, null));
                }
                else
                {
                    // Invalid username or password.
                    HttpContext.Current.Response.StatusCode = 401;
                }
            }
            catch (FormatException)
            {
                // Credentials were not formatted correctly.
                HttpContext.Current.Response.StatusCode = 401;
            }
        }
        private void Session_OnEnd(object sender, EventArgs e)
        {
            lock (padlock)
            {
                if (HttpContext.Current != null)
                {
                    //Global._sessions.Remove(base.Session.SessionID);
                    int itemindex = _sessions.FindIndex(r => r.Session_ID == HttpContext.Current.Session.SessionID);
                    if (itemindex != -1)
                    {
                        // new User_Log().LogoutUser(AppParameters.CodeBase.Parent.FullName.ToString(), _sessions[itemindex]);
                        Task.Run(() => RemoveUserToListAsync(HttpContext.Current.Session)).ConfigureAwait(false);
                        _sessions.Remove(_sessions[itemindex]);
                    }
                    Session["SessionID"] = null;
                }
            }
        }
        private void Session_Start(object sender, EventArgs e)
        {
            lock (padlock)
            {
                try
                {
                    HttpCookie authCookie;
                    Session[SessionKey.UserRole] = GetUserRole(GetGroupNames(HttpContext.Current.Request.LogonUserIdentity.Groups));
                    Session[SessionKey.AceId] = Regex.Replace(HttpContext.Current.Request.LogonUserIdentity.Name, @"(USA\\|ENG\\)", "").Trim();
                    Session[SessionKey.Session_ID] = HttpContext.Current.Session.SessionID;
                    Session[SessionKey.IsAuthenticated] = HttpContext.Current.Request.IsAuthenticated;
                    Session[SessionKey.UserFirstName] = "";
                    Session[SessionKey.UserLastName] = "";
                    Session[SessionKey.Facility_NASS_CODE] = AppParameters.AppSettings.ContainsKey("FACILITY_NASS_CODE") ? AppParameters.AppSettings["FACILITY_NASS_CODE"].ToString() : "";
                    Session[SessionKey.Facility_Id] = AppParameters.AppSettings.ContainsKey("FACILITY_ID") ? AppParameters.AppSettings["FACILITY_ID"].ToString() : "";
                    Session[SessionKey.Facility_Name] = AppParameters.AppSettings.ContainsKey("FACILITY_NAME") ? AppParameters.AppSettings["FACILITY_NAME"].ToString() : "";
                    Session[SessionKey.Environment] = AppParameters.ApplicationEnvironment;
                    Session[SessionKey.FacilityTimeZone] = AppParameters.AppSettings.ContainsKey("FACILITY_TIMEZONE") ? AppParameters.AppSettings["FACILITY_TIMEZONE"].ToString() : "";
                    Session[SessionKey.SoftwareVersion] = AppParameters.VersionInfo;
                    Session[SessionKey.ApplicationFullName] = AppParameters.AppSettings.ContainsKey("APPLICATION_FULLNAME") ? AppParameters.AppSettings["APPLICATION_FULLNAME"].ToString() : "";
                    Session[SessionKey.ApplicationAbbr] = AppParameters.AppSettings.ContainsKey("APPLICATION_NAME") ? AppParameters.AppSettings["APPLICATION_NAME"].ToString() : "";
                    Session[SessionKey.Server_IpAddress] = AppParameters.ServerIpAddress;
                    Session[SessionKey.IpAddress] = Request.ServerVariables["REMOTE_HOST"];
                    Session[SessionKey.Domain] = HttpContext.Current.Request.LogonUserIdentity.Name.ToString().Split('\\')[0].ToUpper();
                    Session[SessionKey.Browser_Type] = HttpContext.Current.Request.Browser.Type;
                    Session[SessionKey.Browser_Name] = HttpContext.Current.Request.Browser.Browser;
                    Session[SessionKey.Browser_Version] = HttpContext.Current.Request.Browser.Version;
                    authCookie = AuthenticationCookie.Create(Session[SessionKey.AceId].ToString(), Converter.ObjectToString(Session), true);
                    Task.Run(() => AddUserToListAsync(Session)).ConfigureAwait(false);
                    Response.Cookies.Add(authCookie);
                }
                catch (Exception ex)
                {
                    new ErrorLogger().ExceptionLog(ex);
                }
             
                //ADUser adUser = new ADUser
                //{
                //    UserId = Regex.Replace(HttpContext.Current.Request.LogonUserIdentity.Name, @"(USA\\|ENG\\)", "").Trim(),
                //    Session_ID = HttpContext.Current.Session.SessionID,
                //    NASSCode = AppParameters.AppSettings["FACILITY_NASS_CODE"].ToString(),
                //    FDBID = AppParameters.AppSettings["FACILITY_ID"].ToString(),
                //    FacilityTimeZone = AppParameters.AppSettings["FACILITY_TIMEZONE"].ToString(),
                //    App_Type = AppParameters.AppSettings["APPLICATION_NAME"].ToString(),
                //    FacilityName = !string.IsNullOrEmpty(AppParameters.AppSettings["FACILITY_NAME"].ToString()) ? AppParameters.AppSettings["FACILITY_NAME"].ToString() : "Site Not Configured",
                //    Server_IpAddress = AppParameters.ServerIpAddress,
                //    IsAuthenticated = HttpContext.Current.Request.IsAuthenticated,
                //    IpAddress = Request.ServerVariables["REMOTE_HOST"],
                //    Login_Date = DateTime.Now,
                //    Software_Version = AppParameters.VersionInfo,
                //    Browser_Type = HttpContext.Current.Request.Browser.Type,
                //    Browser_Name = HttpContext.Current.Request.Browser.Browser,
                //    Browser_Version = HttpContext.Current.Request.Browser.Version,
                //    Domain = HttpContext.Current.Request.LogonUserIdentity.Name.ToString().Split('\\')[0].ToUpper(),
                //    Environment = AppParameters.ApplicationEnvironment,
                //    Role = GetUserRole(GetGroupNames(((WindowsIdentity)HttpContext.Current.Request.LogonUserIdentity).Groups))
                //};

                //Session[SessionKey.AceId] = adUser.UserId;
                //Session[SessionKey.UserFirstName] = adUser.FirstName;
                //Session[SessionKey.UserLastName] = adUser.SurName;
                //Session[SessionKey.UserRole] = adUser.Role;
                //Session[SessionKey.Environment] = AppParameters.ApplicationEnvironment;
                //Session[SessionKey.SoftwareVersion] = adUser.Software_Version;
                //Session[SessionKey.ApplicationFullName] = AppParameters.AppSettings["APPLICATION_FULLNAME"].ToString();
                //Session[SessionKey.ApplicationAbbr] = AppParameters.AppSettings["APPLICATION_NAME"].ToString();
                //Session[SessionKey.IsAuthenticated] = HttpContext.Current.Request.IsAuthenticated;
         
                //Task.Run(() => AddUserToList(adUser));

               

            }
        }

        private async Task AddUserToListAsync(HttpSessionState session)
        {
            try
            {
               await Task.Run(() => new UserLog().LoginUser(session)).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
            }
        }
        private async Task RemoveUserToListAsync(HttpSessionState session)
        {
            try
            {
               await Task.Run(() => new UserLog().LogoutUser(session)).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
            }
        }
        //private void AddUserToList(ADUser adUser)
        //{
        //    try
        //    {
        //        if (_sessions.Count == 0)
        //        {
        //            _sessions.Add(adUser);
        //            Task.Run(() => new User_Log().LoginUser(adUser));
        //        }
        //        else
        //        {
        //            int itemindex = _sessions.FindIndex(r => r.Session_ID == adUser.Session_ID);
        //            if (itemindex != -1 && _sessions.Count > 0)
        //            {
        //                //log out user 
        //                if (!string.IsNullOrEmpty(_sessions[itemindex].Session_ID))
        //                {
        //                    Task.Run(() => new User_Log().LogoutUser(_sessions[itemindex]));
        //                    _sessions.Remove(_sessions[itemindex]);
        //                }
        //                _sessions[itemindex].Login_Date = adUser.Login_Date;
        //                _sessions[itemindex].Session_ID = adUser.Session_ID;
        //                //log of user logging in.
        //                Task.Run(() => new User_Log().LoginUser(adUser));

        //            }
        //            else
        //            {
        //                _sessions.Add(adUser);
        //                Task.Run(() => new User_Log().LoginUser(adUser));
        //            }
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        new ErrorLogger().ExceptionLog(e);
        //    }
        //}
        private string GetUserRole(string groups)
        {
            try
            {
                if (!string.IsNullOrEmpty(groups))
                {
                    List<string> user_groups = groups.Split('|').ToList();

                    //Check for Admin Access 

                    string temp_list = AppParameters.AppSettings.ContainsKey("ROLES_ADMIN") ? AppParameters.AppSettings.Property("ROLES_ADMIN").Value.ToString().Trim() : "";
                    if (!string.IsNullOrEmpty(temp_list))
                    {
                        List<string> Webconfig_roles = temp_list.Split(',').ToList();
                        List<string> commonrolse = Webconfig_roles.Intersect(user_groups, StringComparer.OrdinalIgnoreCase).ToList();
                        if ((commonrolse != null) && commonrolse.Count > 0)
                        {
                            return "Admin".ToUpper();
                        }
                    }
                    //Check for OIE Access
                    temp_list = AppParameters.AppSettings.ContainsKey("ROLES_OIE") ? AppParameters.AppSettings.Property("ROLES_OIE").Value.ToString().Trim() : "";
                    if (!string.IsNullOrEmpty(temp_list))
                    {
                        List<string> Webconfig_roles = temp_list.Split(',').ToList();
                        List<string> commonrolse = Webconfig_roles.Intersect(user_groups, StringComparer.OrdinalIgnoreCase).ToList();
                        if ((commonrolse != null) && commonrolse.Count > 0)
                        {
                            return "OIE".ToUpper();
                        }
                    }
                    //Check for Maintenance Access
                    temp_list = AppParameters.AppSettings.ContainsKey("ROLES_MAINTENANCE") ? AppParameters.AppSettings.Property("ROLES_MAINTENANCE").Value.ToString().Trim() : "";
                    if (!string.IsNullOrEmpty(temp_list))
                    {
                        List<string> Webconfig_roles = temp_list.Split(',').ToList();
                        List<string> commonrolse = Webconfig_roles.Intersect(user_groups, StringComparer.OrdinalIgnoreCase).ToList();
                        if ((commonrolse != null) && commonrolse.Count > 0)
                        {
                            return "MAINTENANCE".ToUpper();
                        }
                    }
                    return "Operator".ToUpper();
                }
                else
                {
                    return "Operator".ToUpper();
                }
            }
            catch (Exception ex)
            {
                new ErrorLogger().ExceptionLog(ex);
                return "Operator".ToUpper();
            }
        }
        private string GetGroupNames(IdentityReferenceCollection groups)
        {
            try
            {
                string item = string.Empty;
                if (groups != null)
                {
                    int propertyCount = groups.Count;
                    int propertyCounter;
                    for (propertyCounter = 0; propertyCounter <= propertyCount - 1; propertyCounter++)
                    {
                        try
                        {
                            string dn = groups[propertyCounter].Translate(typeof(System.Security.Principal.NTAccount)).ToString();

                            int equalsIndex = dn.IndexOf("\\", 1);
                            if ((equalsIndex == -1))
                            {
                                continue;
                            }

                            var groupName = dn.Substring((equalsIndex + 1), (dn.Length - equalsIndex) - 1);
                            if ((propertyCount - 1) == propertyCounter)
                            {
                                item += groupName;
                            }
                            else
                            {
                                item = item + groupName + "|";
                            }
                        }
                        catch (Exception e)
                        {
                            new ErrorLogger().ExceptionLog(e);
                            continue;
                        }
                     
                    }
                }
                return item;
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
                return "";
            }
        }
    }
}
