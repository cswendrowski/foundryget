class FoundryGet {

    // Thanks to Atropos for inspiration: https://gitlab.com/foundrynet/foundryvtt/-/issues/1461#note_365038655

    async installModuleIfMissing(moduleName, version, moduleManifest) {
        const dependency = game.data.modules.find(x => x.id == moduleName);
        if (!dependency || isNewerVersion(version, dependency.data.version)) {
            var message = `Installing module ${moduleName} version ${version}. A restart will be required to activate.`;
            ui.notifications.info(message);
            await SetupConfiguration.installPackage({manifest: moduleManifest});
            return false;
        }
        return true;
    }

    requireModule(yourPackageName, yourPackageManifest, name, version) {
        const dependency = game.data.modules.find(x => x.id == name);
        if (!dependency || isNewerVersion(version, dependency.data.version)) {
            var message = `Module ${yourPackageName} cannot be activated because it depends on ${name} version ${version} which is not currently available. Please install it to proceed, or run "foundryget install ${yourPackageManifest}" to automatically install all dependencies`;
            ui.notifications.warn(message, {permanent: true});
            return false;
        }
        return true;
    }

    requireSystemVersion(yourPackageName, yourPackageManifest, version) {
        if (isNewerVersion(version, game.system.data.version)) {
            var message = `Module ${yourPackageName} cannot be activated because it depends on system version ${version} which is not currently available. Please install it to proceed, or run "foundryget install ${yourPackageManifest}" to automatically install all dependencies`;
            ui.notifications.warn(message, {permanent: true});
            return false;
        }
        return true;
    }

    restartAfterInstall() {
        game.shutDown();
    }
}

Hooks.once('ready', async function() {
    game.foundryGet = new FoundryGet();
    console.log("FoundryGet is now ready");
    Hooks.callAll("foundryget-ready");
});
