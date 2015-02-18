using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;

namespace TravelWithMe.API.Core.Infra
{
    public class ThreadSafeDictionary
    {
        public ThreadSafeDictionary()
        {
            Items = new ConcurrentDictionary<string, string>();
        }

        private ConcurrentDictionary<string, string> Items { get; set; }

        public string this[string name]
        {
            get
            {
                if (string.IsNullOrEmpty(name)) return null;
                string match = null;
                if (Items.TryGetValue(name, out match))
                    return match;
                else return null;
            }
            set { Items[name] = value; }
        }

        public ICollection<string> Keys
        {
            get { return Items.Keys; }
        }
    }
}
