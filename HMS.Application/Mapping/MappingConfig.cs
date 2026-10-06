using HMS.Application.Models.Guest;
using HMS.Application.Models.Hotel;
using HMS.Application.Models.Manager;
using HMS.Application.Models.Reservation;
using HMS.Application.Models.Review;
using HMS.Application.Models.Room;
using HMS.Domain.Entities;
using Mapster;

namespace HMS.Application.Mapping
{
    public class MappingConfig : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            // Hotel Mappings
            config.NewConfig<Hotel, HotelResponseDto>();
            config.NewConfig<Hotel, HotelWithManagerDto>();
            config.NewConfig<HotelCreateUpdateDto, Hotel>();

            // Room Mappings
            config.NewConfig<Room, RoomResponseDto>();
            config.NewConfig<RoomCreateUpdateDto, Room>();

            // Guest Mappings
            config.NewConfig<Guest, GuestResponseDto>();
            config.NewConfig<GuestCreateUpdateDto, Guest>();

            // Manager Mappings
            config.NewConfig<Manager, ManagerResponseDto>();
            config.NewConfig<Manager, GetManagerDto>();
            config.NewConfig<ManagerCreateUpdateDto, Manager>();

            // Reservation Mappings
            config.NewConfig<Reservation, ReservationResponseDto>()
                .Map(dest => dest.GuestFullName, src => $"{src.Guest.FirstName} {src.Guest.LastName}")
                .Map(dest => dest.RoomNames, src => src.ReservationRooms.Select(rr => rr.Room.Name));

            // Review Mappings
            config.NewConfig<Review, ReviewResponseDto>();
            config.NewConfig<CreateReviewDto, Review>();
            config.NewConfig<UpdateReviewDto, Review>();
        }
    }
}
