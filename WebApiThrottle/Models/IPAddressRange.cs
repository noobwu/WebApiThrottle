// ***********************************************************************
// Assembly         : WebApiThrottle.StrongName
// Author           : Administrator
// Created          : 2021-06-20
//
// Last Modified By : Administrator
// Last Modified On : 2019-09-18
// ***********************************************************************
// <copyright file="IPAddressRange.cs" company="stefanprodan.com">
//     Copyright © Stefan Prodan 2016
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace WebApiThrottle
{
    /// <summary>
    /// IP v4 and v6 range helper by jsakamoto
    /// Fork from https://github.com/jsakamoto/ipaddressrange
    /// Implements the <see cref="System.Runtime.Serialization.ISerializable" />
    /// </summary>
    /// <seealso cref="System.Runtime.Serialization.ISerializable" />
    /// <example>
    /// "192.168.0.0/24"
    /// "fe80::/10"
    /// "192.168.0.0/255.255.255.0"
    /// "192.168.0.0-192.168.0.255"
    /// </example>
    [Serializable]
    public class IPAddressRange : ISerializable
    {
        /// <summary>
        /// Gets or sets the begin.
        /// </summary>
        /// <value>The begin.</value>
        public IPAddress Begin { get; set; }

        /// <summary>
        /// Gets or sets the end.
        /// </summary>
        /// <value>The end.</value>
        public IPAddress End { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="IPAddressRange"/> class.
        /// </summary>
        public IPAddressRange()
        {
            this.Begin = new IPAddress(0L);
            this.End = new IPAddress(0L);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IPAddressRange"/> class.
        /// </summary>
        /// <param name="ipRangeString">The ip range string.</param>
        /// <exception cref="FormatException">Unknown IP range string.</exception>
        public IPAddressRange(string ipRangeString)
        {
            // remove all spaces.
            ipRangeString = ipRangeString.Replace(" ", "");

            // Pattern 1. CIDR range: "192.168.0.0/24", "fe80::/10"
            var m1 = Regex.Match(ipRangeString, @"^(?<adr>[\da-f\.:]+)/(?<maskLen>\d+)$", RegexOptions.IgnoreCase);
            if (m1.Success)
            {
                var baseAdrBytes = IPAddress.Parse(m1.Groups["adr"].Value).GetAddressBytes();
                var maskBytes = Bits.GetBitMask(baseAdrBytes.Length, int.Parse(m1.Groups["maskLen"].Value));
                baseAdrBytes = Bits.And(baseAdrBytes, maskBytes);
                this.Begin = new IPAddress(baseAdrBytes);
                this.End = new IPAddress(Bits.Or(baseAdrBytes, Bits.Not(maskBytes)));
                return;
            }

            // Pattern 2. Uni address: "127.0.0.1", ":;1"
            var m2 = Regex.Match(ipRangeString, @"^(?<adr>[\da-f\.:]+)$", RegexOptions.IgnoreCase);
            if (m2.Success)
            {
                this.Begin = this.End = IPAddress.Parse(ipRangeString);
                return;
            }

            // Pattern 3. Begin end range: "169.258.0.0-169.258.0.255"
            var m3 = Regex.Match(ipRangeString, @"^(?<begin>[\da-f\.:]+)-(?<end>[\da-f\.:]+)$", RegexOptions.IgnoreCase);
            if (m3.Success)
            {
                this.Begin = IPAddress.Parse(m3.Groups["begin"].Value);
                this.End = IPAddress.Parse(m3.Groups["end"].Value);
                return;
            }

            // Pattern 4. Bit mask range: "192.168.0.0/255.255.255.0"
            var m4 = Regex.Match(ipRangeString, @"^(?<adr>[\da-f\.:]+)/(?<bitmask>[\da-f\.:]+)$", RegexOptions.IgnoreCase);
            if (m4.Success)
            {
                var baseAdrBytes = IPAddress.Parse(m4.Groups["adr"].Value).GetAddressBytes();
                var maskBytes = IPAddress.Parse(m4.Groups["bitmask"].Value).GetAddressBytes();
                baseAdrBytes = Bits.And(baseAdrBytes, maskBytes);
                this.Begin = new IPAddress(baseAdrBytes);
                this.End = new IPAddress(Bits.Or(baseAdrBytes, Bits.Not(maskBytes)));
                return;
            }

            throw new FormatException("Unknown IP range string.");
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IPAddressRange"/> class.
        /// </summary>
        /// <param name="info">The information.</param>
        /// <param name="context">The context.</param>
        protected IPAddressRange(SerializationInfo info, StreamingContext context)
        {
            var names = new List<string>();
            foreach (var item in info) names.Add(item.Name);

            Func<string, IPAddress> deserialize = (name) => names.Contains(name) ?
                IPAddress.Parse(info.GetValue(name, typeof(object)).ToString()) :
                new IPAddress(0L);

            this.Begin = deserialize("Begin");
            this.End = deserialize("End");
        }

        /// <summary>
        /// Determines whether this instance contains the object.
        /// </summary>
        /// <param name="ipaddress">The ipaddress.</param>
        /// <returns><c>true</c> if [contains] [the specified ipaddress]; otherwise, <c>false</c>.</returns>
        public bool Contains(IPAddress ipaddress)
        {
            if (ipaddress.AddressFamily != this.Begin.AddressFamily) return false;
            var adrBytes = ipaddress.GetAddressBytes();
            return Bits.GE(this.Begin.GetAddressBytes(), adrBytes) && Bits.LE(this.End.GetAddressBytes(), adrBytes);
        }

        /// <summary>
        /// 使用将目标对象序列化所需的数据填充 <see cref="T:System.Runtime.Serialization.SerializationInfo" />。
        /// </summary>
        /// <param name="info">要填充数据的 <see cref="T:System.Runtime.Serialization.SerializationInfo" />。</param>
        /// <param name="context">此序列化的目标（请参见 <see cref="T:System.Runtime.Serialization.StreamingContext" />）。</param>
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Begin", this.Begin != null ? this.Begin.ToString() : "");
            info.AddValue("End", this.End != null ? this.End.ToString() : "");
        }
    }

    /// <summary>
    /// Class Bits.
    /// </summary>
    internal static class Bits
    {
        /// <summary>
        /// Nots the specified bytes.
        /// </summary>
        /// <param name="bytes">The bytes.</param>
        /// <returns>System.Byte[].</returns>
        internal static byte[] Not(byte[] bytes)
        {
            return bytes.Select(b => (byte)~b).ToArray();
        }

        /// <summary>
        /// Ands the specified a.
        /// </summary>
        /// <param name="A">a.</param>
        /// <param name="B">The b.</param>
        /// <returns>System.Byte[].</returns>
        internal static byte[] And(byte[] A, byte[] B)
        {
            return A.Zip(B, (a, b) => (byte)(a & b)).ToArray();
        }

        /// <summary>
        /// Ors the specified a.
        /// </summary>
        /// <param name="A">a.</param>
        /// <param name="B">The b.</param>
        /// <returns>System.Byte[].</returns>
        internal static byte[] Or(byte[] A, byte[] B)
        {
            return A.Zip(B, (a, b) => (byte)(a | b)).ToArray();
        }

        /// <summary>
        /// Ges the specified a.
        /// </summary>
        /// <param name="A">a.</param>
        /// <param name="B">The b.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        internal static bool GE(byte[] A, byte[] B)
        {
            return A.Zip(B, (a, b) => a == b ? 0 : a < b ? 1 : -1)
                .SkipWhile(c => c == 0)
                .FirstOrDefault() >= 0;
        }

        /// <summary>
        /// Les the specified a.
        /// </summary>
        /// <param name="A">a.</param>
        /// <param name="B">The b.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        internal static bool LE(byte[] A, byte[] B)
        {
            return A.Zip(B, (a, b) => a == b ? 0 : a < b ? 1 : -1)
                .SkipWhile(c => c == 0)
                .FirstOrDefault() <= 0;
        }

        /// <summary>
        /// Gets the bit mask.
        /// </summary>
        /// <param name="sizeOfBuff">The size of buff.</param>
        /// <param name="bitLen">Length of the bit.</param>
        /// <returns>System.Byte[].</returns>
        internal static byte[] GetBitMask(int sizeOfBuff, int bitLen)
        {
            var maskBytes = new byte[sizeOfBuff];
            var bytesLen = bitLen / 8;
            var bitsLen = bitLen % 8;
            for (int i = 0; i < bytesLen; i++)
            {
                maskBytes[i] = 0xff;
            }
            if (bitsLen > 0) maskBytes[bytesLen] = (byte)~Enumerable.Range(1, 8 - bitsLen).Select(n => 1 << n - 1).Aggregate((a, b) => a | b);
            return maskBytes;
        }

    }
}
