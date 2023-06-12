using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Factory_of_the_Future.Models
{
    public class RunPerf
    {


        [JsonProperty("mpe_type")]
        public string MpeType { get; set; } = "";

        [JsonProperty("mpe_number")]
        public int MpeNumber { get; set; }

        [JsonProperty("bins")]
        public int Bins { get; set; }

        [JsonProperty("cur_sortplan")]
        public string CurSortplan { get; set; } = "";

        [JsonProperty("cur_thruput_ophr")]
        public int CurThruputOphr { get; set; } = 0;

        [JsonProperty("tot_sortplan_vol")]
        public int TotSortplanVol { get; set; } = 0;

        [JsonProperty("rpg_est_vol")]
        public int RpgEstVol { get; set; } = 0;

        [JsonProperty("act_vol_plan_vol_nbr")]
        public int ActVolPlanVolNbr { get; set; } = 0;

        [JsonProperty("current_run_start")]
        public string CurrentRunStart { get; set; } = "";

        [JsonProperty("current_run_end")]
        public string CurrentRunEnd { get; set; } = "";

        [JsonProperty("cur_operation_id")]
        public int CurOperationId { get; set; } = 0;

        [JsonProperty("bin_full_status")]
        public int BinFullStatus { get; set; } = 0;

        [JsonProperty("bin_full_bins")]
        public string BinFullBins { get; set; } = "";

        [JsonProperty("throughput_status")]
        public int ThroughputStatus { get; set; } = 0;

        [JsonProperty("unplan_maint_sp_status")]
        public int UnplanMaintSpStatus { get; set; } = 0;

        [JsonProperty("op_started_late_status")]
        public int OpStartedLateStatus { get; set; } = 0;

        [JsonProperty("op_running_late_status")]
        public int OpRunningLateStatus { get; set; } = 0;

        [JsonProperty("sortplan_wrong_status")]
        public int SortplanWrongStatus { get; set; } = 0;

        [JsonProperty("unplan_maint_sp_timer")]
        public int UnplanMaintSpTimer { get; set; } = 0;

        [JsonProperty("op_started_late_timer")]
        public int OpStartedLateTimer { get; set; } = 0;

        [JsonProperty("op_running_late_timer")]
        public int OpRunningLateTimer { get; set; } = 0;

        [JsonProperty("rpg_start_dtm")]
        public DateTime RPGStartDtm { get; set; }

        [JsonProperty("rpg_end_dtm")]
        public DateTime RPGEndDtm { get; set; }

        [JsonProperty("expected_throughput")]
        public int ExpectedThroughput { get; set; } = 0;

        [JsonProperty("sortplan_wrong_timer")]
        public int SortplanWrongTimer { get; set; } = 0;

        [JsonProperty("rpg_est_comp_time")]
        public DateTime RpgEstCompTime { get; set; } = new DateTime(1, 1, 1, 0, 0, 0);

        [JsonProperty("hourly_data")]
        public List<HourlyData> HourlyData { get; set; } = new List<HourlyData>();

        [JsonProperty("rpg_expected_thruput")]
        public int RpgExpectedThruput { get; set; } = 0;

        [JsonProperty("ars_recrej3")]
        public int ArsRecrej3 { get; set; } = 0;

        [JsonProperty("sweep_recrej3")]
        public int SweepRecrej3 { get; set; } = 0;
        public string MpeId { get; set; } = "";
    }
    public class HourlyDatum
    {
        [JsonProperty("hour")]
        public DateTime Hour { get; set; }

        [JsonProperty("count")]
        public int Count { get; set; }
    }
}