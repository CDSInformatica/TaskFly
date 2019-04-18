using System;
using System.Collections.Generic;
using System.Text;

namespace TaskFly.Entities
{
    public class UsersTasks
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int SectorId { get; set; }
        public string Sector { get; set; }
        public int TaskCount { get; set; }
    }
}
