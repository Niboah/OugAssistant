if (!document.OugList) {
    class OugList extends HTMLElement {
        constructor(id = null, enableAddButton = true, enableFilter =true) {
            super();
            this.id = id ? id: this.id;
            this.itemid = 0;
            this.data = [];
            this.filteredData = [];

            this.enableAddButton = enableAddButton;
            this.enableFilter = enableFilter;

            this._onClickItem = (el) => alert(el.dataset.data);
            this._onClickAdd = () => { this.#addItem(`<label>${this.data.length}</label>`); };
            this._onClickRemove = () => { return true };
            this._onClickConfirm = () => { return true };

            //#region Template
            this.template = document.createElement('template');
            this.template.innerHTML = `
        <style>
            .ouglist-container_${this.id} {
                position: relative;
                height: 100%;
                padding: 0 1.5rem 0 0.5rem;;
                box-sizing: border-box;
                user-select: none;
                display: flex;
                flex-direction: column;

            }

            .ouglist-wrapper_${this.id} {
                height: 100%;
                overflow: auto;
                border: solid 1px;
                border-radius: 1rem 0rem 0rem 1rem;
            }

            .ouglist-ul_${this.id} {
                list-style: none;
                padding-left: 0;
                margin-top: 0;
            }

            .ouglist-li_${this.id} {
                position: relative;
                width: 100%;
                display: flex;
                border-bottom: solid 1px;
                overflow: hidden;
            }

            .ouglist-item_${this.id} {
                width: 100%;
                transition: transform 0.3s ease;
                z-index: 1; 
                background: var(--bs-body-bg,white); 
                position: relative;
            }


            .ouglist-item_${this.id}.swiped-left {
                transform: translateX(-6rem);
            }

            .ouglist-item_${this.id}.swiped-right {
                transform: translateX(6rem);
            }

            .ouglist-btn-remove_${this.id} {
                position: absolute;
                top: 0;
                right: 0;
                height: 100%;
                width: 6rem;
                background-image: linear-gradient(to right, rgba(255, 0, 0,0.12),
                                                            rgba(255, 0, 0,1));
                background-color: rgba(0,0,0,0);
                border: none;
                cursor: pointer;

                opacity: 0;
                pointer-events: none;
                transition: opacity 0.3s ease;
                z-index: 0; 
            }

            .ouglist-item_${this.id}.swiped-left ~ .ouglist-btn-remove_${this.id} {
                opacity: 1;
                pointer-events: auto;
            }

            .ouglist-btn-confirm_${this.id} {
                position: absolute;
                top: 0;
                left: 0;
                height: 100%;
                width: 6rem;
                background-image: linear-gradient(to right, rgba(0, 255, 0,1),
                                                            rgba(0, 255, 0,0.12));
                background-color: rgba(0,0,0,0);
                border: none;
                cursor: pointer;

                opacity: 0;
                pointer-events: none;
                transition: opacity 0.3s ease;
                z-index: 0;
            }

            .ouglist-item_${this.id}.swiped-right ~ .ouglist-btn-confirm_${this.id} {
                opacity: 1;
                pointer-events: auto;
            }

            .ouglist-btn-add_${this.id} {
                bottom: 0;
                right: 0;
                position: absolute;
                height: 3rem;
                width: 3rem;
                border-radius: 1rem;
                z-index:5;
            }

            .ouglist-filter_${this.id} {
                position: absolute;
                top: ${this.title ? '3rem' : '0rem'} ;
                right: 0;
                width: 1.5rem;
                height: 3rem;
                display: flex;
                align-items: center;
                justify-content: center;
                transition: width 0.1s ease-in-out;
                overflow: hidden;
                transform-origin: right;
                z-index: 5;
            }

            .ouglist-filter_${this.id}:hover {
                width: 3rem;
                height: 3rem;
                z-index: 10;
            }

            .ouglist-filter_${this.id}.expanded {
                width: 100%;
                border-radius: 1rem;
                display: flex;
                transition-delay: 0s;
                z-index: 10;
            }

            .ouglist-filter-icon_${this.id} {
                width: 1.5rem;
                height: 100%;
                border-radius: 0 1rem 1rem 0;
                transition: width 0.1s ease-in-out, border-radius 0.1s ease-in-out;
            }

            .ouglist-filter_${this.id}:hover .ouglist-filter-icon_${this.id} {
                width: 3rem;
                border-radius: 1rem;
            }

            .ouglist-filter_${this.id}.expanded .ouglist-filter-icon_${this.id} {
                width: 3rem;
                border-radius: 1rem 0 0 1rem;
            }

            .ouglist-filter-icon_${this.id} label{
                position: absolute;
                left:0;
                height:100%;
                align-content: center;
                top: 0;
            }

            .ouglist-filter_${this.id}:hover .ouglist-filter-icon_${this.id} label,
            .ouglist-filter_${this.id}.expanded .ouglist-filter-icon_${this.id} label{
                position:initial;
                font-size: 1.5rem;
            }

            .ouglist-input-group_${this.id} {
                transition: width 0.1s ease-in-out, max-width 0.5s  ease-in-out, height 0.1s ease 1s;
                max-width: 0;
                width:0;
                height:0;
                pointer-events: none;
                overflow: hidden;
                z-index: 1;
            }

            .ouglist-input-group_${this.id} input,
            .ouglist-input-group_${this.id} button {
                height: 100%;
                font-size: 1rem;
            }

            .ouglist-input-group_${this.id} input{
                width:100%;
            }

            .ouglist-filter_${this.id}.expanded .ouglist-input-group_${this.id} {
        
                transition-delay: 0s;
                max-width: 100%;
                pointer-events: auto;
                width: 100%;
                display: flex;
                align-items: center;
                height: 100%;
            }

            .ouglist-filter_${this.id}.expanded *:last-child{
                border-radius: 0 1rem 1rem 0;
            }

            .ouglist-backdrop_${this.id} {
                display: none;
            }

            .ouglist-filter_${this.id}.expanded .ouglist-backdrop_${this.id} {
                display: block;
                position: fixed;
                inset: 0;
                background: rgba(74, 144, 226, 0.2);
                backdrop-filter: blur(10px);
                z-index: -1;
            }

            .swipable{
                border-left: solid 0.5rem rgba(0,255,0,0.1);
                border-right: solid 0.5rem rgba(255,0,0,0.1);
            }

        </style>
        <div class="ouglist-container_${this.id}">
            ${this.title ? `<h2 class="ouglist-header_${this.id}">${this.title}</h2>` : ''}
            ${this.enableFilter ? `
            <form class="ouglist-filter_${this.id}">
                <button class="ouglist-filter-icon_${this.id}"><label>🔍</label></button>
                <div class="ouglist-input-group_${this.id}">
                    <input type="text" placeholder="Filtrar por nombre"/>
                    <button type="submit">Filtrar</button>
                </div>
                <div class="ouglist-backdrop_${this.id}" ></div>
            </form>

            `: `` }
            <div class="ouglist-wrapper_${this.id}">
                <ul class="ouglist-ul_${this.id}"></ul>
            </div>
            ${this.enableAddButton ? `<button class="ouglist-btn-add_${this.id}">+</button>` : `` } 
        </div>
        `;
            //#endregion 

            this.appendChild(this.template.content.cloneNode(true));

            //#region  Referencias
            this.header = this.querySelector(`.ouglist-header_${this.id}`);
            this.container = this.querySelector(`.ouglist-container_${this.id}`);
            this.wrapper = this.querySelector(`.ouglist-wrapper_${this.id}`);
            this.ul = this.querySelector(`.ouglist-ul_${this.id}`);
            this.addButton = this.enableAddButton ? this.querySelector(`.ouglist-btn-add_${this.id}`) : null;

            if (this.enableFilter) {
                this.filterForm = this.querySelector(`.ouglist-filter_${this.id}`);
                this.filterForm.filterInputGroup = this.filterForm.querySelector(`.ouglist-input-group_${this.id}`);
                this.filterForm.filterInput = this.filterForm.querySelector(`input`);
                this.filterForm.backdrop = this.filterForm.querySelector(`.ouglist-backdrop_${this.id}`);
            } else {
                this.filterForm = null;
            }
            
            //#endregion

            //#region Swiper
            this._enableSwiper = this.attributes.enableswiper?.value ? this.attributes.enableswiper.value !== "false" : true;
            this.swiperActiveItem = null;
            this.startX = 0;
            this.currentX = 0;
            this.swiperState = 0;
            this.swipeDistance = 3;
            this.diffX = 0; //para evitar click despues del pointermove
            //#endregion

        }

        //#region HTMLElement

        /**
         * Called each time the element is added to the document. The specification recommends that, 
         * as far as possible, developers should implement custom element setup in this callback rather than the constructor.
         */
        connectedCallback() {
            console.log(`connectedCallback ${this.id}`);
            this.#parseExistingLI();
            this.#addObserver();
            this.#render();
            this.#attachEventHandlers();
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
                this._onClickRemove = async (...args) => {
                    try {
                        const result = await fn(...args);
                        return typeof result === 'boolean' ? result : false;
                    } catch (e) {
                        console.warn(e);
                        return false;
                    }
                };
            }
        }

        get onClickConfirm() {
            return this._onClickRemove;
        }

        set onClickConfirm(fn) {
            if (typeof fn === 'function') {
                this._onClickConfirm = async (...args) => {
                    try {
                        const result = await fn(...args);
                        return typeof result === 'boolean' ? result : false;
                    } catch (e) {
                        console.warn(e);
                        return false;
                    }
                };
            }
        }

        get enableSwiper() {
            return this._enableSwiper
        }
        set enableSwiper(val) {
            if (typeof val === 'boolean') {
                this._enableSwiper = val;
                this.#render();
            } 
        }

        clean() {
            this.data = [];
            this.filteredData = []; 
            this.ul.innerHTML = "";
        }
        //#endregion

        //#region private

        #render() {
            const items = this.filteredData.length ? this.filteredData : this.data;
            this.ul.innerHTML = '';
            const fragment = document.createDocumentFragment(); // <--- buffer en memoria
            for (const [i, itemHTML] of items.entries()) {
                itemHTML.innerHTML = this.#parseItem(itemHTML.innerHTML?.trim());
                itemHTML.classList.add(`ouglist-li_${this.id}`);
                fragment.appendChild(itemHTML); // aún no se toca el DOM real
            }

            this.ul.appendChild(fragment); // solo una operación en el DOM
        }

        #attachEventHandlers() {

            this.addButton?.addEventListener('click', (e) => {
                this._onClickAdd(e);
            });

            this.filterForm?.addEventListener('click', () => {
                this.filterForm.classList.toggle('expanded');
            });

            this.filterForm?.backdrop.addEventListener('click', (e) => {
                e.stopPropagation();
                this.filterForm.classList.toggle('expanded');
            });

            this.filterForm?.filterInputGroup.addEventListener('click', (e) => {
                e.stopPropagation();
            });

            this.filterForm?.addEventListener('submit', (e) => {
                e.preventDefault();
                const query = this.filterForm.filterInput.value.toLowerCase();
                this.filteredData = this.data.filter(item => item.outerHTML.trim().toLowerCase().includes(query));
                this.#render();
            });

            this.ul.addEventListener('click', (e) => {
                const item = e.target.closest('li');
                if (item && e.target.classList.contains(`ouglist-btn-remove_${this.id}`)) {
                    if (this._onClickRemove(e, item))
                        this.#removeItem(item);
                } else if (item && e.target.classList.contains(`ouglist-btn-confirm_${this.id}`)) {
                    if (this._onClickConfirm(e, item))
                        this.#removeItem(item);
                }
                else if (item) {
                    if (this.diffX) return;
                    this._onClickItem(e, item);
                }
            });

            this.ul.addEventListener('pointerdown', this.#swipeDown());
            this.ul.addEventListener('pointermove', this.#swipeMove());
            this.ul.addEventListener('pointerup', this.#swipeUp());
        }

        #swipeDown() {
            const that = this;
            return (e) => {
                if (!that._enableSwiper) return;
                const item = e.target.closest(`li div.ouglist-item_${that.id}`);
                if (!item || !that.ul.contains(item)) return;
                if (that.swiperActiveItem && that.swiperActiveItem.parentElement.dataset.taskid != item.parentElement.dataset.taskid) {
                    that.#resetSwipe(that.swiperActiveItem);
                }
                that.swiperActiveItem = item;
                that.startX = e.clientX;
                that.currentX = e.clientX;
                that.diffX = 0;
                that.swiperState = that.swiperState == 0 ? 2 : that.swiperState;
            }
        }

        #swipeMove() {
            const that = this;
            return (e) => {
                if (!that._enableSwiper) return;
                if (that.diffX != 0 || !that.swiperActiveItem || that.swiperState == 0) return;
                that.currentX = e.clientX;
                const tempDiff = that.currentX - that.startX;
                if (Math.abs(tempDiff) > that.swipeDistance) {
                    that.diffX = tempDiff;
                    that.#handleSwipe(that.swiperActiveItem, that.diffX);
                }

            }
        }

        #swipeUp() {
            const that = this;
            return (e) => {
                if (!that._enableSwiper) return;
                that.swiperState == 2 ? 0 : that.swiperState;
            }
        }


        #parseItem(item) {
            let isItem = item.includes(`ouglist-item_${this.id}`);
            let isSwiper = this._enableSwiper;

            if (isItem) {
                if (isSwiper) {
                    if (item.includes(`swipable`)) return item;
                    else return item.replace(`ouglist-item_${this.id}`, `ouglist-item_${this.id} swipable`);
                } else {
                    return item.replace(`ouglist-item_${this.id} swipable`, `ouglist-item_${this.id}`);
                }
            }

            return `
            
            <div class="ouglist-item_${this.id} ${isSwiper ? `swipable`:``}">
                ${item}
            </div>
            <button class="ouglist-btn-confirm_${this.id}">✓</button>
            <button class="ouglist-btn-remove_${this.id}">X</button>
            `;
        }

        #addItem(item) {
            if (!item) throw new Error('item required');

            // Crear nodo real
            const li = document.createElement('li');
            li.dataset.ouglistitemid = this.itemid;
            this.itemid += 1;
            li.classList.add(`ouglist-li_${this.id}`);
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
            // Observar cambios posteriores
            this.observer = new MutationObserver((mutationsList) => {
                for (const mutation of mutationsList) {
                    if (mutation.type === 'childList') {
                        const existingLI = Array.from(this.querySelectorAll('li'));
                        mutation.target.data = existingLI;
                        mutation.target.#render();
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

        #handleSwipe(item, diffX) {
            const li = item.closest('li');
            if (diffX < -this.swipeDistance) {
                if (this.swiperState == 2) this.swiperState = -1;
                else if (this.swiperState == 1) this.swiperState = 0;
            } else if (diffX > this.swipeDistance) {
                if (this.swiperState == 2) this.swiperState = 1;
                else if (this.swiperState == -1) this.swiperState = 0;
            }

            if (this.swiperState == -1) {
                item.classList.add('swiped-left');
            } else if (this.swiperState == 1) {
                item.classList.add('swiped-right');
            } else if (this.swiperState == 0) {
                this.#resetSwipe(item);
            }

            this.swiperState == 2 ? 0 : this.swiperState;
        }

        #resetSwipe(item) {
            item.classList.remove('swiped-right');
            item.classList.remove('swiped-left');
            this.swiperActiveItem = null;
            this.swiperState = 0;
        }
        //#endregion
    }
    window.OugList = OugList;
}
if (!customElements.get('oug-list')) customElements.define('oug-list', OugList);
