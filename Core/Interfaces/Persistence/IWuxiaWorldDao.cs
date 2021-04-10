using System.Collections.Generic;
using Model.WuxiaWorldModel;

namespace Core.Interfaces.Persistence
{
    public interface IWuxiaWorldDao
    {
        bool InsertNovel(Novel entity);
        List<Novel> GetAllNovels();
        Novel GetNovelsById(long id);
        bool UpdateNovel(Novel entity);
        bool DeleteNovel(long id);
        bool InsertVolume(Volume entity);
        List<Volume> GetAllVolumes();
        Volume GetVolumesById(long id);
        bool UpdateVolume(Volume entity);
        bool DeleteVolume(long id);
        bool InsertChapter(Chapter entity);
        List<Chapter> GetAllChapters();
        Chapter GetChaptersById(long id);
        bool UpdateChapter(Chapter entity);
        bool DeleteChapter(long id);
    }
}