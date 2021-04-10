using Models.Annotations;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Models
{
    public class Novel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        #region Fiels

        private string _title;
        private string _novelCode;
        private bool _active;
        private byte[] _novelBytes;
        private List<Volume> _volumes;

        #endregion

        #region Properties

        public string Title
        {
            get => _title;
            set
            {
                if (value == _title) return;
                _title = value;
                OnPropertyChanged();
            }
        }

        public string NovelCode
        {
            get => _novelCode;
            set
            {
                if (value == _novelCode) return;
                _novelCode = value;
                OnPropertyChanged();
            }
        }

        public List<Volume> Volumes
        {
            get => _volumes;
            set
            {
                if (Equals(value, _volumes)) return;
                _volumes = value;
                OnPropertyChanged();
            }
        }

        public byte[] NovelBytes
        {
            get => _novelBytes;
            set
            {
                if (Equals(value, _novelBytes)) return;
                _novelBytes = value;
                OnPropertyChanged();
            }
        }

        public bool Active
        {
            get => _active;
            set
            {
                if (value == _active) return;
                _active = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Contructors

        public Novel()
        {
            Volumes = new List<Volume>();
        }

        #endregion
        
        #region Protected Methods

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        } 
        
        #endregion
    }
}