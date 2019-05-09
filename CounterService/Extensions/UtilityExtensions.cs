using Google.Protobuf;
using System;
using static CounterEvent;

namespace CounterService.Extensions
{
    public static class UtilityExtensions
    {
        public static Guid ToGuid(this ByteString bytes) =>
            new Guid(bytes.ToByteArray());

        public static ByteString ToByteString(this Guid guid) =>
            ByteString.CopyFrom(guid.ToByteArray());

        public static IMessage GetEventPayload(this CounterEvent counterEvent)
        {
            switch (counterEvent.EventCase)
            {
                case EventOneofCase.Added:
                    return counterEvent.Added;
                case EventOneofCase.Decremented:
                    return counterEvent.Decremented;
                case EventOneofCase.Incremented:
                    return counterEvent.Incremented;
                case EventOneofCase.NameChanged:
                    return counterEvent.NameChanged;
                case EventOneofCase.Removed:
                    return counterEvent.Removed;
                default:
                    throw new InvalidOperationException("Invalid event type");
            }
        }

        public static CounterEvent SetEventPayload(this CounterEvent counterEvent, string type, byte[] payload)
        {
            if (Enum.TryParse(type, out EventOneofCase eventType))
            {
                switch (eventType)
                {
                    case EventOneofCase.Added:
                        counterEvent.Added = CounterAdded.Parser.ParseFrom(payload);
                        break;
                    case EventOneofCase.Decremented:
                        counterEvent.Decremented = CounterDecremented.Parser.ParseFrom(payload);
                        break;
                    case EventOneofCase.Incremented:
                        counterEvent.Incremented = CounterIncremented.Parser.ParseFrom(payload);
                        break;
                    case EventOneofCase.NameChanged:
                        counterEvent.NameChanged = CounterNameChanged.Parser.ParseFrom(payload);
                        break;
                    case EventOneofCase.Removed:
                        counterEvent.Removed = CounterRemoved.Parser.ParseFrom(payload);
                        break;
                    default:
                        throw new InvalidOperationException("Invalid event type");
                }

                return counterEvent;
            }

            throw new InvalidOperationException($"Could not parse {type} to an event type");
        }
    }
}
