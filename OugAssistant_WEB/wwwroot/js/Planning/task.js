/**
 * OugAssistant OugTask javascript file
 */
function selectTaskGoalOnChange(event) {
    if (event.target.value == 'New') {
        let goalModal = new bootstrap.Modal(document.getElementById('goalModal'));
        goalModal.show();
    }
}


function addRoutineHour(event, day = -1,value="") {
    if (day == -1)
        day = event.target.dataset.weekday;

    let list = document.getElementById("routineDayTimeList" + day);
    let newhour = 1;
    if (list.lastElementChild)
        newhour =  parseInt(list.lastElementChild.dataset.hour) + 1;

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

    document.querySelectorAll('.removeRoutineHour').forEach(el => { el.onclick = removeRoutineHour });
}


function removeRoutineHour(event) {
    let parent = event.target.parentElement;
    parent.remove();
}

function createGoal(name, description) {
    let body = {
        "name": name,
        "description": description
    }
    return ajaxCall( '/api/Goal', 'POST', body);
}

function readGoal(id) {
    if (id) return ajaxCall('/api/Goal/' + id, 'GET');
    return ajaxCall('/api/Goal', 'GET');
}

function createMission(name, description, priority, goalId, deadtime) {
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

function createEvent(name, description, priority, goalId, eventDateTime, place) {
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

function createRoutine(name, description, priority, goalId, weekTimes) {
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

function readTask(id) {
    if (id)  return ajaxCall('/api/Task/'+id, 'GET');
    return ajaxCall('/api/Task', 'GET');
}

function deleteTask(id) {
    if (id) return ajaxCall('/api/Task/' + id, 'DELETE');
}


function newGoal() {
    let name = document.getElementById('inputGoalName').value;
    let description = document.getElementById('inputGoalDescription').value;
    createGoal(name, description)
        .then(readGoal)
        .then(goals => {
            const select = document.getElementById('selectTaskGoal');
            // Clear existing options, add default options
            select.innerHTML = `<option value=""></option>
								<option value="New">New</option>`;

            goals.forEach(value => {
                const option = document.createElement('option');
                option.value = value.id;
                option.text = value.name;
                option.selected = name == value.name;
                select.appendChild(option);
            });

            goalModal.hide();
        })
}

async function newTask() {
    let name = document.getElementById('inputTaskName').value;
    let description = document.getElementById('inputTaskDescription').value;
    let priority = Number(document.getElementById('inputTaskPriority').value);
    let goalId = document.getElementById('selectTaskGoal').value;

    let type = document.querySelector('.taskType.active').dataset.tasktype;

    let eventDateTime = document.getElementById('inputTaskEventDateTime').value;
    let place = document.getElementById('inputTaskEventPlace').value;

    let deadtime = document.getElementById('inputTaskMissionDeadLine').value;

    let newTaskId;

    if (type == 'mission') {
        newTaskId = await createMission(name, description, priority, goalId, deadtime)
    } else if (type == 'event') {
        newTaskId = await createEvent(name, description, priority, goalId, eventDateTime, place)
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
        newTaskId = await createRoutine(name, description, priority, goalId, weekTimes)
    } else {
        console.log("error")
    }
    const task = await readTask(newTaskId);

    let tasklist = document.getElementById('taskList');
    let aux = `<li class="list-group-item d-flex">
                        <div class="OugTask flex-grow-1" id="${task.id}">
							<label>
								${task.name}
							</label>
						</div>
						<button class="button btn-close float-end"> </button>
					    </li>`
    tasklist.insertAdjacentHTML('afterbegin', aux);

    document.querySelectorAll('.OugTask').forEach(el => { el.onclick = openTask });
    let taskModal = bootstrap.Modal.getOrCreateInstance(document.getElementById('taskModal'));
    taskModal.hide();
}

async function openTask(event) {
    let id = event.target.id;

    let task = await readTask(id);

    document.getElementById('inputTaskName').value = task.name;
    document.getElementById('inputTaskDescription').value = task.description;
    document.getElementById('inputTaskPriority').value = task.priority;
    document.getElementById('selectTaskGoal').value = task.goalId;

    document.querySelectorAll('.taskType').forEach(el => el.classList.remove('active'));
    document.querySelectorAll('.taskTypeContent').forEach(el => el.classList.remove('active', 'show'));

    if (task.eventDateTime) {
        document.getElementById('inputTaskEventDateTime').value = task.eventDateTime;
        document.getElementById('inputTaskEventPlace').value = task.place;

        const triggerEl = document.getElementById('nav-event-tab');
        const tab = new bootstrap.Tab(triggerEl);
        tab.show();

    } else if (task.deadLine) {
        document.getElementById('inputTaskMissionDeadLine').value = task.deadLine;

        const triggerEl = document.getElementById('nav-mission-tab');
        const tab = new bootstrap.Tab(triggerEl);
        tab.show();
    } else if (task.weekTimes) {

        task.weekTimes.forEach((value, i) => {
            if (value.length > 0) {
                document.getElementById('inputTaskRoutineDay' + i).checked = true;
                let collapseElement = document.getElementById('routineDayTimeListContainer'+i);
                const bsCollapse = new bootstrap.Collapse(collapseElement, {
                    toggle: false
                });
                bsCollapse.toggle();
            }
        });
        
        for (let i = 0; i < task.weekTimes.length; i++) {
            let firstHourEl = document.getElementById('inputTaskRoutineTimeOfDay' + i + 0);
            firstHourEl.value = task.weekTimes[i][0];
            let list = document.getElementById('routineDayTimeList'+i)
            for (let j = 1; j < task.weekTimes[i].length;j++){
                addRoutineHour(null, i, task.weekTimes[i][j])
            }
        }
        const triggerEl = document.getElementById('nav-routine-tab');
        const tab = new bootstrap.Tab(triggerEl);
        tab.show();
    }  
    let taskModal = bootstrap.Modal.getOrCreateInstance(document.getElementById('taskModal'));
    taskModal.show();
}

async function eliminateTask(event) {
    let id = event.target.parentElement.id;
    deleteTask(id);
}

(() => {
    document.getElementById('btnNewTask').onclick = newTask;
    document.getElementById('btnNewGoal').onclick = newGoal;
    document.getElementById('selectTaskGoal').onchange = selectTaskGoalOnChange;
    document.querySelectorAll('.OugTask').forEach(el => { el.onclick = openTask });
    document.querySelectorAll('.deleteTask').forEach(el => { el.onclick = eliminateTask });
    document.querySelectorAll('.addRoutineHour').forEach(el => { el.onclick = addRoutineHour });
    document.querySelectorAll('.removeRoutineHour').forEach(el => { el.onclick = removeRoutineHour });
})();