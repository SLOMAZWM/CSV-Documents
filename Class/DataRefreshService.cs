using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Profisys_Zadanie.Class
{
    public static class DataRefreshService
    {
        public static event Action OnDataRefreshRequested;

        public static void RequestDataRefresh()
        {
            OnDataRefreshRequested?.Invoke();
        }
    }

}
