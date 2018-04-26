const path = require("path");
const fableUtils = require("fable-utils");

function resolve(filePath) {
    return path.resolve(__dirname, filePath)
}

var isProduction = process.argv.indexOf("-p") >= 0;
console.log("[Environment]: " + (isProduction ? "production" : "development"));

var babelOptions = fableUtils.resolveBabelOptions({
    plugins: ["transform-es2015-modules-commonjs"]
});

module.exports = {
    entry: resolve("Kaladin.fsproj"),
    outDir: resolve("../temp"),
    babel: babelOptions,
    fable: { define: isProduction ? [] : ["DEBUG"], }
};
