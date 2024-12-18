const utility = require("./utility.js");
const fs = require('fs');
const moment = require("./moment.js");
const format = "YYYY-MM-DD.HH_mm_ss";

var architecture = 64;

var date = moment().format(format);

const settings = JSON.parse(fs.readFileSync(`${__dirname}/export_settings.json`).toString());
const exportPath = `${settings.exportPath}/${date}`;
fs.mkdirSync(exportPath, { recursive: true });
const contentEnginePath = `${__dirname}/../Content`;
const contentGamePath = `${__dirname}/../../Content`;
const constantsPath = `${__dirname}/../../CONSTANTS.json`;
const binPath = `${__dirname}/../../bin/Release/net8.0`;
const win64Path = `${binPath}/win-x64/publish`;
const win32Path = `${binPath}/win-x86/publish`;
const winPath = architecture == 32 ? win32Path : win64Path;
const linPath = `${binPath}/linux-x64/publish`;
const macPath = `${binPath}/osx-x64/publish`;

const target = "all";

var buildNumber = 0;
if (fs.existsSync(`${__dirname}/../../build.txt`)) {
    var build = fs.readFileSync(`${__dirname}/../../build.txt`).toString();
    buildNumber = build.split("\n")[0];
}
buildNumber++;
fs.writeFileSync(`${__dirname}/../../build.txt`, `${buildNumber}\n${date}\n${target}`);

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
    console.log("🪟  Building for Windows...");
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
    console.log("");

    utility.copyFolderRecursiveSync(contentEnginePath, winPath);
    utility.copyFolderRecursiveSync(contentGamePath, winPath);
    utility.copyFileSync(constantsPath, winPath);

    utility.copyFolderRecursiveSync(winPath, exportPath);
    fs.rename(`${exportPath}/publish`, `${exportPath}/win`, (err) => {
        error = err;
        if (error) console.log(error);
    });
}

async function exportForLinux() {
    console.log("🐧  Building for Linux...");
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
    console.log("");

    utility.copyFolderRecursiveSync(contentEnginePath, linPath);
    utility.copyFolderRecursiveSync(contentGamePath, linPath);
    utility.copyFileSync(constantsPath, linPath);

    utility.copyFolderRecursiveSync(linPath, exportPath);
    fs.rename(`${exportPath}/publish`, `${exportPath}/lin`, (err) => {
        error = err;
        if (error) console.log(error);
    })
}

async function exportForMac() {
    console.log("🍎  Building for Mac...");
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
    console.log("");

    utility.copyFolderRecursiveSync(contentEnginePath, macPath);
    utility.copyFolderRecursiveSync(contentGamePath, macPath);
    utility.copyFileSync(constantsPath, macPath);

    fs.mkdirSync(`${exportPath}/mac/${settings.macAppName}/Contents/Resources`, { recursive: true });

    // Copy plist
    // Thingus.app\Contents\info.plist
    utility.copyFileSync(`${__dirname}/Mac/info.plist`, `${exportPath}/mac/${settings.macAppName}/Contents/`);
    
    // Copy Content folder into Resources
    // Thingus.app\Contents\Resources\Content
    utility.copyFolderRecursiveSync(`${macPath}/Content`, `${exportPath}/mac/${settings.macAppName}/Contents/Resources`);
    // Thingus.app\Contents\Resources\icon.icns
    utility.copyFileSync(`${__dirname}/Mac/icon.icns`, `${exportPath}/mac/${settings.macAppName}/Contents/Resources`);
    
    // Copy code stuff into MacOS
    // Thingus.app\Contents\MacOS
    fs.rmSync(`${macPath}/Content`, { recursive: true });
    utility.copyFolderRecursiveSync(`${macPath}`, `${exportPath}/mac/${settings.macAppName}/Contents`);
    fs.rename(`${exportPath}/mac/${settings.macAppName}/Contents/publish`, `${exportPath}/mac/${settings.macAppName}/Contents/MacOS`, (err) => {
        error = err;
        if (error) console.log(error);
    });
}

async function execute() {
    var platforms = '';
    if (target == 'all') platforms = '🪟🐧🍎';
    if (target == 'win') platforms = '🪟';
    if (target == 'lin') platforms = '🐧';
    if (target == 'apple') platforms = '🍎';
    console.log(`🔨 Exporting for ${platforms}...`);
    if (target == "all" || target == "win") await exportForWindows();
    if (target == "all" || target == "lin") await exportForLinux();
    if (target == "all" || target == "mac") await exportForMac();
    console.log("✌️  Done!");
}

execute();