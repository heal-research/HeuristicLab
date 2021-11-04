# HeuristicLab JSON Command Line Interface (HL JSON CLI)
The HeuristicLab JSON command line interface is a new type of application for HeuristicLab. The intended use is the execution of configurable tasks on a command line interface (CLI). We use JSON files to define such tasks. 

---
## Supported tags and respective `Dockerfile` links
* `latest`

---
## Quick reference
* Maintained by: [HEAL](https://heal.heuristiclab.com)
* Download and Documentation of the standalone version: [HeuristicLab](https://dev.heuristiclab.com)
* Build on top of: [mono](https://hub.docker.com/_/mono)

<!--## What is HeuristicLab?-->

---
## Usage
<b>Step - 1:</b> Prepare your [template](#create-a-template) and [configuration](#create-a-configuration) file.

<b>Step - 2:</b> Run the container with the following command: 
```
docker run --rm -it -v <path-to-template-and-config-files>:/files healresearch/heuristiclab-json-cli:latest /files/<template>.json /files/<config>.json /files/<out>.json
```
* Replace `<path-to-template-and-config-files>` with the path of the folder which contains the template (with the linked .hl-file) and configuration files.
* Replace `<template>` with your template JSON file.
* Replace `<config>` with your configuration JSON file.
* Replace `<out>` with a desired output JSON file (this file is overwritten if it already exists).

This command starts the execution of the template file with the desired configuration, and saves the generated results into the `<out>.json` file.

<b>NOTE:</b> The algorithm saves the results of finished runs continuously, this can cause the creation of output files while the algorithm is still running.

---
## <a name="create-a-template"></a>Create a template
<b>Step - 1</b> Checkout the [SVN branch](https://src.heuristiclab.com/svn/core/branches/3026_IntegrationIntoSymSpace) and build the solution files `HeuristicLab.ExtLibs.sln` and `HeuristicLab 3.3.sln`. After the build process, the binaries are located in the folder `bin`.

<b>Step - 2:</b> Start HeuristicLab with the application `Optimizer` and configure the desired problem/algorithm combination.

<b>Step - 3:</b> Start the algorithm and let it run until it generated all types of results. The results are located in the tab `Results`, typically it is sufficient to let the algorithm finish one generation/iteration.

<b>Step - 4:</b> Navigate to: _File_ -> _Export_ -> _Json-Template_ to open the JSON template export dialog.

<b>Step - 5:</b> The first tab `Parameters` lists every configurable parameter. Each selected parameter is saved in the template and is dynamically changeable with a configuration later on. All other (unselected) parameters are fixed and cannot be modified with a configuration file.

<b>Step - 6:</b> The second tab `Results` lists all possible results. Select all types of results which are required for the output.

<b>Step - 7:</b> Set a name for the template and save it to the desired destination. This generates two files (e.g. `Template.json` and `Template.hl`), both of them represent the template. It is possible to move those files into different folders, but you need to adapt the property `HLFileLocation` in the JSON file.
<!--
1. start HeuristicLab and configure your desired problem with the application `Optimizer`
2. start the algorithm and let it run until it generated all types of results (you can see the results in the tab “Results”, typically it is sufficient to let the algorithm finish one generation/iteration)
3. go to: File -> Export -> Json-Template (this opens a new dialog window)
5. select every parameter you wish to configure dynamically with the CLI (everything else is a fixed parameter and cannot be changed with a configuration file)
6. select every result type you need (tab `Results` in the `Export Json` window)
7. set a name for the template
8. click export and save it to your desired destination
9. it generates two files (for example: `Template.json` and `Template.hl`), both of them represent the template; you can move those files, but you need to adapt the property `HLFileLocation` contained in the JSON file
-->

---
## <a name="create-a-configuration"></a>Create a configuration
<b>Step - 1:</b> Create a new JSON file, e.g. `Config.json`.

<b>Step - 2:</b> The content of `Config.json` is an array of JSON objects. Copy all desired `Parameters` of the template JSON file into your configuration file and adapt the values to fit your needs. The following code shows an exemplary content of `Config.json`: 
```json 
[
  {
    "Value": 250,
    "Name": "PopulationSize",
    "Description": "The size of the population of solutions.",
    "Path": "Genetic Algorithm (GA).PopulationSize"
  },
  {
    "Value": 0.35,
    "Name": "MutationProbability",
    "Description": "The probability that the mutation operator is applied on a solution.",
    "Path": "Genetic Algorithm (GA).MutationProbability"
  }
]
```
This example shows the dynamic change of the properties `PopulationSize` to 250 and `MutationProbability` to 0.35.

<b>NOTE - A:</b> Every parameter needs to be part of the list `Parameters` of the template JSON file.

<b>NOTE - B:</b> The following properties cannot get configured in the configuration file and are ignored within the instantiation process:
* `Minimum`
* `Maximum`
* `ConcreteRestrictedItems`
* `ColumnsResizable`
* `RowsResizable`
* `Resizable`

<b>NOTE - C:</b> The property `Path` is necessary to be able to assign it with the associated object in HeuristicLab (works like an unique identifier). Everything else should be optional.

<!--
1. create an empty JSON file
2. the content of the configuration is an array of objects; in the following example, the population size of the algorithm is set to 250 and the mutation probability is set to 0.35:
```json 
[
  {
    "Value": 250,
    "Name": "PopulationSize",
    "Description": "The size of the population of solutions.",
    "Path": "Genetic Algorithm (GA).PopulationSize"
  },
  {
    "Value": 0.35,
    "Name": "MutationProbability",
    "Description": "The probability that the mutation operator is applied on a solution.",
    "Path": "Genetic Algorithm (GA).MutationProbability"
  }
]
```
3. copy all desired `Parameter` objects of the template JSON file into your configuration file (every template file consists of three parts: metadata, parameters and results)
4. adapt the values as you wish
5. every parameter object needs the property `Path` (the path describes the targeted object inside the object-graph in HeuristicLab and works like a unique identifier), everything else should be optional
-->
---
## License
HeuristicLab is licensed under the [GNU GPL v3](http://www.gnu.org/licenses/gpl-3.0.html). More information can be found [here](https://dev.heuristiclab.com/trac.fcgi/wiki/Download#License).

As with all Docker images, these likely also contain other software which may be under other licenses (such as Bash, etc from the base distribution, along with any direct or indirect dependencies of the primary software being contained).

As for any pre-built image usage, it is the image user's responsibility to ensure that any use of this image complies with any relevant licenses for all software contained within.
<!--As usually with docker containers, a container contains other software which may be under other licenses.-->