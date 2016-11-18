// automatically generated by the FlatBuffers compiler, do not modify

namespace NullSpace.HapticFiles
{

using System;
using FlatBuffers;

public struct Tracking : IFlatbufferObject
{
  private Table __p;
  public ByteBuffer ByteBuffer { get { return __p.bb; } }
  public static Tracking GetRootAsTracking(ByteBuffer _bb) { return GetRootAsTracking(_bb, new Tracking()); }
  public static Tracking GetRootAsTracking(ByteBuffer _bb, Tracking obj) { return (obj.__assign(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public void __init(int _i, ByteBuffer _bb) { __p.bb_pos = _i; __p.bb = _bb; }
  public Tracking __assign(int _i, ByteBuffer _bb) { __init(_i, _bb); return this; }

  public bool Enable { get { int o = __p.__offset(4); return o != 0 ? 0!=__p.bb.Get(o + __p.bb_pos) : (bool)false; } }

  public static Offset<Tracking> CreateTracking(FlatBufferBuilder builder,
      bool enable = false) {
    builder.StartObject(1);
    Tracking.AddEnable(builder, enable);
    return Tracking.EndTracking(builder);
  }

  public static void StartTracking(FlatBufferBuilder builder) { builder.StartObject(1); }
  public static void AddEnable(FlatBufferBuilder builder, bool enable) { builder.AddBool(0, enable, false); }
  public static Offset<Tracking> EndTracking(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<Tracking>(o);
  }
};


}
