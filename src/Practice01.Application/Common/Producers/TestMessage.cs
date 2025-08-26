using System.Runtime.Serialization;

namespace Practice01.Application.Common.Producers;

[DataContract]
public class TestMessage
{
    [DataMember(Order = 1)]
    public string Text { get; set; }
}