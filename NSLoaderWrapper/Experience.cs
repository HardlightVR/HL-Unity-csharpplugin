// automatically generated by the FlatBuffers compiler, do not modify

namespace NullSpace.HapticFiles
{

using System;
using FlatBuffers;

public struct Experience : IFlatbufferObject
{
  private Table __p;
  public ByteBuffer ByteBuffer { get { return __p.bb; } }
  public static Experience GetRootAsExperience(ByteBuffer _bb) { return GetRootAsExperience(_bb, new Experience()); }
  public static Experience GetRootAsExperience(ByteBuffer _bb, Experience obj) { return (obj.__assign(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public void __init(int _i, ByteBuffer _bb) { __p.bb_pos = _i; __p.bb = _bb; }
  public Experience __assign(int _i, ByteBuffer _bb) { __init(_i, _bb); return this; }

  public NullSpace.HapticFiles.HapticSample? Items(int j) { int o = __p.__offset(4); return o != 0 ? (NullSpace.HapticFiles.HapticSample?)(new NullSpace.HapticFiles.HapticSample()).__assign(__p.__indirect(__p.__vector(o) + j * 4), __p.bb) : null; }
  public int ItemsLength { get { int o = __p.__offset(4); return o != 0 ? __p.__vector_len(o) : 0; } }

  public static Offset<Experience> CreateExperience(FlatBufferBuilder builder,
      VectorOffset itemsOffset = default(VectorOffset)) {
    builder.StartObject(1);
    Experience.AddItems(builder, itemsOffset);
    return Experience.EndExperience(builder);
  }

  public static void StartExperience(FlatBufferBuilder builder) { builder.StartObject(1); }
  public static void AddItems(FlatBufferBuilder builder, VectorOffset itemsOffset) { builder.AddOffset(0, itemsOffset.Value, 0); }
  public static VectorOffset CreateItemsVector(FlatBufferBuilder builder, Offset<NullSpace.HapticFiles.HapticSample>[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartItemsVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static Offset<Experience> EndExperience(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<Experience>(o);
  }
  public static void FinishExperienceBuffer(FlatBufferBuilder builder, Offset<Experience> offset) { builder.Finish(offset.Value); }
};


}
