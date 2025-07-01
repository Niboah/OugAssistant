/**
 * OugAssistant OugTask javascript file
 */

class GoalList {
    constructor(bootstrap) {
        this.goallist = document.getElementById('ouglist-goallist');
        //this.goallist.onClickItem = this.openTask;
        //this.goallist.onClickAdd = this.openTask;
        //this.goallist.onClickRemove = this.eliminateTask;
        //this.goallist.onClickConfirm = this.confirmTask;

        //#region taskform
        //this.selectedTaskId;

        //this.inputTaskName = document.getElementById('inputTaskName');
        //this.inputTaskDescription = document.getElementById('inputTaskDescription');
        //this.inputTaskPriority = document.getElementById('inputTaskPriority');
        //this.selectTaskGoal = document.getElementById('selectTaskGoal');

        //this.tasktype = document.querySelectorAll('.taskType');
        //this.taskTypeContent = document.querySelectorAll('.taskTypeContent');

        //this.inputTaskEventDateTime = document.getElementById('inputTaskEventDateTime');
        //this.inputTaskEventPlace = document.getElementById('inputTaskEventPlace');

        //this.inputTaskMissionDeadLine = document.getElementById('inputTaskMissionDeadLine');

        //document.getElementById('btnSaveTask').onclick = this.saveTask;
        //#endregion

    }

    //#region modal

    get goalmodal() {
        return window.bootstrap.Modal.getOrCreateInstance(document.getElementById('goalModal'))
    }

    //#endregion

    //#region actions


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
})();
