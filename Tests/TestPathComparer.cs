using CalcBinding.PathAnalysis;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    public class TestPathComparer : IComparer
    {
        public int Compare(object x, object y)
        {
            if (ReferenceEquals(x, null) && ReferenceEquals(y, null))
                return 0;

            if (ReferenceEquals(x, null) || ReferenceEquals(y, null))
                return -1;

            if (ReferenceEquals(x, y))
                return 0;

            var path1 = x as PathToken;
            var path2 = y as PathToken;

            if (ReferenceEquals(path1, null) || ReferenceEquals(path2, null))
                return -1;

            var res = (path1.GetType() == path2.GetType() && path1.Start == path2.Start && path1.End == path2.End && path1.Id.Equals(path2.Id)) ? 0 : -1;

            return res;
        }
    }
}
