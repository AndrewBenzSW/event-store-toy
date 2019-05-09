// Generated by ProtobufGenerator 2019-05-09 10:26:18

#region Using statments

using pb = global::Google.Protobuf;
using pbc = global::Google.Protobuf.Collections;
using pbr = global::Google.Protobuf.Reflection;
using scg = global::System.Collections.Generic;

#endregion

#region File "CounterEvent.cs"

// <auto-generated>
//     Generated by the protocol buffer compiler.  DO NOT EDIT!
//     source: CounterEvent.proto
// </auto-generated>
#pragma warning disable 1591, 0612, 3021
#region Designer generated code

/// <summary>Holder for reflection information generated from CounterEvent.proto</summary>
public static partial class CounterEventReflection {

  #region Descriptor
  /// <summary>File descriptor for CounterEvent.proto</summary>
  public static pbr::FileDescriptor Descriptor {
    get { return descriptor; }
  }
  private static pbr::FileDescriptor descriptor;

  static CounterEventReflection() {
    byte[] descriptorData = global::System.Convert.FromBase64String(
        string.Concat(
          "ChJDb3VudGVyRXZlbnQucHJvdG8aEkNvdW50ZXJBZGRlZC5wcm90bxoYQ291",
          "bnRlckRlY3JlbWVudGVkLnByb3RvGhhDb3VudGVySW5jcmVtZW50ZWQucHJv",
          "dG8aGENvdW50ZXJOYW1lQ2hhbmdlZC5wcm90bxoUQ291bnRlclJlbW92ZWQu",
          "cHJvdG8i/AEKDENvdW50ZXJFdmVudBIKCgJpZBgBIAEoDBIPCgd2ZXJzaW9u",
          "GAIgASgEEh4KBWFkZGVkGAMgASgLMg0uQ291bnRlckFkZGVkSAASKgoLZGVj",
          "cmVtZW50ZWQYBCABKAsyEy5Db3VudGVyRGVjcmVtZW50ZWRIABIqCgtpbmNy",
          "ZW1lbnRlZBgFIAEoCzITLkNvdW50ZXJJbmNyZW1lbnRlZEgAEioKC25hbWVD",
          "aGFuZ2VkGAYgASgLMhMuQ291bnRlck5hbWVDaGFuZ2VkSAASIgoHcmVtb3Zl",
          "ZBgHIAEoCzIPLkNvdW50ZXJSZW1vdmVkSABCBwoFZXZlbnRiBnByb3RvMw=="));
    descriptor = pbr::FileDescriptor.FromGeneratedCode(descriptorData,
        new pbr::FileDescriptor[] { global::CounterAddedReflection.Descriptor, global::CounterDecrementedReflection.Descriptor, global::CounterIncrementedReflection.Descriptor, global::CounterNameChangedReflection.Descriptor, global::CounterRemovedReflection.Descriptor, },
        new pbr::GeneratedClrTypeInfo(null, new pbr::GeneratedClrTypeInfo[] {
          new pbr::GeneratedClrTypeInfo(typeof(global::CounterEvent), global::CounterEvent.Parser, new[]{ "Id", "Version", "Added", "Decremented", "Incremented", "NameChanged", "Removed" }, new[]{ "Event" }, null, null)
        }));
  }
  #endregion

}
#region Messages
public sealed partial class CounterEvent : pb::IMessage<CounterEvent> {
  private static readonly pb::MessageParser<CounterEvent> _parser = new pb::MessageParser<CounterEvent>(() => new CounterEvent());
  private pb::UnknownFieldSet _unknownFields;
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public static pb::MessageParser<CounterEvent> Parser { get { return _parser; } }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public static pbr::MessageDescriptor Descriptor {
    get { return global::CounterEventReflection.Descriptor.MessageTypes[0]; }
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  pbr::MessageDescriptor pb::IMessage.Descriptor {
    get { return Descriptor; }
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public CounterEvent() {
    OnConstruction();
  }

  partial void OnConstruction();

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public CounterEvent(CounterEvent other) : this() {
    id_ = other.id_;
    version_ = other.version_;
    switch (other.EventCase) {
      case EventOneofCase.Added:
        Added = other.Added.Clone();
        break;
      case EventOneofCase.Decremented:
        Decremented = other.Decremented.Clone();
        break;
      case EventOneofCase.Incremented:
        Incremented = other.Incremented.Clone();
        break;
      case EventOneofCase.NameChanged:
        NameChanged = other.NameChanged.Clone();
        break;
      case EventOneofCase.Removed:
        Removed = other.Removed.Clone();
        break;
    }

    _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public CounterEvent Clone() {
    return new CounterEvent(this);
  }

  /// <summary>Field number for the "id" field.</summary>
  public const int IdFieldNumber = 1;
  private pb::ByteString id_ = pb::ByteString.Empty;
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public pb::ByteString Id {
    get { return id_; }
    set {
      id_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
    }
  }

  /// <summary>Field number for the "version" field.</summary>
  public const int VersionFieldNumber = 2;
  private ulong version_;
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public ulong Version {
    get { return version_; }
    set {
      version_ = value;
    }
  }

  /// <summary>Field number for the "added" field.</summary>
  public const int AddedFieldNumber = 3;
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public global::CounterAdded Added {
    get { return eventCase_ == EventOneofCase.Added ? (global::CounterAdded) event_ : null; }
    set {
      event_ = value;
      eventCase_ = value == null ? EventOneofCase.None : EventOneofCase.Added;
    }
  }

  /// <summary>Field number for the "decremented" field.</summary>
  public const int DecrementedFieldNumber = 4;
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public global::CounterDecremented Decremented {
    get { return eventCase_ == EventOneofCase.Decremented ? (global::CounterDecremented) event_ : null; }
    set {
      event_ = value;
      eventCase_ = value == null ? EventOneofCase.None : EventOneofCase.Decremented;
    }
  }

  /// <summary>Field number for the "incremented" field.</summary>
  public const int IncrementedFieldNumber = 5;
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public global::CounterIncremented Incremented {
    get { return eventCase_ == EventOneofCase.Incremented ? (global::CounterIncremented) event_ : null; }
    set {
      event_ = value;
      eventCase_ = value == null ? EventOneofCase.None : EventOneofCase.Incremented;
    }
  }

  /// <summary>Field number for the "nameChanged" field.</summary>
  public const int NameChangedFieldNumber = 6;
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public global::CounterNameChanged NameChanged {
    get { return eventCase_ == EventOneofCase.NameChanged ? (global::CounterNameChanged) event_ : null; }
    set {
      event_ = value;
      eventCase_ = value == null ? EventOneofCase.None : EventOneofCase.NameChanged;
    }
  }

  /// <summary>Field number for the "removed" field.</summary>
  public const int RemovedFieldNumber = 7;
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public global::CounterRemoved Removed {
    get { return eventCase_ == EventOneofCase.Removed ? (global::CounterRemoved) event_ : null; }
    set {
      event_ = value;
      eventCase_ = value == null ? EventOneofCase.None : EventOneofCase.Removed;
    }
  }

  private object event_;
  /// <summary>Enum of possible cases for the "event" oneof.</summary>
  public enum EventOneofCase {
    None = 0,
    Added = 3,
    Decremented = 4,
    Incremented = 5,
    NameChanged = 6,
    Removed = 7,
  }
  private EventOneofCase eventCase_ = EventOneofCase.None;
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public EventOneofCase EventCase {
    get { return eventCase_; }
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public void ClearEvent() {
    eventCase_ = EventOneofCase.None;
    event_ = null;
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public override bool Equals(object other) {
    return Equals(other as CounterEvent);
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public bool Equals(CounterEvent other) {
    if (ReferenceEquals(other, null)) {
      return false;
    }
    if (ReferenceEquals(other, this)) {
      return true;
    }
    if (Id != other.Id) return false;
    if (Version != other.Version) return false;
    if (!object.Equals(Added, other.Added)) return false;
    if (!object.Equals(Decremented, other.Decremented)) return false;
    if (!object.Equals(Incremented, other.Incremented)) return false;
    if (!object.Equals(NameChanged, other.NameChanged)) return false;
    if (!object.Equals(Removed, other.Removed)) return false;
    if (EventCase != other.EventCase) return false;
    return Equals(_unknownFields, other._unknownFields);
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public override int GetHashCode() {
    int hash = 1;
    if (Id.Length != 0) hash ^= Id.GetHashCode();
    if (Version != 0UL) hash ^= Version.GetHashCode();
    if (eventCase_ == EventOneofCase.Added) hash ^= Added.GetHashCode();
    if (eventCase_ == EventOneofCase.Decremented) hash ^= Decremented.GetHashCode();
    if (eventCase_ == EventOneofCase.Incremented) hash ^= Incremented.GetHashCode();
    if (eventCase_ == EventOneofCase.NameChanged) hash ^= NameChanged.GetHashCode();
    if (eventCase_ == EventOneofCase.Removed) hash ^= Removed.GetHashCode();
    hash ^= (int) eventCase_;
    if (_unknownFields != null) {
      hash ^= _unknownFields.GetHashCode();
    }
    return hash;
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public override string ToString() {
    return pb::JsonFormatter.ToDiagnosticString(this);
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public void WriteTo(pb::CodedOutputStream output) {
    if (Id.Length != 0) {
      output.WriteRawTag(10);
      output.WriteBytes(Id);
    }
    if (Version != 0UL) {
      output.WriteRawTag(16);
      output.WriteUInt64(Version);
    }
    if (eventCase_ == EventOneofCase.Added) {
      output.WriteRawTag(26);
      output.WriteMessage(Added);
    }
    if (eventCase_ == EventOneofCase.Decremented) {
      output.WriteRawTag(34);
      output.WriteMessage(Decremented);
    }
    if (eventCase_ == EventOneofCase.Incremented) {
      output.WriteRawTag(42);
      output.WriteMessage(Incremented);
    }
    if (eventCase_ == EventOneofCase.NameChanged) {
      output.WriteRawTag(50);
      output.WriteMessage(NameChanged);
    }
    if (eventCase_ == EventOneofCase.Removed) {
      output.WriteRawTag(58);
      output.WriteMessage(Removed);
    }
    if (_unknownFields != null) {
      _unknownFields.WriteTo(output);
    }
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public int CalculateSize() {
    int size = 0;
    if (Id.Length != 0) {
      size += 1 + pb::CodedOutputStream.ComputeBytesSize(Id);
    }
    if (Version != 0UL) {
      size += 1 + pb::CodedOutputStream.ComputeUInt64Size(Version);
    }
    if (eventCase_ == EventOneofCase.Added) {
      size += 1 + pb::CodedOutputStream.ComputeMessageSize(Added);
    }
    if (eventCase_ == EventOneofCase.Decremented) {
      size += 1 + pb::CodedOutputStream.ComputeMessageSize(Decremented);
    }
    if (eventCase_ == EventOneofCase.Incremented) {
      size += 1 + pb::CodedOutputStream.ComputeMessageSize(Incremented);
    }
    if (eventCase_ == EventOneofCase.NameChanged) {
      size += 1 + pb::CodedOutputStream.ComputeMessageSize(NameChanged);
    }
    if (eventCase_ == EventOneofCase.Removed) {
      size += 1 + pb::CodedOutputStream.ComputeMessageSize(Removed);
    }
    if (_unknownFields != null) {
      size += _unknownFields.CalculateSize();
    }
    return size;
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public void MergeFrom(CounterEvent other) {
    if (other == null) {
      return;
    }
    if (other.Id.Length != 0) {
      Id = other.Id;
    }
    if (other.Version != 0UL) {
      Version = other.Version;
    }
    switch (other.EventCase) {
      case EventOneofCase.Added:
        if (Added == null) {
          Added = new global::CounterAdded();
        }
        Added.MergeFrom(other.Added);
        break;
      case EventOneofCase.Decremented:
        if (Decremented == null) {
          Decremented = new global::CounterDecremented();
        }
        Decremented.MergeFrom(other.Decremented);
        break;
      case EventOneofCase.Incremented:
        if (Incremented == null) {
          Incremented = new global::CounterIncremented();
        }
        Incremented.MergeFrom(other.Incremented);
        break;
      case EventOneofCase.NameChanged:
        if (NameChanged == null) {
          NameChanged = new global::CounterNameChanged();
        }
        NameChanged.MergeFrom(other.NameChanged);
        break;
      case EventOneofCase.Removed:
        if (Removed == null) {
          Removed = new global::CounterRemoved();
        }
        Removed.MergeFrom(other.Removed);
        break;
    }

    _unknownFields = pb::UnknownFieldSet.MergeFrom(_unknownFields, other._unknownFields);
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public void MergeFrom(pb::CodedInputStream input) {
    uint tag;
    while ((tag = input.ReadTag()) != 0) {
      switch(tag) {
        default:
          _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, input);
          break;
        case 10: {
          Id = input.ReadBytes();
          break;
        }
        case 16: {
          Version = input.ReadUInt64();
          break;
        }
        case 26: {
          global::CounterAdded subBuilder = new global::CounterAdded();
          if (eventCase_ == EventOneofCase.Added) {
            subBuilder.MergeFrom(Added);
          }
          input.ReadMessage(subBuilder);
          Added = subBuilder;
          break;
        }
        case 34: {
          global::CounterDecremented subBuilder = new global::CounterDecremented();
          if (eventCase_ == EventOneofCase.Decremented) {
            subBuilder.MergeFrom(Decremented);
          }
          input.ReadMessage(subBuilder);
          Decremented = subBuilder;
          break;
        }
        case 42: {
          global::CounterIncremented subBuilder = new global::CounterIncremented();
          if (eventCase_ == EventOneofCase.Incremented) {
            subBuilder.MergeFrom(Incremented);
          }
          input.ReadMessage(subBuilder);
          Incremented = subBuilder;
          break;
        }
        case 50: {
          global::CounterNameChanged subBuilder = new global::CounterNameChanged();
          if (eventCase_ == EventOneofCase.NameChanged) {
            subBuilder.MergeFrom(NameChanged);
          }
          input.ReadMessage(subBuilder);
          NameChanged = subBuilder;
          break;
        }
        case 58: {
          global::CounterRemoved subBuilder = new global::CounterRemoved();
          if (eventCase_ == EventOneofCase.Removed) {
            subBuilder.MergeFrom(Removed);
          }
          input.ReadMessage(subBuilder);
          Removed = subBuilder;
          break;
        }
      }
    }
  }

}

#endregion


#endregion Designer generated code

#endregion
