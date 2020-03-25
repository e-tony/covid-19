from pymosaik import Mosaik
from pymosaik.mosaik_types import SchemaCreationMode

from schemas.edges import *
from schemas.nodes import *


def create_graph_schema(msk: Mosaik):
    msk.schema.exists(HasAffiliation.schema(), SchemaCreationMode.CreateOnly)
    msk.schema.exists(AffiliatedTo.schema(), SchemaCreationMode.CreateOnly)
    msk.schema.exists(LocatedIn.schema(), SchemaCreationMode.CreateOnly)
    msk.schema.exists(HasInstitution.schema(), SchemaCreationMode.CreateOnly)
    msk.schema.exists(BasedOn.schema(), SchemaCreationMode.CreateOnly)
    msk.schema.exists(HasAuthor.schema(), SchemaCreationMode.CreateOnly)
    msk.schema.exists(AuthorOf.schema(), SchemaCreationMode.CreateOnly)
    msk.schema.exists(PublishedIn.schema(), SchemaCreationMode.CreateOnly)
    msk.schema.exists(HasPaper.schema(), SchemaCreationMode.CreateOnly)
    msk.schema.exists(MentionsDisease.schema(), SchemaCreationMode.CreateOnly)
    msk.schema.exists(DiseaseAppearsIn.schema(), SchemaCreationMode.CreateOnly)
    msk.schema.exists(MentionsGene.schema(), SchemaCreationMode.CreateOnly)
    msk.schema.exists(GeneAppearsIn.schema(), SchemaCreationMode.CreateOnly)

    msk.schema.exists(Paper.schema(), SchemaCreationMode.CreateOnly)
    msk.schema.exists(Author.schema(), SchemaCreationMode.CreateOnly)
    msk.schema.exists(Affiliation.schema(), SchemaCreationMode.CreateOnly)
    msk.schema.exists(Location.schema(), SchemaCreationMode.CreateOnly)
    msk.schema.exists(Journal.schema(), SchemaCreationMode.CreateOnly)
    msk.schema.exists(Disease.schema(), SchemaCreationMode.CreateOnly)
