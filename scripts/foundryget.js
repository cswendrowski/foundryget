class FoundryGet {

    // Thanks to Atropos for inspiration: https://gitlab.com/foundrynet/foundryvtt/-/issues/1461#note_365038655

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
}

Hooks.once('ready', async function() {
    game.foundryGet = new FoundryGet();
    console.log("FoundryGet is now ready");
    Hooks.callAll("foundryget-ready");
});
