const path = require("path");
const fableUtils = require("fable-utils");

function resolve(filePath) {
    return path.resolve(__dirname, filePath)
}

var isProduction = process.argv.indexOf("-p") >= 0;
console.log("[Environment]: " + (isProduction ? "production" : "development"));

var port = process.argv[process.argv.indexOf("--port") + 1];

if (process.argv.indexOf("--port") == -1) {
    console.error("Missing or invalid --port argument.");
    process.exit(1);
}

var babelOptions = fableUtils.resolveBabelOptions({
    plugins: ["transform-es2015-modules-commonjs"]
});

module.exports = {
    entry: resolve("Kaladin.fsproj"),
    outDir: resolve("build"),
    babel: babelOptions,
    fable: { define: isProduction ? [] : ["DEBUG"], }
};
