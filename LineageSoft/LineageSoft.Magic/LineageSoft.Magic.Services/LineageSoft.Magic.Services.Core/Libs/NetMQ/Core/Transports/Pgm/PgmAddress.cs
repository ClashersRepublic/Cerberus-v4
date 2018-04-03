﻿namespace LineageSoft.Magic.Services.Core.Libs.NetMQ.Core.Transports.Pgm
{
    using System;
    using System.Net;
    using System.Net.Sockets;
    using JetBrains.Annotations;

    internal sealed class PgmAddress : Address.IZAddress
    {
        /// <exception cref="InvalidException">Unable to parse the address's port number, or the IP address could not be parsed.</exception>
        public PgmAddress([NotNull] string network)
        {
            this.Resolve(network, true);
        }

        public PgmAddress()
        {
        }

        /// <exception cref="InvalidException">Unable to parse the address's port number, or the IP address could not be parsed.</exception>
        public void Resolve(string name, bool ip4Only)
        {
            int delimiter = name.LastIndexOf(':');
            if (delimiter < 0)
            {
                throw new InvalidException($"In PgmAddress.Resolve({name},{ip4Only}), delimiter ({delimiter}) must be non-negative.");
            }

            // Separate the address/port.
            string addrStr = name.Substring(0, delimiter);
            string portStr = name.Substring(delimiter + 1);

            if (addrStr.Contains(";"))
            {
                int semiColonDelimiter = addrStr.IndexOf(";");
                string interfaceIP = addrStr.Substring(0, semiColonDelimiter);
                addrStr = addrStr.Substring(semiColonDelimiter + 1);

                this.InterfaceAddress = IPAddress.Parse(interfaceIP);
            }
            else
            {
                this.InterfaceAddress = null;
            }

            // Remove square brackets around the address, if any.
            if (addrStr.Length >= 2 && addrStr[0] == '[' && addrStr[addrStr.Length - 1] == ']')
            {
                addrStr = addrStr.Substring(1, addrStr.Length - 2);
            }

            int port;
            // Allow 0 specifically, to detect invalid port error in atoi if not
            if (portStr == "*" || portStr == "0")
            {
                // Resolve wildcard to 0 to allow auto-selection of port
                port = 0;
            }
            else
            {
                // Parse the port number (0 is not a valid port).
                port = Convert.ToInt32(portStr);

                if (port == 0)
                {
                    throw new InvalidException($"In PgmAddress.Resolve({name},{ip4Only}), portStr ({portStr}) must denote a valid nonzero integer.");
                }
            }

            if (addrStr == "*")
            {
                addrStr = "0.0.0.0";
            }

            IPAddress ipAddress;
            if (!IPAddress.TryParse(addrStr, out ipAddress))
            {
                throw new InvalidException($"In PgmAddress.Resolve({name},{ip4Only}), addrStr ({addrStr}) must be a valid IPAddress.");
            }

            this.Address = new IPEndPoint(ipAddress, port);
        }

        [CanBeNull]
        public IPAddress InterfaceAddress { get; private set; }

        public IPEndPoint Address { get; private set; }

        public override string ToString()
        {
            if (this.Address == null)
            {
                return string.Empty;
            }

            IPEndPoint endpoint = this.Address;

            return endpoint.AddressFamily == AddressFamily.InterNetworkV6
                ? this.Protocol + "://[" + endpoint.AddressFamily + "]:" + endpoint.Port
                : this.Protocol + "://" + endpoint.Address + ":" + endpoint.Port;
        }

        public string Protocol
        {
            get
            {
                return Core.Address.PgmProtocol;
            }
        }
    }
}