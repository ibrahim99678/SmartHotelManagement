using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHotelManagement.Model
{
    public class Entity
    {
        public int CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }= DateTime.UtcNow;
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedAt { get; set; }= DateTime.UtcNow;
    }
}
