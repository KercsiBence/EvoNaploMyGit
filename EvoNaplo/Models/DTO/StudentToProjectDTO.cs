using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EvoNaplo.Models.DTO
{
    public class StudentToProjectDTO
    {
        public int studentId { get; set; }
        public int fromProjectId { get; set; }
        public int toProjectId { get; set; }
        
    }
}
