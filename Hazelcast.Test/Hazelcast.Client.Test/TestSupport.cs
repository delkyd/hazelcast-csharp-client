// Copyright (c) 2008-2015, Hazelcast, Inc. All Rights Reserved.
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
// http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Hazelcast.Core;
using Hazelcast.Util;
using NUnit.Framework;

namespace Hazelcast.Client.Test
{
    internal static class TestSupport
    {
        private const int TimeoutSeconds = 30;
        private static readonly Random Random = new Random();

        public static void AssertCompletedEventually<T>(Task<T> task, int timeoutSeconds = TimeoutSeconds,
            string taskName = "")
        {
            Assert.IsTrue(task.Wait(timeoutSeconds*1000),
                "Task " + taskName + " did not complete in " + timeoutSeconds + " seconds");
        }

        public static void AssertOpenEventually(CountdownEvent latch, int timeoutSeconds = TimeoutSeconds,
            string message = null)
        {
            var completed = latch.Wait(timeoutSeconds*1000);
            if (message == null)
            {
                Assert.IsTrue(completed,
                    string.Format("CountDownLatch failed to complete within {0} seconds , count left: {1}",
                        timeoutSeconds,
                        latch.CurrentCount));
            }
            else
            {
                Assert.IsTrue(completed,
                    string.Format("{0}, CountDownLatch failed to complete within {1} seconds , count left: {2}", message,
                        timeoutSeconds,
                        latch.CurrentCount));
            }
        }

        public static void AssertTrueEventually(Func<bool> assertFunc, int timeoutSeconds = TimeoutSeconds,
            string assertion = null)
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();

            while (stopWatch.ElapsedMilliseconds < timeoutSeconds*1000)
            {
                if (assertFunc()) return;
                Thread.Sleep(250);
            }

            Assert.Fail("Could not verify assertion " + assertion + " after " + timeoutSeconds + " seconds");
        }

        public static void AssertTrueEventually(Action asserAction, int timeoutSeconds = TimeoutSeconds,
            string assertion = null)
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();

            Exception last = null;
            while (stopWatch.ElapsedMilliseconds < timeoutSeconds*1000)
            {
                try
                {
                    asserAction();
                    return;
                }
                catch (AssertionException e)
                {
                    Thread.Sleep(250);
                    last = e;
                }
            }
            Assert.Fail("Could not verify assertion " + assertion + " after " + timeoutSeconds + " seconds: " + last);
        }

        public static T[] RandomArray<T>(Func<T> randFunc, int size = 0)
        {
            var array = new T[size == 0 ? Random.Next(5) + 1 : size];
            for (var i = 0; i < array.Length; i++)
            {
                array[i] = randFunc();
            }
            return array;
        }

        public static bool RandomBool()
        {
            return Random.Next() > 0;
        }

        public static byte RandomByte()
        {
            return (byte) Random.Next();
        }

        public static byte[] RandomBytes()
        {
            var bytes = new byte[10];
            Random.NextBytes(bytes);
            return bytes;
        }

        public static char RandomChar()
        {
            return RandomString()[0];
        }

        public static double RandomDouble()
        {
            return Random.NextDouble();
        }

        public static float RandomFloat()
        {
            return (float) Random.NextDouble();
        }

        public static int RandomInt()
        {
            return Random.Next();
        }

        public static long RandomLong()
        {
            var buffer = new byte[8];
            Random.NextBytes(buffer);
            return BitConverter.ToInt64(buffer, 0);
        }

        public static short RandomShort()
        {
            return (short) Random.Next();
        }

        public static string RandomString()
        {
            return Guid.NewGuid().ToString();
        }

        public static Task<bool> WaitForClientState(IHazelcastInstance instance, LifecycleEvent.LifecycleState state)
        {
            var task = new TaskCompletionSource<bool>();
            var regId = instance.GetLifecycleService().AddLifecycleListener(new LifecycleListener(l =>
            {
                if (l.GetState() == state)
                {
                    task.TrySetResult(true);
                }
            }));

            task.Task.ContinueWith(f => { instance.GetLifecycleService().RemoveLifecycleListener(regId); })
                .IgnoreExceptions();

            return task.Task;
        }
    }
}