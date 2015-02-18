using System;
using System.Web.Caching;
using System.Web;
using System.Threading;

namespace TravelWithMe.API.Core.Caching.Http
{
    public static class ApplicationCache
    {
        private static Cache Current
        {
            get
            {
                return HttpRuntime.Cache;
            }
        }

        private static ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();

        public static object Add(string key, object value, CacheDependency dependencies, DateTime absoluteExpiration, TimeSpan slidingExpiration, CacheItemPriority priority, CacheItemRemovedCallback onRemoveCallback)
        {
            _lock.EnterWriteLock();
            try
            {
                return ApplicationCache.Current.Add(key, value, dependencies, absoluteExpiration, slidingExpiration, priority, onRemoveCallback);
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }

        public static void Insert(string key, object value)
        {
            _lock.EnterWriteLock();
            try
            {
                ApplicationCache.Current.Insert(key, value);
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }

        public static void Insert(string key, object value, CacheDependency dependencies)
        {
            _lock.EnterWriteLock();
            try
            {
                ApplicationCache.Current.Insert(key, value, dependencies);
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }

        public static void Insert(string key, object value, CacheDependency dependencies, DateTime absoluteExpiration, TimeSpan slidingExpiration)
        {
            _lock.EnterWriteLock();
            try
            {
                ApplicationCache.Current.Insert(key, value, dependencies, absoluteExpiration, slidingExpiration);
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }

        public static void Insert(string key, object value, CacheDependency dependencies, DateTime absoluteExpiration, TimeSpan slidingExpiration, CacheItemUpdateCallback onUpdateCallback)
        {
            _lock.EnterWriteLock();
            try
            {
                ApplicationCache.Current.Insert(key, value, dependencies, absoluteExpiration, slidingExpiration, onUpdateCallback);
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }

        public static void Insert(string key, object value, CacheDependency dependencies, DateTime absoluteExpiration, TimeSpan slidingExpiration, CacheItemPriority priority, CacheItemRemovedCallback onRemoveCallback)
        {
            _lock.EnterWriteLock();
            try
            {
                ApplicationCache.Current.Insert(key, value, dependencies, absoluteExpiration, slidingExpiration, priority, onRemoveCallback);
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }

        public static object Remove(string key)
        {
            _lock.EnterWriteLock();
            try
            {
                return ApplicationCache.Current.Remove(key);
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }

        public static object GetValue(string key)
        {
            try
            {
                _lock.EnterReadLock();
                try
                {
                    return ApplicationCache.Current[key];
                }
                finally
                {
                    _lock.ExitReadLock();
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static void SetValue(string key, object value)
        {
            try
            {
                _lock.EnterWriteLock();
                try
                {
                    ApplicationCache.Current.Insert(key, value);
                }
                finally
                {
                    _lock.ExitWriteLock();
                }
            }
            catch (Exception ex)
            {
            }
        }
    }
}
