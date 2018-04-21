// transmission-remote-dotnet
// http://code.google.com/p/transmission-remote-dotnet/
// Copyright (C) 2009 Alan F
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.

#if !DOTNET35
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;
using Jayrock.Json;
using Jayrock.Json.Conversion;

namespace TransmissionRemoteDotnet
{
    class TcpSingleInstance : IDisposable
    {
        private readonly int _port;
        private readonly TcpListener _listener;

        public TcpSingleInstance(int port)
        {
            try
            {
                _listener = new TcpListener(IPAddress.Parse("127.0.0.1"), port);
                _listener.Start();
                _port = ((IPEndPoint) _listener.LocalEndpoint).Port;
                IsFirstInstance = true;
            }
            catch
            {
                _port = port;
                IsFirstInstance = false;
            }
        }

        #region ISingleInstance Members

        public event EventHandler<ArgumentsReceivedEventArgs> ArgumentsReceived;

        public bool IsFirstInstance { get; }

        public void ListenForArgumentsFromSuccessiveInstances()
        {
            if (!IsFirstInstance)
                throw new InvalidOperationException("This is not the first instance.");
            ThreadPool.QueueUserWorkItem(ListenForArguments);
        }

        private void ListenForArguments(object state)
        {
            // TODO: Do something with recursive execution.
            StreamReader reader = null;
            try
            {
                Stream clientStream = _listener.AcceptTcpClient().GetStream();
                reader = new StreamReader(clientStream);
                List<string> arguments = new List<string>();
                foreach (string arg in (JsonArray)JsonConvert.Import(reader.ReadLine()))
                {
                    if (!string.IsNullOrEmpty(arg))
                        arguments.Add(arg);
                }
                ThreadPool.QueueUserWorkItem(CallOnArgumentsReceived, arguments.ToArray());
            }
            catch
            { }
            finally
            {
                reader?.Close();
                ListenForArguments(null);
            }
        }

        private void CallOnArgumentsReceived(object state)
        {
            ArgumentsReceived?.Invoke(this, new ArgumentsReceivedEventArgs { Args = (string[])state });
        }

        public bool PassArgumentsToFirstInstance(string[] arguments)
        {
            var client = new TcpClient();
            client.Connect("127.0.0.1", _port);
            StreamWriter writer = new StreamWriter(client.GetStream());
            writer.WriteLine(new JsonArray(arguments).ToString());
            writer.Close();
            return true;
        }

        #endregion

        #region IDisposable
        private bool _disposed;

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                _listener.Stop();
                _disposed = true;
            }
        }

        ~TcpSingleInstance()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
#endif