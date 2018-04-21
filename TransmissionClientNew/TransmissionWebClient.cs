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

using System;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using TransmissionRemoteDotnet.Forms;
using TransmissionRemoteDotnet.Settings;

namespace TransmissionRemoteDotnet
{
    public class TransmissionWebClient : WebClient
    {
        private readonly bool _authenticate;
        private readonly bool _rpc;

        public static string XTransmissionSessionId { get; set; }

        public TransmissionWebClient(bool rpc, bool authenticate)
        {
            _rpc = rpc;
            _authenticate = authenticate;
        }

        public static bool ValidateServerCertificate(
                    object sender,
                    X509Certificate certificate,
                    X509Chain chain,
                    SslPolicyErrors sslPolicyErrors)
        {
            return sslPolicyErrors != SslPolicyErrors.RemoteCertificateNotAvailable; // we need certificate, but accept untrusted
        }

        public event EventHandler<ResultEventArgs> Completed;
        internal void OnCompleted(ICommand result)
        {
            Completed?.Invoke(this, new ResultEventArgs { Result = result });
        }

        protected override WebRequest GetWebRequest(Uri address)
        {
            WebRequest request = base.GetWebRequest(address);
            try
            {
                if (request?.GetType() == typeof(HttpWebRequest))
                    SetupWebRequest((HttpWebRequest)request, _rpc, _authenticate);
            }
            catch (PasswordEmptyException)
            {
                CancelAsync();
            }
            return request;
        }

        public static void SetupWebRequest(HttpWebRequest request, bool rpc, bool authenticate)
        {
            request.KeepAlive = false;
            request.Headers.Add(HttpRequestHeader.AcceptEncoding, "gzip, deflate");
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            request.UserAgent = $"{AboutDialog.AssemblyProduct}/{AboutDialog.AssemblyVersion}";
            if (XTransmissionSessionId != null && authenticate && rpc)
                request.Headers["X-Transmission-Session-Id"] = XTransmissionSessionId;
            if (!rpc)
                request.CookieContainer = PersistentCookies.GetCookieContainerForUrl(request.RequestUri);
            TransmissionServer settings = Program.Settings.Current;
            if (settings.AuthEnabled && authenticate)
            {
                request.Credentials = new NetworkCredential(settings.Username, settings.ValidPassword);
                request.PreAuthenticate = Program.DaemonDescriptor.Version < 1.40 || Program.DaemonDescriptor.Version >= 1.6;
            }
            if (settings.Proxy.ProxyMode == ProxyMode.Enabled)
            {
                request.Proxy = new WebProxy(settings.Proxy.Host, settings.Proxy.Port);
                if (settings.Proxy.AuthEnabled)
                {
                    string[] user = settings.Proxy.Username.Split("\\".ToCharArray(), 2);
                    if (user.Length > 1)
                        request.Proxy.Credentials = new NetworkCredential(user[1], settings.Proxy.ValidPassword, user[0]);
                    else
                        request.Proxy.Credentials = new NetworkCredential(settings.Proxy.Username, settings.Proxy.ValidPassword);
                }
            }
            else if (settings.Proxy.ProxyMode == ProxyMode.Disabled)
            {
                request.Proxy = null;
            }
        }
    }
}