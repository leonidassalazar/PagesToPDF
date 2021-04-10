using Core.Enums;
using Model.WuxiaWorldModel;

namespace Core.Events.WuxiaWorld
{
    public class NovelChangedEventArgs
    {
        public Novel Novel { get; set; }
        public DataOperationType DataOperationType { get; set; }
        public NovelChangedEventArgs(Novel novel, DataOperationType type)
        {
            Novel = novel;
            DataOperationType = type;
        }
    }
}
