const utility = require("./utility.js");

var architecture = 32;

const homePath = require('os').homedir();
const desktopPath = `${homePath}/OneDrive/Desktop`;

const contentEnginePath = `${__dirname}/../Content`;
const contentGamePath = `${__dirname}/../../Content`;
const constantsPath = `${__dirname}/../../CONSTANTS.json`;
const binPath = `${__dirname}/../../bin/Release/net8.0`;
const win64Path = `${binPath}/win-x64/publish`;
const win32Path = `${binPath}/win-x86/publish`;
const winPath = architecture == 32 ? win32Path : win64Path;
const linPath = `${binPath}/linux-x64/publish`;
const macPath = `${binPath}/osx-x64/publish`;


var progressCount = 0;
function printProgress() {
    var loading = [
        "t",
        "h",
        "i",
        "n",
        "g",
        "u",
        "s",
    ];
    var l = loading[progressCount % loading.length];
    process.stdout.write(`${l}`);
    progressCount++;
}

async function exportForWindows() {
    console.log("🪟 Building for Windows...");
    var go = false;
    var shellError = false;

    utility.shell(`dotnet publish -c Release -r ${(architecture == 32 ? "win-x86" : "win-x64")} /p:PublishReadyToRun=false /p:TieredCompilation=false --self-contained`).then(function (success) {
        shellError = !success;
        go = true;
    });

    while (true) {
        printProgress();
        await utility.sleep(100);
        if (go) break;
    }
    console.log("");

    utility.copyFolderRecursiveSync(contentEnginePath, winPath);
    utility.copyFolderRecursiveSync(contentGamePath, winPath);
    utility.copyFileSync(constantsPath, winPath);
}

async function exportForLinux() {
    console.log("🐧 Building for Linux...");
    var go = false;
    var shellError = false;

    utility.shell("dotnet publish -c Release -r linux-x64 /p:PublishReadyToRun=false /p:TieredCompilation=false --self-contained").then(function (success) {
        shellError = !success;
        go = true;
    });

    while (true) {
        printProgress();
        await utility.sleep(100);
        if (go) break;
    }
    console.log("");

    utility.copyFolderRecursiveSync(contentEnginePath, linPath);
    utility.copyFolderRecursiveSync(contentGamePath, linPath);
    utility.copyFileSync(constantsPath, linPath);
}

async function exportForMac() {
    console.log("🍎 Building for Mac...");
    var go = false;
    var shellError = false;

    utility.shell("dotnet publish -c Release -r osx-x64 /p:PublishReadyToRun=false /p:TieredCompilation=false --self-contained").then(function (success) {
        shellError = !success;
        go = true;
    });

    while (true) {
        printProgress();
        await utility.sleep(100);
        if (go) break;
    }
    console.log("");

    utility.copyFolderRecursiveSync(contentEnginePath, macPath);
    utility.copyFolderRecursiveSync(contentGamePath, macPath);
    utility.copyFileSync(constantsPath, macPath);
}

async function execute() {
    console.log("🔨 Exporting...");
    await exportForWindows();
    await exportForLinux();
    await exportForMac();
    console.log("✌️ Done!");
}

execute();