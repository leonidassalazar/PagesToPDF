
using Core.Annotations;
using Core.Interfaces;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Core
{
    public class TabContentWrapper : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private bool _isSelected;
        private string _icon;
        private string _title;
        private int _index;
        private ITabView _content;
        private ITabViewModel _contentViewModel;

        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                _isSelected = value;
                OnPropertyChanged();
            }
        }
        public string Icon
        {
            get => _icon;
            set
            {
                _icon = value;
                OnPropertyChanged();
            }
        }
        public string Title
        {
            get => _title;
            set
            {
                _title = value;
                OnPropertyChanged();
            }
        }
        public int Index
        {
            get => _index;
            set
            {
                _index = value;
                OnPropertyChanged();
            }
        }
        public ITabView Content
        {
            get => _content;
            set
            {
                _content = value;
                OnPropertyChanged();
            }
        }

        public ITabViewModel ContentViewModel
        {
            get => _contentViewModel;
            set
            {
                _contentViewModel = value;
                OnPropertyChanged();
            }
        }

        public TabContentWrapper(ITabView tabView, ITabViewModel viewModel)
        {
            Icon = tabView.Icon;
            Title = tabView.Title;
            Index = tabView.Index;
            Content = tabView;
            ContentViewModel = viewModel;
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
