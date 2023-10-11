using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Week7Sample.Model;

namespace Week7Sample.Data.Repositories.Interfaces
{
    public interface IUserRepository
    {
        User AddNew(User entity);
        User Update(User entity);

        bool Delete(User entity);

        bool Delete(List<User> entities);

        User GetById(string id);

        User GetByEmail(string email);

        IEnumerable<User> GetUsersBypagination(List<User> list, int perpage, int page);

        IEnumerable<User> GetAll();

    }
}
