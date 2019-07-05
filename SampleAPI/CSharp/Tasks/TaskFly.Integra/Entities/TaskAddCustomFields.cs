using System;
using System.Collections.Generic;
using System.Text;

namespace TaskFly.Entities
{
    public class TaskAddCustomFields
    {
        public int CustomFieldId { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
    }
}
