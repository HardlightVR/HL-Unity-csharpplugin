﻿using System;
using UnityEngine;
using System.Runtime.InteropServices;
using NullSpace.SDK.Internal;
using System.ServiceProcess;
using System.Runtime.Remoting.Messaging;
using System.Collections.Generic;

namespace NullSpace.SDK
{

	/// <summary>
	/// Represents version information, containing a major and minor version
	/// </summary>
	public struct VersionInfo
	{
		public uint Major;
		public uint Minor;

		/// <summary>
		/// Returns Major.Minor
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return string.Format("{0}.{1}", Major, Minor);
		}
	}

	/// <summary>
	/// Internal testing tool; do not depend upon this. May change at any time.
	/// </summary>
	public struct EffectSampleInfo
	{
		public UInt16 Strength;
		public UInt32 Family;
		public AreaFlag Area;
		public EffectSampleInfo(UInt16 strength, UInt32 family, AreaFlag area)
		{
			Strength = strength;
			Family = family;
			Area = area;
		}
	}

	public class HapticsLoadingException : System.Exception
	{
		public HapticsLoadingException() : base() { }
		public HapticsLoadingException(string message) : base(message) { }
		public HapticsLoadingException(string message, System.Exception inner) : base(message, inner) { }


		protected HapticsLoadingException(System.Runtime.Serialization.SerializationInfo info,
			System.Runtime.Serialization.StreamingContext context)
		{ }
	}

	/// <summary>
	/// Wrapper around the main access point of the plugin, NSVR_Plugin
	/// </summary>
	public static class NSVR
	{
		
		internal static unsafe NSVR_System* _ptr;
		internal static bool _created = false;

	
		/// <summary>
		/// Main point of access to the plugin, implements IDisposable
		/// </summary>
		public unsafe sealed class NSVR_Plugin : IDisposable
		{
			internal static bool _disposed = false;

			internal static unsafe NSVR_System* Ptr
			{
				get
				{
					if (_created && !_disposed && _ptr != null)
					{

						return _ptr;
					}
					else
					{
						throw new MemberAccessException("[NSVR] You must have a NS Manager prefab in your scene!\n");

					}

				}
			}

			public NSVR_Plugin()
			{
				_disposed = false;
				if (_created)
				{
					Debug.LogWarning("[NSVR] NSVR_Plugin should only be created by the NullSpace SDK\n");
					return;
				}

				fixed (NSVR_System** system_ptr = &_ptr)
				{
					if (Interop.NSVR_FAILURE(Interop.NSVR_System_Create(system_ptr)))
					{
						Debug.LogError("[NSVR] NSVR_Plugin could not be instantiated\n");

					}
					else
					{
						_created = true;
					}
				}
				

			}

			
			/// <summary>
			/// Internal testing tool; do not depend upon this. May change at any time.
			/// </summary>
			/// <returns></returns>
			public Dictionary<AreaFlag, EffectSampleInfo> SampleCurrentlyPlayingEffects()
			{
				Dictionary<AreaFlag, EffectSampleInfo> result = new Dictionary<AreaFlag, EffectSampleInfo>();
				UInt16[] strengths = new UInt16[16];
				UInt32[] areas = new UInt32[16];
				UInt32[] families = new UInt32[16];
				uint totalCount = 0;
				Interop.NSVR_Immediate_Sample(Ptr, strengths, areas, families, 16, ref totalCount);

				for (int i = 0; i < totalCount; i++)
				{
					result[(AreaFlag)areas[i]] = new EffectSampleInfo(strengths[i], families[i], (AreaFlag)areas[i]);
				}

				return result;
			}

			

			/** END INTERNAL **/
			/// <summary>
			/// Pause all currently active effects
			/// </summary>
			public void PauseAll()
			{
				Interop.NSVR_System_Haptics_Pause(Ptr);
			}


			/// <summary>
			/// Resume all effects that were paused with a call to PauseAll()
			/// </summary>
			public void ResumeAll()
			{
				Interop.NSVR_System_Haptics_Resume(Ptr);
			}

			/// <summary>
			/// Destroy all effects (invalidates any HapticHandles)
			/// </summary>
			public void ClearAll()
			{
				Interop.NSVR_System_Haptics_Destroy(Ptr);
			}

			/// <summary>
			/// Return the plugin version
			/// </summary>
			/// <returns></returns>
			public static VersionInfo GetPluginVersion()
			{
				uint version = Interop.NSVR_Version_Get();
				VersionInfo v = new VersionInfo();
				v.Minor = version & 0xFFFF;
				v.Major = (version & 0xFFFF0000) >> 16;
				return v;
			}

			/// <summary>
			/// Poll the status of suit connection 
			/// </summary>
			/// <returns>Connected if the service is running and a suit is plugged in, else Disconnected</returns>
			public DeviceConnectionStatus TestDeviceConnection()
			{

				Interop.NSVR_DeviceInfo deviceInfo = new Interop.NSVR_DeviceInfo();
				
				if (Interop.NSVR_SUCCESS(Interop.NSVR_System_GetDeviceInfo(Ptr, ref deviceInfo)))
				{
					return DeviceConnectionStatus.Connected;
				}


				return DeviceConnectionStatus.Disconnected;
			}

		

			public ServiceConnectionStatus TestServiceConnection()
			{
				Interop.NSVR_ServiceInfo serviceInfo = new Interop.NSVR_ServiceInfo();
				int value = Interop.NSVR_System_GetServiceInfo(Ptr, ref serviceInfo);

				//	Debug.Log(string.Format("Value is {0}", value));
				if (Interop.NSVR_SUCCESS(value))
				{
					return ServiceConnectionStatus.Connected;
				} else
				{
					return ServiceConnectionStatus.Disconnected;
				}
			}

			/// <summary>
			/// Enable tracking on the suit
			/// </summary>
			public void EnableTracking()
			{
				Interop.NSVR_System_Tracking_Enable(Ptr);

			}

			/// <summary>
			/// Disable tracking on the suit 
			/// </summary>
			public void DisableTracking()
			{
				Interop.NSVR_System_Tracking_Disable(Ptr);
			}

			/// <summary>
			/// Enable or disable tracking
			/// </summary>
			/// <param name="enableTracking">If true, enables tracking. Else disables tracking.</param>
			public void SetTrackingEnabled(bool enableTracking)
			{
				if (enableTracking)
				{
					Interop.NSVR_System_Tracking_Enable(Ptr);
				}
				else
				{
					Interop.NSVR_System_Tracking_Disable(Ptr);

				}
			}

			/// <summary>
			/// Poll the suit for the latest tracking data
			/// </summary>
			/// <returns>A data structure containing all valid quaternion data</returns>
			public TrackingUpdate PollTracking()
			{
				Interop.NSVR_TrackingUpdate t = new Interop.NSVR_TrackingUpdate();
				Interop.NSVR_System_Tracking_Poll(Ptr, ref t);

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

					fixed (NSVR_System** ptr = &_ptr)
					{
						Interop.NSVR_System_Release(ptr);
					}

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

			/// <summary>
			/// Disposes the plugin. After calling dispose, the plugin cannot be used again.
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
	}

	/// <summary>
	/// Able to hold tracking data for chest and arm IMUs
	/// </summary>
	public struct TrackingUpdate
	{
		public UnityEngine.Quaternion Chest;
		public UnityEngine.Quaternion LeftUpperArm;
		public UnityEngine.Quaternion LeftForearm;
		public UnityEngine.Quaternion RightUpperArm;
		public UnityEngine.Quaternion RightForearm;
	}

	/// <summary>
	/// Use a HapticHandle to Play, Pause, or Stop an effect. A HapticHandle represents a particular instance of an effect.
	/// </summary>
	public sealed class HapticHandle : IDisposable
	{
		private IntPtr _handle;
		private CommandWithHandle _creator; 

		internal delegate void CommandWithHandle(IntPtr handle);

		internal HapticHandle(CommandWithHandle creator)
		{
			//The reason we are storing the creator is so that people can clone the handle
			_creator = creator;
			Interop.NSVR_PlaybackHandle_Create(ref _handle);
			_creator(_handle);
		}

		/// <summary>
		/// Cause the associated effect to play. If paused, play will resume where it left off. If stopped, play will resume from the beginning. 
		/// </summary>
		/// <returns>Reference to this HapticHandle</returns>
		public HapticHandle Play()
		{
			Interop.NSVR_PlaybackHandle_Command(_handle, Interop.NSVR_PlaybackCommand.Play);
			return this;
		}

		/// <summary>
		/// Cause the associated effect to immediately play from the beginning.
		/// Identical to Stop().Play()
		/// </summary>
		/// <returns></returns>
		public HapticHandle Replay()
		{
			return this.Stop().Play();
		}

		/// <summary>
		/// Cause the associated effect to pause. 
		/// </summary>
		/// <returns>Reference to this HapticHandle</returns>
		public HapticHandle Pause()
		{
			Interop.NSVR_PlaybackHandle_Command(_handle, Interop.NSVR_PlaybackCommand.Pause);
			return this;
		}

		/// <summary>
		/// Cause the associated effect to stop. Will reset the effect to the beginning in a paused state. 
		/// </summary>
		/// <returns>Reference to this HapticHandle</returns>
		public HapticHandle Stop()
		{
			Interop.NSVR_PlaybackHandle_Command(_handle, Interop.NSVR_PlaybackCommand.Reset);
			return this;
		}


		/// <summary>
		/// Clone this HapticHandle, allowing an identical effect to be controlled independently 
		/// </summary>
		/// <returns></returns>
		public HapticHandle Clone()
		{
			HapticHandle newHandle = new HapticHandle(this._creator);
			return newHandle;
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
					Interop.NSVR_PlaybackHandle_Release(ref _handle);
				}

				disposedValue = true;
			}
		}

		// TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
		~HapticHandle() {
		   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
		   Dispose(false);
		}

		/// <summary>
		/// Dispose the handle, releasing its resources from the plugin. After disposing a handle, it cannot be used again.
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
	
	
}



