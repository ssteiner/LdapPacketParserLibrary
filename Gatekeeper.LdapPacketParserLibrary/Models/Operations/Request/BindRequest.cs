using System.Numerics;

namespace Gatekeeper.LdapPacketParserLibrary.Models.Operations.Request
{
    public class BindRequest : IProtocolOp
    {
        public const int Tag = 0;
        public readonly BigInteger Version;
        public readonly string Name;
        public readonly string Authentication;

        public BindRequest(
            BigInteger version,
            string name,
            string authentication)
        {
            Version = version;
            Name = name;
            Authentication = authentication;
        }
    }
}
