namespace HMS.Domain.Entities
{
    public class Manager
    {
        public Guid Id { get; set; }

        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string PersonalNumber { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;

        public Guid UserId { get; set; }

        public Guid HotelId { get; set; }
        public Hotel Hotel { get; set; } = null!;
    }
}
