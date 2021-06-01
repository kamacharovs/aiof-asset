using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aiof.asset.data
{
    public class AssetEvent
    {
        public string EventType { get; set; }
        public EventSource Source { get; set; } = new EventSource();
        public EventUser User { get; set; } = new EventUser();
        public EventEntity Entity { get; set; } = new EventEntity();
    }
    public class EventSource
    {
        public string Api { get; } = Constants.ApiName;
        public string Ip { get; set; }
    }
    public class EventUser
    {
        public int? Id { get; set; }
        public Guid? PublicKey { get; set; }
    }
    public class EventEntity
    {
        public int? Id { get; set; }
        public string Type { get; set; }
        public object Payload { get; set; }
    }

    public class AssetAddedEvent : AssetEvent
    {
        public AssetAddedEvent()
        {
            EventType = Constants.AssetAddedEvent;
        }
    }
    public class AssetUpdatedEvent : AssetEvent
    {
        public AssetUpdatedEvent()
        {
            EventType = Constants.AssetUpdatedEvent;
        }
    }
    public class AssetDeletedEvent : AssetEvent
    {
        public AssetDeletedEvent()
        {
            EventType = Constants.AssetDeletedEvent;
        }
    }
}
