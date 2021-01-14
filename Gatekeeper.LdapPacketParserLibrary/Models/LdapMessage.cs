using System.Numerics;
using Gatekeeper.LdapPacketParserLibrary.Models.Operations;

namespace Gatekeeper.LdapPacketParserLibrary.Models
{
    public class LdapMessage
    {
        public readonly BigInteger MessageId;
        public readonly IProtocolOp ProtocolOp;

        public LdapMessage(
            BigInteger messageId,
            IProtocolOp protocolOp
            )
        {
            MessageId = messageId;
            ProtocolOp = protocolOp;
        }
    }
}