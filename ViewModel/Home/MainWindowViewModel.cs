using Core;
using Core.Interfaces;
using Core.Interfaces.ViewModel;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace WPF.ViewModel.Home
{
    public class MainWindowViewModel : INotifyPropertyChanged, IMainWindowViewModel
    {
        public event PropertyChangedEventHandler PropertyChanged;

        #region Fields

        private readonly IViewManager _viewManager;
        private ObservableCollection<TabContentWrapper> _tabs;
        private TabContentWrapper _homeVieWrapper;

        #endregion

        #region Properties

        public ObservableCollection<TabContentWrapper> Tabs
        {
            get => _tabs;
            set
            {
                _tabs = value;
                OnPropertyChanged();
            }
        }

        public TabContentWrapper HomeVieWrapper
        {
            get => _homeVieWrapper;
            set
            {
                _homeVieWrapper = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Contructor

        public MainWindowViewModel(IViewManager viewManager)
        {
            _viewManager = viewManager;

            Tabs = new ObservableCollection<TabContentWrapper>();
        }

        #endregion


        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
