using Gatekeeper.LdapPacketParserLibrary.Models.Operations.Request;
using System.Formats.Asn1;

namespace Gatekeeper.LdapPacketParserLibrary.Decoder
{
    internal class AbandonRequestDecoder: IApplicationDecoder<AbandonRequest>
    {
        public AbandonRequest TryDecode(AsnReader reader, byte[] input)
        {
            return new AbandonRequest();
        }
    }
}
