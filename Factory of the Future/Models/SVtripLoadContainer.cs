using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Factory_of_the_Future.Models
{
    public class EmptyLoadCountEventDtm
    {
        [JsonProperty("year")]
        public int Year { get; set; }

        [JsonProperty("month")]
        public int Month { get; set; }

        [JsonProperty("dayOfMonth")]
        public int DayOfMonth { get; set; }

        [JsonProperty("hourOfDay")]
        public int HourOfDay { get; set; }

        [JsonProperty("minute")]
        public int Minute { get; set; }

        [JsonProperty("second")]
        public int Second { get; set; }
    }

    public class EmptyUnloadCountEventDtm
    {
        [JsonProperty("year")]
        public int Year { get; set; }

        [JsonProperty("month")]
        public int Month { get; set; }

        [JsonProperty("dayOfMonth")]
        public int DayOfMonth { get; set; }

        [JsonProperty("hourOfDay")]
        public int HourOfDay { get; set; }

        [JsonProperty("minute")]
        public int Minute { get; set; }

        [JsonProperty("second")]
        public int Second { get; set; }
    }

    public class EventDtm
    {
        [JsonProperty("year")]
        public int Year { get; set; }

        [JsonProperty("month")]
        public int Month { get; set; }

        [JsonProperty("dayOfMonth")]
        public int DayOfMonth { get; set; }

        [JsonProperty("hourOfDay")]
        public int HourOfDay { get; set; }

        [JsonProperty("minute")]
        public int Minute { get; set; }

        [JsonProperty("second")]
        public int Second { get; set; }
    }

    public class LoadedCtrHuDetail
    {
        [JsonProperty("SQLTypeName")]
        public string SQLTypeName { get; set; }

        [JsonProperty("barcode")]
        public string Barcode { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("eventDtm")]
        public EventDtm EventDtm { get; set; }

        [JsonProperty("siteId")]
        public string SiteId { get; set; }

        [JsonProperty("updtUserId")]
        public string UpdtUserId { get; set; }

        [JsonProperty("containerTypeCode")]
        public string ContainerTypeCode { get; set; }

        [JsonProperty("huCount")]
        public int HuCount { get; set; }

        [JsonProperty("pieceCount")]
        public int PieceCount { get; set; }

        [JsonProperty("mailClassCode")]
        public string MailClassCode { get; set; }

        [JsonProperty("mailTypeCode")]
        public string MailTypeCode { get; set; }
    }

    public class ManuallyEnteredCount
    {
        [JsonProperty("SQLTypeName")]
        public string SQLTypeName { get; set; }

        [JsonProperty("containerTypeCode")]
        public string ContainerTypeCode { get; set; }

        [JsonProperty("emptyLoadCount")]
        public int EmptyLoadCount { get; set; }

        [JsonProperty("emptyLoadCountEventDtm")]
        public EmptyLoadCountEventDtm EmptyLoadCountEventDtm { get; set; }

        [JsonProperty("emptyUnloadCount")]
        public int EmptyUnloadCount { get; set; }

        [JsonProperty("emptyUnloadCountEventDtm")]
        public EmptyUnloadCountEventDtm EmptyUnloadCountEventDtm { get; set; }

        [JsonProperty("unscanLdCount")]
        public int UnscanLdCount { get; set; }

        [JsonProperty("unscanUnldCount")]
        public int UnscanUnldCount { get; set; }

        [JsonProperty("destBundleCount")]
        public int DestBundleCount { get; set; }

        [JsonProperty("destPieceCount")]
        public int DestPieceCount { get; set; }

        [JsonProperty("destPackageCount")]
        public int DestPackageCount { get; set; }
    }

    public class SVtripLoadContainers
    {
        [JsonProperty("SQLTypeName")]
        public string SQLTypeName { get; set; }

        [JsonProperty("loadPercent")]
        public int LoadPercent { get; set; }

        [JsonProperty("loadCount")]
        public int LoadCount { get; set; }

        [JsonProperty("unloadCount")]
        public int UnloadCount { get; set; }

        [JsonProperty("loadedCtrHuDetails")]
        public List<LoadedCtrHuDetail> LoadedCtrHuDetails { get; set; }

        [JsonProperty("manuallyEnteredCounts")]
        public List<ManuallyEnteredCount> ManuallyEnteredCounts { get; set; }
    }

}