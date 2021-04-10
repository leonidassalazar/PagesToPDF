using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Core.Interfaces.ViewModel
{
    public interface IMainWindowViewModel
    {
        event PropertyChangedEventHandler PropertyChanged;
        ObservableCollection<TabContentWrapper> Tabs { get; set; }
        TabContentWrapper HomeVieWrapper { get; set; }
    }
}