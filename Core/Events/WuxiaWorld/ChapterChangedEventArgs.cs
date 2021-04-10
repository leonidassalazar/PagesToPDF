using Core.Enums;
using Model.WuxiaWorldModel;

namespace Core.Events.WuxiaWorld
{
    public class ChapterChangedEventArgs
    {
        public Chapter Chapter { get; set; }
        public DataOperationType DataOperationType { get; set; }
        public ChapterChangedEventArgs(Chapter chapter, DataOperationType type)
        {
            Chapter = chapter;
            DataOperationType = type;
        }
    }
}
