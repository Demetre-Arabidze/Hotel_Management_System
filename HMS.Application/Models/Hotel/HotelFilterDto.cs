using System;
using System.Collections.Generic;
using System.Text;

namespace HMS.Application.Models.Hotel
{
    public class HotelFilterDto
    {
        public string? Country { get; set; }
        public string? City { get; set; }
        public decimal? MinRating { get; set; }
    }
}
