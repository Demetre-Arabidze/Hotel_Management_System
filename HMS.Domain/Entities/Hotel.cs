using System.ComponentModel.DataAnnotations;

namespace HMS.Domain.Entities
{
    public class Hotel
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = null!;
        public decimal Rating { get; set; }
        public string Country { get; set; } = null!;
        public string City { get; set; } = null!;
        public string Address { get; set; } = null!;

        public ICollection<Manager> Managers { get; set; } = new List<Manager>();
        public ICollection<Room> Rooms { get; set; } = new List<Room>();
    }
}
