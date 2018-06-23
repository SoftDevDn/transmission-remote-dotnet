using System;

namespace TransmissionRemoteDotnet
{
    public class ResultEventArgs : EventArgs
    {
        public ICommand Result { get; set; }
    }
}
