using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Factory_of_the_Future.Models
{
    public class RPGPlan
    {
        [JsonProperty("mods_date")]
        public DateTime ModsDate { get; set; }

        [JsonProperty("machine_num")]
        public int MachineNum { get; set; }

        [JsonProperty("sort_program_name")]
        public string SortProgramName { get; set; }

        [JsonProperty("rpg_start_dtm")]
        public DateTime RpgStartDtm { get; set; }

        [JsonProperty("rpg_end_dtm")]
        public DateTime RpgEndDtm { get; set; }

        [JsonProperty("rpg_pieces_fed")]
        public int RpgPiecesFed { get; set; }

        [JsonProperty("mail_operation_nbr")]
        public int MailOperationNbr { get; set; }

        [JsonProperty("rpg_expected_thruput")]
        public int RpgExpectedThruput { get; set; }

        [JsonProperty("mpew_start_15min_dtm")]
        public DateTime MpewStart15minDtm { get; set; }

        [JsonProperty("mpew_end_15min_dtm")]
        public DateTime MpewEnd15minDtm { get; set; }

        [JsonProperty("mpe_type")]
        public string MpeType { get; set; }

        [JsonProperty("mpe_name")]
        public string MpeName { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }
    }
}