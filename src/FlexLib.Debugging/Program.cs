﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Flex.Smoothlake.FlexLib;

namespace FlexLib.Debugging
{
    class MainClass
    {

        static void Main(string[] args)
        {

            API.RadioAdded += async (radio) =>
            {
                radio.Connect();
                var adapters = await radio.WaitForPanadaptersAsync();
                Console.WriteLine(string.Format("{0} of adapters",adapters.Count()));
                adapters.ForEach(p=>p.Close(true));
                var panAdapter = radio.GetOrCreatePanadapterSync(0, 0);
                //var panAdapters = await WaitForPanadaptersAsync(radio);
                //var panAdapter = panAdapters.FirstOrDefault();
                //if (panAdapter == null)
                //{
                //    panAdapter = await radio.CreatePanadapterAsync(0, 0);                	
                //}

                panAdapter.DAXIQChannel = 1;
				radio.SliceList
    				.Where(s => s.Panadapter == panAdapter)
    				.ToList()
    				.ForEach(s => s.Remove(true));

				var iq = radio.FindIQStreamByDAXIQChannel(1);
                if (iq == null)
                {
                    iq = await radio.CreateIQStreamAsync(1);
                }

                iq.DataReady += IQ_Data_Received;
                iq.SampleRate = 192000;


            };

            API.Init();
            Console.WriteLine("Press any key to exit...");
            Console.ReadLine();
			API.CloseSession();
        }

        private static async Task<List<Panadapter>> WaitForPanadaptersAsync(Radio radio, int waitMs = 2000)
        {
            CancellationTokenSource cts = new CancellationTokenSource(waitMs);
            var token = cts.Token;
            return await Task.Run(() =>
            {
                // Wait for panadapters
                while (radio.PanadapterList.Count == 0)
                {
                    if (token.IsCancellationRequested)
                    {
                        return new List<Panadapter>();
                    }
                }
                // Return the first panAdapter
                return radio.PanadapterList;
            }, token);
        }

        private static void IQ_Data_Received(IQStream iq_stream, float[] data)
        {
            Console.Write("Yes sir");
        }
    }
}
