using Common.Utils;
using Interface.Annotations;
using Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Interface.ViewModel
{
    public class NovelViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        #region Fields

        private ObservableCollection<Novel> _novels;
        private readonly Configuration _configuration;

        #endregion

        #region Properties

        public ObservableCollection<Novel> Novels
        {
            get => _novels;
            set
            {
                _novels = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Constructors

        public NovelViewModel()
        {
            Novels = new ObservableCollection<Novel>();
            _configuration = Configuration.GetInstance();
            LoadNovels();
        }

        #endregion

        #region Private Methods

        private void LoadNovels()
        {
            var novels = new List<Novel>();

            var json = JsonConvert.DeserializeObject<JObject>(_configuration.GetNode("wuxiaworld"));
            novels.AddRange(json["novels"].ToObject(typeof(List<Novel>)) as List<Novel>);

            json = JsonConvert.DeserializeObject<JObject>(_configuration.GetNode("novelfull"));
            novels.AddRange(json["novels"].ToObject(typeof(List<Novel>)) as List<Novel>);

            foreach (var novel in novels)
            {
                Novels.Add(novel);
            }
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
