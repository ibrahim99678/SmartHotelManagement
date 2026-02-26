using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHotelManagement.BLL.Interfaces;

public interface IRoomChangeService
{
    Task ChangeRoomAsync(int reservationId, int newRoomId, string reason);
}
