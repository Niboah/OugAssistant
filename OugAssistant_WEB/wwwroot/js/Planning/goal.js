/**
 * OugAssistant OugTask javascript file
 */

class GoalList {
    constructor() {
        this.init();
    }

    init() {
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
        this.childGoalList = new OugList('ChildGoalList', false, false);
        this.childGoalList.enableSwiper = false;
        this.goalTaskList = new OugList('GoalTasksList', false, false);
        this.goalTaskList.enableSwiper = false;
        this.childGoalListwrapper.appendChild(this.childGoalList);
        this.goalTaskListwrapper.appendChild(this.goalTaskList);
        //#endregion

        document.getElementById('btnSaveGoal').onclick = this.saveGoal;

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
                        that.goalTaskList.clean();
                        that.childGoalList.clean();
                        const goal = await this.readGoal(id);
                        that.inputGoalName.value = goal.name;
                        that.inputGoalDescription.value = goal.description;

                        if (goal.parentGoal?.id) {
                            const option = document.createElement("option");
                            option.value = goal.parentGoal.id;
                            option.text = goal.parentGoal.name;
                            that.inputParentGoal.appendChild(option);
                            that.inputParentGoal.value = goal.parentGoal?.id
                        }
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


    get saveGoal() {
        const that = this;
        return async () => {
            let goalId = that.selectedGoalId;

            let name = document.getElementById('inputGoalName').value;
            let description = document.getElementById('inputGoalDescription').value;
            let parentGoalId = document.getElementById('inputParentGoal').value;

            if (goalId) {
                await that.updateGoal(goalId,name, description, parentGoalId)
            } else {
               await that.createGoal(name, description, parentGoalId)
            }
            
            that.refresh();
            that.goalmodal.hide();
        }
    }

    //#endregion

    //#region api call

    //#region Goal

    createGoal(name, description, parentGoalId) {
        let body = {
            "Name": name,
            "Description": description,
            "ParentGoalId": parentGoalId
        }
        return ougFetch('/api/Goal', 'POST', body)
            .then(result => {
                alert(JSON.stringify(result));
                return result;
            });;
    }

    readGoal(id) {
        if (id) return ougFetch('/api/Goal/' + id, 'GET');
        return this.getGoals();
    }

    updateGoal(id, name, description, parentGoalId) {
        let body = {
            "Id" : id,
            "Name": name,
            "Description": description,
            "ParentGoalId": parentGoalId
        }
        return ougFetch('/api/Goal/'+id, 'PATCH', body)
            .then(result => {
                alert(JSON.stringify(result));
                return result;
            });;
    }
    

    getGoals() {
        return ougFetch('/api/Goal', 'GET');
    }

    refresh() {
        ougFetch('/Planning/GoalList', 'GET')
            .then(html => {
                document.getElementById('goal_container').innerHTML = html;
                this.init();
            });
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


    renderGoal(goal) {
        const template = document.getElementById('goal-template');
        const clone = template.content.cloneNode(true);

        const li = clone.querySelector('li');
        li.dataset.goalid = goal.id;

        clone.querySelector('.goal-name').textContent = goal.name;
        clone.querySelector('.goal-taskCount').textContent = goal.tasks?.length;
        clone.querySelector('.goal-description').textContent = goal.description;
        clone.querySelector('.goal-level').textContent = goal.level;

        return clone;
    }

    //#endregion

}

let goalList;
window.addEventListener('DOMContentLoaded', () => {
    goalList = new GoalList();
});
