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
using System.Threading.Tasks;
using Hazelcast.Core;

namespace Hazelcast.Client.Spi
{
    public interface IClientExecutionService
    {
        Task Schedule(Action command, long delay, TimeUnit unit);
        Task<object> ScheduleWithFixedDelay(Action command, long initialDelay, long period, TimeUnit unit);
        void Shutdown();
        Task Submit(Action action);
        Task<T> Submit<T>(Func<T> function);
    }
}