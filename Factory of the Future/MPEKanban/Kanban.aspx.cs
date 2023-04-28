using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Factory_of_the_Future.MPEKanban
{
    public partial class MPEKanban : System.Web.UI.Page
    {

        public static ConcurrentDictionary<string, Kanban> Kanban_List = new ConcurrentDictionary<string, Kanban>();
        public string kanbanlist_string = "";

        protected void Page_Load(object sender, EventArgs e)
        {

            if (Kanban_List.Count == 0)
            {
                try
                {
                    var currentdir = System.AppDomain.CurrentDomain.BaseDirectory;
                    kanbanlist_string = new FileIO().Read(string.Format("{0}/MPEKanban/Contents/", currentdir), "Kanban.json");

                    if (!string.IsNullOrEmpty(kanbanlist_string))
                    {

                        JArray temp = JArray.Parse(kanbanlist_string);
                        if (temp.HasValues)
                        {
                            List<Kanban> tempLocation = temp.ToObject<List<Kanban>>();
                            foreach (Kanban kanbanitem in tempLocation)
                            {
                                Kanban_List.TryAdd(kanbanitem.MPENAME, kanbanitem);
                            }
                        }

                    }

                }
                catch (Exception ex)
                {
                    new ErrorLogger().ExceptionLog(ex);

                }
            }
        }

    }


    public class Kanban
    {
        public string STATUS { get; set; } = "";
        public string MPENAME { get; set; } = "";
        public string MAIL_COUNT { get; set; } = "";
    }
}