//
// BEncodedNumber.cs
//
// Authors:
//   Alan McGovern alan.mcgovern@gmail.com
//
// Copyright (C) 2006 Alan McGovern
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//


using System;
using System.Diagnostics.CodeAnalysis;

namespace TransmissionRemoteDotnet.MonoTorrent
{
    /// <summary>
    /// Class representing a BEncoded number
    /// </summary>
    public class BEncodedNumber : BEncodedValue, IComparable<BEncodedNumber>
    {
        #region Member Variables
        /// <summary>
        /// The value of the BEncodedNumber
        /// </summary>
        private long Number { get; set; }
        #endregion


        #region Constructors
        public BEncodedNumber()
            : this(0)
        {
        }

        /// <summary>
        /// Create a new BEncoded number with the given value
        /// </summary>
        /// <param name="initialValue">The inital value of the BEncodedNumber</param>
        public BEncodedNumber(long initialValue)
        {
            Number = initialValue;
        }

        public static implicit operator BEncodedNumber(long value)
        {
            return new BEncodedNumber(value);
        }
        #endregion


        #region Encode/Decode Methods

        /// <summary>
        /// Encodes this number to the supplied byte[] starting at the supplied offset
        /// </summary>
        /// <param name="buffer">The buffer to write the data to</param>
        /// <param name="offset">The offset to start writing the data at</param>
        /// <returns></returns>
        public override int Encode(byte[] buffer, int offset)
        {
            long number = Number;

            int written = offset;
            buffer[written++] = (byte)'i';

            if (number < 0)
            {
                buffer[written++] = (byte)'-';
                number = -number;
            }
            // Reverse the number '12345' to get '54321'
            long reversed = 0;
            for (long i = number; i != 0; i /= 10)
                reversed = reversed * 10 + i % 10;

            // Write each digit of the reversed number to the array. We write '1'
            // first, then '2', etc
            for (long i = reversed; i != 0; i /= 10)
                buffer[written++] = (byte)(i % 10 + '0');

            if (number == 0)
                buffer[written++] = (byte)'0';

            // If the original number ends in one or more zeros, they are lost
            // when we reverse the number. We add them back in here.
            for (long i = number; i % 10 == 0 && number != 0; i /= 10)
                buffer[written++] = (byte)'0';

            buffer[written++] = (byte)'e';
            return written - offset;
        }


        /// <summary>
        /// Decodes a BEncoded number from the supplied RawReader
        /// </summary>
        /// <param name="reader">RawReader containing a BEncoded Number</param>
        internal override void DecodeInternal(RawReader reader)
        {
            if (reader == null)
                throw new ArgumentNullException(nameof(reader));

            try
            {
                if (reader.ReadByte() != 'i')              // remove the leading 'i'
                    throw new BEncodingException("Invalid data found. Aborting.");

                int letter;
                while ((letter = reader.PeekChar()) != -1 && letter != 'e')
                {
                    // TODO: Maybe should completely use a nuget package for BEncode.
                    //if (letter < '0' || letter > '9')
                    //throw new BEncodingException("Invalid number found.");
                    Number = Number * 10 + (letter - '0');
                    reader.ReadChar();
                }
                if (reader.ReadByte() != 'e')        //remove the trailing 'e'
                    throw new BEncodingException("Invalid data found. Aborting.");
            }
            catch (BEncodingException ex)
            {
                throw new BEncodingException("Couldn't decode number", ex);
            }
            catch
            {
                throw new BEncodingException("Couldn't decode number");
            }
        }
        #endregion


        #region Helper Methods
        /// <summary>
        /// Returns the length of the encoded string in bytes
        /// </summary>
        /// <returns></returns>
        public override int LengthInBytes()
        {
            long number = Number;
            int count = 2; // account for the 'i' and 'e'

            if (number == 0)
                return count + 1;

            if (number < 0)
            {
                number = -number;
                count++;
            }
            for (long i = number; i != 0; i /= 10)
                count++;

            return count;
        }


        public int CompareTo(object other)
        {
            if (other is BEncodedNumber || other is long || other is int)
                return CompareTo((BEncodedNumber)other);

            return -1;
        }

        public int CompareTo(BEncodedNumber other)
        {
            if (other == null)
                throw new ArgumentNullException(nameof(other));

            return Number.CompareTo(other.Number);
        }


        public int CompareTo(long other)
        {
            return Number.CompareTo(other);
        }
        #endregion


        #region Overridden Methods
        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (!(obj is BEncodedNumber obj2))
                return false;

            return Number == obj2.Number;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [SuppressMessage("ReSharper", "NonReadonlyMemberInGetHashCode")]
        public override int GetHashCode()
        {
            return Number.GetHashCode();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Number.ToString();
        }
        #endregion
    }
}
