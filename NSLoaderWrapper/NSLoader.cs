﻿using System;
using NullSpace.SDK.Internal;
using static NullSpace.SDK.Internal.Interop;
using UnityEngine;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using NullSpace.HapticFiles;
using FlatBuffers;
using NullSpace.HapticFiles.Mixed;

namespace NullSpace.SDK
{

	public class HapticsLoadingException : System.Exception
	{
		public HapticsLoadingException() : base() { }
		public HapticsLoadingException(string message) : base(message) { }
		public HapticsLoadingException(string message, System.Exception inner) : base(message, inner) { }


		protected HapticsLoadingException(System.Runtime.Serialization.SerializationInfo info,
			System.Runtime.Serialization.StreamingContext context)
		{ }
	}


	public static class NSVR
	{
		internal static IntPtr _ptr;
		internal static bool _created = false;
		internal static string GetError()
		{
			IntPtr ptr = Interop.NSVR_GetError(NSVR.NSVR_Plugin.Ptr);
			string s = Marshal.PtrToStringAnsi(ptr);
			Interop.NSVR_FreeError(ptr);
			return s;
		}





		public sealed class NSVR_Plugin : IDisposable
		{
			internal static bool _disposed = false;

			internal static IntPtr Ptr
			{
				get
				{
					if (_created && !_disposed && _ptr != null)
					{

						return _ptr;
					}
					else
					{
						throw new MemberAccessException("[NSVR] You must have a NS Manager prefab in your scene!");

					}

				}
			}
			public NSVR_Plugin(string path)
			{
				_disposed = false;
				if (_created)
				{
					Debug.LogWarning("[NSVR] NSVR_Plugin should only be created by the NullSpace SDK");
					return;
				}
				_ptr = Interop.NSVR_Create();
				//New plugin doesn't do FS stuff?
				//Interop.NSVR_InitializeFromFilesystem(_ptr, path);
				_created = true;

			}


			public void PauseAll()
			{
				Interop.NSVR_DoEngineCommand(Ptr, (short)Interop.EngineCommand.PAUSE_ALL);
			}

			public void ResumeAll()
			{
				Interop.NSVR_DoEngineCommand(Ptr, (short)Interop.EngineCommand.PLAY_ALL);
			}

			public void ClearAll()
			{
				Interop.NSVR_DoEngineCommand(Ptr, (short)Interop.EngineCommand.CLEAR_ALL);
			}



			public SuitStatus PollStatus()
			{
				return (SuitStatus)Interop.NSVR_PollStatus(Ptr);
			}

			public void SetTrackingEnabled(bool wantTracking)
			{
				if (wantTracking)
				{
					Interop.NSVR_DoEngineCommand(Ptr, (short)Interop.EngineCommand.ENABLE_TRACKING);
				}
				else
				{
					Interop.NSVR_DoEngineCommand(Ptr, (short)Interop.EngineCommand.DISABLE_TRACKING);

				}
			}

			public TrackingUpdate PollTracking()
			{
				InteropTrackingUpdate t = new InteropTrackingUpdate();
				Interop.NSVR_PollTracking(Ptr, ref t);

				TrackingUpdate update = new TrackingUpdate();
				update.Chest = new UnityEngine.Quaternion(t.chest.x, t.chest.y, t.chest.z, t.chest.w);
				update.LeftUpperArm = new UnityEngine.Quaternion(t.left_upper_arm.x, t.left_upper_arm.y, t.left_upper_arm.z, t.left_upper_arm.w);
				update.RightUpperArm = new UnityEngine.Quaternion(t.right_upper_arm.x, t.right_upper_arm.y, t.right_upper_arm.z, t.right_upper_arm.w);
				update.LeftForearm = new UnityEngine.Quaternion(t.left_forearm.x, t.left_forearm.y, t.left_forearm.z, t.left_forearm.w);
				update.RightForearm = new UnityEngine.Quaternion(t.right_forearm.x, t.right_forearm.y, t.right_forearm.z, t.right_forearm.w);
				return update;
			}

			#region IDisposable Support
			private bool disposedValue = false; // To detect redundant calls

			void Dispose(bool disposing)
			{
				if (!disposedValue)
				{
					if (disposing)
					{
						// TODO: dispose managed state (managed objects).
					}

					// TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
					// TODO: set large fields to null.

					_created = false;
					Interop.NSVR_Delete(_ptr);

					disposedValue = true;
					_disposed = true;
				}
			}

			// TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
			~NSVR_Plugin()
			{
				//   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
				Dispose(false);
			}

			// This code added to correctly implement the disposable pattern.
			public void Dispose()
			{
				// Do not change this code. Put cleanup code in Dispose(bool disposing) above.
				Dispose(true);
				// TODO: uncomment the following line if the finalizer is overridden above.
				GC.SuppressFinalize(this);
			}
			#endregion


		}
	}

	public struct TrackingUpdate
	{
		public UnityEngine.Quaternion Chest;
		public UnityEngine.Quaternion LeftUpperArm;
		public UnityEngine.Quaternion LeftForearm;
		public UnityEngine.Quaternion RightUpperArm;
		public UnityEngine.Quaternion RightForearm;
	}

