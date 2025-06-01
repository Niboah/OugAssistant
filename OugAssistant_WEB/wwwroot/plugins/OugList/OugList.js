class OugList extends HTMLElement {
    constructor() {
        super();
        this.itemid = 0;
        this.data = [];
        this.filteredData = [];

        this._onClickItem = (el) => alert(el.dataset.data);
        this._onClickAdd = () => { this.#addItem(`<label>${this.data.length}</label>`); };
        this._onClickRemove = () => { return true };

        //#region Template
        const template = document.createElement('template');
        template.innerHTML = `
        <style>
            .ouglist-container {
                position: relative;
                height: 100%;
                padding: 0 1.5rem;
                box-sizing: border-box;
                user-select: none;
            }

            .ouglist-wrapper {
                height: 100%;
                overflow: auto;
                border: solid 1px;
                border-radius: 1rem 0rem 0rem 1rem;
            }

            .ouglist-ul {
                list-style: none;
                padding-left: 0;
                margin-top: 0;
            }

            .ouglist-li {
                position: relative;
                width: 100%;
                display: flex;
                border-bottom: solid 1px;
                overflow: hidden;
            }

            .ouglist-item {
                width: 100%;
                transition: transform 0.3s ease;
                z-index: 1; 
                background: var(--bs-body-bg,white); 
                position: relative;
            }


            .ouglist-item.swiped {
                transform: translateX(-2rem);
            }

            .ouglist-btn-remove {
                position: absolute;
                top: 0;
                right: 0;
                height: 100%;
                width: 2rem;
                background: red;
                border: none;
                cursor: pointer;

                opacity: 0;
                pointer-events: none;
                transition: opacity 0.3s ease;
                z-index: 0; 
            }

            .ouglist-item.swiped ~ .ouglist-btn-remove {
                opacity: 1;
                pointer-events: auto;
            }

            .ouglist-btn-add {
                bottom: 0;
                right: 0;
                position: absolute;
                height: 3rem;
                width: 3rem;
                border-radius: 1rem;
            }

            .ouglist-filter {
                position: absolute;
                top: 0;
                right: 0;
                width: 1.5rem;
                height: 3rem;
                display: flex;
                align-items: center;
                justify-content: center;
                transition: width 0.1s ease-in-out;
                overflow: hidden;
                transform-origin: right;
                z-index: 10;
            }

            .ouglist-filter:hover {
                width: 3rem;
                height: 3rem;
                z-index: 10;
            }

            .ouglist-filter.expanded {
                width: 100%;
                border-radius: 1rem;
                display: flex;
                transition-delay: 0s;
                z-index: 10;
            }

            .ouglist-filter-icon {
                width: 1.5rem;
                height: 100%;
                border-radius: 0 1rem 1rem 0;
                transition: width 0.1s ease-in-out, border-radius 0.1s ease-in-out;
            }

            .ouglist-filter:hover .ouglist-filter-icon {
                width: 3rem;
                border-radius: 1rem;
            }

            .ouglist-filter.expanded .ouglist-filter-icon {
                width: 3rem;
                border-radius: 1rem 0 0 1rem;
            }

            .ouglist-filter-icon label{
                position: absolute;
                left:0;
                height:100%;
                align-content: center;
                top: 0;
            }

            .ouglist-filter:hover .ouglist-filter-icon label,
            .ouglist-filter.expanded .ouglist-filter-icon label{
                position:initial;
                font-size: 1.5rem;
            }

            .ouglist-input-group {
                transition: width 0.1s ease-in-out, max-width 0.5s  ease-in-out, height 0.1s ease 1s;
                max-width: 0;
                width:0;
                height:0;
                pointer-events: none;
                overflow: hidden;
                z-index: 1;
            }

            .ouglist-input-group input,
            .ouglist-input-group button {
                height: 100%;
                font-size: 1rem;
            }

            .ouglist-input-group input{
                width:100%;
            }

            .ouglist-filter.expanded .ouglist-input-group {
        
                transition-delay: 0s;
                max-width: 100%;
                pointer-events: auto;
                width: 100%;
                display: flex;
                align-items: center;
                height: 100%;
            }

            .ouglist-filter.expanded *:last-child{
                border-radius: 0 1rem 1rem 0;
            }

            .ouglist-backdrop {
                display: none;
            }

            .ouglist-filter.expanded .ouglist-backdrop {
                display: block;
                position: fixed;
                inset: 0;
                background: rgba(74, 144, 226, 0.2);
                backdrop-filter: blur(10px);
                z-index: -1;
            }

        </style>
        <div class="ouglist-container">
            <form class="ouglist-filter">
                <button class="ouglist-filter-icon"><label>🔍</label></button>
                <div class="ouglist-input-group">
                    <input type="text" placeholder="Filtrar por nombre"/>
                    <button type="submit">Filtrar</button>
                </div>
                <div class="ouglist-backdrop" ></div>
            </form>
            <div class="ouglist-wrapper">
                <ul class=ouglist-ul></ul>
            </div>
            <button class="ouglist-btn-add">+</button>
        </div>
        `;
        //#endregion 

        this.appendChild(template.content.cloneNode(true));

        //#region  Referencias
        this.container = this.querySelector('.ouglist-container');
        this.wrapper = this.querySelector('.ouglist-wrapper');
        this.ul = this.querySelector('.ouglist-ul');
        this.addButton = this.querySelector('.ouglist-btn-add');
        this.filterForm = this.querySelector('.ouglist-filter');
        this.filterForm.filterInputGroup = this.filterForm.querySelector('.ouglist-input-group');
        this.filterForm.filterInput = this.filterForm.querySelector('input');
        this.filterForm.backdrop = this.filterForm.querySelector('.ouglist-backdrop');
        //#endregion

        //#region Swiper
        this.activeItem = null;
        this.startX = 0;
        this.currentX = 0;
        this.touching = false;
        this.diffX = null;
        //#endregion
    }

    //#region HTMLElement

    /**
     * Called each time the element is added to the document. The specification recommends that, 
     * as far as possible, developers should implement custom element setup in this callback rather than the constructor.
     */
    connectedCallback() {
        console.log(`connectedCallback ${this.id}`);
        this.#attachEventHandlers();
        this.#addObserver();
        this.#render();
    }
    /**
     * Called each time the element is removed from the document.
     */
    disconnectedCallback() {
        console.log(`disconnectedCallback ${this.id}`);
        this.#removeObserver();
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

    //#region public

    get onClickItem() {
        return this._onClickItem;
    }

    set onClickItem(fn) {
        if (typeof fn === 'function') {
            this._onClickItem = fn;
        }
    }

    get onClickAdd() {
        return this._onClickAdd;
    }

    set onClickAdd(fn) {
        if (typeof fn === 'function') {
            this._onClickAdd = fn;
        }
    }

    get onClickRemove() {
        return this._onClickRemove;
    }

    set onClickRemove(fn) {
        if (typeof fn === 'function') {
            this._onClickRemove = fn;
        }
    }

    //#endregion

    //#region private

    #render() {
        const items = this.filteredData.length ? this.filteredData : this.data;
        this.ul.innerHTML = '';
        const fragment = document.createDocumentFragment(); // <--- buffer en memoria
        for (const [i, itemHTML] of items.entries()) {
            itemHTML.innerHTML = this.#parseItem(itemHTML.innerHTML.trim());
            itemHTML.classList.add('ouglist-li');
            fragment.appendChild(itemHTML); // aún no se toca el DOM real
        }

        this.ul.appendChild(fragment); // solo una operación en el DOM
    }

    #attachEventHandlers() {
        this.addButton.addEventListener('click', (e) => {
            this._onClickAdd(e);
        });

        this.filterForm.addEventListener('click', () => {
            this.filterForm.classList.toggle('expanded');
        });

        this.filterForm.backdrop.addEventListener('click', (e) => {
            e.stopPropagation();
            this.filterForm.classList.toggle('expanded');
        });

        this.filterForm.filterInputGroup.addEventListener('click', (e) => {
            e.stopPropagation();
        });

        this.filterForm.addEventListener('submit', (e) => {
            e.preventDefault();
            const query = this.filterForm.filterInput.value.toLowerCase();
            this.filteredData = this.data.filter(item => item.outerHTML.trim().toLowerCase().includes(query));
            this.#render();
        });

        this.ul.addEventListener('click', (e) => {
            const item = e.target.closest('li');
            if (item && e.target.classList.contains('ouglist-btn-remove')) {

                if (this.onClickRemove(e, item))
                    this.#removeItem(item);
            }
            else if (item) {
                if (this.diffX) return;
                this._onClickItem(e, item);
            }
        });

        this.ul.addEventListener('pointerdown', (e) => {
            const item = e.target.closest('li div.ouglist-item');
            if (!item || !this.ul.contains(item)) return;
            this.diffX = null;
            this.activeItem = item;
            this.touching = true;
            this.startX = e.clientX;
            this.currentX = e.clientX;
        });

        this.ul.addEventListener('pointermove', (e) => {
            if (!this.touching || !this.activeItem) return;
            this.currentX = e.clientX;
            this.diffX = this.currentX - this.startX;
            this.#handleSwipe(this.activeItem, this.diffX);

        });

        this.ul.addEventListener('pointerup', (e) => {
            if (!this.touching || !this.activeItem) return;
            this.touching = false;
            this.diffX = this.currentX - this.startX;
            this.#handleSwipe(this.activeItem, this.diffX);
            this.activeItem = null;
        });
    }

    #parseItem(item) {
        if (item.includes('ouglist-item')) return item;
        return `
            <div class="ouglist-item">
                ${item}
            </div>
            <button class="ouglist-btn-remove">X</button>
            `;
    }

    #addItem(item) {
        if (!item) throw new Error('item required');

        // Crear nodo real
        const li = document.createElement('li');
        li.dataset.ouglistitemid = this.itemid;
        this.itemid += 1;
        li.classList.add('ouglist-li');
        li.innerHTML = this.#parseItem(item);

        this.ul.insertBefore(li, this.ul.firstChild);
        this.data.push(li);
    }

    #removeItem(item) {
        const id = item.dataset.ouglistitemid;
        item.remove();
        this.data = this.data.filter(x => {
            return x.dataset?.ouglistitemid !== id;
        });
    }

    #parseExistingLI() {
        const existingLI = Array.from(this.querySelectorAll('li'));
        for (let li of existingLI) {
            li.dataset.ouglistitemid = this.itemid;
            this.itemid += 1;
            this.data.push(li);
            li.remove();
        }
    }

    #addObserver() {
        this.#parseExistingLI();

        // Observar cambios posteriores
        this.observer = new MutationObserver((mutationsList) => {
            for (const mutation of mutationsList) {
                if (mutation.type === 'childList') {
                    const addedLis = Array.from(this.querySelectorAll('li'));
                    this.data = addedLis
                    this.#render();
                }
            }
        });

        this.observer.observe(this, { childList: true, subtree: false });
    }

    #removeObserver() {
        console.log(`disconnectedCallback ${this.id}`);
        if (this.observer) {
            this.observer.disconnect();
        }
    }

    #handleSwipe(itemDiv, diffX) {
        if (diffX < -30) {
            itemDiv.classList.add('swiped');
        } else if (diffX > 30) {
            itemDiv.classList.remove('swiped');
        }
    }

    //#endregion
}

customElements.define('oug-list', OugList);

