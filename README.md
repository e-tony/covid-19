# Covid-19 Dataset Search

This repository contains the source code used to create the [Covid-19 AI-powered search](https://covid.curiosity.ai/#/home). The data in the website comes from the [COVID-19 Open Research Dataset Challenge (CORD-19) Kaggle challenge](https://www.kaggle.com/allen-institute-for-ai/CORD-19-research-challenge/tasks?taskId=561). This website uses our AI-enabled graph and search technology to enable you to explore the dataset using machine-learning-based synonyms and find similar papers using word and graph embeddings.


![](https://raw.githubusercontent.com/curiosity-ai/covid-19/master/assets/similar-papers.png)


The code here is used to [ingest the data](https://github.com/curiosity-ai/covid-19/tree/master/data-ingestion) in the back-end Mosaik instance, and to build the [front-end](https://github.com/curiosity-ai/covid-19/tree/master/front-end) that renders each of the data types in the system. 

To run the data ingestion, [download the dataset](https://www.kaggle.com/allen-institute-for-ai/CORD-19-research-challenge/tasks?taskId=561), unzip it to a folder, and then run the data ingestion project with the following arguments:

```bash
git clone https://github.com/curiosity-ai/covid-19/
cd covid-19
cd data-ingestion
dotnet run --server https://covid.curiosity.ai/api/ --token {AUTH_TOKEN} --folder {PATH_TO_DATA}
```

This script will create the required nodes & edges on the graph, and create the relationships between papers, authors, institutions & locations.

To build and publish the front-end project, we recomend you to install our CLI tool:

```bash
dotnet tool install mosaik.cli --global
```

You can test the front-end locally by building the front-end project, and then running the following from the base folder of this repository:

```bash
cd covid-19
mosaik serve --server https://covid.curiosity.ai/api/ --folder .\bin\Debug\net461\bridge\
```

Note: Building the front-end project using the ```dotnet build command``` is not yet supported. You can either install the [Bridge.Net CLI](https://bridge.net/download/), or build the project on Visual Studio.


## Acknowledgements

The dataset used in the website was created by the Allen Institute for AI in partnership with the Chan Zuckerberg Initiative, Georgetown University’s Center for Security and Emerging Technology, Microsoft Research, and the National Library of Medicine - National Institutes of Health, in coordination with The White House Office of Science and Technology Policy.

The online site and this repository, are provided free of charge, without any warranty, either expressed or implied. Licenses for the underlying papers are noted in the individual references.

If you want your own account to explore the data, access to a free developer license of our system, or any other information, get in touch under hello@curiosity.ai or follow us on [twitter](https://twitter.com/curiosity_ai).

