using Shortlist.Web.Models;

namespace Shortlist.Web.Services
{
    public interface IRentalListingsService
    {
        Task<List<RentalListingCard>> SearchRentalsAsync(RentalListingsSearchViewModel search, CancellationToken cancellationToken = default);
    }
}