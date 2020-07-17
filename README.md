[![](https://img.shields.io/badge/FoundryGet-compatible-success)](https://github.com/cswendrowski/foundryget)

![](./foundrygetlogo.png)

The unofficial package manager CLI for FoundryVTT

# The FoundryGet Module

## Module Installation

```
https://raw.githubusercontent.com/cswendrowski/foundryget/master/module.json
```

## Module API

### Install Module if Missing

Checks if module with id `name` exists. If so, checks if `version` is older or equal to the installed version. If either of these is not true, installs the module from `moduleManifest` and returns false.

```js
async installModuleIfMissing(moduleName, version, moduleManifest)
```

### Restart After Install

Returns the game to the Setup screen so that the user may relaunch the world. Required for a module installation to take place.

```js
restartAfterInstall()
```

### Require Module

Checks if module with id `name` exists. If so, checks if `version` is older or equal to the installed version. If either of these is not true, returns false and displays a UI warning.

```js
requireModule(yourPackageName, yourPackageManifest, name, version)
```

### Require System Version

Checks if `version` is older or equal to the installed system version. Return true / false based on this, and a UI warning if false.

```js
requireSystemVersion(yourPackageName, yourPackageManifest, version)
```

### FoundryGet Ready Hook

Once the API is ready, a `foundryget-ready` hook is fired

### Examples

```js
Hooks.once('foundryget-ready', async function() {

  var systemRequirement = game.foundryGet.requireSystemVersion("npc-chatter", "https://raw.githubusercontent.com/cswendrowski/FoundryVtt-Npc-Chatter/master/module.json", "2.0.0");
  var compendiumRequirement = game.foundryGet.requireModule("npc-chatter", "https://raw.githubusercontent.com/cswendrowski/FoundryVtt-Npc-Chatter/master/module.json", "13a-dark-alleys-compendium", "1.0.0");

  if (!systemRequirement || !compendiumRequirement) {
    // Cancel module initialization
    return;
  }

  game.npcChatter = new NpcChatter();
  console.log("Npc Chatter is now ready");
  
});
```


```js
Hooks.once('foundryget-ready', async function() {

  var systemRequirement = game.foundryGet.requireSystemVersion("npc-chatter", "https://raw.githubusercontent.com/cswendrowski/FoundryVtt-Npc-Chatter/master/module.json", "2.0.0");

  if (!systemRequirement) {
    // Cancel module initialization
    return;
  }

  var compendiumWasAlreadyInstalled = game.foundryGet.installModuleIfMissing("13a-dark-alleys-compendium", "1.0.0", "https://github.com/mk572/FoundryVTT-Dark-Alleys-Compendium/releases/download/latest/module.json");

  if (!compendiumWasAlreadyInstalled) {
    game.foundryGet.restartAfterInstall();
    return;
  }

  game.npcChatter = new NpcChatter();
  console.log("Npc Chatter is now ready");
  
});
```

# The FoundryGet CLI

## Warning!!

This tool directly writes (and overrites) data in your Foundry Data folder. Please make a backup of your systems, modules, and worlds before using

## Usecases

FoundryVtt's packages have grown in size and complexity. There are currently "Systems", which define a playable set of rules for a tabletop game, and "Modules", which expand on either core Foundry functionality or a System. As a server owner, managing your installation can get out of hand due to a variety of reasons outlined (and resolved) below:

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


### Dependency chains that would break Foundry
Currently, if Module C depends on Method `DoThing` in Library A version `1.0.0` and Module D depends on the renamed version of that method `ExecuteThingAction` in Library A version `2.0.0`, Foundry will install both packages without complaint, and one or the other will be broken depending on if you have Library A version `1.0.0` or `2.0.0` installed.

FoundryGet will detect such invalid dependency chains before installation, and gracefully exit without installing a setup that would leave one of the Modules in a broken state.

## CLI Installation

Download a Release manually https://github.com/cswendrowski/foundryget/releases
Or install as a Foundry package from `https://raw.githubusercontent.com/cswendrowski/foundryget/master/module.json`

Unless you want your calls to look like `C:\users\me\downloads\foundryget\FoundryGet.exe -?`, register the appropriate build such as `/modules/foundryget/windows/` as a PATH variable

### PATH variable registration links
[Windows](https://www.computerhope.com/issues/ch000549.htm)

[Linux](https://opensource.com/article/17/6/set-path-linux)

[Mac](https://stackoverflow.com/questions/7703041/editing-path-variable-on-mac)

## Usage

### General
**Help**
```
foundryget -?
```

### Install
**Help**
```
foundryget install -?
```

**[As Module] Install**
```
foundryget install https://raw.githubusercontent.com/cswendrowski/FoundryVTT-13th-Age-Expanded/master/module.json
```

**[Downloaded] Install**
```
foundryget install https://raw.githubusercontent.com/cswendrowski/FoundryVTT-13th-Age-Expanded/master/module.json -d "C:\Users\Me\AppData\Local\FoundryVTT\Data"
```

### Install
**Help**
```
foundryget update -?
```

**[As Module] Update**
```
foundryget update
```

**[Downloaded] Update**
```
foundryget update -d "C:\Users\Me\AppData\Local\FoundryVTT\Data"
```


## I'm a System / Module developer. How do I setup my Package to use this?

1) Set your manifest.json `version` field to a [Semantic Version 2.0 compatible version](https://semver.org/), such as `1.0.0`
2) If the version you want to use isn't recognized by Foundry, such as `1.0.0-beta5`, set a Foundry-compatible version such as `beta1.0.0` to the `version` field, and set the Semantic-compatible one to the *new* `semanticVersion` field.
3) Declare your Dependencies in the *new* `dependencies` field

Your final changes should look something like this:
```json
  "semanticVersion": "2.0.0",
  "version": "2.0.0",
  "dependencies": [
    {
      "name": "archmage",
      "manifest": "https://gitlab.com/asacolips-projects/foundry-mods/archmage/-/raw/1.5.0/system.json",
      "version": "1.5.0"
    }
  ],
```

See a full example here: https://github.com/cswendrowski/FoundryVTT-13th-Age-Expanded/blob/master/module.json

## I'm a Library Module / Module developer. Why should I use SemVer?

Let's say you write a cool Logging module. It is invoked by `SuperCoolLogger("this is my log")`. You release it as version `1.0.0`, and Module C and D both use it.

If you decide to rename your method to something less flashy such as `EnhancedLogger("this is my log")` and push it as version `1.0.1` or `1.1.0`, you have *broken the rules of SemVer*. These rules are important because of tools like FoundryGet!

If Module C releases a version `2.0.0` that uses the new `EnhancedLogger` and depends on version `1.1.0`, and Module D doesn't update and stays on `SuperCoolLogger`, FoundryGet (like any SemVer-enabled tool), will look at `1.1.0` and install it as the correct version, update Module C to `2.0.0`, and break Module D in the progress if it's also installed. By releasing version `1.1.0`, you promised SemVer that you didn't break an API, and so FoundryGet believed you. Foundry is now in a broken state (this is no different than what happens under the current built-in package manager)

Given the same update, except you *kept* both methods in existance, then you *have fullfilled your promise*! `1.1.0` will work for both Module C version `2.0.0` calling `EnhancedLogger` and Module D calling `SuperCoolLogger`.

If you do the rename and release it as version `2.0.0` instead, FoundryGet will detect that Module C and D have incompatible dependencies, not update Module C to version `2.0.0`, *and will gracefully exit without breaking the current Foundry install*.

SemVer is a promise, and that promise lets tools be smart about version updates in a way they couldn't be otherwise.

Read more here: https://www.jvandemo.com/a-simple-guide-to-semantic-versioning/
