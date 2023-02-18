class ElementCodeBuilder {

    // InLines: should be array of Lines.
    constructor(InLines) {
        this.lines = InLines;
    }

    // class SectionConst
    static SectionConst = class {
        constructor(InString) {
            this.string = InString;
        }

        // toString interface
        toString(InHost) {
            return this.string;
        }
    }

    // class SectionVar
    static SectionVar = class {
        constructor(InVarName) {
            // Variable name of the section.
            this.varName = InVarName;

            // mark if the section is dirty.
            this.dirty = true;

            // Cache of the section.
            this.cache = "";
        }

        // toString interface
        toString(InHost) {
            if (this.dirty) {
                this._updateCache(InHost);
            }
            return this.cache;
        }

        _updateCache(InHost) {
            let v = InHost[this.varName];
            const tv = typeof v;

            // if pod
            if (tv == "number"
                || tv == "string"
                || tv == "boolean"
            ) {
                this.cache = v.toString();
            }
            // if CO
            else if (v instanceof ContentObject) {
                this.cache = v.GenElementCodes();
            }
            // if CO array
            else if (v instanceof Array) {
                this.cache = "";
                for (let subIdx = 0; subIdx < v.length; subIdx++) {
                    const sub = v[subIdx];
                    if (sub instanceof ContentObject) {
                        this.cache += sub.GenElementCodes();
                    }
                }
            }
            else {
                this.cache = "$ERROR_TYPE";
            }
        }
    }

    // class Line
    static Line = class {

        // InIndent: this line's indent in all strings.
        // InSections: sections which construct the line, should be array of TemplateStringSectionXXX objects
        constructor(InIndent, InSections) {
            this.indent = InIndent;
            this.sections = InSections;
        }

        // merge all sections into a string, and return it.
        toString(InHost) {
            let result = "";
            for (let i = 0; i < this.sections.length; i++) {
                result += this.sections[i].toString(InHost);
            }
            return result;
        }

        static New(InString) {
            return new ElementCodeBuilder.Line(0, ElementCodeBuilder.Line.splitSections(InString));
        }

        // split string into sections.
        static splitSections(InString) {
            // current state: 0 idle, 1 var
            let state = 0;
            let idx = 0;
            let sections = [];
            while (true) {
                if (state == 0) {
                    let nextVarStart = InString.indexOf("%{", idx);
                    if (nextVarStart != -1) {
                        // merge to %{ and set state to 1
                        let str = InString.substring(idx, nextVarStart);
                        sections.push(new ElementCodeBuilder.SectionConst(str));

                        idx = nextVarStart + 2;
                        state = 1;
                    }
                    else {
                        // merge to end and break the parser loop.
                        let str = InString.substring(idx);
                        sections.push(new ElementCodeBuilder.SectionConst(str));
                        break;
                    }
                }
                else if (state == 1) {
                    let nextVarEnd = InString.indexOf("}%", idx);
                    if (nextVarEnd != -1) {
                        // merge to }% and set state back to 0
                        let varname = InString.substring(idx, nextVarEnd);
                        sections.push(new ElementCodeBuilder.SectionVar(varname));

                        idx = nextVarEnd + 2;
                        state = 0;
                    }
                    else {
                        // error, break
                        break;
                    }
                }
            }
            return sections;
        }

    }

    // return array of strings
    toStringLines(InHost) {
        let result = [];
        for (var i = 0; i < this.lines.length; i++) {
            result.push(this.lines[i].toString(InHost));
        }
        return result;
    }


}



class ContentObject {
    constructor(InParentContentObject, InElemLines) {
        // add this to parent's children
        this.parent = InParentContentObject;
        if (InParentContentObject) {
            InParentContentObject.children.push(this);
        }

        // dataBindings registered to this object.
        this.dataBindings = [];

        // Html element code templates
        this.HtmlElementCodeLines = InElemLines;
        // code builder.
        this.codeBuilder = ContentObject._parseCodeTemplate(this.HtmlElementCodeLines);

        // content child of this ContentObject.
        this.children = [];

    }

    destroy() {
        // destroy all data-bindings
        for (let i = 0; i < this.dataBindings.length; i++) {
            let db = this.dataBindings[i];
            db.destroy();
        }

        // unref data-bindings
        this.dataBindings = [];

        // recursive children's destroy
        for (let i = 0; i < this.children.length; i++) {
            let c = this.children[i];
            c.destroy();
        }

        // unref children
        this.children = [];

        // mark this CO has already been destroyed.
        this.destroyed = true;

    }

    GenElementCodeLines(InIndent) {
        // gather all entries of this ContentObject.
        let strLns = this.codeBuilder.toStringLines(this);

        return strLns;
    }

    GenElementCodes() {
        let lns = this.GenElementCodeLines(0);
        let result = "";
        lns.forEach(ln => {
            result += (ln + "\n");
        });
        return result;
    }

    // Mark this CO's content is dirty, which means HTML-elements generated by this CO should be updated.
    markDirty() {
        // TODO
    }

    // Parse code lines into code string builder.
   static _parseCodeTemplate(InCodeLines) {
        let strBuildLns = [];
        for (let i = 0; i < InCodeLines.length; i++) {
            const ln = InCodeLines[i];
            let strBuildLn = ElementCodeBuilder.Line.New(ln);
            strBuildLns.push(strBuildLn);
        }
        return new ElementCodeBuilder(strBuildLns);
    }


}



//
// User-Control Library
//

/// Panel Control
class Panel extends ContentObject {

    constructor(InParent) {
        super(InParent, [
            "<div class=\"copanel\" id=\"%{Name}%\">",
            "    %{children}%",
            "</div>",
        ]);
    }

}


/// Label Control
class Label extends ContentObject {
    constructor(InParent) {
        super(InParent, [
                "%{Text}%",
            ]);

        this.Text = "";
    }

    get Text() {
        return this._Text;
    }

    set Text(v) {
        this._Text = v;
        this.markDirty();
    }

}

///// Button Control
//class Button extends ContentObject {
//    constructor(InParent) {
//        super(InParent, [
//                "<div class=\"cobutton\" id=\"{Name}\">",
//                "    {Text}",
//                "</div>",
//            ]);
//
//        // TODO Select from Text and children.
//    }
//
//}

