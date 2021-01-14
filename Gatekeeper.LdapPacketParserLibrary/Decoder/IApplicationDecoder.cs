using System;
using System.Formats.Asn1;
using Gatekeeper.LdapPacketParserLibrary.Models.Operations;

namespace Gatekeeper.LdapPacketParserLibrary.Decoder
{
    internal interface IApplicationDecoder<T> where T : IProtocolOp
    {
        T TryDecode(AsnReader reader, byte[] input);
    }
}
