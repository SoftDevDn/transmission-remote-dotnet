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

/* This factory class is responsible for dispatching JSON requests and
 * torrent uploads, and also creating an object using the command design
 * pattern which contains the logic for updating the UI. */

using System;
using System.Text;
using System.Net;
using Jayrock.Json;
using Jayrock.Json.Conversion;
using System.IO;
using TransmissionRemoteDotnet.Commands;
using TransmissionRemoteDotnet.Forms;
using TransmissionRemoteDotnet.Localization;

namespace TransmissionRemoteDotnet
{
    public static class CommandFactory
    {
        private static readonly Encoding TransmissionEncoding = Encoding.UTF8;
        private static int _requestid;
        /* 
         * If this doesnt good, we should write a own converter like T:
         * libtransmission/bencode.c:1308
         */
        private static byte[] GetBytes(string data)
        {
            return TransmissionEncoding.GetBytes(data);
        }
        private static string GetString(byte[] data)
        {
            return TransmissionEncoding.GetString(data);
        }

        public static TransmissionWebClient RequestAsync(JsonObject data)
        {
            return RequestAsync(data, true);
        }

        public static TransmissionWebClient RequestAsync(JsonObject data, bool allowRecursion)
        {
            TransmissionWebClient wc = new TransmissionWebClient(true, true);
            byte[] bdata = GetBytes(data.ToString());
            int r = _requestid++;
#if LOGRPC
            Program.LogDebug("RPC request: " + r, data.ToString());
#endif
            wc.UploadDataCompleted += wc_UploadDataCompleted;
            wc.UploadDataAsync(new Uri(Program.Settings.Current.RpcUrl), null, bdata, new TransmissonRequest(r, bdata, allowRecursion));
            return wc;
        }

