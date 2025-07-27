/**
 * OugAssistant OugTask javascript file
 */

class TasksList {
    constructor(bootstrap) {
        this.init();
    }

    init() {
        this.tasklist = document.getElementById('ouglist-tasklist');
        this.tasklist.onClickItem = this.openTask;
        this.tasklist.onClickAdd = this.openTask;
        this.tasklist.onClickRemove = this.eliminateTask;
        this.tasklist.onClickConfirm = this.confirmTask;

        //region taskform
        this.selectedTaskId;

        this.inputTaskName = document.getElementById('inputTaskName');
        this.inputTaskDescription = document.getElementById('inputTaskDescription');
        this.inputTaskPriority = document.getElementById('inputTaskPriority');
        this.selectTaskGoal = document.getElementById('selectTaskGoal');

        this.tasktype = document.querySelectorAll('.taskType');
        this.taskTypeContent = document.querySelectorAll('.taskTypeContent');

        this.inputTaskEventDateTime = document.getElementById('inputTaskEventDateTime');
        this.inputTaskEventPlace = document.getElementById('inputTaskEventPlace');

        this.inputTaskMissionDeadLine = document.getElementById('inputTaskMissionDeadLine');

        document.getElementById('btnSaveTask').onclick = this.saveTask;
        //#endregion


        //region goalform
        document.getElementById('selectTaskGoal').onchange = this.selectTaskGoalOnChange;
        document.getElementById('btnSaveGoal').onclick = this.saveGoal;
        //#endregion


        document.querySelectorAll('.addRoutineHour').forEach(el => { el.onclick = this.addRoutineHour() });
        document.querySelectorAll('.removeRoutineHour').forEach(el => { el.onclick = this.removeRoutineHour });
    }

    //#region modal

    get taskmodal() {
        return window.bootstrap.Modal.getOrCreateInstance(document.getElementById('taskModal'))
    }

    get goalmodal() {
        return window.bootstrap.Modal.getOrCreateInstance(document.getElementById('goalModal'))
    }

    //#endregion

    //#region actions

    get openTask() {
        const that = this;
        return async (e, item) => {
            try {
                let id;
                if (e.target.classList.contains('ouglist-btn-add')) {
                    const now = new Date();
                    this.inputTaskName.value = "";
                    this.inputTaskDescription.value = "";;
                    this.inputTaskPriority.value = 0;
                    this.selectTaskGoal.value = "";
                    this.inputTaskEventDateTime.value = now.toISOString().slice(0, 16); ;
                    this.inputTaskEventPlace.value = "";
                    this.inputTaskMissionDeadLine.value = now.toISOString().slice(0, 16);

                    this.tasktype.forEach(el => el.classList.remove('active'));
                    this.taskTypeContent.forEach(el => el.classList.remove('active', 'show'));

                    const triggerEl = document.getElementById('nav-event-tab');
                    const tab = new bootstrap.Tab(triggerEl);
                    tab.show();

                } else if (item) {
                    id = item.dataset.taskid;
                    if (id) {
                        const task = await this.readTask(id);

                        this.inputTaskName.value = task.name;
                        this.inputTaskDescription.value = task.description;
                        this.inputTaskPriority.value = task.priority;
                        this.selectTaskGoal.value = task.goal?.id;

                        this.tasktype.forEach(el => el.classList.remove('active'));
                        this.taskTypeContent.forEach(el => el.classList.remove('active', 'show'));

                        if (task.eventDateTime) {
                            this.inputTaskEventDateTime.value = task.eventDateTime;
                            this.inputTaskEventPlace.value = task.place;

                            const triggerEl = document.getElementById('nav-event-tab');
                            const tab = new bootstrap.Tab(triggerEl);
                            tab.show();

                        } else if (task.deadLine) {
                            this.inputTaskMissionDeadLine.value = task.deadLine;

                            const triggerEl = document.getElementById('nav-mission-tab');
                            const tab = new bootstrap.Tab(triggerEl);
                            tab.show();
                        } else if (task.weekTimes) {

                            task.weekTimes.forEach((value, i) => {
                                if (value.length > 0) {
                                    document.getElementById('inputTaskRoutineDay' + i).checked = true;
                                    let collapseElement = document.getElementById('routineDayTimeListContainer' + i);
                                    const bsCollapse = new bootstrap.Collapse(collapseElement, {
                                        toggle: false
                                    });
                                    bsCollapse.toggle();
                                }
                            });

                            for (let i = 0; i < task.weekTimes.length; i++) {
                                let firstHourEl = document.getElementById('inputTaskRoutineTimeOfDay' + i + 0);
                                firstHourEl.value = task.weekTimes[i][0];
                                let list = document.getElementById('routineDayTimeList' + i)
                                for (let j = 1; j < task.weekTimes[i].length; j++) {
                                    that.addRoutineHour(null, i, task.weekTimes[i][j])();
                                }
                            }
                            const triggerEl = document.getElementById('nav-routine-tab');
                            const tab = new bootstrap.Tab(triggerEl);
                            tab.show();
                        }

                    }
                }
                that.selectedTaskId = id;
                that.taskmodal.show();
            } catch (ex) {
                console.error(ex);
            }

        }

    }

