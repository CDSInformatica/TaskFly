using System;
using System.Collections.Generic;
using System.Text;

namespace TaskFly.Entities
{
    public class TaskComments
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public int? UserId { get; set; }
        public string User { get; set; }
        public int PhaseId { get; set; }
        public string Phase { get; set; }
        public string Description { get; set; }
    }
}
