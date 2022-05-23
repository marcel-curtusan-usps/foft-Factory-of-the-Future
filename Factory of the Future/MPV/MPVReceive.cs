using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Websocket.Client;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Factory_of_the_Future.MPV
{
    public static class MPVReceive
    {
        public static WebsocketClient client = null;
        private static readonly object MPVLock = new object();
       
        public static void Init()
        {
            try
            {
                client = new WebsocketClient(new Uri(@"ws://56.143.124.241:8085/selsRt/ebrServer"));
                client.ReconnectTimeout = TimeSpan.FromSeconds(30);
                JObject options = new JObject();
                options.Add("action", "ebrTerminalConnection");
                options.Add("name", "ebr-all");

                client.MessageReceived.Subscribe(message =>
                {
                    if (!message.Text.Contains("Connected to ebrSocketAPI server"))
                    {
                        try
                        {
                            JObject jo = JObject.Parse(message.Text);
                            BroadCastMPVTag2(jo);
                        }
                        catch (Exception ex)
                        {
                            // intentionally not using exception
                        }
                    }
                         
                });
                client.Send(options.ToString());
                client.Start();
            }
               catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private static JArray allPersons = new JArray();


      
        private static void BroadCastMPVTag2(JObject allData)
        {
            lock (MPVLock)
            {
                JObject result = new JObject();
                /*
                JObject tagData = (JObject)allData["tag"];
                string badgeID = tagData["tagId"].ToString();
                JArray zones = new JArray();
               
                
                    JObject zonesData = (JObject)tagData["zones"];
                    foreach (KeyValuePair<string, JToken> z  in zonesData)
                    {
                        JObject thisZone = new JObject();
                        thisZone.Add("id", z.Key);
                        thisZone.Add("name", ((JObject)z.Value)["zoneName"].ToString());
                        
                        zones.Add(thisZone);
                    }
                //JObject personDBData = GetPerson(badgeID, badgeID);


                JObject jPersonExisting = null;
                try
                {
                    jPersonExisting = ((JObject)allPersons[badgeID]);
                    JArray insideZones = (JArray)jPersonExisting["zones"];
                    if (insideZones.Count > 0)
                    {
                        foreach (JObject z in insideZones)
                        {
                            bool includeZone = true;
                            foreach (JObject z2 in zones)
                            {
                                if (tagData["action"].ToString() == "tagMovedOutZone")
                                {
                                    if (z2["id"].ToString() == z["id"].ToString())
                                        includeZone = false;
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                }
                */
                JObject jPerson = new JObject();
                
                jPerson.Add("Employee_Name", "");
                jPerson.Add("Employee_EIN", "");
                jPerson.Add("badgeID", "");
                jPerson.Add("badge_name", "");
                jPerson.Add("lastRingType", "");
                jPerson.Add("lastRingTime", 0);
                jPerson.Add("begintour", 0);
                jPerson.Add("outlunch", 0);
                jPerson.Add("inlunch", 0);
                jPerson.Add("endtour", 0);
                jPerson.Add("Employee_Type", "");
                jPerson.Add("Zones", "EBR-016");
                JArray tagList = new JArray();
                tagList.Add(jPerson);
                string tagCount = allPersons.Count.ToString();
                 result.Add("tagCount", tagCount);
                result.Add("taglist", tagList);
                 //Clients.All.updateMPV2Page(result);
                



            }



        }
        //End TEMP MPV(EBR Replacement)
    }
}