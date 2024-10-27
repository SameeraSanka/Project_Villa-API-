using MagicVilla_API.Model;
using System.Linq.Expressions;

namespace MagicVilla_API.Repository.IRepository
{
    public interface IVillaNumberRepository : IRepository<VillaNumber>
    {
        Task<VillaNumber> UpdateAsync(VillaNumber entity);
    }
}
