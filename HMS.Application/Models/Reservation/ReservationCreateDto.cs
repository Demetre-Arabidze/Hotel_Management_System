namespace HMS.Application.Models.Reservation
{
    public class ReservationCreateDto
    {
        public List<Guid> RoomIds { get; set; } = new();
        public DateOnly CheckInDate { get; set; }
        public DateOnly CheckOutDate { get; set; }
    }
}
