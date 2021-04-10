using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Interfaces
{
    public interface ITabView
    {
        string Icon { get; set; }
        string Title { get; set; }
        object DataContext { get; set; }
        int Index { get; set; }
    }
}