	/// <summary>
	/// HapticHandle is a handle used to control the playback of haptic effects. If you are concerned about performance, please call Dispose on a HapticHandle which is no longer needed. This will tell the engine that it may dispose of the resources dedicated to this HapticHandle.
	/// </summary>
	public sealed class HapticHandle
	{
		/// <summary>
		/// Retain the effect name, so that someone calling ToString() can get useful information
		/// </summary>
		private string _effectName;
		/// <summary>
		/// Handle is used to interact with the engine
		/// </summary>
		private uint _handle;
		/// <summary>
		/// _playDelegate contains knowledge of how to play the effect
		/// </summary>
		private CommandWithHandle _playDelegate;
		/// <summary>
		/// _pauseDelegate contains knowledege of how to pause the effect
		/// </summary>
		private CommandWithHandle _pauseDelegate;
		/// <summary>
		/// _resetDelegate contains knowledge of how to reset the effect
		/// </summary>
		private CommandWithHandle _resetDelegate;
		/// <summary>
		/// Store this for clone functionality
		/// </summary>
		private CommandWithHandle _createDelegate;

		internal HapticHandle() { }
		internal HapticHandle(CommandWithHandle play, CommandWithHandle pause, CommandWithHandle create, CommandWithHandle reset, string name)
		{
			_effectName = name;
			_playDelegate = play;
			_pauseDelegate = pause;
			_resetDelegate = reset;
			_createDelegate = create;
			//Grab a handle from the DLL
			_handle = Interop.NSVR_GenHandle(NSVR.NSVR_Plugin.Ptr);
			//This is a network call to the engine, to load the effect up
			_createDelegate(_handle);
		}

		internal HapticHandle(CommandWithHandle play, CommandWithHandle pause, CommandWithHandle create, CommandWithHandle reset)
		{
			_effectName = "Custom effect";
			_playDelegate = play;
			_pauseDelegate = pause;
			_resetDelegate = reset;
			_handle = Interop.NSVR_GenHandle(NSVR.NSVR_Plugin.Ptr);


			create(_handle);

		}

		/// <summary>
		/// Clone this HapticHandle
		/// </summary>
		/// <returns>A new HapticHandle</returns>
		public HapticHandle Clone()
		{
			return new HapticHandle(_playDelegate, _pauseDelegate, _createDelegate, _resetDelegate, _effectName);

		}

		/// <summary>
		/// Play the associated effect
		/// </summary>
		/// <returns></returns>
		public HapticHandle Play()
		{
			_playDelegate(_handle);
			return this;
		}

		/// <summary>
		/// Cause the effect to stop playing and return to time 0. Will not continue playing until Play is called again
		/// </summary>
		/// <returns></returns>
		public HapticHandle Reset()
		{
			_resetDelegate(_handle);
			return this;
		}

		/// <summary>
		/// Cause the effect to pause. Can be resumed by calling Play
		/// </summary>
		/// <returns></returns>
		public HapticHandle Pause()
		{
			_pauseDelegate(_handle);
			return this;
		}

		/// <summary>
		/// Return information about this handle, such as the internal Handle ID and an indication of the disposed status
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			if (disposedValue)
			{
				return string.Format("[Disposed] Handle ID {0} playing effect {1}", _handle, _effectName);
			}
			else
			{
				return string.Format("Handle ID {0} playing effect {1}", _handle, _effectName);
			}
		}

		#region IDisposable Support
		private bool disposedValue = false; // To detect redundant calls

		void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				if (disposing)
				{
					// TODO: dispose managed state (managed objects).
				}
				if (!NSVR.NSVR_Plugin._disposed)
				{
					Interop.NSVR_DoHandleCommand(NSVR.NSVR_Plugin.Ptr, _handle, (short)Interop.Command.RELEASE);
				}


				// TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
				// TODO: set large fields to null.

				disposedValue = true;
			}
		}

		// TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
		//	 ~HapticHandle() {
		//   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
		//  Dispose(false);
		// }

		/// <summary>
		/// Allow the engine to reclaim resources associated with this HapticHandle. After calling this, 
		/// the HapticHandle will no longer be useable. 
		/// </summary>
		public void Dispose()
		{
			// Do not change this code. Put cleanup code in Dispose(bool disposing) above.
			Dispose(true);
			// TODO: uncomment the following line if the finalizer is overridden above.
			GC.SuppressFinalize(this);
		}
		#endregion
	}

	public abstract class Playable
	{

		internal Playable() { }
		internal static CommandWithHandle GenerateCommandDelegate(Interop.Command c)
		{
			return new CommandWithHandle(x => Interop.NSVR_DoHandleCommand(NSVR.NSVR_Plugin.Ptr, x, (short)c));
		}

		internal CommandWithHandle _Play()
		{
			return GenerateCommandDelegate(Interop.Command.PLAY);
		}

		internal CommandWithHandle _Reset()
		{
			return GenerateCommandDelegate(Interop.Command.RESET);
		}

		internal CommandWithHandle _Pause()
		{
			return GenerateCommandDelegate(Interop.Command.PAUSE);
		}


	}
}