        static void wc_UploadDataCompleted(object sender, UploadDataCompletedEventArgs e)
        {
            if (!e.Cancelled)
            {
                ICommand cmd;
                if (e.Error != null)
                {
                    var ex = e.Error as WebException;
                    string title = OtherStrings.Error;
                    if (ex?.Response != null)
                    {
                        HttpWebResponse response = (HttpWebResponse)ex.Response;
                        if (response.StatusCode == HttpStatusCode.Conflict && ((TransmissonRequest)e.UserState).allowRecursion)
                        {
                            try
                            {
                                string sessionid = ex.Response.Headers["X-Transmission-Session-Id"];
                                if (!string.IsNullOrEmpty(sessionid))
                                {
                                    TransmissionWebClient.XTransmissionSessionId = sessionid;
                                    ((TransmissionWebClient) sender).UploadDataAsync(new Uri(Program.Settings.Current.RpcUrl), null, ((TransmissonRequest)e.UserState).data, new TransmissonRequest(((TransmissonRequest)e.UserState).requestid, ((TransmissonRequest)e.UserState).data, false));
                                    return;
                                }
                            }
                            catch { }
                        }
                        else if (response.StatusCode == HttpStatusCode.NotFound)
                        {
                            title = OtherStrings.NotFound;
                            ex = new WebException(OtherStrings.NotFoundError);
                        }
                        else if (response.StatusCode == HttpStatusCode.Forbidden || response.StatusCode == HttpStatusCode.Unauthorized)
                        {
                            using (StreamReader sr = new StreamReader(response.GetResponseStream(), Encoding.GetEncoding(response.CharacterSet)))
                            {
                                String s = sr.ReadToEnd();
                                if (s.Contains("Unauthorized IP Address."))
                                {
                                    title = OtherStrings.UnauthorizedIP;
                                    ex = new WebException(OtherStrings.UnauthorizedIPError);
                                }
                                else if (s.Contains("Unauthorized User"))
                                {
                                    title = OtherStrings.UnauthorizedUser;
                                    ex = new WebException(OtherStrings.UnauthorizedUserError);
                                }
                            }
                        }
                    }
                    cmd = new ErrorCommand(title, ex?.Message, false);
                }
                else
                {
                    try
                    {
#if LOGRPC
                        Program.LogDebug("RPC response: " + ((TransmissonRequest)e.UserState).requestid, GetString(e.Result));
#endif
                        JsonObject jsonResponse = (JsonObject)JsonConvert.Import(GetString(e.Result));
                        if ((string)jsonResponse["result"] != "success")
                        {
                            string response = (string)jsonResponse["result"];
                            if (response.StartsWith("http error"))
                            {
                                int i = response.IndexOf(':');
                                if (i >= 0)
                                    response = response.Substring(i + 2);
                                else
                                    response = response.Remove(0, 11); /* strlen("http error") = 11 */
                                cmd = new ErrorCommand(OtherStrings.UnsuccessfulRequest, string.Format(OtherStrings.HttpError, Environment.NewLine, response), true);
                            }
                            else
                                cmd = new ErrorCommand(OtherStrings.UnsuccessfulRequest, response, true);
                            if (Toolbox.ToShort(jsonResponse[ProtocolConstants.KEY_TAG]).Equals((short)ResponseTag.UpdateBlocklist))
                                RemoteSettingsDialog.BlocklistUpdateDone(-1);
                        }
                        else
                        {
                            switch (Toolbox.ToShort(jsonResponse[ProtocolConstants.KEY_TAG]))
                            {
                                case (short)ResponseTag.TorrentGet:
                                    cmd = new TorrentGetCommand(jsonResponse);
                                    break;
                                case (short)ResponseTag.SessionGet:
                                    cmd = new SessionCommand(jsonResponse, ((WebClient) sender).Headers);
                                    break;
                                case (short)ResponseTag.SessionStats:
                                    cmd = new SessionStatsCommand(jsonResponse);
                                    break;
                                case (short)ResponseTag.UpdateFiles:
                                    cmd = new UpdateFilesCommand(jsonResponse);
                                    break;
                                case (short)ResponseTag.PortTest:
                                    cmd = new PortTestCommand(jsonResponse);
                                    break;
                                case (short)ResponseTag.UpdateBlocklist:
                                    cmd = new UpdateBlocklistCommand(jsonResponse);
                                    break;
                                case (short)ResponseTag.DoNothing:
                                    cmd = new NoCommand();
                                    break;
                                default:
                                    cmd = new ErrorCommand(OtherStrings.UnknownResponseTag, e.Result != null ? GetString(e.Result) : "null", false);
                                    break;
                            }
                        }
                    }
                    catch (InvalidCastException)
                    {
                        cmd = new ErrorCommand(OtherStrings.UnableToParse, e.Result != null ? GetString(e.Result) : "Null", false);
                    }
                    catch (JsonException ex)
                    {
                        cmd = new ErrorCommand($"{OtherStrings.UnableToParse} ({ex.GetType()})", GetString(e.Result), false);
                    }
                    catch (Exception ex)
                    {
                        cmd = new ErrorCommand(ex, false);
                    }
                }
                try
                {
                    cmd.Execute();
                }
                catch (Exception ee)
                { // just for debugging...
                    Console.WriteLine(ee.Message);
                    Program.LogDebug(ee.Source, ee.Message);
                }
                ((TransmissionWebClient) sender).OnCompleted(cmd);
            }
            else
            {
                if (!Program.Connected)
                {
                    Program.Connected = false;
                    Program.Form.UpdateStatus("", false);
                    Program.Form.connectButton.Enabled = Program.Form.connectToolStripMenuItem.Enabled = true;
                }
            }
        }
    }

    struct TransmissonRequest
    {
        public byte[] data;
        public bool allowRecursion;
        public int requestid;
        public TransmissonRequest(int requestid, byte[] data, bool allowRecursion)
        {
            this.requestid = requestid;
            this.data = data;
            this.allowRecursion = allowRecursion;
        }
    }
}