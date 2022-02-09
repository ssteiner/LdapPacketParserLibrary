using System;
using System.Collections.Generic;
using System.Formats.Asn1;
using System.Numerics;
using Gatekeeper.LdapPacketParserLibrary.Models.Operations.Request;
using static Gatekeeper.LdapPacketParserLibrary.Models.Operations.Request.SearchRequest;

namespace Gatekeeper.LdapPacketParserLibrary.Decoder
{
    internal class SearchRequestDecoder : IApplicationDecoder<SearchRequest>
    {

        private System.Text.Encoding valueDecoder = System.Text.Encoding.UTF8;


        public SearchRequest TryDecode(AsnReader reader, byte[] input)
        {
            SearchRequest searchRequest = new SearchRequest
            {
                RawPacket = input,
            };

            Asn1Tag bindRequestApplication = new Asn1Tag(TagClass.Application, 3);
            AsnReader subReader = reader.ReadSequence(bindRequestApplication);
            searchRequest.BaseObject = System.Text.Encoding.ASCII.GetString(subReader.ReadOctetString());

            searchRequest.Scope = subReader.ReadEnumeratedValue<SearchRequest.ScopeEnum>();
            searchRequest.DerefAliases = subReader.ReadEnumeratedValue<SearchRequest.DerefAliasesEnum>();

            BigInteger sizeLimit = subReader.ReadInteger();
            searchRequest.SizeLimit = (int)sizeLimit;
            BigInteger timeLimit = subReader.ReadInteger();
            searchRequest.TimeLimit = (int)timeLimit;
            searchRequest.TypesOnly = subReader.ReadBoolean();

            searchRequest.Filter = DecodeSearchFilter(subReader);
            searchRequest.AttributeSelection = DecodeAttributeList(subReader);

            return searchRequest;
        }

        private List<string> DecodeAttributeList(AsnReader reader)
        {
            var collectionReader = reader.ReadSequence();
            var attributes = new List<string>();
            ReadOnlyMemory<byte> tempItem;
            while (collectionReader.HasData)
            {
                if (collectionReader.TryReadPrimitiveOctetString(out ReadOnlyMemory<byte> contents))
                    tempItem = contents;
                else
                    tempItem = collectionReader.ReadOctetString();
                attributes.Add(System.Text.Encoding.ASCII.GetString(tempItem.ToArray()));
            }
            return attributes;
        }

        private TFilter DecodeAttributeValueAssertionFilter<TFilter>(AsnReader reader) where TFilter : AttributeValueAssertionFilter, new()
        {
            AsnReader subReader = reader.ReadSequence(new Asn1Tag(TagClass.ContextSpecific, reader.PeekTag().TagValue));
            string attributeDescription = System.Text.Encoding.ASCII.GetString(subReader.ReadOctetString());
            string assertionValue = valueDecoder.GetString(subReader.ReadOctetString());
            return new TFilter { AssertionValue = assertionValue, AttributeDesc = attributeDescription };
        }

        private List<IFilterChoice> DecodeRecursiveFilterSets(AsnReader reader)
        {
            AsnReader subReader = reader.ReadSetOf(new Asn1Tag(TagClass.ContextSpecific, reader.PeekTag().TagValue));
            List<IFilterChoice> filters = new List<IFilterChoice>();

            while (subReader.HasData)
            {
                filters.Add(DecodeSearchFilter(subReader));
            }

            return filters;
        }

        private SubstringFilter DecodeSubstringFilter(AsnReader reader)
        {
            AsnReader subReader = reader.ReadSequence(new Asn1Tag(TagClass.ContextSpecific, 4));

            string attributeDescription = System.Text.Encoding.ASCII.GetString(subReader.ReadOctetString());

            SubstringFilter filter = new SubstringFilter
            {
                AttributeDesc = attributeDescription,
            };

            AsnReader substringSequenceReader = subReader.ReadSequence();
            while (substringSequenceReader.HasData)
            {
                switch (substringSequenceReader.PeekTag().TagValue)
                {
                    case 0:
                        filter.Initial = valueDecoder.GetString(substringSequenceReader.ReadOctetString(new Asn1Tag(TagClass.ContextSpecific, 0)));
                        break;
                    case 1:
                        filter.Any.Add(valueDecoder.GetString(substringSequenceReader.ReadOctetString(new Asn1Tag(TagClass.ContextSpecific, 1))));
                        break;
                    case 2:
                        filter.Final = valueDecoder.GetString(substringSequenceReader.ReadOctetString(new Asn1Tag(TagClass.ContextSpecific, 2)));
                        break;
                }
            }

            return filter;
        }

        private IFilterChoice DecodeSearchFilter(AsnReader reader)
        {
            switch (reader.PeekTag().TagValue)
            {
                case 0:
                    return new AndFilter { Filters = DecodeRecursiveFilterSets(reader) };
                case 1:
                    return new OrFilter { Filters = DecodeRecursiveFilterSets(reader) };
                case 2:
                    return new NotFilter { Filter = DecodeSearchFilter(reader) };
                case 3:
                    return DecodeAttributeValueAssertionFilter<EqualityMatchFilter>(reader);
                case 4:
                    return DecodeSubstringFilter(reader);
                case 5:
                    return DecodeAttributeValueAssertionFilter<GreaterOrEqualFilter>(reader);
                case 6:
                    return DecodeAttributeValueAssertionFilter<LessOrEqualFilter>(reader);
                case 7:
                    return new PresentFilter { Value = System.Text.Encoding.ASCII.GetString(reader.ReadOctetString(new Asn1Tag(TagClass.ContextSpecific, reader.PeekTag().TagValue))) };
                case 8:
                    return DecodeAttributeValueAssertionFilter<ApproxMatchFilter>(reader);
                default:
                    throw new NotImplementedException("Cannot decode the tag: " + reader.PeekTag().TagValue);
            }
        }
    }
}
