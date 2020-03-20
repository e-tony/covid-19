using System;
using Mosaik.Library;
using System.Linq;

namespace Covid.Schemas
{
    
    //Helper class to hold the names of each edge
    public static class E
    {
        public const string HasAffiliation = nameof(HasAffiliation);
        public const string AffiliatedTo   = nameof(AffiliatedTo);  
        public const string LocatedIn      = nameof(LocatedIn);     
        public const string HasInstitution = nameof(HasInstitution);
        public const string BasedOn        = nameof(BasedOn);       
        public const string HasAuthor      = nameof(HasAuthor);
        public const string AuthorOf       = nameof(AuthorOf);
        public const string PublishedIn    = nameof(PublishedIn);
        public const string HasPaper       = nameof(HasPaper);
    }

    [Node]
    public class Paper
    {
        [Key] public string Doi { get; set; }
        [Timestamp] public DateTimeOffset PublishTime { get; set; }
        [Property] public string Sha { get; set; }
        [Property] public string Source { get; set; }
        [Property] public string Title { get; set; }
        [Property] public string License { get; set; }
        [Property] public string Abstract { get; set; }
        [Property] public string FullText { get; set; }
        [Property] public string Authors { get; set; }
        [Property] public string Journal { get; set; }
        [Property] public string Pmcid { get; set; }
        [Property] public string PubmedId { get; set; }
        [Property] public string MicrosoftAcademicPaperID { get; set; }
        [Property] public string WHOCovidence { get; set; }
    }

    [Node]
    public class Author
    {
        [Key] public string FullName { get { return string.Join(" ", new[] { First }.Concat(Middle).Concat(new[] { Last })); } }
        [Property] public string First { get; set; }
        [Property] public string[] Middle { get; set; }
        [Property] public string Last { get; set; }
        [Property] public string Suffix { get; set; }
        [Property] public string Email { get; set; }
    }

    [Node]
    public class Affiliation
    {
        [Key] public string FullName { get { return $"{Laboratory}, {Institution}".Trim(new char[] { ' ', ',' }); } }
        [Property] public string Laboratory { get; set; }
        [Property] public string Institution { get; set; }
    }

    [Node]
    public class Location
    {
        [Key] public string FullName { get { return $"{Settlement}, {Country}".Trim(new char[] { ' ', ',' }); } }
        public string Settlement { get; set; }
        public string Country { get; set; }
    }


    [Node]
    public class Journal
    {
        [Key] public string Name { get; set; }
    }


}
