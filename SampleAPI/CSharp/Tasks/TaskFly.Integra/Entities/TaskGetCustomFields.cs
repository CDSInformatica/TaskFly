using System;
using System.Collections.Generic;
using System.Text;

namespace TaskFly.Entities
{
    public class TaskGetCustomFields
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool Required { get; set; }
        public string Type { get; set; }
    }
}
