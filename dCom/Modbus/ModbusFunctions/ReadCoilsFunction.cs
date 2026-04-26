using Common;
using Modbus.FunctionParameters;
using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;

namespace Modbus.ModbusFunctions
{
    /// <summary>
    /// Class containing logic for parsing and packing modbus read coil functions/requests.
    /// </summary>
    public class ReadCoilsFunction : ModbusFunction
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReadCoilsFunction"/> class.
        /// </summary>
        /// <param name="commandParameters">The modbus command parameters.</param>
		public ReadCoilsFunction(ModbusCommandParameters commandParameters) : base(commandParameters)
        {
            CheckArguments(MethodBase.GetCurrentMethod(), typeof(ModbusReadCommandParameters));
        }

        /// <inheritdoc/>
        public override byte[] PackRequest()
        {
            //TO DO: IMPLEMENT
            byte[] request = new byte[12];

            ModbusReadCommandParameters ModBusRead = this.CommandParameters as ModbusReadCommandParameters;

            Buffer.BlockCopy(BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)ModBusRead.TransactionId)), 0, request, 0, 2);
            Buffer.BlockCopy(BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)ModBusRead.ProtocolId)), 0, request, 2, 2);
            Buffer.BlockCopy(BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)ModBusRead.Length)), 0, request, 4, 2);

            request[6] = ModBusRead.UnitId;
            request[7] = ModBusRead.FunctionCode;

            Buffer.BlockCopy(BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)ModBusRead.StartAddress)), 0, request, 8, 2);
            Buffer.BlockCopy(BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)ModBusRead.Quantity)), 0, request, 10, 2);

            return request;
        }

        /// <inheritdoc />
        public override Dictionary<Tuple<PointType, ushort>, ushort> ParseResponse(byte[] response)
        {
            //TO DO: IMPLEMENT
            ModbusReadCommandParameters ModBusRead = this.CommandParameters as ModbusReadCommandParameters;

            Dictionary<Tuple<PointType, ushort>, ushort> dict = new Dictionary<Tuple<PointType, ushort>, ushort>();

            int count = 0;
            ushort address = ModBusRead.StartAddress;
            ushort value;
            byte mask = 1;

            for (int i = 0; i < response[8]; i++)
            {
                byte temp = response[9 + i];

                for (int j = 0; j < 8; j++)
                {
                    value = (ushort)(temp & mask);
                    temp >>= 1;
                    dict.Add(new Tuple<PointType, ushort>(PointType.DIGITAL_OUTPUT, address), value);
                    count++;
                    address++;

                    if (count == ModBusRead.Quantity)
                    {
                        break;
                    }
                }
            }
            return dict;
        }
    }
}