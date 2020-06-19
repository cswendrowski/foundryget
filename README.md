![](./foundrygetlogo.png)

The unofficial package manager CLI for FoundryVTT

## Usecases

FoundryVtt's packages have grown in size and complexity. There are currently "Systems", which define a playable set of rules for a tabletop game, and "Modules", which expand on either core Foundry functionality or a System.


### System Dependent Modules
For Modules that extend a System, such as [13th Age Expanded](https://foundryvtt.com/packages/13th-age-expanded/) (which, obviously, extends 13th Age), the Module can currently define an undocumented `minimumSystemVersion`, but this does not install the System automatically when the Module is installed.

Given that 13th Age Expanded declares a dependency on Archmage of version `1.5.0`,

1) If the user does not currently have Archmage installed, FoundryGet will automatically install Archmage `1.5.0` and then install 13th Age Expanded
2) If the user currently has Archmage `1.5.5` installed, FoundryGet will consider that dependency fulfilled, and only install 13th Age Expanded


### "Library" Module Dependent Systems / Modules
There have been a recent new type of Module that would be best called a "Library", such as the wonderful [Settings Extender](https://gitlab.com/foundry-azzurite/settings-extender/) and [Babele](https://gitlab.com/riccisi/foundryvtt-babele) Modules.

Currently, if a System or Module relies on Settings Extender, they either have to trust it's installed or include a copy in their code.

Given that Module A depends on Settings Extender version `1.0.0`,

1) If the user does not currently have Settings Extender installed, FoundryGet will automatically install Settings Extender `1.0.0` and then install Module A
2) If the user currently has Settings Extender `1.1.3` installed, FoundryGet will consider that dependency fulfilled, and only install Module A


### Downstream dependencies
Some "Library" modules might depend on others, as is the case with Babele depending on Settings Extender

Given that Module B depends on Babele version `1.17.0`, and Babele depends on Settings Extender version `1.1.0`,

1) FoundryGet will detect the downstream dependency and fulfill Settings Extender `1.1.0` as per previous usecases
2) FoundryGet will detect the dependency and fulfill Babele `1.17.0` as per previous usecases
3) FoundryGet will install Module B


### Systems / Modules that share a dependency
The previous Usecases get more powerful when you consider Systems / Modules that rely on the same dependency

Given the previous example modules,
* Module A depends on Settings Extender version `1.0.0`
* Module B depends on Babele version `1.17.0` which in turn depends on Settings Extender version `1.1.0`

FoundryGet will detect this shared dependency, and since Settings Extender `1.1.0` fulfills the dependency of Module A on version `1.1.0`, FoundryGet will install one copy of version `1.1.0`
