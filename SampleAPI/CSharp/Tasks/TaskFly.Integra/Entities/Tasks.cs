using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskFly.Entities
{
    public class Tasks
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public int UserId { get; set; }
        public string User { get; set; }
        public int CreatedById { get; set; }
        public string CreatedBy { get; set; }
        public int? CustomerId { get; set; }
        public string Customer { get; set; }
        public int? ProjectId { get; set; }
        public string Project { get; set; }
        public string Description { get; set; }
        public int? PhaseId { get; set; }
        public string Phase { get; set; }
        public int? PriorityId { get; set; }
        public string Priority { get; set; }
        public int? TypeId { get; set; }
        public string Type { get; set; }
        public DateTime EstimatedDate { get; set; }
        public string StartTime { get; set; }
        public string Effort { get; set; }
        public decimal EstimatedCost { get; set; }
        public int? GroupId { get; set; }
        public string Group { get; set; }
        public int Delay { get; set; }
        public bool TimeTrackerOn { get; set; }
        public DateTime? CompletedData { get; set; }
        public DateTime CreatedDate { get; set; }
        public string TimePerformed { get; set; }
        public string TAG { get; set; }
        public int TotalComments { get; set; }
    }
}
