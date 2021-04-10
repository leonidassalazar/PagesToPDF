using Core.Enums;
using Model.WuxiaWorldModel;

namespace Core.Events.WuxiaWorld
{
    public class VolumeChangedEventArgs
    {
        public Volume Volume { get; set; }
        public DataOperationType DataOperationType { get; set; }
        public VolumeChangedEventArgs(Volume volume, DataOperationType type)
        {
            Volume = volume;
            DataOperationType = type;
        }
    }
}
