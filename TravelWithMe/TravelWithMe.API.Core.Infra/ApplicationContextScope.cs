using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Web;

namespace TravelWithMe.API.Core.Infra
{
    public class ApplicationContextScope : IDisposable
    {
        [ThreadStatic]
        private static int _level;

        public ApplicationContextScope()
            : this(ApplicationContext.Current)
        {
        }

        public ApplicationContextScope(ApplicationContext context)
        {
            _level++;
            if (_level == 1)
                ApplicationContext.Current = context;
        }

        #region IDisposable Members

        public void Dispose()
        {
            _level--;
            if (_level == 0)
                ApplicationContext.Current = null;
        }

        #endregion
    }
}
