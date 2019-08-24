using JysanBot.DTOs;
using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JysanBot.Database
{
    public class GenericRepository<T> where T : class
    {
        private readonly LiteDatabase _database;

        public IEnumerable<T> Read()
        {
            var collection = _database.GetCollection<T>();
            return collection.FindAll();
        }

        public T Read(int id)
        {
            var collection = _database.GetCollection<T>();
            return collection.FindById(id);
        }

        public void Update (T item)
        {
            var collection = _database.GetCollection<T>();
            collection.Update(item);
        }

        public void Add(T item)
        {
            var collection = _database.GetCollection<T>();
            collection.Insert(item);
        }

        public GenericRepository()
        {
            _database = new LiteDatabase(EnvironmentVariables.DatabaseUrl);
        }

        internal void Add(User user)
        {
            throw new NotImplementedException();
        }
    }
}
