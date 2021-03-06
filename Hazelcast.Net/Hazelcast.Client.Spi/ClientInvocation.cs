﻿// Copyright (c) 2008-2015, Hazelcast, Inc. All Rights Reserved.
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

using Hazelcast.Client.Connection;
using Hazelcast.Client.Protocol;
using Hazelcast.IO;
using Hazelcast.Util;

namespace Hazelcast.Client.Spi
{
    internal class ClientInvocation
    {
        private readonly ClientConnection _boundConnection;
        private readonly SettableFuture<IClientMessage> _future;
        private readonly string _memberUuid;
        private readonly IClientMessage _message;
        private readonly int _partitionId = -1;

        // the point at which the request should be considered timed out
        private readonly long _invocationTimeMillis; 

        public ClientInvocation(IClientMessage message)
        {
            _message = message;
            _future = new SettableFuture<IClientMessage>();
            _invocationTimeMillis = Clock.CurrentTimeMillis();
        }

        public ClientInvocation(IClientMessage message, int partitionId) : this(message)
        {
            _partitionId = partitionId;
        }

        public ClientInvocation(IClientMessage message, string memberUuid) : this(message)
        {
            _memberUuid = memberUuid;
        }

        public ClientInvocation(IClientMessage message, Address address) : this(message)
        {
            Address = address;
        }

        public ClientInvocation(IClientMessage message, ClientConnection boundConnection)
            : this(message)
        {
            _boundConnection = boundConnection;
        }

        public string MemberUuid
        {
            get { return _memberUuid; }
        }

        public int PartitionId
        {
            get { return _partitionId; }
        }

        public IClientMessage Message
        {
            get { return _message; }
        }

        public SettableFuture<IClientMessage> Future
        {
            get { return _future; }
        }

        /// <summary>
        /// Connection that was used to execute this invocation
        /// </summary>
        public ClientConnection SentConnection { get; set; }

        public ClientConnection BoundConnection
        {
            get { return _boundConnection; }
        }

        public Address Address { get; private set; }

        public long InvocationTimeMillis
        {
            get { return _invocationTimeMillis; }
        }
    }
}