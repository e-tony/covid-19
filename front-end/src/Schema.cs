using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Covid
{
    // This class is an auto-generated helper for all existing node & edge schema names on your graph.
    // You can get an updated version of it by downloading the template project again.

    public static class Schema
    {
        public static class N
        {
            public const string Paper = nameof(Paper);
            public const string Author = nameof(Author);
            public const string Affiliation = nameof(Affiliation);
            public const string Location = nameof(Location);
            public const string Journal = nameof(Journal);
        }

        public static class E
        {
            public const string AffiliatedTo = nameof(AffiliatedTo);
            public const string LocatedIn = nameof(LocatedIn);
            public const string HasInstitution = nameof(HasInstitution);
            public const string BasedOn = nameof(BasedOn);
            public const string HasAuthor = nameof(HasAuthor);
            public const string AuthorOf = nameof(AuthorOf);
            public const string PublishedIn = nameof(PublishedIn);
            public const string HasPaper = nameof(HasPaper);
            public const string HasAffiliation = nameof(HasAffiliation);
        }
    }
}
