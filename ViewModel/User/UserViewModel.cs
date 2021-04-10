using System.ComponentModel;
using System.Runtime.CompilerServices;
using Core.Interfaces;

namespace WPF.ViewModel.User
{
    public class UserViewModel : ITabViewModel
    {
        public event PropertyChangedEventHandler PropertyChanged;

        #region Fields

        private readonly IViewManager _viewManager;

        #endregion

        #region Properties



        #endregion

        #region Constructor

        public UserViewModel(IViewManager viewManager)
        {
            _viewManager = viewManager;
        }

        #endregion

        [Annotations.NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
