class ContentObject
{

    constructor(InParentContentObject, InElemLines) {

        // add this to parent's children
        this.parent = InParentContentObject;
        if (InParentContentObject) {
            InParentContentObject.children.push(this);
        }

        this.dataBindings = [];

        this.HtmlElementCodeLines = InElemLines;

        // content children of this ContentObject.
        this.children = [];

    }

    GenElementCodes() {
        // gather all entries of this ContentObject.
        let props = new Map(Object.entries(this));
        let result = "";

        // foreach code lines
        for (let i = 0; i < this.HtmlElementCodeLines.length; i++) {
            // replace {properties} and {{collection properties}} in code-lines.
            let ln = this.HtmlElementCodeLines[i];

            props.forEach(function (v, k, m) {
                // replace {properties}
                const key = `{${k}}`;
                const collKey = `{{${k}}}`;
                if (ln.indexOf(key) == -1
                 && ln.indexOf(collKey) == -1
                 ) {
                    return;
                 }

                const tv = typeof v;
                if (tv == "number"
                    || tv == "string"
                    || tv == "boolean"
                ) {
                    ln = ln.replaceAll(key, v.toString());
                }

                // replace {collection properties}
                if (v instanceof Array) {
                    let childStrings = "";
                    for (let subIdx = 0; subIdx < v.length; subIdx++) {
                        const sub = v[subIdx];
                        if (sub instanceof ContentObject) {
                            childStrings += sub.GenElementCodes() + "\n";
                        }
                    }
                    ln = ln.replaceAll(collKey, childStrings);
                }
                
            });

            result += `${ln}\n`;
        }

        return result;
    }

}