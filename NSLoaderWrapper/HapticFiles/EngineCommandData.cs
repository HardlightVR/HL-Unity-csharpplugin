// automatically generated by the FlatBuffers compiler, do not modify

namespace NullSpace.HapticFiles
{

using System;
using FlatBuffers;

public struct EngineCommandData : IFlatbufferObject
{
  private Table __p;
  public ByteBuffer ByteBuffer { get { return __p.bb; } }
  public static EngineCommandData GetRootAsEngineCommandData(ByteBuffer _bb) { return GetRootAsEngineCommandData(_bb, new EngineCommandData()); }
  public static EngineCommandData GetRootAsEngineCommandData(ByteBuffer _bb, EngineCommandData obj) { return (obj.__assign(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public void __init(int _i, ByteBuffer _bb) { __p.bb_pos = _i; __p.bb = _bb; }
  public EngineCommandData __assign(int _i, ByteBuffer _bb) { __init(_i, _bb); return this; }

  public EngineCommand Command { get { int o = __p.__offset(4); return o != 0 ? (EngineCommand)__p.bb.GetShort(o + __p.bb_pos) : EngineCommand.NO_COMMAND; } }

  public static Offset<EngineCommandData> CreateEngineCommandData(FlatBufferBuilder builder,
      EngineCommand command = EngineCommand.NO_COMMAND) {
    builder.StartObject(1);
    EngineCommandData.AddCommand(builder, command);
    return EngineCommandData.EndEngineCommandData(builder);
  }

  public static void StartEngineCommandData(FlatBufferBuilder builder) { builder.StartObject(1); }
  public static void AddCommand(FlatBufferBuilder builder, EngineCommand command) { builder.AddShort(0, (short)command, 0); }
  public static Offset<EngineCommandData> EndEngineCommandData(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<EngineCommandData>(o);
  }
};


}