

class OugList extends HTMLElement {
    constructor() {
        super();

        this.data = [];
        this.filteredData = [];

        this.onClickItem = (el) => alert(el.dataset.data);

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
            //#region filter-form
            .filter-form {
                position: absolute;
                top: 0;
                right: 0;
                width: 3rem;
                height: 3rem;
                border-radius: 1rem;
                background-color: var(--bs-body-bg, #fff);
                color: var(--bs-btn-color, #fff);
                border: 1px solid var(--bs-border-color, #ced4da);
                display: flex;
                align-items: center;
                justify-content: center;
                cursor: pointer;
                font-weight: 600;
                font-size: 2rem;
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
                font-size: 2rem;
                display: flex;
                transition-delay: 0s;
                z-index:1;
            }

            .input-group {
                opacity: 0;
                visibility: hidden;
                transition:
                    max-height 0.5s ease,
                    opacity 0.5s ease,
                    visibility 0s linear 0.5s;
                max-height: 0;
                pointer-events: none;
                overflow: hidden;
                z-index: 2;
            }

            .filter-form.expanded .input-group {
                opacity: 1;
                visibility: visible;
                transition-delay: 0s;
                max-height: 100%;
                pointer-events: auto;
                width: 100%;
                display: flex;
                align-items: center;
                margin-right:1rem;
            }

             .input-group input[type="text"] {
              width: 100%;
              flex: 1; /* toma todo el ancho disponible */
              padding: 0.375rem 0.75rem;
              font-size: 1rem;
              line-height: 1.5;
              color: #212529;
              background-color: #fff;
              border: 1px solid #ced4da;
              border-radius: 0.375rem;
              transition: border-color 0.15s ease-in-out, box-shadow 0.15s ease-in-out;
            }

            .input-group input[type="text"]:focus {
              border-color: #86b7fe;
              box-shadow: 0 0 0 0.25rem rgba(13, 110, 253, 0.25);
              outline: none;
            }


            .input-group button[type="submit"] {
              padding: 0.375rem 0.75rem;
              font-size: 1rem;
              line-height: 1.5;
              color: #fff;
              background-color: #0d6efd;
              border: 1px solid #0d6efd;
              border-radius: 0.375rem;
              cursor: pointer;
              transition: background-color 0.15s ease-in-out,
                          border-color 0.15s ease-in-out,
                          box-shadow 0.15s ease-in-out;
            }

            .input-group button[type="submit"]:hover {
              background-color: #0b5ed7;
              border-color: #0a58ca;
            }

            //#region backdrop
            .filter-form.expanded .backdrop {
                display:block;
                position: fixed;
                inset: 0;
                background: transparent; 
                z-index: 1;
            }

            .filter-form .backdrop {
                 display: none;
            }
            //#endregion

            //#endregion

            .ougli {
                width: 100%;
                display: flex;
            }

            .ougli.swiped {
                transform: translateX(-4rem); /* ajusta según el tamaño del botón */
            }

            .remove-btn {
                position: absolute;
                right: 0.5rem;
                top: 50%;
                transform: translateY(-50%);
                border: none;
                width: 2rem;
                height: 2rem;
                background: red;
                color: white;
                border-radius: 50%;
                cursor: pointer;
                opacity: 0;
                transition: opacity 0.3s ease;
            }

            .ougli.swiped .remove-btn {
                opacity: 1;
                z-index: 1;
            }
          
           

        </style>
        <div class="container">
            <form class="filter-form">
            <label>🔍</label>
                <div class="input-group">
                    <input type="text" placeholder="Filtrar por nombre"/>
                    <button type="submit">Filtrar</button>
                </div>
                
                <div class="backdrop" ></div>
            </form>
            <div class="wrapper">
                <ul></ul>
            </div>
            <button class="add-btn">+</button>
        </div>
        `;
        //#endregion 

        this.appendChild(template.content.cloneNode(true));

        //#region  Referencias
        this.container = this.querySelector('.container');
        this.wrapper = this.querySelector('.wrapper');
        this.ul = this.querySelector('ul');
        this.addButton = this.querySelector('.add-btn');
        this.filterForm = this.querySelector('.filter-form');
        this.filterForm.filterInputGroup = this.filterForm.querySelector('.input-group');
        this.filterForm.filterInput = this.filterForm.querySelector('input');
        this.filterForm.backdrop = this.filterForm.querySelector('.backdrop');
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
        this.addObserver();
        this.render();
    }
    /**
     * Called each time the element is removed from the document.
     */
    disconnectedCallback() {
        console.log(`disconnectedCallback ${this.id}`);
        this.removeObserver();
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
        const items = this.filteredData.length ? this.filteredData : this.data;
        this.ul.innerHTML = '';
        const fragment = document.createDocumentFragment(); // <--- buffer en memoria

        for (const itemHTML of items) {
            itemHTML.innerHTML = `
            <div>
                ${itemHTML.innerHTML.trim()}
            </div>
            <button class="remove-btn">X</button>
        `;
            itemHTML.classList.add('ougli');
            this.addSwipeSupport(itemHTML);
            fragment.appendChild(itemHTML); // aún no se toca el DOM real
        }

        this.ul.appendChild(fragment); // solo una operación en el DOM
    }

    attachEventHandlers() {
        this.addButton.addEventListener('click', () => this.addItem());

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
            this.filteredData = this.data.filter(item => item.toLowerCase().includes(query));
            this.render();
        });

        //// Delegación para quitar item
        //this.ul.addEventListener('click', (e) => {
        //    if (e.target.classList.contains('remove-btn')) {
        //        const li = e.target.closest('li');
        //        if(li) this.removeItem(li);
        //    } else if (e.target.nodeName==='LI') {
        //        //this.onClickItem(e.target);
        //    }
        //});
    }

    addItem(item = `Item ${this.data.length + 1}`) {
        if (!item) throw new Error('item required');

        // Crear nodo real
        const li = document.createElement('li');
        li.classList.add('ougli');
        li.innerHTML = `
        <div>
            <label>${item}</label>
        </div>
        <button class="remove-btn">X</button>
    `;

        this.addSwipeSupport(li);

        // Agregar al DOM al principio
        this.ul.insertBefore(li, this.ul.firstChild);

        // Guardar referencia real
        this.data.unshift(li);
    }

    removeItem(item) {
        const html = item.outerHTML.trim();
        item.remove();
        const index = this.data.indexOf(html);
        if (index !== -1) this.data.splice(index, 1);
    }

    parseExistingLI() {
        const existingLI = Array.from(this.querySelectorAll('li'));
        for (let li of existingLI)
        {
            this.data.push(li);
            li.remove();
        }
    }

    addObserver() {
        this.parseExistingLI();

        // Observar cambios posteriores
        this.observer = new MutationObserver((mutationsList) => {
            for (const mutation of mutationsList) {
                if (mutation.type === 'childList') {
                    const addedLis = Array.from(this.querySelectorAll('li'));
                    this.data = addedLis
                    this.render();
                }
            }
        });

        this.observer.observe(this, { childList: true, subtree: false });
    }

    removeObserver() {
        console.log(`disconnectedCallback ${this.id}`);
        if (this.observer) {
            this.observer.disconnect();
        }
    }

    addSwipeSupport(li) {
        let startX = 0, currentX = 0, touching = false;

        // Soporte para táctil
        li.addEventListener('touchstart', e => {
            touching = true;
            startX = e.touches[0].clientX;
        });

        li.addEventListener('touchmove', e => {
            if (!touching) return;
            currentX = e.touches[0].clientX;
        });

        li.addEventListener('touchend', () => {
            if (!touching) return;
            touching = false;
            this.handleSwipe(li, currentX - startX);
        });

        // Soporte para mouse
        li.addEventListener('mousedown', e => {
            touching = true;
            startX = e.clientX;
        });

        li.addEventListener('mousemove', e => {
            if (!touching) return;
            currentX = e.clientX;
        });

        li.addEventListener('mouseup', () => {
            if (!touching) return;
            touching = false;
            this.handleSwipe(li, currentX - startX);
        });
    }

    handleSwipe(li, diffX) {
        if (diffX < -30) {
            li.classList.add('swiped');
        } else if (diffX > 30) {
            li.classList.remove('swiped');
        }
    }


}

customElements.define('oug-list', OugList);

