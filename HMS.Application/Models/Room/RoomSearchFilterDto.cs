using System;
using System.Collections.Generic;
using System.Text;

namespace HMS.Application.Models.Room
{
    public class RoomSearchFilterDto
    {
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public DateOnly? TargetDate { get; set; }
    }
}
