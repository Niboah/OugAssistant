/**
 * OugAssistant OugTask javascript file
 */
class TaskModal {

    constructor(modalId = "taskModal") {
        this.modalId = modalId;
        this.modal = document.getElementById(modalId);
        this.modalBoostrap = window.bootstrap.Modal.getOrCreateInstance(this.modal);

        this.cleanTemplate = this.modal.innerHTML;

        this.selectedTaskId;

        this.inputTaskName = this.modal.querySelector('#inputTaskName');
        this.inputTaskDescription = this.modal.querySelector('#inputTaskDescription');
        this.inputTaskPriority = this.modal.querySelector('#inputTaskPriority');
        this.selectParentTask = this.modal.querySelector('#selectParentTask');
        this.selectTaskGoal = this.modal.querySelector('#selectTaskGoal');

        this.tasktype = this.modal.querySelectorAll('.taskType');
        this.taskTypeContent = this.modal.querySelectorAll('.taskTypeContent');

        this.inputTaskEventDateTime = this.modal.querySelector('#inputTaskEventDateTime');
        this.inputTaskEventPlace = this.modal.querySelector('#inputTaskEventPlace');

        this.inputTaskMissionDeadLine = this.modal.querySelector('#inputTaskMissionDeadLine');

        this.modal.querySelector('#btnSaveTask').onclick = this.save;
        this.modal.querySelectorAll('.addRoutineHour').forEach(el => { el.onclick = this.addRoutineHour() });
        this.modal.querySelectorAll('.removeRoutineHour').forEach(el => { el.onclick = this.removeRoutineHour });
    }
    get show() {
        const that = this;
        return async (taskId = null) => {
            try {
                //#region Reset
                this.selectedTaskId = "";

                const DateTime = luxon.DateTime;
                let now = DateTime.now();
                now = now.set({ hour: 23, minute: 59, second: 59 });
                this.inputTaskName.value = "";
                this.inputTaskName.disabled = false;
                this.inputTaskDescription.value = "";;
                this.inputTaskPriority.value = 0;
                this.selectTaskGoal.value = "";
                this.selectTaskGoal.disabled = false;
                this.selectParentTask.value = "";
                this.selectParentTask.disabled = false;
                this.inputTaskEventDateTime.value = now.toISO().slice(0, 16);
                this.inputTaskEventPlace.value = "";
                this.inputTaskMissionDeadLine.value = now.toISO().slice(0, 16);

                this.tasktype.forEach(el => el.classList.remove('active'));
                this.taskTypeContent.forEach(el => el.classList.remove('active', 'show'));

                document.querySelectorAll('.routineDayTime').forEach(el => el.remove());

                const triggerEl = document.getElementById('nav-event-tab');
                const tab = window.bootstrap.Tab.getOrCreateInstance(triggerEl);
                tab.show();
                //#endregion 

                if (taskId) {
                    that.selectedTaskId = taskId;

                    const task = await this.readTask(taskId);

                    this.inputTaskName.value = task.name;
                    this.inputTaskName.disabled = true;
                    this.inputTaskDescription.value = task.description;
                    this.inputTaskPriority.value = task.priority;
                    this.selectTaskGoal.value = task.goal?.id;
                    this.selectTaskGoal.disabled = true;
                    this.selectParentTask.value = task.parentTask?.id;
                    this.selectParentTask.disabled = true;
                    this.tasktype.forEach(el => el.classList.remove('active'));
                    this.taskTypeContent.forEach(el => el.classList.remove('active', 'show'));


                    if (task.type == "OugEvent") {
                        this.inputTaskEventDateTime.value = task.eventDateTime;
                        this.inputTaskEventPlace.value = task.place;

                        const triggerEl = document.getElementById('nav-event-tab');
                        const tab = window.bootstrap.Tab.getOrCreateInstance(triggerEl);
                        tab.show();

                    } else if (task.type == "OugMission") {
                        this.inputTaskMissionDeadLine.value = task.deadLine;

                        const triggerEl = document.getElementById('nav-mission-tab');
                        const tab = window.bootstrap.Tab.getOrCreateInstance(triggerEl);
                        tab.show();
                    } else if (task.type == "OugRoutine") {

                        task.routines.forEach((value, i) => {
                            if (value.length > 0) {
                                document.getElementById('inputTaskRoutineDay' + i).checked = true;
                                let collapseElement = document.getElementById('routineDayTimeListContainer' + i);
                                const bsCollapse = new bootstrap.Collapse(collapseElement, {
                                    toggle: false
                                });
                                bsCollapse.toggle();
                            }
                        });

                        for (let i = 0; i < task.routines.length; i++) {
                            let firstHourEl = document.getElementById('inputTaskRoutineTimeOfDay' + i + 0);
                            firstHourEl.value = task.routines[i][0];
                            let list = document.getElementById('routineDayTimeList' + i)
                            for (let j = 1; j < task.routines[i].length; j++) {
                                that.addRoutineHour(i, task.routines[i][j])();
                            }
                        }
                        const triggerEl = document.getElementById('nav-routine-tab');
                        const tab = window.bootstrap.Tab.getOrCreateInstance(triggerEl);
                        tab.show();
                    }
                }
                this.modalBoostrap.show();

            } catch (ex) {
                alert(ex);
            }
        }
    }
    get save() {
        const that = this;
        return async () => {
            try {
                let name = that.inputTaskName.value;
                let description = that.inputTaskDescription.value;
                let priority = Number(that.inputTaskPriority.value);
                let goalId = that.selectTaskGoal.value;
                let parentTaskId = that.selectParentTask.value;

                let type = document.querySelector('.taskType.active').dataset.tasktype;

                let eventDateTime = that.inputTaskEventDateTime.value;
                let place = that.inputTaskEventPlace.value;

                let deadtime = that.inputTaskMissionDeadLine.value;

                if (that.selectedTaskId) {

                    //#region update

                    let routines = document.querySelectorAll('.taskRoutineDay');
                    routines = Array.from(routines).map(el => {
                        if (el.checked) {
                            let weekday = el.dataset.weekday;
                            let hourlist = document.getElementById('routineDayTimeList' + weekday);
                            hourlist = Array.from(hourlist.children).map(h => {
                                let hour = h.dataset.hour;
                                let input = document.getElementById("inputTaskRoutineTimeOfDay" + weekday + hour);
                                return input.value;
                            });
                            console.log(hourlist);
                            return hourlist
                        } else {
                            return [];
                        }

                    }
                    );
                    await that.updateTask(that.selectedTaskId, name, description, priority, goalId, type, parentTaskId, eventDateTime, place, deadtime, routines)

                    //#endregion

                } else {

                    //#region new
                    let routines = document.querySelectorAll('.taskRoutineDay');
                    routines = Array.from(routines).map(el => {
                        if (el.checked) {
                            let weekday = el.dataset.weekday;
                            let hourlist = document.getElementById('routineDayTimeList' + weekday);
                            hourlist = Array.from(hourlist.children).map(h => {
                                let hour = h.dataset.hour;
                                let input = document.getElementById("inputTaskRoutineTimeOfDay" + weekday + hour);
                                if (input != "") return input.value;
                            });
                            console.log(hourlist);
                            return hourlist
                        } else {
                            return [];
                        }

                    });

                    await that.createTask(name, description, priority, goalId, type, parentTaskId, eventDateTime, place, deadtime, routines)

                    //#endregion

                }
                location.reload();
                that.taskmodal.hide();
            } catch (ex) {
                alert(ex);
            }
        }

    }

