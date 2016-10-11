using System.Collections.Generic;

namespace JW18001.Models
{
    internal class DataStore
    {
        public string StartDateTime { get; set; }
        public string EndDateTime { get; set; }
        public string ItemCode { get; set; }
        public string WorkOrder { get; set; }
        public string SerialNumber { get; set; }
        public string Outcome { get; set; }
        public List<string> ItemTestname { get; set; }
        public List<string> ItemTestOutcome { get; set; }
        public List<string> ItemTestStartDateTime { get; set; }
        public List<string> ItemTestEndDateTime { get; set; }
        public List<string> SubItemTestname { get; set; }
        public List<string> SubItemTestOutcome { get; set; }
        public List<string> SubItemTestDescription { get; set; }
        public List<string> SubItemTestcValue { get; set; }
        public string ItemName { get; set; }
        public int OperationSequence { get; set; }
        public string SiteCode { get; set; }
        public string Description { get; set; }
        public string Product { get; set; }
        public string ProductLine { get; set; }
        public string SystemOperator { get; set; }
        public string AteName { get; set; }
        public string AteVersion { get; set; }
        public string CDefinitionName { get; set; }
        public string CDefinitionVersion { get; set; }
        public string OpName { get; set; }
        public string OpNumber { get; set; }
        public string OpTeam { get; set; }
        public string OpDepartment { get; set; }
        public string Remark { get; set; }
    }
}