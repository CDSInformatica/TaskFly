using System;
using System.Collections.Generic;
using System.Text;

namespace TaskFly.Entities
{
    public class TaskComment
    {
        public int? PhaseId { get; set; }
        public int? SendToUserId { get; set; }
        public string Description { get; set; }
    }
}
