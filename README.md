# Toucan

Travis: [![Build Status](https://travis-ci.org/EntropyZero/Toucan.svg?branch=master)](https://travis-ci.org/EntropyZero/Toucan)

Toucan is a .NET Core (DNX Core) library targeting ASP.NET Core MVC applications. Toucan was inspired by the Rails gems [CanCan](https://github.com/CanCanCommunity/cancancan) and [Canard](https://github.com/james2m/canard), and is intended to be a resource authorization library for MVC applications that provides for declarative resource loading and authorization without requiring developers to load and authorize resources imperatively in controller actions. All resource permissions are defined in a single location and not duplicated across controllers, views, and database queries.

## Installation

Toucan distributed as a NuGet package. If using the package console:

```powershell
Install-Package Toucan
```

Or, you can register the dependency in your project.json file:

```json
"dependencies":{
  "Toucan": "1.0.0-*"
}
```

and run

```bash
dnu restore
```

## Getting Started

After you have a referenced Toucan in your project, it is as simple as following 3 guidelines:

1. Controllers that will make use of Toucan must implement IToucanController. An abstract base ToucanController is also provided if you would prefer. This provides a property collection and generic model getter method for accessing loaded models.
1. Use the LoadAndAuthorizeAttribute to specify which models to load and authorize by model type and action name.
1. Add Toucan services and configuration during ServicesConfiguration. Configuration currently is a work in progress and the Fluent API, while functional, is clunky and repetitive.

The basic flow is:

1. Create your model, views and add a controller inheriting from ToucanController.
1. Add LoadAndAuthorize attributes to your controller to indicate which models to load. This is convention driven, and loads by the Id in the route.
1. Implement a role permission scheme during configuration. The AddToucan extension method takes a lambda which may use to call an abilities class that has a method for configuring permissions. See the sample app for a minor example.

A sample app is included in the sources that shows the basics of working with Toucan.

## Roadmap

Right now there are 3 major items that needs to be addressed as I drive the project to its 1.0 release:

* A much improved permissions configuration story
  * Improve the Fluent API, or
  * Provide for some form of DSL/file spec for loading permission definitions
* Handling for nested controller routes with multiple resources for loading
* Improved documentation and samples

### Contributing

I'm happy to have help :-). If you want to contribute code, fork the repo, make your branch and send me a pull request. If you have feature ideas, or bugs,open an issue and I'll take it up from there.
