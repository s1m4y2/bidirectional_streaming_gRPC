using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models
{
    public class Car
    {
        public int Id { get; set; }
        public string CarId { get; set; }  // örnek: "car1", "car2"
        public string Status { get; set; } // örnek: "Running", "Maintenance"
    }

}
