

class OugList extends HTMLElement {
    constructor() {
        super();
        this.attachShadow({ mode: 'open' });

        this.data = [];
        this.filteredData = [];

        this.onClickItem = null;


        //#region Template
        const template = document.createElement('template');
        template.innerHTML = `
        <style>
            .container {
                position: relative;
                height: 100%;
                padding: 1rem 1.5rem 0 1.5rem;
                box-sizing: border-box;
            }
            .wrapper {
                height: 100%;
                overflow: auto;
                border: solid 1px black;
            }
            ul {
                list-style: none;
                padding-left: 0;
                margin-top:0;
            }
            .add-btn {
                bottom: 0;
                right: 0;
                position: absolute;
                height: 3rem;
                width: 3rem;
                border-radius: 1rem;
            }
            .filter-form {
                position: absolute;
                top: 0;
                right: 0;
                width: 3rem;
                height: 3rem;
                border-radius: 0.375rem; /* border-radius btn */
                background-color: var(--bs-primary, #0d6efd);
                color: var(--bs-btn-color, #fff);
                border: 1px solid var(--bs-primary, #0d6efd);
                display: flex;
                align-items: center;
                justify-content: center;
                cursor: pointer;
                font-weight: 600;
                font-size: 1.25rem;
                line-height: 1;
                user-select: none;
                transition: width 0.5s ease, background-color 0.15s ease, border-color 0.15s ease, box-shadow 0.15s ease;
                box-shadow: 0 0.5rem 1rem rgb(
                var(--bs-primary-rgb, 13, 110, 253) / 0.5
                );
                overflow: hidden;
                transform-origin: right;
                transition: width 0.5s ease, opacity 0.5s ease, visibility 0s linear 0.5s;
                z-index:1;
            }

            .filter-form:hover {
              background-color: var(--bs-primary-dark, #0b5ed7);
              border-color: var(--bs-primary-dark, #0a58ca);
              box-shadow: 0 0.5rem 1.5rem rgb(
                var(--bs-primary-rgb, 13, 110, 253) / 0.7
              );
            }

            .filter-form.expanded {
                width: 100%;
                height: 3rem;
                background-color: var(--bs-body-bg, #fff);
                border-color: var(--bs-border-color, #ced4da);
                color: var(--bs-body-color, #212529);
                cursor: auto;
                border-radius: 1rem;
                box-shadow: none;
                font-weight: normal;
                font-size: 1rem;
                display: flex;
                transition-delay: 0s;
                z-index:10;
            }

            .input-Group {
                opacity: 0;
                visibility: hidden;
                transition:
                    max-height 0.5s ease,
                    opacity 0.5s ease,
                    visibility 0s linear 0.5s;
                /* para evitar interacción al estar escondido */
                max-height: 0;
                pointer-events: none;
                overflow: hidden;
            }
            .filter-form.expanded .input-Group {
                opacity: 1;
                visibility: visible;
                transition-delay: 0s;
                /* para interacción al estar expuesto */
                max-height: 100%;
                pointer-events: auto;
            }
            li {
                position: relative;
                display: flex;
                justify-content: space-between;
                align-items: center;
                padding: 0.75rem;
                border: solid 1px black;
                cursor: pointer;
            }
            li:hover {
                height: 4rem;
            }
            .remove-btn {
                border: none;
                width: 1.5em;
                height: 1.5em;
                background: none;
                cursor: pointer;
                font-weight: bold;
                color: red;
            }
        </style>
        <div class="container">
            <form class="filter-form">
                <label>🔍</label>
                <div class="input-Group">
                    <input type="text" placeholder="Filtrar por nombre"/>
                    <button type="submit">Filtrar</button>
                </div>
            </form>
            <div class="wrapper">
                <ul></ul>
            </div>
            <button class="add-btn">+</button>
        </div>
        `;
        //#endregion 

        this.shadowRoot.appendChild(template.content.cloneNode(true));

        //#region  Referencias
        this.container = this.shadowRoot.querySelector('.container');
        this.wrapper = this.shadowRoot.querySelector('.wrapper');
        this.ul = this.shadowRoot.querySelector('ul');
        this.addButton = this.shadowRoot.querySelector('.add-btn');
        this.filterForm = this.shadowRoot.querySelector('.filter-form');
        this.filterInputGroup = this.shadowRoot.querySelector('.input-Group');
        this.filterInput = this.filterInputGroup.querySelector('input');
        //#endregion 
    }

    //#region HTMLElement

    /**
     * Called each time the element is added to the document. The specification recommends that, 
     * as far as possible, developers should implement custom element setup in this callback rather than the constructor.
     */
    connectedCallback() {
        console.log(`connectedCallback ${this.id}`);
        this.attachEventHandlers();
        this.render();
    }
    /**
     * Called each time the element is removed from the document.
     */
    disconnectedCallback() {
        console.log(`disconnectedCallback ${this.id}`);
    }

    /**
     * When defined, this is called instead of connectedCallback() and disconnectedCallback() 
     * each time the element is moved to a different place in the DOM via Element.moveBefore(). 
     * Use this to avoid running initialization/cleanup code in the connectedCallback() and disconnectedCallback() 
     * callbacks when the element is not actually being added to or removed from the DOM.
     * See Lifecycle callbacks and state-preserving moves for more details.
     */
    connectedMoveCallback() {
        console.log(`connectedMoveCallback ${this.id}`);
    }

    /**
     * Called each time the element is moved to a new document.
     */
    adoptedCallback() {
        console.log(`adoptedCallback ${this.id}`);
    }

    /**
     * Called when attributes are changed, added, removed, or replaced. See Responding to attribute changes for more details about this callback.
     */
    attributeChangedCallback(name, oldValue, newValue) {
        console.log(`attributeChangedCallback ${this.id}`);
    }

    //#endregion

    render() {

        this.ul.innerHTML = '';

        const items = this.filteredData.length ? this.filteredData : this.data;

        for (const item of items) {
            addItem(item);
        }
    }

    attachEventHandlers() {
        this.addButton.addEventListener('click', () => this.addItem());

        this.filterForm.addEventListener('click', () => {
            this.filterForm.classList.toggle('expanded');
        });

        this.filterInputGroup.addEventListener('click', (e) => {
            e.stopPropagation();
        });

        this.filterForm.addEventListener('submit', (e) => {
            e.preventDefault();
            const query = this.filterInput.value.toLowerCase();
            this.filteredData = this.data.filter(item => item.toLowerCase().includes(query));
            this.renderList();
        });

        // Delegación para quitar item
        this.ul.addEventListener('click', (e) => {
            if (e.target.classList.contains('remove-btn')) {
                const li = e.target.closest('li');
                if(li) this.removeItem(li);
            } else if (e.target.nodeName==='LI') {
                this.onClickItem(e.target);
            }
        });
    }

    addItem(item = `Item ${this.data.length + 1}`,callback = this.onClickItem) {
        if (!item) throw new Error('item required');

        this.data.push(item);

        const li = document.createElement('li');
        li.textContent = item;

        const removeBtn = document.createElement('button');
        removeBtn.classList.add('remove-btn');
        removeBtn.textContent = 'X';

        li.appendChild(removeBtn);
        this.ul.appendChild(li);
    }

    removeItem(item) {
        const itemText = item.firstChild.textContent;
        item.remove();
        const index = this.data.indexOf(itemText);
        if (index !== -1) this.data.splice(index, 1);
    }
}

customElements.define('oug-list', OugList);

