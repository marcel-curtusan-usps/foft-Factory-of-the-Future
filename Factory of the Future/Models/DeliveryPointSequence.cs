using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace Factory_of_the_Future.Models
{
    public class DeliveryPointSequence
    {

        [JsonProperty("run_start_modsday")]
        public string RunStartModsday { get; set; } = "";

        [JsonProperty("sortplan_name_perf")]
        public string SortplanNamePerf { get; set; } = "";

        [JsonProperty("current_operation_id")]
        public string CurrentOperationId { get; set; } = "";

        [JsonProperty("pieces_fed_1st_cnt")]
        public string PiecesFed1stCnt { get; set; } = "";

        [JsonProperty("pieces_rejected_1st_cnt")]
        public string PiecesRejected1stCnt { get; set; } = "";

        [JsonProperty("pieces_to_2nd_pass")]
        public string PiecesTo2ndPass { get; set; } = "";

        [JsonProperty("op_time_1st")]
        public string OpTime1st { get; set; } = "";

        [JsonProperty("thruput_1st_pass")]
        public string Thruput1stPass { get; set; } = "";

        [JsonProperty("pieces_fed_2nd_cnt")]
        public string PiecesFed2ndCnt { get; set; } = "";

        [JsonProperty("pieces_rejected_2nd_cnt")]
        public string PiecesRejected2ndCnt { get; set; } = "";

        [JsonProperty("op_time_2nd")]
        public string OpTime2nd { get; set; } = "";

        [JsonProperty("thruput_2nd_pass")]
        public string Thruput2ndPass { get; set; } = "";

        [JsonProperty("pieces_remaining")]
        public string PiecesRemaining { get; set; } = "";

        [JsonProperty("thruput_optimal_cfg")]
        public string ThruputOptimalCfg { get; set; } = "";

        [JsonProperty("time_to_comp_optimal")]
        public int TimeToCompOptimal { get; set; } = 0;

        [JsonProperty("thruput_actual")]
        public string ThruputActual { get; set; } = "";

        [JsonProperty("time_to_comp_actual")]
        public int TimeToCompActual { get; set; } = 0;

        [JsonProperty("rpg_2nd_pass_end")]
        public string Rpg2ndPassEnd { get; set; } = "";

        [JsonProperty("time_to_2nd_pass_optimal")]
        public string TimeTo2ndPassOptimal { get; set; } = "";

        [JsonProperty("rec_2nd_pass_start_optimal")]
        public string Rec2ndPassStartOptimal { get; set; } = "";

        [JsonProperty("time_to_2nd_pass_actual")]
        public string TimeTo2ndPassActual { get; set; } = "";

        [JsonProperty("rec_2nd_pass_start_actual")]
        public string Rec2ndPassStartActual { get; set; } = "";

        [JsonProperty("time_to_comp_optimal_DateTime")]
        public DateTime TimeToCompOptimalDateTime { get; set; } 

        [JsonProperty("time_to_comp_actual_DateTime")]
        public DateTime TimeToCompActualDateTime { get; set; }
    }
}