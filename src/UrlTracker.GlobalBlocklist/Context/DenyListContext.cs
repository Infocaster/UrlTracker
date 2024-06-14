using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UrlTracker.GlobalBlocklist.Context
{
    public interface IDenyListContext
    {
        IReadOnlyCollection<string> List { get; }

        void SetList(IEnumerable<string> newList);
    }

    internal class DenyListContext : IDenyListContext
    {
        private List<string> _list = new();

        public IReadOnlyCollection<string> List => _list;
        public void SetList(IEnumerable<string> newList)
        {
            _list = newList.ToList();
        }
    }
}
