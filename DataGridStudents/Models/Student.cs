using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataGridStudents.Models
{
    class Student
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Phone { get; set; }
        public string GroupName { get; set; }
        public int GroupId { get; set; }

        public Student(int Id = -1, string FullName = "None", string Phone = "None", string GroupName = "None")
        {
            this.Id = Id;
            this.FullName = FullName;
            this.Phone = Phone;
            this.GroupName = GroupName;
        }
    }
}
