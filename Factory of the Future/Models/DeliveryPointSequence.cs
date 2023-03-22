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
        public DateTime RunStartModsday { get; set; } = new DateTime(1, 1, 1, 0, 0, 0);

        [JsonProperty("sortplan_name_perf")]
        public string SortplanNamePerf { get; set; } = "";

        [JsonProperty("current_operation_id")]
        public int CurrentOperationId { get; set; } = 0;

        [JsonProperty("pieces_fed_1st_cnt")]
        public int PiecesFed1stCnt { get; set; } = 0;

        [JsonProperty("pieces_rejected_1st_cnt")]
        public int PiecesRejected1stCnt { get; set; } = 0;

        [JsonProperty("pieces_to_2nd_pass")]
        public int PiecesTo2ndPass { get; set; } = 0;

        [JsonProperty("op_time_1st")]
        public int OpTime1st { get; set; } = 0;

        [JsonProperty("thruput_1st_pass")]
        public int Thruput1stPass { get; set; } = 0;

        [JsonProperty("pieces_fed_2nd_cnt")]
        public int PiecesFed2ndCnt { get; set; } = 0;

        [JsonProperty("pieces_rejected_2nd_cnt")]
        public int PiecesRejected2ndCnt { get; set; } = 0;

        [JsonProperty("op_time_2nd")]
        public int OpTime2nd { get; set; } = 0;

        [JsonProperty("thruput_2nd_pass")]
        public int Thruput2ndPass { get; set; } = 0;

        [JsonProperty("pieces_remaining")]
        public int PiecesRemaining { get; set; } = 0;

        [JsonProperty("thruput_optimal_cfg")]
        public int ThruputOptimalCfg { get; set; } = 0;

        [JsonProperty("time_to_comp_optimal")]
        public int TimeToCompOptimal { get; set; } = 0;

        [JsonProperty("thruput_actual")]
        public int ThruputActual { get; set; } = 0;

        [JsonProperty("time_to_comp_actual")]
        public int TimeToCompActual { get; set; } = 0;

        [JsonProperty("rpg_2nd_pass_end")]
        public DateTime Rpg2ndPassEnd { get; set; } = new DateTime(1, 1, 1, 0, 0, 0);

        [JsonProperty("time_to_2nd_pass_optimal")]
        public int TimeTo2ndPassOptimal { get; set; } = 0;

        [JsonProperty("rec_2nd_pass_start_optimal")]
        public DateTime Rec2ndPassStartOptimal { get; set; } = new DateTime(1, 1, 1, 0, 0, 0);

        [JsonProperty("time_to_2nd_pass_actual")]
        public int TimeTo2ndPassActual { get; set; } = 0;

        [JsonProperty("rec_2nd_pass_start_actual")]
        public DateTime Rec2ndPassStartActual { get; set; } = new DateTime(1, 1, 1, 0, 0, 0);

        [JsonProperty("time_to_comp_optimal_DateTime")]
        public DateTime TimeToCompOptimalDateTime
        {
            get
            {
                return DateTime.Now.AddSeconds(TimeToCompOptimal);
            }
            set { return; }
        }

        [JsonProperty("time_to_comp_actual_DateTime")]
        public DateTime TimeToCompActualDateTime
        {
            get
            {
                return DateTime.Now.AddSeconds(TimeToCompOptimal);
            }
            set { return; }
        }
    }
}