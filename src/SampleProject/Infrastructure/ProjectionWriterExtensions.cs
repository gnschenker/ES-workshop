using System;
using System.Threading.Tasks;

namespace SampleProject.Infrastructure
{
    public static class ProjectionWriterExtensions
    {
        public static async Task Add<TId, TView>(this IProjectionWriter<TId, TView> writer, TId key, TView item) where TView : class
        {
            await writer.AddOrUpdate(key, () => item, null, false);
        }

        public static async Task Add<TId, TView>(this IProjectionWriter<TId, TView> writer, TId key, Func<TView> addFactory) where TView : class
        {
            await writer.AddOrUpdate(key, addFactory, null, false);
        }

        public static async Task Update<TId, TView>(this IProjectionWriter<TId, TView> writer, TId key, Action<TView> update) where TView : class
        {
            await writer.AddOrUpdate(key, null, x => { update(x); return x; });
        }

        public static async Task Update<TId, TView>(this IProjectionWriter<TId, TView> writer, TId key, Func<TView, TView> update) where TView : class
        {
            await writer.AddOrUpdate(key, null, update);
        }

        public static async Task UpdateEnforcingNew<TId, TView>(this IProjectionWriter<TId, TView> writer, TId key, Action<TView> update) 
            where TView : class, new()
        {
            await writer.AddOrUpdate(key, () => new TView(), x => { update(x); return x; });
        }
    }
}