    createTask(name, description, priority, goalId, type, parentTaskId, eventDateTime, place, deadtime, routines) {

        let body = {
            "Name": name,
            "Description": description,
            "Priority": priority,
            "GoalIdList": [goalId],
            "Type": type,
        }
        body.ParentTaskId = parentTaskId ? parentTaskId : null;
        body.EventDateTime = eventDateTime ? eventDateTime : null;
        body.Place = place ? place : null;
        body.DeadLine = deadtime ? deadtime : null;
        body.Routines = routines ? routines : null;

        return ougFetch('/api/Task', 'POST', body)
            .then(result => {
                alert("Done");
                return result.id;
            }).catch(ex => alert(ex));;
    }

    updateTask(id, name, description, priority, goalId, type, parentTaskId, eventDateTime, place, deadtime, routines) {
        let body = {
            "Id": id,
            "Name": name,
            "Description": description,
            "Priority": priority,
            "GoalIdList": [goalId],
            "Type": type,
        }
        body.ParentTaskId = parentTaskId ? parentTaskId : null;
        body.EventDateTime = eventDateTime ? eventDateTime : null;
        body.Place = place ? place : null;
        body.DeadLine = deadtime ? deadtime : null;
        body.Routines = routines ? routines : null;
        return ougFetch('/api/Task/' + id, 'PATCH', body)
            .then(result => {
                alert("Done");
                return result.id;
            }).catch(ex => alert(ex));
    }

