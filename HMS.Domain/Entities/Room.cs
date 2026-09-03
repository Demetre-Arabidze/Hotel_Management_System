namespace HMS.Domain.Entities
{
    public class Room
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = null!;
        public decimal Price { get; set; }

        public Guid HotelId { get; set; }
        public Hotel Hotel { get; set; } = null!;

        public ICollection<ReservationRoom> ReservationRooms { get; set; } = new List<ReservationRoom>();
    }
}
