using Backend.Domain.Inputs.DTO;
using Backend.Domain.Modals;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Backend.Repositories.Interfaces
{
    public interface ITypeRepository
    {


        Task CreateAsync<T>(DbSet<T> set, AbstractTypeInput input) where T : AbstractType, new();
        Task DeleteAsync<T>(DbSet<T> set, int ID) where T : AbstractType;

        Task<T> GetAsync<T>(DbSet<T> set, int ID) where T : AbstractType;



    }
}
