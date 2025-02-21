using Kendo.Models;

namespace Kendo.Repositories.Interfaces
{
    public interface ICityHelper
    {
        List<tblCity> GetAll();
        tblCity GetOne(int id);
        void Insert(tblCity data);
        void Update(tblCity data);
        void Delete(tblCity data);
    }
}