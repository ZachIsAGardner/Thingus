// Clear built content

const fs = require('fs');

const root = `${__dirname}/../..`;

const paths = [
    `${root}/bin`,
    `${root}/obj`,
];

console.log("Removing directories...");
var i = 0;
paths.forEach(path => {
    if (fs.existsSync(path)) {
        fs.rmSync(path, { recursive: true }, (err) => {
            if (err) throw err;
        });
        console.log(`${path} is removed!`);
    }
    i++;
});
console.log("Directories removed successfully!");
