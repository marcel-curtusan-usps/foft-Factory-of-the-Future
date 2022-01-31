//Retrieve 24 Hour Clock Message
function GetTwentyFourMessage(time) {
    var format = 'HH:mm:ss';
    var currtime = moment(moment(time).format(format), format);

    if (currtime.isBetween(moment('00:00:00', format), moment('00:30:00', format))) {
        return '00:30 Outgoing Secondary Completed';
    }
    if (currtime.isBetween(moment('00:30:00', format), moment('02:30:00', format))) {
        return '02:30 FedEx and Commercial Assigned';
    }
    if (currtime.isBetween(moment('02:30:00', format), moment('05:00:00', format))) {
        return '05:00 DPS Second Pass(919) Completed';
    }
    if (currtime.isBetween(moment('05:00:00', format), moment('07:00:00', format))) {
        return '00:00-07:00 100% Trips On Time';
    }
    if (currtime.isBetween(moment('07:00:00', format), moment('15:00:00', format))) {
        return '15:00 MMP Mail Cleared';
    }
    if (currtime.isBetween(moment('15:00:00', format), moment('20:00:00', format))) {
        return '20:00 80% Mail Canceled';
    }
    if (currtime.isBetween(moment('20:00:00', format), moment('23:59:59', format))) {
        return '24:00 Outgoing Primary Completed';
    }
}

//Move 24 Hour Clock Hands
function SetClockHands(time) {
    var hr = moment(time).hour();
    var min = moment(time).minute();
    var sec = moment(time).second();
    var hr_rotation = 15 * hr + min / 4;
    var min_rotation = 6 * min;
    var sec_rotation = 6 * sec;

    hour.style.transform = `rotate(${hr_rotation}deg)`;
    minute.style.transform = `rotate(${min_rotation}deg)`;
    second.style.transform = `rotate(${sec_rotation}deg)`;
}

//Add 24 Hour Clock
function Initiate24HourClock() {
    var ClockMessage = L.Control.extend({
        options: {
            position: 'topright'
        },
        onAdd: function (map) {
            var container = L.DomUtil.create('label');
            container.id = "twentyfourmessage";
            container.className = "btn btn-secondary btn-sm";
            container.style = "width: 150px";
            return container;
        }
    });
    map.addControl(new ClockMessage());

    $('#tfhcContent').click(function () { $('#twentyfourmessage').popover('hide'); });

    $('#twentyfourmessage').popover({
        html: true,
        trigger: 'click',
        content: $('#tfhcContent'),
    });

    $('#twentyfourmessage').on('click', function () {
        // close the sidebar
        sidebar.close();
    });
}



