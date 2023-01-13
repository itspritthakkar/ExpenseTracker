let calendar = document.querySelector('.calendar')

const month_names = ['January', 'February', 'March', 'April', 'May', 'June', 'July', 'August', 'September', 'October', 'November', 'December']

isLeapYear = (year) => {
    return (year % 4 === 0 && year % 100 !== 0 && year % 400 !== 0) || (year % 100 === 0 && year % 400 ===0)
}

getFebDays = (year) => {
    return isLeapYear(year) ? 29 : 28
}

getMonthNo = (monthName) => {
    if (monthName.length > 3) {
        monthName = monthName.slice(0, 3);
    }
    return "JanFebMarAprMayJunJulAugSepOctNovDec".indexOf(monthName) / 3 + 1;
}

attachDateHandler = () => {
    const dates = document.querySelectorAll('.calendar-day-hover');

    dates.forEach(date => {
        date.addEventListener('click', function handleClick(event) {
            currDate = new Date(year_picker.innerHTML, getMonthNo(month_picker.innerHTML) - 1, this.innerHTML);
            var curDateSelector = document.querySelector(".curr-date");
            if (curDateSelector != null) {
                curDateSelector.classList.toggle("curr-date");
            }
            this.classList.toggle("curr-date");
            document.querySelector(".custom-date-picker").value = currDate.getFullYear() + "-" + ('0' + (currDate.getMonth() + 1)).slice(-2) + "-" + ('0' + currDate.getDate()).slice(-2);
        });
    });
}

generateCalendar = (month, year) => {

    let calendar_days = calendar.querySelector('.calendar-days')
    let calendar_header_year = calendar.querySelector('#year')

    let days_of_month = [31, getFebDays(year), 31, 30, 31, 30, 31, 31, 30, 31, 30, 31]

    calendar_days.innerHTML = ''

    if (currDate == undefined || currDate == null) {
        let currDate = new Date();
    }
    if (month == null) month = currDate.getMonth()
    if (!year) year = currDate.getFullYear()

    let curr_month = `${month_names[month]}`
    month_picker.innerHTML = curr_month
    calendar_header_year.innerHTML = year

    // get first day of month
    
    let first_day = new Date(year, month, 1)

    for (let i = 0; i <= days_of_month[month] + first_day.getDay() - 1; i++) {
        let day = document.createElement('div')
        if (i >= first_day.getDay()) {
            day.classList.add('calendar-day-hover')
            day.innerHTML = i - first_day.getDay() + 1;
            if (i - first_day.getDay() + 1 === currDate.getDate() && year === currDate.getFullYear() && month === currDate.getMonth()) {
                day.classList.add('curr-date')
            }
        }
        calendar_days.appendChild(day)
    }
    attachDateHandler();
}

let month_list = calendar.querySelector('.month-list')

month_names.forEach((e, index) => {
    let month = document.createElement('div')
    month.innerHTML = `<div data-month="${index}">${e}</div>`
    month.querySelector('div').onclick = () => {
        month_list.classList.remove('show')
        curr_month.value = index
        generateCalendar(index, curr_year.value)
        calendar_days = calendar.querySelector('.calendar-days')
        calendar_days.lastChild.click();
    }
    month_list.appendChild(month)
})

let month_picker = calendar.querySelector('#month-picker')
let year_picker = calendar.querySelector('#year');

month_picker.onclick = () => {
    month_list.classList.add('show')
}

function makeCalendar() {
    let curr_month = {value: currDate.getMonth()}
    let curr_year = {value: currDate.getFullYear()}

    generateCalendar(curr_month.value, curr_year.value)

    document.querySelector('#prev-year').onclick = () => {
        --curr_year.value
        generateCalendar(curr_month.value, curr_year.value)
    }

    document.querySelector('#next-year').onclick = () => {
        ++curr_year.value
        generateCalendar(curr_month.value, curr_year.value)
    }
}