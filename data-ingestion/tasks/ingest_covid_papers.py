import csv
import json
import os
from os import path

from pymosaik import Mosaik
from tqdm import tqdm

from tasks.schema_helper import create_graph_schema

from schemas.edges import *
from schemas.nodes import *


def file_len(fname):
    with open(fname, encoding="utf-8") as f:
        for i, l in enumerate(f):
            pass
    return i + 1


def papers(server_uri, token, folder):
    csvFile = path.join(folder, "metadata.csv")
    assert os.path.exists(csvFile), \
        f"""
Error --------------
Metadata file not found on the folder:{csvFile}
Check the --folder argument, it should point to the unzipped root folder of the dataset.
    """

    shaToFile = {}
    for root, dirs, files in os.walk(folder):
        files = [f for f in files if f.endswith(".json")]
        shaToFile.update(zip([f.strip(".json") for f in files], [os.path.join(root, f) for f in files]))

    with Mosaik(server_uri, token) as msk:
        # msk.debug_mode(True)

        msk.indexes.pause("Corvid19.DataIngestion")

        create_graph_schema(msk)

        with msk.open_session().auto_commit(1000) as session:

            total_lines = file_len(csvFile)
            with open(csvFile, newline='', encoding="utf-8") as csvfile:
                csvreader = csv.reader(csvfile, delimiter=',')
                csvreader_no_header = iter(csvreader)
                header = next(csvreader_no_header)
                header = {k: index for index, k in enumerate(header)}

                for row in tqdm(csvreader_no_header, total=total_lines):
                    if row[header["doi"]] is None or row[header["doi"]] == "":
                        continue
                    paper = Paper.from_csv_row(row, header)
                    paper_json = None
                    if paper.Sha() in shaToFile:
                        with open(shaToFile[paper.Sha()], "r") as f:
                            paper_json = json.load(f)
                            paper._FullText = "\n".join([f"{section_text['section']}\n{section_text['text']}" for section_text in paper_json['body_text']])

                    paperUID = session.add_or_update(paper)

                    if not (row[header['journal']] is None or row[header['journal']] == ""):
                        journalUID = session.add_or_update(Journal(row[header['journal']]))
                        session.add_unique_edge(paperUID, PublishedIn.name(), journalUID)
                        session.add_unique_edge(journalUID, HasPaper.name(), paperUID)

                    if paper_json:
                        for author in paper_json['metadata']['authors']:
                            if len(author['first'] + author['last']) < 4:
                                continue
                            authorUID = session.add_or_update(Author(First=author['first'],
                                                                     Middle=author['middle'],
                                                                     Last=author['last'],
                                                                     Suffix=author['suffix'],
                                                                     Email=author['email']))
                            session.add_unique_edge(paperUID, HasAuthor.name(), authorUID)
                            session.add_unique_edge(authorUID, AuthorOf.name(), paperUID)

                            if 'affiliation' in author \
                                    and (
                                    not (author['affiliation'].get('laboratory', None) is None
                                         or author['affiliation']['laboratory'] == '')
                                    or
                                    not (author['affiliation'].get('institution', None) is None
                                         or author['affiliation']['institution'] == '')):
                                affiliationUID = session.add_or_update(Affiliation(author['affiliation']['laboratory'], author['affiliation']['institution']))

                                if 'location' in author['affiliation'] \
                                        and ('settlement' in author['affiliation']['location']
                                             or 'country' in author['affiliation']['location']):
                                    locationUID = session.add_or_update(Location(author['affiliation']['location'].get('settlement', ""),
                                                                                 # todo ignoring 'region'
                                                                                 author['affiliation']['location'].get('country', "")))

                                    session.add_unique_edge(affiliationUID, LocatedIn.name(), locationUID)
                                    session.add_unique_edge(locationUID, HasInstitution.name(), affiliationUID)
                                    session.add_unique_edge(authorUID, BasedOn.name(), locationUID)
                                    session.add_unique_edge(locationUID, HasAuthor.name(), authorUID)

                                session.add_unique_edge(authorUID, HasAffiliation.name(), affiliationUID)
                                session.add_unique_edge(affiliationUID, AffiliatedTo.name(), authorUID)

        msk.indexes.resume("Covid19.DataIngestion")
