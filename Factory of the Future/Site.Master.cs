using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Factory_of_the_Future
{
    public partial class Site : System.Web.UI.MasterPage
    { 
        public string User { get { return Session[SessionKey.UserRole].ToString(); } }
        protected void Page_Load(object sender, EventArgs e)
        {

        }
    }
}