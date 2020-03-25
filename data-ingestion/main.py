import argparse

from tasks.ingest_covid_papers import papers
from tasks.ingest_diseases_ontology import diseases

if __name__ == '__main__':

    parser = argparse.ArgumentParser(prog='covid', description='Data Ingestion for Covid-19 Research Papers')
    parser.add_argument('--server_uri', '-s', metavar='http://...', type=str, required=True, help='URI for the Mosaik server API endpoint, example: http://localhost:5000/api/')
    parser.add_argument('--token', '-t', metavar='eyJhbGciOiJI...', type=str, required=True, help='Library Token, needs to be created on the Mosaik website')

    subparsers = parser.add_subparsers(dest='task_name')

    parser_papers = subparsers.add_parser('papers', help="Ingest the COVID-19 Open Research Dataset Challenge (CORD-19) dataset into a Mosaik server." +
                                                         "You can download the original dataset here: "
                                                         "https://www.kaggle.com/allen-institute-for-ai/CORD-19-research-challenge/tasks." +
                                                         "Extract it to a folder and pass the path with the --folder argument.")
    parser_papers.add_argument('--folder', '-f', metavar='c://...', type=str, required=True, help='Path to data source')

    parser_diseases = subparsers.add_parser('diseases', help="Ingest the https://github.com/DiseaseOntology/HumanDiseaseOntology/ dataset. Call with --file \\path\\to\\doid.json")
    parser_diseases.add_argument('--file', '-f', metavar='c://...', type=str, required=True, help='Path to data source')

    args = parser.parse_args()

    if args.task_name == 'papers':
        papers(args.server_uri, args.token, args.folder)
    elif args.task_name == 'diseases':
        diseases(args.server_uri, args.token, args.file)
