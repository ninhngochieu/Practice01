const fs = require('fs');
const path = require('path');

module.exports = async function (context) {
    const now = new Date();
    const formatted = now.toISOString().replace(/[:.]/g, '-');
    const testName = context.request.name || "Response";

    const dir = path.resolve(__dirname, "../responses");
    if (!fs.existsSync(dir)) {
        fs.mkdirSync(dir, { recursive: true });
    }

    const filename = path.join(dir, `${formatted}_${testName}.json`);

    fs.writeFileSync(filename, context.response.body);
    console.log("Saved response to", filename);
};
