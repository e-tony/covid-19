using System;
using System.Collections.Generic;
using J = Newtonsoft.Json.JsonPropertyAttribute;
using N = Newtonsoft.Json.NullValueHandling;

namespace Covid.Schemas
{
    public partial class DiseasesOntologyJson
    {
        [J("graphs")] public DoidGraph[] Graphs { get; set; }
    }

    public partial class DoidGraph
    {
        [J("nodes")] public DoidNode[] Nodes { get; set; }
        [J("equivalentNodesSets")] public object[] EquivalentNodesSets { get; set; }
        [J("logicalDefinitionAxioms")] public object[] LogicalDefinitionAxioms { get; set; }
        [J("domainRangeAxioms")] public object[] DomainRangeAxioms { get; set; }
        [J("propertyChainAxioms")] public object[] PropertyChainAxioms { get; set; }
    }

    public partial class DoidNode
    {
        [J("id")] public Uri Id { get; set; }
        [J("meta")] public DoidMeta Meta { get; set; }
        [J("type")] public string Type { get; set; }
        [J("lbl")] public string Lbl { get; set; }
    }

    public partial class DoidMeta
    {
        [J("definition", NullValueHandling = N.Ignore)] public DoidDefinition Definition { get; set; }
        [J("synonyms", NullValueHandling = N.Ignore)] public DoidSynonym[] Synonyms { get; set; }
        [J("basicPropertyValues")] public DoidBasicPropertyValue[] BasicPropertyValues { get; set; }
        [J("subsets", NullValueHandling = N.Ignore)] public Uri[] Subsets { get; set; }
        [J("xrefs", NullValueHandling = N.Ignore)] public DoidXref[] Xrefs { get; set; }
    }

    public partial class DoidBasicPropertyValue
    {
        [J("pred")] public Uri Pred { get; set; }
        [J("val")] public string Val { get; set; }
    }

    public partial class DoidDefinition
    {
        [J("val")] public string Val { get; set; }
        [J("xrefs")] public string[] Xrefs { get; set; }
    }

    public partial class DoidSynonym
    {
        [J("pred")] public string Pred { get; set; }
        [J("val")] public string Val { get; set; }
        [J("xrefs")] public string[] Xrefs { get; set; }
    }

    public partial class DoidXref
    {
        [J("val")] public string Val { get; set; }
    }
}