##Toucan
[![Build Status](https://travis-ci.org/EntropyZero/Toucan.svg?branch=master)](https://travis-ci.org/EntropyZero/Toucan)
Toucan is a .NET Core (DNX Core) library for ASP.NET 5 and MVC 6. Toucan was inspired by the Rails gems [CanCan](https://github.com/CanCanCommunity/cancancan) and [Canard](https://github.com/james2m/canard), and is intended to provide a resource authorization library for MVC applications that provides for declarative resource loading and authorization without requiring developers to load and authorize resources imperatively in controller actions. All resource permissions are defined in a single location and not duplicated across controllers, views, and database queries.

### Not stable
The branching strategy is in flux as this is a new project and my development effort is driving toward an initial releasable NuGet package for consumption against the RC1 DNX Core 5 runtimes. 

As work settles down the vision is that Master will be stable with the tagged milestone releases, while the Dev branch will be the unstable mainline for forward development.

### Getting Started

Until there is a build system producing NuGet packages as an artifact, the best way to get started is to pull a copy of the repo, and use the DNX tools to create a local NuGet Package that may be referenced by your project.

For more information on the DNX CLI tools, see the Working with DNX Projects documentation at <https://docs.asp.net/en/latest/dnx/projects.html>

After you have a referenced dependency, it is as simple as following 3 steps:

1. Controllers that will make use of Toucan must implement IToucanController. An abstract base ToucanController is also provided if you would prefer.

2. Use LoadAndAuthorizeAttribute to specify which models to load and authorize by model type and action name.

3. Add Toucan services and configuration during ServicesConfiguration.

A sample app is included in the sources that shows the basics of working with Toucan.

### Roadmap

Right now there are 5 major items that needs to be addressed as I drive the project to its first release:

* Working CI for the project with appropriate artifacts
* A much improved permissions configuration story
  * Improve the Fluent API, or
  * Provide for some form of DSL/file spec for loading permission definitions
* Support for more than the DNX Core 5 runtime
* Handling for nested routes with multiple resources
* Improved documentation and samples

### Contributing

I'm happy to have help :-). If you want to contribute code, fork the repo, make your branch and send me a pull request. If you have feature ideas, or bugs,open an issue and I'll take it up from there.


  
