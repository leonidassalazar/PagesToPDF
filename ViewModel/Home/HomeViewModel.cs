using Core.Interfaces;
using Core.Interfaces.ViewModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace WPF.ViewModel.Home
{
    public class HomeViewModel : IHomeViewModel
    {
        public event PropertyChangedEventHandler PropertyChanged;

        #region Fields

        private readonly IViewManager _viewManager;

        #endregion

        #region Properties



        #endregion

        #region Constructor

        public HomeViewModel(IViewManager viewManager)
        {
            _viewManager = viewManager;
        }

        #endregion

        #region Commands

        

        #endregion

        [Annotations.NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
