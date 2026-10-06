using HMS.Application.Models.Manager;

namespace HMS.Application.Models.Hotel
{
    public class HotelWithManagerDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Rating { get; set; }
        public string Address { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;

        public List<GetManagerDto> Managers { get; set; } = new();
    }
}
