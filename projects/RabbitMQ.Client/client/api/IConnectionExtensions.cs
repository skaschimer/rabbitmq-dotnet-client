﻿using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace RabbitMQ.Client
{
    public static class IConnectionExtensions
    {
        /// <summary>
        /// Asynchronously close this connection and all its channels.
        /// </summary>
        /// <remarks>
        /// Note that all active channels and sessions will be
        /// closed if this method is called. It will wait for the in-progress
        /// close operation to complete. This method will not return to the caller
        /// until the shutdown is complete. If the connection is already closed
        /// (or closing), then this method will do nothing.
        /// It can also throw <see cref="IOException"/> when socket was closed unexpectedly.
        /// </remarks>
        public static Task CloseAsync(this IConnection connection,
            CancellationToken cancellationToken = default)
        {
            return connection.CloseAsync(Constants.ReplySuccess, "Goodbye", InternalConstants.DefaultConnectionCloseTimeout, false,
                cancellationToken);
        }

        /// <summary>
        /// Asynchronously close this connection and all its channels.
        /// </summary>
        /// <remarks>
        /// The method behaves in the same way as <see cref="CloseAsync(IConnection, CancellationToken)"/>, with the only
        /// difference that the connection is closed with the given connection close code and message.
        /// <para>
        /// The close code (See under "Reply Codes" in the AMQP specification).
        /// </para>
        /// <para>
        /// A message indicating the reason for closing the connection.
        /// </para>
        /// </remarks>
        public static Task CloseAsync(this IConnection connection, ushort reasonCode, string reasonText,
            CancellationToken cancellationToken = default)
        {
            return connection.CloseAsync(reasonCode, reasonText, InternalConstants.DefaultConnectionCloseTimeout, false,
                cancellationToken);
        }

        /// <summary>
        /// Asynchronously close this connection and all its channels
        /// and wait with a timeout for all the in-progress close operations to complete.
        /// </summary>
        /// <remarks>
        /// Note that all active channels and sessions will be
        /// closed if this method is called. It will wait for the in-progress
        /// close operation to complete with a timeout. If the connection is
        /// already closed (or closing), then this method will do nothing.
        /// It can also throw <see cref="IOException"/> when socket was closed unexpectedly.
        /// If timeout is reached and the close operations haven't finished, then socket is forced to close.
        /// <para>
        /// To wait infinitely for the close operations to complete use <see cref="System.Threading.Timeout.InfiniteTimeSpan"/>.
        /// </para>
        /// </remarks>
        public static Task CloseAsync(this IConnection connection, TimeSpan timeout,
            CancellationToken cancellationToken = default)
        {
            return connection.CloseAsync(Constants.ReplySuccess, "Goodbye", timeout, false,
                cancellationToken);
        }

        /// <summary>
        /// Asynchronously close this connection and all its channels
        /// and wait with a timeout for all the in-progress close operations to complete.
        /// </summary>
        /// <remarks>
        /// The method behaves in the same way as <see cref="CloseAsync(IConnection, TimeSpan, CancellationToken)"/>, with the only
        /// difference that the connection is closed with the given connection close code and message.
        /// <para>
        /// The close code (See under "Reply Codes" in the AMQP 0-9-1 specification).
        /// </para>
        /// <para>
        /// A message indicating the reason for closing the connection.
        /// </para>
        /// <para>
        /// Operation timeout.
        /// </para>
        /// </remarks>
        public static Task CloseAsync(this IConnection connection, ushort reasonCode, string reasonText, TimeSpan timeout,
            CancellationToken cancellationToken = default)
        {
            return connection.CloseAsync(reasonCode, reasonText, timeout, false,
                cancellationToken);
        }

        /// <summary>
        /// Asynchronously abort this connection and all its channels.
        /// </summary>
        /// <remarks>
        /// Note that all active channels and sessions will be closed if this method is called.
        /// In comparison to normal <see cref="CloseAsync(IConnection, CancellationToken)"/> method, <see cref="AbortAsync(IConnection, CancellationToken)"/> will not throw
        /// <see cref="IOException"/> during closing connection.
        ///This method waits infinitely for the in-progress close operation to complete.
        /// </remarks>
        public static Task AbortAsync(this IConnection connection,
            CancellationToken cancellationToken = default)
        {
            return connection.CloseAsync(Constants.ReplySuccess, "Connection close forced", InternalConstants.DefaultConnectionAbortTimeout, true,
                cancellationToken);
        }

        /// <summary>
        /// Asynchronously abort this connection and all its channels.
        /// </summary>
        /// <remarks>
        /// The method behaves in the same way as <see cref="AbortAsync(IConnection, CancellationToken)"/>, with the only
        /// difference that the connection is closed with the given connection close code and message.
        /// <para>
        /// The close code (See under "Reply Codes" in the AMQP 0-9-1 specification)
        /// </para>
        /// <para>
        /// A message indicating the reason for closing the connection
        /// </para>
        /// </remarks>
        public static Task AbortAsync(this IConnection connection, ushort reasonCode, string reasonText,
            CancellationToken cancellationToken = default)
        {
            return connection.CloseAsync(reasonCode, reasonText, InternalConstants.DefaultConnectionAbortTimeout, true,
                cancellationToken);
        }

        /// <summary>
        /// Asynchronously abort this connection and all its channels and wait with a
        /// timeout for all the in-progress close operations to complete.
        /// </summary>
        /// <remarks>
        /// This method, behaves in a similar way as method <see cref="AbortAsync(IConnection, CancellationToken)"/> with the
        /// only difference that it explicitly specifies a timeout given
        /// for all the in-progress close operations to complete.
        /// If timeout is reached and the close operations haven't finished, then socket is forced to close.
        /// <para>
        /// To wait infinitely for the close operations to complete use <see cref="Timeout.Infinite"/>.
        /// </para>
        /// </remarks>
        public static Task AbortAsync(this IConnection connection, TimeSpan timeout,
            CancellationToken cancellationToken = default)
        {
            return connection.CloseAsync(Constants.ReplySuccess, "Connection close forced", timeout, true,
                cancellationToken);
        }

        /// <summary>
        /// Asynchronously abort this connection and all its channels and wait with a
        /// timeout for all the in-progress close operations to complete.
        /// </summary>
        /// <remarks>
        /// The method behaves in the same way as <see cref="AbortAsync(IConnection, TimeSpan, CancellationToken)"/>, with the only
        /// difference that the connection is closed with the given connection close code and message.
        /// <para>
        /// The close code (See under "Reply Codes" in the AMQP 0-9-1 specification).
        /// </para>
        /// <para>
        /// A message indicating the reason for closing the connection.
        /// </para>
        /// </remarks>
        public static Task AbortAsync(this IConnection connection, ushort reasonCode, string reasonText, TimeSpan timeout,
            CancellationToken cancellationToken = default)
        {
            return connection.CloseAsync(reasonCode, reasonText, timeout, true,
                cancellationToken);
        }
    }
}
