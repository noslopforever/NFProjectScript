class ContentObject {
    constructor(InParentContentObject) {
        // add this to parent's children
        this.parent = InParentContentObject;
        if (InParentContentObject
            && InParentContentObject.children 
            && InParentContentObject.children instanceof Array
            ) {
            InParentContentObject.children.push(this);
        }

        // dataBindings registered to this object.
        this.dataBindings = [];
        // event handler map.
        this.eventHandlers = new Map();

        // content child of this ContentObject.
        this.children = [];

        this.dataContext = null;

        this.destroying = false;
    }

    destroy() {
        this.destroying = true;

        // destroy all data-bindings
        for (let i = 0; i < this.dataBindings.length; i++) {
            let db = this.dataBindings[i];
            db.destroy();
        }
        // unref data-bindings
        this.dataBindings = [];

        // unref dataContext
        this.dataContext = null;

        // recursive children's destroy
        for (let i = 0; i < this.children.length; i++) {
            let c = this.children[i];
            c.destroy();
        }
        // unref children
        this.children = [];

        // mark this CO has already been destroyed.
        this.destroying = false;
        this.destroyed = true;

        // tell parent that I'm destoryed()
        if (this.parent && this.removeChild) {
            this.parent.removeChild(this);
        }
    }


    removeChild(child) {
        if (this.destoying) {
            // do nothing because all children should be unreferenced soon in destroy()
            return;
        }
        let index = this.children.indexOf(child);
        this.children.slice(index, 1);
    }

    get dataContext() {
        if (this._dataContext) {
            return this._dataContext;
        }
        if (this.parent) {
            return this.parent.dataContext;
        }
        return null;
    }

    set dataContext(InDC) {
        this._dataContext = InDC;
        // Update data-bindings
        for (let i = 0; i < this.dataBindings.length; i++) {
            let db = this.dataBindings[i];
            db.UpdateDataBinding();
        }

    }

    get DataContext() {
        return this.dataContext;
    }

    set DataContext(InDC) {
        this.dataContext = InDC;
    }

    createElements() {
        this._gscoElement = this.createElementsOverride();
        this._gscoElement._gscoContentObject = this;

        // create all event-listeners
        this.eventHandlers.forEach(function (v, k){
            this._AttachEventListener(this._gscoElement, k);
        }, this);

        {
            // TODO bind children to root's children ('call appendChild').
            for (let i = 0; i < this.children.length; i++) {
                const child = this.children[i];
                let childEl = child.createElements();
                this._gscoElement.appendChild(childEl);
            }
        }

        return this._gscoElement;
    }

    createElementsOverride() {
        return null;
    }

    // Mark this CO's content is dirty, which means HTML-elements generated by this CO should be updated.
    markDirty() {
        // TODO
    }

    addUIEventHandler(InEventName, InEventHandler) {
        this.eventHandlers.set(InEventName, InEventHandler);

        let el = this._gscoElement;
        if (el) {
            this._AttachEventListener(el, InEventName);
        }
    }

    _AttachEventListener(el, InEventName) {
        el.addEventListener(InEventName, function () {
            let co = this._gscoContentObject;
            let eh = co.eventHandlers.get(InEventName);

            // If click handler is valid, invoke it.
            if (co && eh) {
                if (typeof eh == "function") {
                    eh.apply(co);
                }
                else if (eh instanceof DataContextCall) {
                    eh.apply(co.dataContext);
                }
            }
        });
    }

}



//
// User-Control Library
//

/// Panel Control
class Panel extends ContentObject {

    constructor(InParent) {
        super(InParent);
    }

    createElementsOverride() {
        let el = document.createElement("div");
        el.setAttribute("class", "copanel");
        return el;
    }

}


/// Label Control
class Label extends ContentObject {
    constructor(InParent) {
        super(InParent);
        this._Text = "Default Label Text";
    }

    createElementsOverride() {
        let el = document.createElement("div");
        el.setAttribute("class", "colabel");
        {
            this.textNode = document.createTextNode(this.Text);
            el.appendChild(this.textNode);
        }
        return el;
    }

    get Text() {
        return this._Text;
    }

    set Text(v) {
        this._Text = v;
        if (this.textNode) {
            this.textNode.nodeValue = v;
        }
    }

}

/// Button Control
class Button extends ContentObject {
   constructor(InParent) {
       super(InParent);

       this._Text = "Default Button Text";
       // TODO Select from Text and children.
    }
    createElementsOverride() {
        let el = document.createElement("button");
        el.setAttribute("class", "cobutton");
        {
            this.textNode = document.createTextNode(this._Text);
            el.appendChild(this.textNode);
        }
        return el;
    }

    get Text() {
        return this._Text;
    }

    set Text(v) {
        this._Text = v;
        if (this.textNode) {
            this.textNode.nodeValue = v;
        }
    }
}


// nps Editor conception.
class Editor {
    constructor() {

        // Model of the editor
        this.Model = null;

        // UI root of the editor
        this.UIRoot = new Panel(this);

        // Editor should always be a data source.
        this.DSComp = new DataSourceComponent();

    }

    // show this editor.
    showUI(elem) {
        elem.appendChild(this.UIRoot.createElements());
    }

}

// nps app.applet conception.
class Applet {
    constructor() {
    }
    main() {
        let showRoot = $("#UIRoot")[0];
        for (const prop in this) {
            const propObj = this[prop];
            const showUIFn = propObj.showUI;
            if (typeof showUIFn == "function") {
                showUIFn.apply(propObj, [showRoot]);
            }
        }
    }

}