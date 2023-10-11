using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Week7Sample.Data.Repositories.Interfaces;
using Week7Sample.Model;

namespace Week7Sample.Data.Repositories.Implementation
{
    public class UserRepository : IUserRepository
    {
        private readonly Week7Context _context;

        public UserRepository(Week7Context context)
        {
            _context = context;
        }

        public User AddNew(User entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            _context.Add(entity);
            var user = _context.SaveChanges();
            if (user > 0)

                return entity;

            return null;
        }

        public User[] AddNew(User[] entities)
        {
            if (entities.Count() < 1)
                throw new ArgumentNullException(nameof(entities));

            _context.AddRange(entities);
            var user = _context.SaveChanges();
            if (user > 0)

                return entities;

            return null;
        }

        public bool Delete(User entity)
        {
            if (entity == null)
                throw new NotImplementedException(nameof(entity));

            _context.Remove(entity);
            var user = _context.SaveChanges();
            if (user > 0)

                return true;

            return false;
        }

        public bool Delete(List<User> entities)
        {
            if (entities.Count() < 1)
                throw new ArgumentNullException(nameof(entities));

            _context.RemoveRange(entities);
            var user = _context.SaveChanges();
            if (user > 0)

                return true;

            return false;
        }

        public IEnumerable<User> GetAll()
        {
            return _context.Users.ToList();
        }

        public User GetById(string id)
        {
            return _context.Users.FirstOrDefault(x => x.Id == id);
        }

        //        public IEnumerable<User> GetUsersBypagination(int page, int pageSize)
        public IEnumerable<User> GetUsersBypagination(List<User> list, int perpage, int page)
        {
            page = page < 1 ? 1 : page;
            perpage = perpage < 1 ? 5 : page;

            if (list.Count > 0)
            {
                var paginated = list.Skip((page - 1) * perpage).Take(perpage).ToList();

                return paginated;
            }

            return new List<User>();
            //return _context.Users.Skip((page - 1) * pageSize).Take(pageSize).ToList();

        }
        public User Update(User entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            _context.Update(entity);
            var user = _context.SaveChanges();
            if (user > 0)

                return entity;

            return null;

        }

        public User PartialUpdate(User entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            var existingUser = _context.Users.FirstOrDefault(x => x.Id == entity.Id);

            if (existingUser == null)
            {
                return null;
            }

            existingUser.LastName = entity.LastName;

            _context.Update(existingUser);
            var user = _context.SaveChanges();

            if (user > 0)
            {
                return existingUser;
            }

            return null;
        }
    }
}
