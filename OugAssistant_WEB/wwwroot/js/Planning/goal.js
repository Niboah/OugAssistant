/**
 * OugAssistant OugTask javascript file
 */

class GoalList {
    constructor(bootstrap) {
        this.goallist = document.getElementById('ouglist-goallist');
        this.goallist.onClickItem = this.openGoal;
        this.goallist.onClickAdd = this.openGoal;
        this.goallist.onClickRemove = this.eliminateGoal;
        this.goallist.enableSwiper = false;

    }

    //#region modal

    get goalmodal() {
        return window.bootstrap.Modal.getOrCreateInstance(document.getElementById('goalModal'))
    }

    //#endregion

    //#region actions

    get openGoal() {
        const that = this;
        return async (e, item) => {
            try {
                let id;
                if (e.target.classList.contains('ouglist-btn-add')) {
                    

                } else if (item) {
                    id = item.dataset.goalId;
                    if (id) {
                        const goal = await this.readGoal(id);

                    }
                }

                that.goalmodal.show();
            } catch (ex) {
                console.error(ex);
            }
        }
    }

    //#endregion

    //#region api call

    //#region Goal

    createGoal(name, description) {
        let body = {
            "name": name,
            "description": description
        }
        return ajaxCall('/api/Goal', 'POST', body)
            .then(result => {
                alert(JSON.stringify(result));
                return result;
            });;
    }

    readGoal(id) {
        if (id) return ajaxCall('/api/Goal/' + id, 'GET');
        return ajaxCall('/api/Goal', 'GET');
    }

    updateGoal(id) {
        if (id) return ajaxCall('/api/Goal/' + id, 'PATCH');
    }

    //#endregion

    //#endregion

}

let goalList;
window.addEventListener('DOMContentLoaded', () => {
    goalList = new GoalList();
});
