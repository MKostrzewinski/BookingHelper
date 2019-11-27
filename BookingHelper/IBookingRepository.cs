using System.Linq;

namespace BookingHelper
{
    interface IBookingRepository
    {
        IQueryable<Booking> GetActiveBookings(int? excludedBookingId = null);
    }
}