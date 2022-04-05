using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.Linq;

namespace Factory_of_the_Future
{
    public class FindACEUser
    {
        private readonly string[] _propertiesToLoad = new string[]
  {
                        ADProperties.ContainerName,
                        ADProperties.LoginName,
                        ADProperties.MemberOf,
                        ADProperties.FirstName,
                        ADProperties.MiddleName,
                        ADProperties.SurName,
                        ADProperties.PostalCode
  };

        internal bool User(ADUser ACEUser, out ADUser user)
        {
            user = ACEUser;
            try
            {
                using (PrincipalContext domainContext = new PrincipalContext(ContextType.Domain, AppParameters.AppSettings.Property("Domain").Value.ToString().Trim(), AppParameters.AppSettings.Property("ADUSAContainer").Value.ToString().Trim()))
                {
                    using (var foundUser = UserPrincipal.FindByIdentity(domainContext, IdentityType.SamAccountName, (string)ACEUser.UserId))
                    {
                        if (foundUser != null)
                        {
                            DirectoryEntry directoryEntry = foundUser.GetUnderlyingObject() as DirectoryEntry;
                            directoryEntry.RefreshCache(new string[] { "tokenGroups" });

                            DirectorySearcher search = new DirectorySearcher(directoryEntry)
                            {
                                Filter = string.Format("({0}={1})", "SAMAccountName", (string)ACEUser.UserId)
                            };
                            search.PropertiesToLoad.AddRange(_propertiesToLoad);
                            SearchResult result = search.FindOne();
                            if (result == null)
                            {
                                return false;
                            }
                            string getPropertyValue(ResultPropertyValueCollection p, int i) => p.Count == 0 ? null : (string)p[i];

                            user.FirstName = getPropertyValue(result.Properties[ADProperties.FirstName], 0);
                            user.MiddleName = getPropertyValue(result.Properties[ADProperties.MiddleName], 0);
                            user.SurName = getPropertyValue(result.Properties[ADProperties.SurName], 0);
                            user.ZipCode = getPropertyValue(result.Properties[ADProperties.PostalCode], 0);
                            user.EmailAddress = foundUser.EmailAddress;
                            user.Phone = !string.IsNullOrEmpty(foundUser.VoiceTelephoneNumber) ? foundUser.VoiceTelephoneNumber : "";
                            user.EIN = !string.IsNullOrEmpty(foundUser.EmployeeId) ? foundUser.EmployeeId : "";
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                new ErrorLogger().ExceptionLog(ex);
                return false;
            }
        }


    }
}