    get saveTask() {
        const that = this;
        return async () => {
            try {
                let taskId = that.selectedTaskId;

                let name = this.inputTaskName.value;
                let description = this.inputTaskDescription.value;
                let priority = Number(this.inputTaskPriority.value);
                let goalId = this.selectTaskGoal.value;

                let type = document.querySelector('.taskType.active').dataset.tasktype;

                let eventDateTime = this.inputTaskEventDateTime.value;
                let place = this.inputTaskEventPlace.value;

                let deadtime = this.inputTaskMissionDeadLine.value;

                if (taskId) {

                    //#region update
                    if (type == 'mission') {
                        taskId = await this.updateMission(taskId, name, description, priority, goalId, deadtime)
                    } else if (type == 'event') {
                        taskId = await this.updateEvent(taskId, name, description, priority, goalId, eventDateTime, place)
                    } else if (type == 'routine') {

                        let weekTimes = document.querySelectorAll('.taskRoutineDay');
                        weekTimes = Array.from(weekTimes).map(el => {
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
                        taskId = await updateRoutine(taskId, name, description, priority, goalId, weekTimes)
                    } else {
                        console.log("error")
                    }
                    //#endregion

                } else {

                    //#region new

                    if (type == 'mission') {
                        taskId = await this.createMission(name, description, priority, goalId, deadtime)
                    } else if (type == 'event') {
                        taskId = await this.createEvent(name, description, priority, goalId, eventDateTime, place)
                    } else if (type == 'routine') {

                        let weekTimes = document.querySelectorAll('.taskRoutineDay');
                        weekTimes = Array.from(weekTimes).map(el => {
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
                        taskId = await this.createRoutine(name, description, priority, goalId, weekTimes)
                    } else {
                        console.log("error")
                    }
                    //#endregion

                }
                that.refresh();
                that.taskmodal.hide();
                
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
                return await that.deleteTask(id);
            } catch (ex) {
                console.error(ex);
                return false;
            }
        }
    }

    get selectTaskGoalOnChange() {
        const that = this;
        return (e) => {
            if (e.target.value == 'New') {
                that.goalmodal.show();
            }
        }

    }

    get saveGoal() {
        const that = this;
        return () => {
            let name = document.getElementById('inputGoalName').value;
            let description = document.getElementById('inputGoalDescription').value;
            that.createGoal(name, description)
                .then(goal => {
                    const select = document.getElementById('selectTaskGoal');
                    // Clear existing options, add default options
                    select.innerHTML = `<option value=""></option>
								<option value="New">New</option>`;

                    const option = document.createElement('option');
                    option.value = goal.id;
                    option.text = goal.name;
                    option.selected = name == goal.name;
                    select.appendChild(option);

                    that.goalmodal.hide();
                })

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

    addRoutineHour(day = -1, value = "") {
        const that = this;

        return (event) => {
            if (day == -1)
                day = event.target.dataset.weekday;

            let list = document.getElementById("routineDayTimeList" + day);
            let newhour = 1;
            if (list.lastElementChild)
                newhour = parseInt(list.lastElementChild.dataset.hour) + 1;

            let aux = `
		<div class="row w-100 m-0 position-relative" id="routineDayTime${day}${newhour}" data-weekday="${day}" data-hour="${newhour}">
			<div class="form-floating col p-0 ">
				<input type="time" class="form-control taskRoutineTimeOfDay" id="inputTaskRoutineTimeOfDay${day}${newhour}" placeholder="Hour ${day}${newhour}" value=${value}>
				<label for="inputTaskRoutineTimeOfDay${day}${newhour}">Hour ${day}${newhour}</label>
			</div>
			<button class="col-1 removeRoutineHour btn btn-close position-absolute top-0 end-0" data-weekday="${day}" data-hour="${newhour}"></button>
		</div>
    `;

            list.insertAdjacentHTML('beforeend', aux);

            document.querySelectorAll('.removeRoutineHour').forEach(el => { el.onclick = that.removeRoutineHour });
        }

    }

    removeRoutineHour(event) {
        let parent = event.target.parentElement;
        parent.remove();
    }

    //#endregion

    //#region api call

    //#region Task

    readTask(id) {
        if (id) return ajaxCall('/api/Task/' + id, 'GET');
        return ajaxCall('/api/Task', 'GET');
    }

    deleteTask(id) {
        if (id) return ajaxCall('/api/Task/' + id, 'DELETE');
    }

    createMission(name, description, priority, goalId, deadtime) {
        let body = {
            "name": name,
            "description": description,
            "priority": priority,
            "goalId": goalId,
            "deadLine": deadtime
        }
        return ajaxCall('/api/Task/Mission', 'POST', body)
            .then(result => {
                alert(JSON.stringify(result));
                return result.id;
            });
    }

    updateMission(id, name, description, priority, goalId, deadtime) {
        let body = {
            "Id": id,
            "name": name,
            "description": description,
            "priority": priority,
            "goalId": goalId,
            "deadLine": deadtime
        }
        return ajaxCall('/api/Task/Mission/' + id, 'PATCH', body)
            .then(result => {
                alert(JSON.stringify(result));
                return result.id;
            });
    }

    createEvent(name, description, priority, goalId, eventDateTime, place) {
        let body = {
            "name": name,
            "description": description,
            "priority": priority,
            "goalId": goalId,
            "eventDateTime": eventDateTime,
            "place": place
        }
        return ajaxCall('/api/Task/Event', 'POST', body)
            .then(result => {
                alert(JSON.stringify(result));
                return result.id;
            });
    }

    updateEvent(id, name, description, priority, goalId, eventDateTime, place) {
        let body = {
            "Id": id,
            "name": name,
            "description": description,
            "priority": priority,
            "goalId": goalId,
            "eventDateTime": eventDateTime,
            "place": place
        }
        return ajaxCall('/api/Task/Event/' + id, 'PATCH', body)
            .then(result => {
                alert(JSON.stringify(result));
                return result.id;
            });
    }

    createRoutine(name, description, priority, goalId, weekTimes) {
        let body = {
            "name": name,
            "description": description,
            "priority": priority,
            "goalId": goalId,
            "weekTimes": weekTimes
        }
        return ajaxCall('/api/Task/Routine', 'POST', body)
            .then(result => {
                alert(JSON.stringify(result));
                return result.id;
            });
    }

    updateRoutine(id, name, description, priority, goalId, weekTimes) {
        let body = {
            "Id": id,
            "name": name,
            "description": description,
            "priority": priority,
            "goalId": goalId,
            "weekTimes": weekTimes
        }
        return ajaxCall('/api/Task/Routine/' + id, 'PATCH', body)
            .then(result => {
                alert(JSON.stringify(result));
                return result.id;
            });
    }

    finishTask(id) {
        if (id) return ajaxCall('/api/Task/Finish/' + id, 'PATCH');
    }

    refresh() {
        ajaxCall('/Planning/TaskList', 'GET')
            .then(html => {
                document.getElementById('task_container').innerHTML = html;

                this.init();
            });
    }

    //#endregion

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

let taskList;
window.addEventListener('DOMContentLoaded', () => {
    taskList = new TasksList();
});
