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

using System.Text;

namespace Hazelcast.IO.Serialization
{
    internal sealed class StreamSerializerAdapter<T> : ISerializerAdapter
    {
        private readonly IStreamSerializer<T> _serializer;
        private readonly SerializationService _service;

        public StreamSerializerAdapter(SerializationService service, IStreamSerializer<T> serializer)
        {
            _service = service;
            _serializer = serializer;
        }

        /// <exception cref="System.IO.IOException"></exception>
        public void Write(IObjectDataOutput output, object obj)
        {
            _serializer.Write(output, (T) obj);
        }

        /// <exception cref="System.IO.IOException"></exception>
        public object Read(IObjectDataInput input)
        {
            return _serializer.Read(input);
        }

        public int GetTypeId()
        {
            return _serializer.GetTypeId();
        }

        public void Destroy()
        {
            _serializer.Destroy();
        }

        public ISerializer GetImpl()
        {
            return _serializer;
        }

        public override bool Equals(object o)
        {
            if (this == o)
            {
                return true;
            }
            if (o == null || GetType() != o.GetType())
            {
                return false;
            }
            var that = (StreamSerializerAdapter<T>) o;
            if (_serializer != null ? !_serializer.Equals(that._serializer) : that._serializer != null)
            {
                return false;
            }
            return true;
        }

        public override int GetHashCode()
        {
            return _serializer != null ? _serializer.GetHashCode() : 0;
        }

        public override string ToString()
        {
            var sb = new StringBuilder("SerializerAdapter{");
            sb.Append("serializer=").Append(_serializer);
            sb.Append('}');
            return sb.ToString();
        }
    }
}