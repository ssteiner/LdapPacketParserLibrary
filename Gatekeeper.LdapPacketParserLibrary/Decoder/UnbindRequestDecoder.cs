using Gatekeeper.LdapPacketParserLibrary.Models.Operations.Request;
using System.Formats.Asn1;

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