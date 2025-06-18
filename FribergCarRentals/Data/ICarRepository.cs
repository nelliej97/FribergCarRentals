using FribergCarRentals.Models;

namespace FribergCarRentals.Data
{
    public interface ICarRepository
    {
        Task<List<Car>> GetAllAsync();
        Task<Car?> GetByIdAsync(int id);
        Task AddAsync(Car car);
        Task UpdateAsync(Car car);
        Task DeleteAsync(Car car);
        Task<bool> ExistsAsync(int id);
    }
}
