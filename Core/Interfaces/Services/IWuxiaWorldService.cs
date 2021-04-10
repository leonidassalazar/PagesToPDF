using System;
using System.Collections.Generic;
using Core.Events.WuxiaWorld;
using Model.WuxiaWorldModel;

namespace Core.Interfaces.Services
{
    public interface IWuxiaWorldService
    {
        event EventHandler<NovelChangedEventArgs> NovelEventDownload;
        event EventHandler<VolumeChangedEventArgs> VolumeEventDownload;
        event EventHandler<ChapterChangedEventArgs> ChapterEventDownload;
        string Password { get; set; }
        string UserEmail { get; set; }
        Dictionary<string, string> NovelCodeToName { get; set; }
        bool SaveBook { get; set; }
        bool SaveVolumes { get; set; }
        bool SaveChapters { get; set; }
        string SaveLocal { get; set; }
        bool Login();
        void CreateAndSaveBookAllNovels();
        bool CreateAndSaveBook(string novelCode);
        void CreateBook(Novel book);
        void CreateVolumes(Novel book);
        Novel CreateChapters(string novelCode);
        byte[] DownloadChapter(Chapter chapter);
    }
}