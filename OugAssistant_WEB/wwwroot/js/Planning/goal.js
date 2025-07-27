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

        //region goalform
        this.selectedGoalId;

        this.inputGoalName = document.getElementById('inputGoalName');
        this.inputGoalDescription = document.getElementById('inputGoalDescription');
        this.inputParentGoal = document.getElementById('inputParentGoal');

        this.childGoalListwrapper = document.getElementById('ChildGoalListWrapper');
        this.goalTaskListwrapper = document.getElementById('GoalTasksListWrapper');
        this.childGoalList = new OugList('ChildGoalList');
        this.childGoalList.enableSwiper = false;
        this.goalTaskList = new OugList('GoalTasksList');
        this.goalTaskList.enableSwiper = false;
        this.childGoalListwrapper.appendChild(this.childGoalList);
        this.goalTaskListwrapper.appendChild(this.goalTaskList);
        //#endregion
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
                if (e.target.classList.contains('ouglist-btn-add_ouglist-goallist')) {

                    that.inputGoalName.value = "";
                    that.inputGoalDescription.value = "";
                    that.inputParentGoal.value = "";

                    const goals = await this.getGoals();
                    that.inputParentGoal.disabled = false;
                    for (let i = 0; i < goals.length; i++) {
                        const goal = goals[i];
                        const option = document.createElement("option");
                        option.value = goal.id;
                        option.text = goal.name;
                        that.inputParentGoal.appendChild(option);
                    }

                    that.goalTaskList.clean();
                    that.childGoalList.clean();

                } else if (item) {
                    id = item.dataset.goalid;
                    if (id) {
                        
                        const goal = await this.readGoal(id);
                        that.inputGoalName.value = goal.name;
                        that.inputGoalDescription.value = goal.description;
                        that.inputParentGoal.value = goal.inputParentGoal ? goal.inputParentGoal : '-';
                        that.inputParentGoal.disabled = true;
                        for (let i = 0; i < goal.tasks.length; i++) {
                            const tempHTML = this.renderTask(goal.tasks[i]);
                            that.goalTaskList.appendChild(tempHTML);
                        }

                        for (let i = 0; i < goal.childGoals.length; i++) {
                            const tempHTML = this.renderGoal(goal.childGoals[i]);
                            that.childGoalList.appendChild(tempHTML);
                        }

                    }
                }
                that.selectedGoalId = id;
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
        return this.getGoals();
    }

    updateGoal(id) {
        if (id) return ajaxCall('/api/Goal/' + id, 'PATCH');
    }

    getGoals() {
        return ajaxCall('/api/Goal', 'GET');
    }

    //#endregion

    //#endregion

    //#region utils
    renderTask(task) {
        const template = document.getElementById('task-template');
        const clone = template.content.cloneNode(true);

        const li = clone.querySelector('li');
        li.dataset.taskid = task.id;

        clone.querySelector('.task-name').textContent = task.name;
        clone.querySelector('.task-goal-name').textContent = task.goal.name;
        clone.querySelector('.task-description').textContent = task.description;

        // Mostrar el tipo correspondiente
        if (task.type === 'OugMission') {
            const missionEl = clone.querySelector('.mission-type');
            missionEl.hidden = false;
            missionEl.querySelector('.task-deadline').textContent = task.deadLine;
        } else if (task.type === 'OugEvent') {
            const eventEl = clone.querySelector('.event-type');
            eventEl.hidden = false;
            eventEl.querySelector('.task-place').textContent = task.place;
            eventEl.querySelector('.task-datetime').textContent = task.eventDateTime;
        } else if (task.type === 'OugRoutine') {
            const routineEl = clone.querySelector('.routine-type');
            routineEl.hidden = false;
            routineEl.querySelector('.task-week-times').textContent = task.weekTimes?.length || 0;
        }

        return clone;
    }

    //#endregion

}

let goalList;
window.addEventListener('DOMContentLoaded', () => {
    goalList = new GoalList();
});
