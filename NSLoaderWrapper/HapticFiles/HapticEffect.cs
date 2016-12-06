// automatically generated by the FlatBuffers compiler, do not modify

namespace NullSpace.HapticFiles
{

using System;
using FlatBuffers;

public struct HapticEffect : IFlatbufferObject
{
  private Table __p;
  public ByteBuffer ByteBuffer { get { return __p.bb; } }
  public static HapticEffect GetRootAsHapticEffect(ByteBuffer _bb) { return GetRootAsHapticEffect(_bb, new HapticEffect()); }
  public static HapticEffect GetRootAsHapticEffect(ByteBuffer _bb, HapticEffect obj) { return (obj.__assign(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public void __init(int _i, ByteBuffer _bb) { __p.bb_pos = _i; __p.bb = _bb; }
  public HapticEffect __assign(int _i, ByteBuffer _bb) { __init(_i, _bb); return this; }

  public float Time { get { int o = __p.__offset(4); return o != 0 ? __p.bb.GetFloat(o + __p.bb_pos) : (float)0.0f; } }
  public string Effect { get { int o = __p.__offset(6); return o != 0 ? __p.__string(o + __p.bb_pos) : null; } }
  public ArraySegment<byte>? GetEffectBytes() { return __p.__vector_as_arraysegment(6); }
  public float Strength { get { int o = __p.__offset(8); return o != 0 ? __p.bb.GetFloat(o + __p.bb_pos) : (float)0.0f; } }
  public float Duration { get { int o = __p.__offset(10); return o != 0 ? __p.bb.GetFloat(o + __p.bb_pos) : (float)0.0f; } }
  public int Repeat { get { int o = __p.__offset(12); return o != 0 ? __p.bb.GetInt(o + __p.bb_pos) : (int)0; } }

  public static Offset<HapticEffect> CreateHapticEffect(FlatBufferBuilder builder,
      float time = 0.0f,
      StringOffset effectOffset = default(StringOffset),
      float strength = 0.0f,
      float duration = 0.0f,
      int repeat = 0) {
    builder.StartObject(5);
    HapticEffect.AddRepeat(builder, repeat);
    HapticEffect.AddDuration(builder, duration);
    HapticEffect.AddStrength(builder, strength);
    HapticEffect.AddEffect(builder, effectOffset);
    HapticEffect.AddTime(builder, time);
    return HapticEffect.EndHapticEffect(builder);
  }

  public static void StartHapticEffect(FlatBufferBuilder builder) { builder.StartObject(5); }
  public static void AddTime(FlatBufferBuilder builder, float time) { builder.AddFloat(0, time, 0.0f); }
  public static void AddEffect(FlatBufferBuilder builder, StringOffset effectOffset) { builder.AddOffset(1, effectOffset.Value, 0); }
  public static void AddStrength(FlatBufferBuilder builder, float strength) { builder.AddFloat(2, strength, 0.0f); }
  public static void AddDuration(FlatBufferBuilder builder, float duration) { builder.AddFloat(3, duration, 0.0f); }
  public static void AddRepeat(FlatBufferBuilder builder, int repeat) { builder.AddInt(4, repeat, 0); }
  public static Offset<HapticEffect> EndHapticEffect(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<HapticEffect>(o);
  }
  public static void FinishHapticEffectBuffer(FlatBufferBuilder builder, Offset<HapticEffect> offset) { builder.Finish(offset.Value); }
};


}