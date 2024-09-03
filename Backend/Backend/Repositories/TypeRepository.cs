using Backend.Data;
using Backend.Domain.Inputs.DTO;
using Backend.Domain.Modals;
using Backend.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Backend.Repositories
{
    public class TypeRepository : ITypeRepository
    {
        private readonly GameDbContext context;
        private readonly ILogger<TypeRepository> logger;

        public TypeRepository(GameDbContext context, ILogger<TypeRepository> logger)
        {
            this.context = context;
            this.logger = logger;
        }

        public async Task CreateAsync<T>(DbSet<T> set, AbstractTypeInput input) where T : AbstractType, new() 
        {
            T newType = new T() { Description = input.Description, TypeID = input.ID, Name = input.Name };
            await context.AddAsync(newType);
            await context.SaveChangesAsync();
        }

        public async Task DeleteAsync<T>(DbSet<T> set, int ID) where T : AbstractType
        {
            T deleteType = await GetAsync(set, ID);
            if (deleteType != null)
            {
                set.Remove(deleteType);
                await context.SaveChangesAsync();
            }
            else
            {
                throw new Exception($"Object not found! ID : {ID}");
            }
           
        }

        public async Task<T> GetAsync<T>(DbSet<T> set, int ID) where T : AbstractType => await set.FindAsync(ID);
    }
}
