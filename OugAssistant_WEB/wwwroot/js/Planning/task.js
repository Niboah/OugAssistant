
const host = 'https://localhost:44385'

function selectTaskGoalOnChange(event) {
    if (event.target.value == 'New') {
        const myModal = new bootstrap.Modal(document.getElementById('goalModal'));
        myModal.show();
    }
}

function createGoal(name, description) {
    let body = {
        "name": name,
        "description": description
    }
    return ajaxCall(host + '/api/Goal', 'POST', body);
}

function readGoal() {
    return ajaxCall(host + '/api/Goal', 'GET')
}

function createRoutine(weekDay, timeDay) {
    let body = {
        "weekDay": weekDay,
        "timeDay": timeDay
    }
    return ajaxCall(host + '/api/Routine', 'POST', body)
        .then(result => { return result.id });
}

async function createMission(name, description, priority, goalId, deadtime) {
    let body = {
        "name": name,
        "description": description,
        "priority": priority,
        "goalId": goalId,
        "deadLine": deadtime
    }
    ajaxCall(host + '/api/Task/Mission', 'POST', body)
        .then(result => alert(result));
}

async function createEvent(name, description, priority, goalId, eventDateTime, place) {
    let body = {
        "name": name,
        "description": description,
        "priority": priority,
        "goalId": goalId,
        "eventDateTime": eventDateTime,
        "place": place
    }
    ajaxCall(host + '/api/Task/Event', 'POST', body)
        .then(result => alert(result));
}

async function createRoutineTask(name, description, priority, goalId, routineId) {
    let body = {
        "name": name,
        "description": description,
        "priority": priority,
        "goalId": goalId,
        "routineId": routineId
    }
    ajaxCall(host + '/api/Task/Routine', 'POST', body)
        .then(result => alert(result));
}

function readTask() {
    return ajaxCall(host + '/api/Task', 'GET')
}

function newGoal() {
    let name = document.getElementById('inputGoalName').value;
    let description = document.getElementById('inputGoalDescription').value;
    createGoal(name, description)
        .then(readGoal)
        .then(goals => {
            const select = document.getElementById('selectTaskGoal');
            // Clear existing options, add default options
            select.innerHTML = `<option selected value=""></option>
										<option value="New">New</option>`;

            goals.forEach(value => {
                const option = document.createElement('option');
                option.value = value.id;
                option.text = value.name;
                option.selected = name == value.name;
                select.appendChild(option);
            });

            const myModal = bootstrap.Modal.getInstance(document.getElementById('goalModal'));
            myModal.hide();
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

    let routineDaysElements = document.querySelectorAll('.taskRoutineDay');
    let routineTimeOfDayElements = document.querySelectorAll('.taskRoutineTimeOfDay');

    let routineDays = Array.from(routineDaysElements).map((value, index) => value.checked ?index:null);
    let routineTimeOfDay = Array.from(routineTimeOfDayElements).map(el => el.value);

    if (type == 'mission') {
        await createMission(name, description, priority, goalId, deadtime)
    } else if (type == 'event') {
        await createEvent(name, description, priority, goalId, eventDateTime, place)
    } else if (type == 'routine') {
        let routineId = await createRoutine(routineDays, routineTimeOfDay);
        await createRoutineTask(name, description, priority, goalId, routineId)
    } else {
        console.log("error")
    }
    const tasks = await readTask();
    let tasklist = document.getElementById('taskList');
    // Clear existing options, add default options
    tasklist.innerHTML = "";

    tasks.forEach(value => {
        tasklist.innerHTML += `<li class="list-group-item">
						<label>
							${value.name}
						</label>
						<button class="button btn-close float-end"> </button>
					</li>`;
    });

    const myModal = bootstrap.Modal.getInstance(document.getElementById('taskModal'));
    myModal.hide();
}



(() => {
    document.getElementById('btnNewTask').onclick = newTask;
    document.getElementById('btnNewGoal').onclick = newGoal;
    document.getElementById('selectTaskGoal').onchange = selectTaskGoalOnChange;
})();