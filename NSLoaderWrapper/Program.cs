﻿using System;

using NullSpace.SDK;


namespace NSLoaderWrapper
{
	class Program
	{
	
		static void test()
		{
			Sequence s = new Sequence("ns.basic.click_click_click");
			var handle1 = s.CreateHandle(AreaFlag.All_Areas);
			var handle2 = s.CreateHandle(AreaFlag.All_Areas);

			var handle3 = s.CreateHandle(AreaFlag.All_Areas);

			var handle4 = s.CreateHandle(AreaFlag.All_Areas);

			//s.CreateHandle(AreaFlag.All_Areas).Dispose();

			//s.CreateHandle(AreaFlag.All_Areas).Dispose();
		}	
		static void Main(string[] args)
		{
			//	Stopwatch sw = new Stopwatch();



			//	loader.PlayEffect(1, 1, 1.9f, 1.0f, 1);
			//loader.PlayEffect(1, 1, 1.9f, 1.0f, 1);
			//bool toggle = true;
			Wrapper.NSVR_Plugin loader = new Wrapper.NSVR_Plugin(@"C:\Users\NullSpace Team\Documents\API_Environment\Assets\StreamingAssets");
		//	Sequence s = new Sequence("ns.basic.click_click_click");
			//s.CreateHandle(AreaFlag.All_Areas).Play();
			Pattern e = new Pattern("ns.basic.test");
			var h = e.CreateHandle();
			//Console.ReadLine();
			h.Play();
			//Console.ReadLine();
			//h.Pause();
			//Console.ReadLine();
			//h.Play();

			//while (true)
			//	{
			//	//handle1.Reset();
			//handle1.Play();


			//Console.WriteLine("HI");
			//Console.ReadLine();
			//	handle1.Play();


			//loader.SetTrackingEnabled(true);

			//	loader.SetTrackingEnabled(toggle);
			//	toggle = !toggle;
			//loader.PlaySequence("ns.basic.click_click_click", 0);
			//	int stat = loader.PollStatus();
			//	Console.WriteLine("Status: " + stat);
			//	Interop.Quaternion q = loader.PollTracking();
			//	Console.WriteLine("Quat: " + q.x + ", " + q.y + ", " + q.z + "," + q.w);

			//loader.PlayPattern("ns.body_jolt", 0);


			//	}
			//	loader.Dispose();
		}

	}
}
