using System.Formats.Asn1;
using System.Numerics;
using Gatekeeper.LdapPacketParserLibrary.Models.Operations.Request;

namespace Gatekeeper.LdapPacketParserLibrary.Decoder
{
    internal class UnbindRequestDecoder : IApplicationDecoder<UnbindRequest>
    {
        public UnbindRequest TryDecode(AsnReader reader, byte[] input)
        {
            return new UnbindRequest();
        }
    }
}