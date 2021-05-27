using System.Web;
using System.Collections.Generic;

namespace Empire.Shared.Utilities
{
    public class CacheAdapter<T> where T : class
    {
        #region Constants
        public const int SlidingExpirationHours = 4;
        #endregion

        #region Properties
        public string Key
        {
            get;
            set;
        }
        #endregion

        #region Constructors
        public CacheAdapter()
        {
            this.Key = typeof(T).FullName;
        }
        #endregion

        #region Methods
        public T GetSingle()
        {
            return HttpContext.Current.Items[this.Key] as T;
        }
        public ICollection<T> GetCollection()
        {
            return HttpContext.Current.Items[this.Key] as ICollection<T>;
        }

        public void PutSingle(T oItem)
        {
            this.Put(oItem);
        }
        public void PutCollection(ICollection<T> lstItems)
        {
            this.Put(lstItems);
        }

        protected void Put(object oItem)
        {
            HttpContext.Current.Items[this.Key] = oItem;

            //HttpContext.Current.Cache.Add(this.Key, oItem, null, Cache.NoAbsoluteExpiration, TimeSpan.FromHours(SlidingExpirationHours), CacheItemPriority.Default, null);
        }

        #endregion
    }
}