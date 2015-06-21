using System;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Dapper;

namespace SampleProject.Infrastructure
{
    public class SqlServerProjectionWriter<TId, T> : IProjectionWriter<TId, T>
        where T : class
    {
        private readonly string _connectionString;
        private readonly string _insertSql;
        private readonly string _selectSql;
        private readonly string _updateSql;

        public SqlServerProjectionWriter(string connectionString)
        {
            _connectionString = connectionString;
            var type = typeof (T);
            const BindingFlags flags = BindingFlags.Instance | BindingFlags.Public;
            var names = string.Join(",", type.GetProperties(flags).Select(x => x.Name));
            var parameters = string.Join(",", type.GetProperties().Select(x => "@" + x.Name));
            var list = type.GetProperties()
                .Where(x => x.Name.ToLower() != "id")
                .Select(x => string.Format("{0}=@{0}", x.Name));
            var updates = string.Join(",", list);
            _selectSql = string.Format("SELECT * FROM {0} WHERE Id=@id", type.Name);
            _insertSql = string.Format("INSERT INTO {0}({1}) VALUES({2})", type.Name, names, parameters);
            _updateSql = string.Format("UPDATE {0} SET {1} WHERE Id=@id", type.Name, updates);
        }

        public async Task<T> AddOrUpdate(TId key, Func<T> addFactory, Func<T, T> update, bool probablyExists = true)
        {
            if (probablyExists)
            {
                return await Update(key, addFactory, update);
            }
            var item = addFactory();
            await Add(item);
            return item;
        }

        private async Task Add(T item)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                using (var tx = conn.BeginTransaction())
                {
                    await conn.ExecuteAsync(_insertSql, item, tx);
                    tx.Commit();
                }
            }
        }

        private async Task<T> Update(TId id, Func<T> addFactory, Func<T, T> update)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                using (var tx = conn.BeginTransaction())
                {
                    var item = (await conn.QueryAsync<T>(_selectSql, new {id}, tx)).SingleOrDefault();
                    if (item != null)
                    {
                        update(item);
                        await conn.ExecuteAsync(_updateSql, item, tx);
                        tx.Commit();
                        return item;
                    }

                    if (addFactory == null)
                        throw new InvalidOperationException("Item does not exists");

                    item = addFactory();
                    update(item);
                    await conn.ExecuteAsync(_insertSql, item, tx);
                    tx.Commit();
                    return item;
                }
            }
        }
    }
}