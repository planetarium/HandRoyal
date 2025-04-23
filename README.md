# HandRoyal

A Libplanet-based full chain prototype game,
serves basic rule of the rock scissors papers.

This project is forked from the [sample project] that uses [Libplanet node].

[sample project]: https://github.com/planetarium/SampleNode
[Libplanet node]: https://github.com/planetarium/Libplanet


## Build
Requires .NET SDK 8.0 to build this project.
Following command builds the project.

```
dotnet build
```

## Run

```
dotnet run --project ./src/HandRoyal.Executable/HandRoyal.Executable.project
```

Default value of the graphql endpoint is http://localhost:5259/graphql.



## HandRoyal-frontend

A frontend for this game written in typescript is also provided.
You can take a look at: [HandRoyal-frontend].

[HandRoyal-frontend]: https://github.com/planetarium/HandRoyal-frontend
