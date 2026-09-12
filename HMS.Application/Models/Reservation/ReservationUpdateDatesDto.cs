namespace HMS.Application.Models.Reservation
{
    public class ReservationUpdateDatesDto
    {
        public DateOnly CheckInDate { get; set; }
        public DateOnly CheckOutDate { get; set; }
    }
}
