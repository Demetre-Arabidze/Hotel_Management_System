using HMS.Application.Models.Guest;
using HMS.Application.Models.Hotel;
using HMS.Application.Models.Manager;
using HMS.Application.Models.Reservation;
using HMS.Application.Models.Room;
using HMS.Domain.Entities;
using Mapster;

namespace HMS.Application.Mapping
{
    public class MappingConfig : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<Hotel, HotelResponseDto>();
            config.NewConfig<HotelCreateUpdateDto, Hotel>();

            config.NewConfig<Room, RoomResponseDto>();
            config.NewConfig<RoomCreateUpdateDto, Room>();

            config.NewConfig<Guest, GuestResponseDto>();
            config.NewConfig<GuestCreateUpdateDto, Guest>();

            config.NewConfig<Manager, ManagerResponseDto>();
            config.NewConfig<ManagerCreateUpdateDto, Manager>();

            config.NewConfig<Reservation, ReservationResponseDto>()
                .Map(dest => dest.GuestFullName, src => $"{src.Guest.FirstName} {src.Guest.LastName}")
                .Map(dest => dest.RoomNames, src => src.ReservationRooms.Select(rr => rr.Room.Name));
        }
    }
}
