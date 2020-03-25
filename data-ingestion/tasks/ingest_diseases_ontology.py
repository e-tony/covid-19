import json
import os

from pymosaik import Mosaik
from pymosaik.mosaik_types import Language

from schemas.nodes import *
from tasks.schema_helper import create_graph_schema


def diseases(server_uri, token, file):
    assert os.path.exists(file), f"File not found: {file}"
    with Mosaik(server_uri, token) as msk:
        msk.debug_mode(True)

        msk.indexes.pause("Corvid19.DataIngestion")

        create_graph_schema(msk)

        with msk.open_session().auto_commit(1000) as session:
            with open(file, "r") as f:
                ontology_json = json.load(f)
                for node in ontology_json['graphs'][0]['nodes']:
                    if node['type'] != "CLASS":
                        continue

                    if 'lbl' not in node \
                            or 'id' not in node \
                            or "_" in node['lbl'] \
                            or "obsolete" in node['lbl'].lower():
                        continue

                    if node['lbl'] == "disease" or node['lbl'] == "syndrome":
                        continue

                    xrefs = []
                    if 'meta' in node:
                        if 'xrefs' in node['meta']:
                            xrefs = list(set([x['val'] for x in node['meta']['xrefs']]))

                    synonyms = []
                    if 'meta' in node:
                        if 'synonyms' in node['meta']:
                            synonyms = list(set([x['val'] for x in node['meta']['synonyms']]))

                    disease = Disease(Id=node['id'],
                                      Label=node['lbl'],
                                      XRefs=xrefs,
                                      Synonyms=synonyms)
                    diseaseUID = session.add_or_update(disease)

                    session.add_alias(diseaseUID, Language.English, alias=node['lbl'], ignore_case=False)

                    for synonym in synonyms:
                        session.add_alias(diseaseUID, Language.English, alias=synonym, ignore_case=False)

        msk.indexes.resume("Covid19.DataIngestion")
