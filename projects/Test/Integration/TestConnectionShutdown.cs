// This source code is dual-licensed under the Apache License, version
// 2.0, and the Mozilla Public License, version 2.0.
//
// The APL v2.0:
//
//---------------------------------------------------------------------------
//   Copyright (c) 2007-2020 VMware, Inc.
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       https://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.
//---------------------------------------------------------------------------
//
// The MPL v2.0:
//
//---------------------------------------------------------------------------
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
//
//  Copyright (c) 2007-2020 VMware, Inc.  All rights reserved.
//---------------------------------------------------------------------------

using System;
using System.IO;
using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;
using RabbitMQ.Client.Framing.Impl;
using RabbitMQ.Client.Impl;
using Xunit;
using Xunit.Abstractions;

namespace Test.Integration
{
    public class TestConnectionShutdown : IntegrationFixture
    {
        public TestConnectionShutdown(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public async Task TestCleanClosureWithSocketClosedOutOfBand()
        {
            var tcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
            _channel.ChannelShutdown += (channel, args) =>
            {
                tcs.SetResult(true);
            };

            var c = (AutorecoveringConnection)_conn;
            await c.CloseFrameHandlerAsync();

            try
            {
                await _conn.CloseAsync(TimeSpan.FromSeconds(4));
            }
            catch (AlreadyClosedException ex)
            {
                Assert.IsAssignableFrom<IOException>(ex.InnerException);
            }

            await WaitAsync(tcs, TimeSpan.FromSeconds(5), "channel shutdown");
        }

        [Fact]
        public async Task TestAbortWithSocketClosedOutOfBand()
        {
            var tcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
            _channel.ChannelShutdown += (channel, args) =>
            {
                tcs.SetResult(true);
            };

            var c = (AutorecoveringConnection)_conn;
            await c.CloseFrameHandlerAsync();

            await _conn.AbortAsync();

            // default Connection.Abort() timeout and then some
            await WaitAsync(tcs, TimeSpan.FromSeconds(6), "channel shutdown");
        }

        [Fact]
        public async Task TestDisposedWithSocketClosedOutOfBand()
        {
            try
            {
                var tcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);

                _conn.ConnectionShutdown += (conn, args) =>
                {
                    tcs.TrySetResult(true);
                };

                _channel.ChannelShutdown += (channel, args) =>
                {
                    tcs.TrySetResult(true);
                };

                var c = (AutorecoveringConnection)_conn;
                await c.CloseFrameHandlerAsync();

                _conn.Dispose();

                TimeSpan waitSpan = TimeSpan.FromSeconds(3);
                if (IsRunningInCI)
                {
                    waitSpan = TimeSpan.FromSeconds(10);
                }

                await WaitAsync(tcs, waitSpan, "connection or channel shutdown");
            }
            finally
            {
                _channel = null;
                _conn = null;
            }
        }

        [Fact]
        public async Task TestShutdownSignalPropagationToChannels()
        {
            var tcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);

            _channel.ChannelShutdown += (channel, args) =>
            {
                tcs.SetResult(true);
            };

            await _conn.CloseAsync();

            await WaitAsync(tcs, TimeSpan.FromSeconds(3), "channel shutdown");
        }

        [Fact]
        public async Task TestShutdownSignalPropagationToChannelsUsingDispose()
        {
            try
            {
                var tcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);

                _channel.ChannelShutdown += (channel, args) =>
                {
                    tcs.SetResult(true);
                };

                _conn.Dispose();
                _conn = null;

                await WaitAsync(tcs, TimeSpan.FromSeconds(3), "channel shutdown");
            }
            finally
            {
                _channel = null;
                _conn = null;
            }
        }

        [Fact]
        public async Task TestConsumerDispatcherShutdown()
        {
            var m = (AutorecoveringChannel)_channel;
            var tcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);

            _channel.ChannelShutdown += (channel, args) =>
            {
                tcs.SetResult(true);
            };
            Assert.False(m.ConsumerDispatcher.IsShutdown, "dispatcher should NOT be shut down before Close");
            await _conn.CloseAsync();
            await WaitAsync(tcs, TimeSpan.FromSeconds(3), "channel shutdown");
            Assert.True(m.ConsumerDispatcher.IsShutdown, "dispatcher should be shut down after Close");
        }
    }
}
