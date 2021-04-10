using Core.Interfaces.Persistence;
using Core.Utils;
using Microsoft.EntityFrameworkCore;
using Model.WuxiaWorldModel;
using Persistence.Context;
using System.Collections.Generic;
using System.Linq;

namespace Persistence
{
    public class WuxiaWorldDao : IWuxiaWorldDao
    {
        private readonly WuxiaWorldContext _dbContext;
        public WuxiaWorldDao()
        {
            _dbContext = WuxiaWorldContext.GetWuxiaWorldContext();
        }

        #region Novels

        #region Create

        public bool InsertNovel(Novel entity)
        {
            var newNovel = _dbContext.Novels.Add(entity);
            _dbContext.SaveChanges();
            return newNovel.Entity.NovelId != 0;
        }

        #endregion

        #region Read

        public List<Novel> GetAllNovels()
        {
            return _dbContext.Novels.ToList();
        }

        public Novel GetNovelsById(long id)
        {
            return _dbContext.Novels.FirstOrDefault(q => q.NovelId == id);
        }

        #endregion

        #region Update

        public bool UpdateNovel(Novel entity)
        {
            var newEntity = _dbContext.Novels.FirstOrDefault(q => q.NovelId == entity.NovelId);
            if (newEntity != null)
            {
                EntitiesUtil.CopyPropertiesSameType(source: entity, destiny: newEntity);
                _dbContext.Attach(newEntity).State = EntityState.Modified;

                _dbContext.SaveChanges();
                return true;
            }

            return false;
        }

        #endregion

        #region Delete

        public bool DeleteNovel(long id)
        {
            var newEntity = _dbContext.Novels.FirstOrDefault(q => q.NovelId == id);
            if (newEntity != null)
            {
                _dbContext.Remove(newEntity);

                _dbContext.SaveChanges();
                return true;
            }

            return false;
        }

        #endregion

        #endregion


        #region Volumes

        #region Create

        public bool InsertVolume(Volume entity)
        {
            var newVolume = _dbContext.Volumes.Add(entity);
            _dbContext.SaveChanges();
            return newVolume.Entity.VolumeId != 0;
        }

        #endregion

        #region Read

        public List<Volume> GetAllVolumes()
        {
            return _dbContext.Volumes.ToList();
        }

        public Volume GetVolumesById(long id)
        {
            return _dbContext.Volumes.FirstOrDefault(q => q.VolumeId == id);
        }

        #endregion

        #region Update

        public bool UpdateVolume(Volume entity)
        {
            var newEntity = _dbContext.Volumes.FirstOrDefault(q => q.VolumeId == entity.VolumeId);
            if (newEntity != null)
            {
                EntitiesUtil.CopyPropertiesSameType(source: entity, destiny: newEntity);
                _dbContext.Attach(newEntity).State = EntityState.Modified;

                _dbContext.SaveChanges();
                return true;
            }

            return false;
        }

        #endregion

        #region Delete

        public bool DeleteVolume(long id)
        {
            var newEntity = _dbContext.Volumes.FirstOrDefault(q => q.VolumeId == id);
            if (newEntity != null)
            {
                _dbContext.Remove(newEntity);

                _dbContext.SaveChanges();
                return true;
            }

            return false;
        }

        #endregion

        #endregion

        #region Chapters

        #region Create

        public bool InsertChapter(Chapter entity)
        {
            var newChapter = _dbContext.Chapters.Add(entity);
            _dbContext.SaveChanges();
            return newChapter.Entity.ChapterId != 0;
        }

        #endregion

        #region Read

        public List<Chapter> GetAllChapters()
        {
            return _dbContext.Chapters.ToList();
        }

        public Chapter GetChaptersById(long id)
        {
            return _dbContext.Chapters.FirstOrDefault(q => q.ChapterId == id);
        }

        #endregion

        #region Update

        public bool UpdateChapter(Chapter entity)
        {
            var newEntity = _dbContext.Chapters.FirstOrDefault(q => q.ChapterId == entity.ChapterId);
            if (newEntity != null)
            {
                EntitiesUtil.CopyPropertiesSameType(source: entity, destiny: newEntity);
                _dbContext.Attach(newEntity).State = EntityState.Modified;

                _dbContext.SaveChanges();
                return true;
            }

            return false;
        }

        #endregion

        #region Delete

        public bool DeleteChapter(long id)
        {
            var newEntity = _dbContext.Chapters.FirstOrDefault(q => q.ChapterId == id);
            if (newEntity != null)
            {
                _dbContext.Remove(newEntity);

                _dbContext.SaveChanges();
                return true;
            }

            return false;
        }

        #endregion

        #endregion


    }
}
