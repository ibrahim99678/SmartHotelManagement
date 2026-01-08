namespace Contract.Request
{
    public class CreateRoomTypeRequest
    {
        public string RoomTypeName { get; internal set; }
        public decimal DefaultRate { get; internal set; }
    }
}