    addRoutineHour(day = -1, value = "") {
        const that = this;

        return (event) => {
            try {
                if (day == -1)
                    day = event.target.dataset.weekday;

                let list = document.getElementById("routineDayTimeList" + day);
                let newhour = 1;
                if (list.lastElementChild)
                    newhour = parseInt(list.lastElementChild.dataset.hour) + 1;

                let aux = `
		<div class="row w-100 m-0 position-relative routineDayTime" id="routineDayTime${day}${newhour}" data-weekday="${day}" data-hour="${newhour}">
			<div class="form-floating col p-0 ">
				<input type="time" class="form-control taskRoutineTimeOfDay" id="inputTaskRoutineTimeOfDay${day}${newhour}" placeholder="Hour ${day}${newhour}" value=${value}>
				<label for="inputTaskRoutineTimeOfDay${day}${newhour}">Hour ${day}${newhour}</label>
			</div>
			<button class="col-1 removeRoutineHour btn btn-close position-absolute top-0 end-0" data-weekday="${day}" data-hour="${newhour}"></button>
		</div>
    `;

                list.insertAdjacentHTML('beforeend', aux);

                document.querySelectorAll('.removeRoutineHour').forEach(el => { el.onclick = that.removeRoutineHour });
            } catch (ex) {
                alert(ex);
            };

        }

    }

    removeRoutineHour(event) {
        let parent = event.target.parentElement;
        parent.remove();
    }

    readTask(id) {
        if (id) return ougFetch('/api/Task/' + id, 'GET');
        return ougFetch('/api/Task', 'GET');
    }
}


class TasksList {
    constructor() {
        this.tasklist = document.getElementById('ouglist-tasklist');
        this.tasklist.onClickItem = this.openTask;
        this.tasklist.onClickAdd = this.openTask;
        this.tasklist.onClickRemove = this.eliminateTask;
        this.tasklist.onClickConfirm = this.confirmTask;

        this.modal = new TaskModal();
    }
    //#region actions

    get openTask() {
        const that = this;
        return (e, item) => {
            try {
                const id = item?.dataset?.taskid;
                if (id) that.modal.show(id);
                else that.modal.show();
            } catch (ex) {
                console.error(ex);
            }
        }

    }

    get eliminateTask() {
        const that = this;
        return async (e, item) => {
            try {
                let id = item.dataset.taskid;
                const result = await that.deleteTask(id);
                return result == "";
            } catch (ex) {
                console.error(ex);
                return false;
            }
        }
    }

    get confirmTask() {
        const that = this;
        return async (e, item) => {
            try {
                let id = item.dataset.taskid;
                const result = await that.finishTask(id);
                return result == "";
            } catch (ex) {
                console.error(ex);
                return false;
            }
        }
    }


    //#endregion

    //#region api call

    //#region Task

    readTask(id) {
        if (id) return ougFetch('/api/Task/' + id, 'GET');
        return ougFetch('/api/Task', 'GET');
    }

    deleteTask(id) {
        if (id && confirm("Delete it")) return ougFetch('/api/Task/' + id, 'DELETE');
        else return false;
    }

    finishTask(id) {
        if (id) return ougFetch('/api/Task/Finish/' + id, 'PATCH');
    }

    refresh() {
        location.reload();
    }

    //#endregion

    //#endregion
}

let taskList;
window.addEventListener('DOMContentLoaded', () => {
    taskList = new TasksList();
});
