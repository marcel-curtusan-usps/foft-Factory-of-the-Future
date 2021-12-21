﻿/// A simple template method for replacing placeholders enclosed in curly braces.
if (!String.prototype.supplant) {
    String.prototype.supplant = function (o) {
        return this.replace(/{([^{}]*)}/g,
            function (a, b) {
                var r = o[b];
                return typeof r === 'string' || typeof r === 'number' ? r : a;
            }
        );
    };
}
var User = {};
var connectattp = 0;
var connecttimer;
var timezone = {};
var map = null;
//side bar setup
var sidebar = L.control.sidebar({
    container: 'sidebar', position: 'left', autopan: false
});
var container = new L.FeatureGroup();

let fotfmanager = $.connection.FOTFManager;
$(() => {
    $("form").submit(function () { return false; });

    setHeight();
    $(window).resize(function () {
        setHeight();
    });

    //on close clear all IDS inputs

    $('#CTS_Details_Modal').on('hidden.bs.modal', () => {
        $CTSDetails_Table = $('table[id=ctsdetailstable]');
        $CTSDetails_Table_Header = $CTSDetails_Table.find('thead');
        $CTSDetails_Table_Header.empty();
        $CTSDetails_Table_Body = $CTSDetails_Table.find('tbody')
        $CTSDetails_Table_Body.empty();
    });
    $('#CTS_Details_Modal').on('shown.bs.modal', () => {
        $("#modal-preloader").show();
    });
    $('#UserTag_Modal').on('hidden.bs.modal', function () {
        $(this)
            .find("input[type=text],textarea,select")
            .css({ "border-color": "#D3D3D3" })
            .val('')
            .end()
            .find("span[class=text]")
            .css("border-color", "#FF0000")
            .val('')
            .text("")
            .end()
            .find('input[type=checkbox]')
            .prop('checked', false).change();

        if (!$('#UserTag_Modal').hasClass('in')) {
            $('#UserTag_Modal').addClass('modal-open');
        }
    });
    //on close clear all inputs
    $('#Zone_Modal').on('hidden.bs.modal', function () {
        $(this)
            .find("input[type=text],textarea,select")
            .css({ "border-color": "#D3D3D3" })
            .val('')
            .end()
            .find("span[class=text]")
            .css("border-color", "#FF0000")
            .val('')
            .text("")
            .end()
            .find('input[type=checkbox]')
            .prop('checked', false).change();
    });
    //on open set rules
    $('#Zone_Modal').on('shown.bs.modal', () => {
        $('span[id=error_machinesubmitBtn]').text("");
        $('button[id=machinesubmitBtn]').prop('disabled', true);
        //Request Type Keyup
        $('input[type=text][name=machine_name]').keyup(function () {
            if (!checkValue($('input[type=text][name=machine_name]').val())) {
                $('input[type=text][name=machine_name]').css("border-color", "#FF0000").removeClass('is-valid').addClass('is-invalid');
                $('span[id=error_machine_name]').text("Please Enter Machine Name");
            }
            else {
                $('input[type=text][name=machine_name]').css("border-color", "#2eb82e").removeClass('is-invalid').addClass('is-valid');
                $('span[id=error_machine_name]').text("");
            }

            enablezoneSubmit();
        });
        //Connection name Validation
        if (!checkValue($('input[type=text][name=machine_name]').val())) {
            $('input[type=text][name=machine_name]').css("border-color", "#FF0000").removeClass('is-valid').addClass('is-invalid');
            $('span[id=error_machine_name]').text("Please Enter Machine Name");
        }
        else {
            $('input[type=text][name=machine_name]').css("border-color", "#2eb82e").removeClass('is-invalid').addClass('is-valid');
            $('span[id=error_machine_name]').text("");
        }
        //Request Type Keyup
        $('input[type=text][name=machine_number]').keyup(function () {
            if (!checkValue($('input[type=text][name=machine_number]').val())) {
                $('input[type=text][name=machine_number]').css("border-color", "#FF0000").removeClass('is-valid').addClass('is-invalid');
                $('span[id=error_machine_number]').text("Please Enter Machine Number");
            }
            else {
                $('input[type=text][name=machine_number]').css("border-color", "#2eb82e").removeClass('is-invalid').addClass('is-valid');
                $('span[id=error_machine_number]').text("");
            }

            enablezoneSubmit();
        });
        //Request Type Validation
        if (!checkValue($('input[type=text][name=machine_number]').val())) {
            $('input[type=text][name=machine_number]').css("border-color", "#FF0000").removeClass('is-valid').addClass('is-invalid');
            $('span[id=error_machine_number]').text("Please Enter Machine Number");
        }
        else {
            $('input[type=text][name=machine_number]').css("border-color", "#2eb82e").removeClass('is-invalid').addClass('is-valid');
            $('span[id=error_machine_number]').text("");
        }
        //Request Type Validation
        if (checkValue($('input[type=text][name=zone_ldc]').val())) {
            $('input[type=text][name=zone_ldc]').css("border-color", "#2eb82e").removeClass('is-invalid').addClass('is-valid');
            $('span[id=error_zone_ldc]').text("");
        }
        else {
            $('input[type=text][name=zone_ldc]').css("border-color", "#D3D3D3").removeClass('is-invalid').removeClass('is-valid');
            $('span[id=error_zone_ldc]').text("");
        }
        //Request zone LDC Keyup
        $('input[type=text][name=zone_ldc]').keyup(function () {
            if (checkValue($('input[type=text][name=zone_ldc]').val())) {
                $('input[type=text][name=zone_ldc]').css("border-color", "#2eb82e").removeClass('is-invalid').addClass('is-valid');
                $('span[id=error_zone_ldc]').text("");
            }
            else {
                $('input[type=text][name=zone_ldc]').css("border-color", "#D3D3D3").removeClass('is-invalid').removeClass('is-valid');
                $('span[id=error_zone_ldc]').text("");
            }
        });
    });
    $('input[id=warning_condition]').on("slide", function (slideEvt) {
        $('span[id=warning_conditionpickvalue]').text(slideEvt.value);
    });

    $.extend(fotfmanager.client, {
        updateClock: async (timer) => {
            $('#localTime').val(moment(timer).format('H:mm:ss'));
            ///badgecomplaint();
            zonecurrentStaff();
            var visible = sidebar._getTab("reports");
            if (visible) {
                if (visible.classList.length) {
                    if (visible.classList.contains('active')) {
                        GetUserInfo();
                    }
                }
            }

        },
        /*re-factor the CTS Calls*/
        //updateCTSDepartedStatus: async (CTSData) => {
        //    try {
        //        $ctsdockDcardtop_Table = $('table[id=ctsdockdepartedtable]');
        //        $ctsdockDcardtop_Table_Body = $ctsdockDcardtop_Table.find('tbody');
        //        $ctsdockDcardtop_row_template = '<tr data-id=ctsOB_{routetrip} data-route={route} data-trip={trip}  data-door={door}  class={trbackground}>' +
        //            '<td class="text-center">{schd}</td>' +
        //            '<td class="text-center">{departed}</td>' +
        //            '<td class="{background}">{btnloadDoor}</td>' +
        //            '<td class="text-center">{leg}</td>' +
        //            '<td data-toggle="tooltip" title="{dest}">{dest}</td>' +
        //            '<td class="text-center">{close}</td>' +
        //            '<td class="text-center">{load}</td>' +
        //            '<td>{btnloadPercent}</td>' +
        //            '</tr>"';

        //        function formatctsdockDcardtoprow(properties) {
        //            return $.extend(properties, {
        //                schd: checkValue(properties.ScheduledTZ) ? formatTime(properties.ScheduledTZ) : "",
        //                departed: checkValue(properties.DepartedTZ) ? formatTime(properties.DepartedTZ) : "",
        //                door: checkValue(properties.Door) ? properties.Door : "",
        //                routetrip: properties.Route + properties.Trip,
        //                route: properties.Route,
        //                trip: properties.Trip,
        //                leg: properties.Leg,
        //                dest: properties.Destination,
        //                load: properties.Load,
        //                background: checkValue(properties.Door) ? "" : "purpleBg",
        //                trbackground: "",// Gettimediff(properties.ScheduledTZ),
        //                close: properties.Closed,
        //                btnloadPercent: Load_btn_details(properties),
        //                btnloadDoor: Load_btn_door(properties),
        //                dataproperties: properties
        //            });
        //        }
        //        var findtrdataid = $ctsdockDcardtop_Table_Body.find('tr[data-id=ctsOB_' + CTSData.Route + CTSData.Trip + ']');
        //        if (findtrdataid.length > 0) {
        //            if (CTSData.CTS_Remove) {
        //                $ctsdockDcardtop_Table_Body.find('tr[data-id=ctsOB_' + CTSData.Route + CTSData.Trip + ']').remove();
        //            }
        //            else {
        //                $ctsdockDcardtop_Table_Body.find('tr[data-id=ctsOB_' + CTSData.Route + CTSData.Trip + ']').replaceWith($ctsdockDcardtop_row_template.supplant(formatctsdockDcardtoprow(CTSData)));
        //            }
        //        }
        //        else {
        //            if (!CTSData.CTS_Remove) {
        //                $ctsdockDcardtop_Table_Body.append($ctsdockDcardtop_row_template.supplant(formatctsdockDcardtoprow(CTSData)));
        //            }
        //        }
        //    } catch (e) {
        //        console.log(e);
        //    }
        //},
        //updateCTSLocalDepartedStatus: async (CTSlocalData) => {
        //    try {
        //        $ctslocaldockDcardtop_Table = $('table[id=ctslocaldockdepartedtable]');
        //        $ctslocaldockDcardtop_Table_Body = $ctslocaldockDcardtop_Table.find('tbody');
        //        $ctslocalcard_row_template = '<tr data-id=localctsOB_{routetrip} data-route={route} data-trip={trip} data-door={door} class={trbackground}>' +
        //            '<td class="text-center">{schd}</td>' +
        //            '<td class="text-center">{departed}</td>' +
        //            '<td class="{background}">{btnloadDoor}</td>' +
        //            '<td class="text-center">{leg}</td>' +
        //            '<td data-toggle="tooltip" title="{dest}">{dest}</td>' +
        //            '<td class="text-center">{close}</td>' +
        //            '<td class="text-center">{load}</td>' +
        //            '<td>{loadPercent}</td>' +
        //            '</tr>"';

        //        function formatctslocaldockDcardtoprow(properties) {
        //            return $.extend(properties, {
        //                schd: checkValue(properties.ScheduledTZ) ? moment(properties.ScheduledTZ).format("HH:mm") : "",
        //                departed: checkValue(properties.DepartedTZ) ? moment(properties.DepartedTZ).format("HH:mm") : "",
        //                door: checkValue(properties.Door) ? properties.Door : "",
        //                routetrip: properties.Route + properties.Trip,
        //                route: properties.Route,
        //                trip: properties.Trip,
        //                leg: properties.Leg,
        //                dest: properties.Destination,
        //                load: properties.Load,
        //                background: checkValue(properties.Door) ? "" : "purpleBg",
        //                trbackground: "",// Gettimediff(properties.ScheduledTZ),
        //                close: properties.Closed,
        //                loadPercent: '<button class="btn btn-outline-info btn-sm btn-block" disabled name="ctsdetails">' + properties.LoadPercent + '%</button>',
        //                btnloadDoor: Load_btn_door(properties),
        //                dataproperties: properties
        //            });
        //        }
        //        var findtrdataid = $ctslocaldockDcardtop_Table_Body.find('tr[data-id=localctsOB_' + CTSlocalData.Route + CTSlocalData.Trip + ']');
        //        if (findtrdataid.length > 0) {
        //            if (CTSlocalData.CTS_Remove) {
        //                $ctslocaldockDcardtop_Table_Body.find('tr[data-id=localctsOB_' + CTSlocalData.Route + CTSlocalData.Trip + ']').remove();
        //            }
        //            else {
        //                $ctslocaldockDcardtop_Table_Body.find('tr[data-id=localctsOB_' + CTSlocalData.Route + CTSlocalData.Trip + ']').replaceWith($ctslocalcard_row_template.supplant(formatctslocaldockDcardtoprow(CTSlocalData)));
        //            }
        //        }
        //        else {
        //            if (!CTSlocalData.CTS_Remove) {
        //                $ctslocaldockDcardtop_Table_Body.append($ctslocalcard_row_template.supplant(formatctslocaldockDcardtoprow(CTSlocalData)));
        //            }
        //        }
        //    } catch (e) {
        //        console.log(e)
        //    }
        //},
        //updateCTSInboundStatus: async (CTSInboundData) => {
        //    try {
        //        $ctsIncardtop_Table = $('table[id=ctsintoptable]');
        //        $ctsIncardtop_Table_Body = $ctsIncardtop_Table.find('tbody');
        //        $ctsIncardtop_row_template = '<tr data-id="in_{routetrip}" data-door="{door}">' +
        //            '<td class="text-center" class="{inbackground}">{sch_Arrive}</td>' +
        //            '<td class="text-center" class="{inbackground}">{arrived}</td>' +
        //            '<td class="text-center" class="{background}">{btnloadDoor}</td>' +
        //            '<td class="text-center">{leg_Origin}</td>' +
        //            '<td data-toggle="tooltip" title="{site_Name}">{site_Name}</td>' +
        //            '</tr>';

        //        function formatctsIncardtoprow(properties) {
        //            return $.extend(properties, {
        //                sch_Arrive: checkValue(properties.ScheduledTZ) ? formatTime(properties.ScheduledTZ) : "",
        //                arrived: checkValue(properties.ActualTZ) ? formatTime(properties.ActualTZ) : "",
        //                routetrip: properties.RouteID + properties.TripID,
        //                door: properties.hasOwnProperty("doorNumber") ? properties.doorNumber : "",
        //                route: properties.RouteID,
        //                trip: properties.TripID,
        //                inbackground: "", //GettimediffforInbound(properties.Scheduled, properties.Actual),
        //                leg_Origin: properties.LegOrigin,
        //                site_Name: properties.SiteName,
        //                btnloadDoor: Load_btn_door(properties)
        //            });
        //        }
        //        var findtrdataid = $ctsIncardtop_Table_Body.find('tr[data-id=in_' + CTSInboundData.RouteID + CTSInboundData.TripID + ']');
        //        if (findtrdataid.length > 0) {
        //            if (CTSInboundData.CTS_Remove) {
        //                $ctsIncardtop_Table_Body.find('tr[data-id=in_' + CTSInboundData.RouteID + CTSInboundData.TripID + ']').remove();
        //            }
        //            else {
        //                $ctsIncardtop_Table_Body.find('tr[data-id=in_' + CTSInboundData.RouteID + CTSInboundData.TripID + ']').replaceWith($ctsIncardtop_row_template.supplant(formatctsIncardtoprow(CTSInboundData)));
        //            }
        //        }
        //        else {
        //            if (!CTSInboundData.CTS_Remove) {
        //                $ctsIncardtop_Table_Body.append($ctsIncardtop_row_template.supplant(formatctsIncardtoprow(CTSInboundData)));
        //            }
        //        }
        //    } catch (e) {
        //        console.log(e)
        //    }
        //},
        //updateCTSOutoundStatus: async (CTSOutoundData) => {
        //    try {
        //        $ctsOutcardtop_Table = $('table[id=ctsouttoptable]');
        //        $ctsOutcardtop_Table_Body = $ctsOutcardtop_Table.find('tbody');
        //        $ctsOutcardtop_row_template = '<tr data-id=out_{routetrip} data-door={door} >' +
        //            '<td class="text-center" class="{inbackground}">{sch_Arrive}</td>' +
        //            '<td class="text-center" class="{inbackground}">{arrived}</td>' +
        //            '<td class="text-center" class="{background}">{btnloadDoor}</td>' +
        //            '<td class="text-center">{leg_Origin}</td>' +
        //            '<td data-toggle="tooltip" title="{site_Name}">{site_Name}</td>' +
        //            '</tr>';

        //        function formatctsOutcardtoprow(properties) {
        //            return $.extend(properties, {
        //                sch_Arrive: checkValue(properties.ScheduledTZ) ? moment(properties.ScheduledTZ).format("HH:mm") : "",
        //                arrived: checkValue(properties.ActualTZ) ? moment(properties.ActualTZ).format("HH:mm") : "",
        //                routetrip: properties.RouteID + properties.TripID,
        //                door: properties.hasOwnProperty("doorNumber") ? properties.doorNumber : "",
        //                route: properties.RouteID,
        //                trip: properties.TripID,
        //                firstlegDest: checkValue(properties.FirstLegDest) ? properties.FirstLegDest : "",
        //                firstlegSite: checkValue(properties.FirstLegSite) ? properties.FirstLegSite : "",
        //                btnloadDoor: Load_btn_door(properties)
        //            });
        //        }
        //        var findtrdataid = $ctsOutcardtop_Table_Body.find('tr[data-id=out_' + CTSOutoundData.RouteID + CTSOutoundData.TripID + ']');
        //        if (findtrdataid.length > 0) {
        //            if (CTSOutoundData.CTS_Remove) {
        //                $ctsOutcardtop_Table_Body.find('tr[data-id=out_' + CTSOutoundData.RouteID + CTSOutoundData.TripID + ']').remove();
        //            }
        //            else {
        //                $ctsOutcardtop_Table_Body.find('tr[data-id=out_' + CTSOutoundData.RouteID + CTSOutoundData.TripID + ']').replaceWith($ctsOutcardtop_row_template.supplant(formatctsOutcardtoprow(CTSOutoundData)));
        //            }
        //        }
        //        else {
        //            if (!CTSOutoundData.CTS_Remove) {
        //                $ctsOutcardtop_Table_Body.append($ctsOutcardtop_row_template.supplant(formatctsOutcardtoprow(CTSOutoundData)));
        //            }
        //        }
        //    } catch (e) {
        //        console.log(e);
        //    }
        //},
        
    });
    //setup map
    map = L.map('map', {
        crs: L.CRS.Simple,
        renderer: L.canvas({ padding: 0.5 }),
        preferCanvas: true,
        markerZoomAnimation: false,
        minZoom: 1,
        maxZoom: 5,
        zoomControl: false,
        measureControl: true,
        tap: false,
        layers: [polygonMachine, vehicles, agvLocations, container, stagingAreas, circleMarker, dockDoors, locatorMarker]
    });
    var overlayMaps = {
        "Vehicles Tag": vehicles,
        "SELS Tag": circleMarker,
        "Locators": locatorMarker,
        "AGV Locations": agvLocations,
        "MPE Work Areas": polygonMachine,
        "Dock Doors": dockDoors,
        "Staging Areas": stagingAreas,
        "Viewports": viewPortsAreas,
        "EBR Areas": ebrAreas,
        "Exit Areas": exitAreas,
        "Work Area": walkwayAreas,
        "Polygon Holes": polyholesAreas
    };

    /**zoom function ingress or decrees the size of the icon */
    //map.on('zoom', function () {
    //    var currentZoom = map.getZoom();
    //    if (currentZoom > 12) {
    //        markerslayer.eachLayer(function (layer) {
    //            if (layer.feature.properties.num < 0.5)
    //                return layer.setIcon(ar_icon_1);
    //            else if (feature.properties.num < 1.0)
    //                return layer.setIcon(ar_icon_2);
    //        });
    //    } else {
    //        markerslayer.eachLayer(function (layer) {
    //            if (layer.feature.properties.num < 0.5)
    //                return layer.setIcon(ar_icon_1_double_size);
    //            else if (feature.properties.num < 1.0)
    //                return layer.setIcon(ar_icon_2_double_size);
    //        });
    //    }
    //});

    var timedisplay = L.Control.extend({
        options: {
            position: 'topright'
        },
        onAdd: function () {
            var Domcntainer = L.DomUtil.create('input');
            Domcntainer.id = "localTime";
            Domcntainer.type = "button";
            Domcntainer.className = "btn btn-secondary btn-sm";
            return Domcntainer;
        }
    });
    map.addControl(new timedisplay());
  

   
    map.addControl(sidebar);
    sidebar.on('content', function (ev) {
        sidebar.options.autopan = false;
        switch (ev.id) {
            case 'autopan':
                break;
            case 'setting':
                Edit_AppSetting("app_settingtable");
                break;
            case 'reports':
                GetUserInfo();
                break;
            case 'userprofile':
                GetUserProfile();
                break;
            case 'agvnotificationinfo':
                GetAGVnotificationinfoInfo("agvnotificationtable", "vehicle");
                break;
            case 'ctsnotificationinfo':
                GetCTSnotificationinfoInfo("ctsnotificationtable", "cts");
                break;
            case 'notificationsetup':
                LoadNotificationsetup({}, "notificationsetuptable");
                break;
            default:
                sidebar.options.autopan = false;
                break;
        }
    });

    $('#fotf-sidebar-close').on('click', function () {
        // close the sidebar
        sidebar.close();
    });

    //Sidebar BS Tooltips
    $(function () {
        $('[data-toggle="tooltip"]').tooltip()
    })

    // only allow one popover to be open at once
    // bind to body so that this applies to any dynamically generated popovers, such as #faqToggle / #legendToggle
    $('body').popover({
        selector: '[data-toggle=popover]',
        trigger: "click"
    }).on("show.bs.popover", function (e) {
        // hide all other popovers
        $('[data-toggle=popover]').not(e.target).popover('hide');
        $('#layersContent').hide();
    });
    //Hide sidebar
    $('[role=tablist]').click(function (e) {
        $('[data-toggle=popover]').popover('hide');
        $('#layersContent').hide();
    });

    //add View Ports
    L.easyButton({
        position: 'bottomright',
        states: [{
            stateName: 'viewport',
            icon: '<div id="viewportsToggle" data-toggle="popover"><i class="pi-iconViewport align-self-center" title="Viewports"></i></div>'
        }]
    }).addTo(map);

    $('#viewportsToggle').popover({
        html: true,
        trigger: 'click',
        content: $('#viewportsContent'),
        title: 'Viewports'
    });
    $('#viewportsToggle').on('click', function () {
        // close the sidebar
        sidebar.close();
    });
    ////select zone
    //$('#zoneselect').change(function (e) {
    //    $('input[type=checkbox][name=followvehicle]').prop('checked', false).change();
    //    var selcValue = this.value;
    //    LoadMachineDetails(selcValue);

    //});
    // Add layers button - Originally on top right - REMOVE IF No longer use
    //L.control.layers(null, overlayMaps, { position: 'bottomright'}).addTo(map);

    // Add Layer Popover - Proposed
    L.control.layers(null, overlayMaps, { position: 'bottomright', collapsed: false }).addTo(map);
    $('.leaflet-control-layers').addClass('layerPopover');
    $('div .layerPopover').attr('id', 'layersContent');
    $('#layersContent').prepend('<div class="layersArrow"></div>');
    $('.leaflet-control-layers').hide();

    // Add Layer Control Button
    L.easyButton({
        position: 'bottomright',
        states: [{
            stateName: 'layer',
            icon: '<div id="layersToggle" data-toggle="layerPopover"><i class="pi-iconLayer align-self-center" title="Layer Controls"></i></div>'
        }]
    }).addTo(map);

    $('#layersToggle').on('click', function () {
        //Toggle layer Popover
        $('#layersContent').toggle();
        // close the sidebar
        sidebar.close();
        // close other popover
        $('[data-toggle=popover]').popover('hide');
    });

    //add zoom button
    new L.Control.Zoom({ position: 'bottomright' }).addTo(map);

    //add connection status
    var conntoggle = L.easyButton({
        position: 'topright',
        states: [
            {
                stateName: 'conn-off',
                icon: '<i class="conectionOff pi-iconPlugOutline" width="18" height="18" title="Connection OFF"></i>',
                title: 'Connection Off'
            },
            {
                stateName: 'conn-on',
                icon: '<i class="conectionOn pi-iconPlugOutline" width="18" height="18" title="Connection On"></i>',
                title: 'Connection On'
            },
            {
                stateName: 'conn-low',
                icon: '<i class="conectionSlow pi-iconPlugOutline" width="18" height="18" title="Connection Slow"></i>',
                title: 'Connection On'
            }]
    });
    conntoggle.addTo(map);

    //add legend
    L.easyButton({
        position: 'bottomright',
        states: [{
            stateName: 'legend',
            icon: '<div id="legendToggle" data-toggle="popover" title="Legend"><i class="pi-iconLegend align-self-center"></i></div>',
            title: 'Legend'
        }]
    }).addTo(map);

    $('#legendToggle').popover({
        html: true,
        trigger: 'click',
        content: $('#legendContent'),
    });
    $('#legendToggle').on('click', function () {
        // close the sidebar
        sidebar.close();
    });

    //add FAQ
    L.easyButton({
        position: 'bottomright',
        states: [{
            stateName: 'faq',
            icon: '<div id="faqToggle" data-toggle="popover" title="Frequently Asked Questions" ><i class="pi-iconFAQ align-self-center"></i></div> ',
            title: 'faq'
        }]
    }).addTo(map);

    $('#faqToggle').popover({
        html: true,
        trigger: 'click',
        content: $('#faqContent'),
    });
    $('#faqToggle').on('click', function () {
        // close the sidebar
        sidebar.close();
    });

    //Full-screen button only for Chrome
    if (window.chrome) {
        var fullscreentoggle = L.easyButton({
            position: 'bottomright',
            leafletClasses: true,
            states: [{
                stateName: 'enter-fullscreen',
                icon: '<i class="pi-iconFullScreen" title="Enter Full Screen"></i>',
                title: 'Enter Full Screen',
                onClick: function (control) {
                    if (document.documentElement.requestFullscreen) {
                        document.documentElement.requestFullscreen();
                    } else if (document.documentElement.msRequestFullscreen) {
                        document.documentElement.msRequestFullscreen();
                    } else if (document.documentElement.mozRequestFullScreen) {
                        document.documentElement.mozRequestFullScreen();
                    } else if (document.documentElement.webkitRequestFullscreen) {
                        document.documentElement.webkitRequestFullscreen(Element.ALLOW_KEYBOARD_INPUT);
                    }
                    control.state('exit-fullscreen');
                }
            }, {
                icon: '<i class="pi-iconFullScreenExit" title="Exit Full Screen"></i>',
                stateName: 'exit-fullscreen',
                title: 'Exit Full Screen',
                onClick: function (control) {
                    if (document.exitFullscreen) {
                        document.exitFullscreen();
                    } else if (document.msExitFullscreen) {
                        document.msExitFullscreen();
                    } else if (document.mozCancelFullScreen) {
                        document.mozCancelFullScreen();
                    } else if (document.webkitExitFullscreen) {
                        document.webkitExitFullscreen();
                    }
                    control.state('enter-fullscreen');
                }
            }]
        });
        fullscreentoggle.addTo(map);
    }

    // Search Tag Name
    $('input[id=inputsearchbtn').keyup(function () {
        startSearch(this.value)
    }).keydown(function (event) {
        if (event.which == 13) {
            event.preventDefault();
        }
    });
    async function startSearch(sc) {
        $search_Table = $('table[id=tagresulttable]');
        $search_Table_Body = $search_Table.find('tbody');
        $search_Table_Body.empty();
        $search_row_template = '<tr data-id="{layer_id}" data-tag="{tag_id}">' +
            '<td>{tag_name}</td>' +
            '<td class="align-middle text-center">' +
            '<button class="btn btn-light btn-sm mx-1 pi-iconEdit" name="tagedit"></button>' +

            '</td>' +
            '</tr>'
            ;
        function formatsearchrow(properties, layer_id) {
            return $.extend(properties, {
                layer_id: layer_id,
                tag_id: properties.id,
                tag_name: checkValue(properties.name) ? properties.name : properties.id
            })
        }
        if (checkValue(sc)) {
            var search = new RegExp(sc, 'i');
            if (map.hasOwnProperty("_layers")) {
                $.map(map._layers, function (layer, i) {
                    if (layer.hasOwnProperty("feature")) {
                        if (/(person)|(Vehicle$)/i.test(layer.feature.properties.Tag_Type)) {
                            if (search.test(layer.feature.properties.name) || search.test(layer.feature.properties.id)) {
                                $search_Table_Body.append($search_row_template.supplant(formatsearchrow(layer.feature.properties, layer._leaflet_id)));
                                if (layer.hasOwnProperty("options")) {
                                    if (layer.options.hasOwnProperty("fillColor")) {
                                        if (!/red/i.test(layer.options.fillColor)) {
                                            layer.setStyle({
                                                fillColor: '#dc3545', //'red',
                                            });
                                        }
                                    }
                                }
                                if (layer.hasOwnProperty("_tooltip")) {
                                    if (layer._tooltip.hasOwnProperty("_container")) {
                                        if (layer._tooltip._container.classList.contains('tooltip-hidden')) {
                                            layer._tooltip._container.classList.remove('tooltip-hidden');
                                        }
                                        if (!layer._tooltip._container.classList.contains('searchflash')) {
                                            layer._tooltip._container.classList.add('searchflash');
                                        }
                                    }
                                }
                            }
                            else {
                                if (!layer.feature.properties.hasOwnProperty("tagVisible")) {
                                    if (!layer._tooltip._container.classList.contains('tooltip-hidden')) {
                                        layer._tooltip._container.classList.add('tooltip-hidden');
                                    }
                                }
                                if (layer.hasOwnProperty("_tooltip")) {
                                    if (layer._tooltip.hasOwnProperty("_container")) {
                                        if (layer._tooltip._container.classList.contains('searchflash')) {
                                            layer._tooltip._container.classList.remove('searchflash');
                                        }
                                    }
                                }
                            }
                        }
                    }
                });
            }
            $('button[name=tagedit]').on('click', function () {
                var td = $(this);
                var tr = $(td).closest('tr'),
                    layer_id = tr.attr('data-id');

                EditUserInfo(layer_id);
            });
            $('button[name=tagdelete]').on('click', function () {
                var td = $(this);
                var tr = $(td).closest('tr'),
                    layer_id = tr.attr('data-id');

                DeleteUserInfo(layer_id);
            });
        }
        else {
            if (map.hasOwnProperty("_layers")) {
                $.map(map._layers, function (layer, i) {
                    if (layer.hasOwnProperty("feature")) {
                        if (/(person)|(Vehicle$)/i.test(layer.feature.properties.Tag_Type)) {
                            if (layer.hasOwnProperty("_tooltip")) {
                                if (layer._tooltip.hasOwnProperty("_container")) {
                                    if (layer._tooltip._container.classList.contains('searchflash')) {
                                        layer._tooltip._container.classList.remove('searchflash');
                                    }
                                    if (!layer.feature.properties.hasOwnProperty("tagVisible")) {
                                        if (!layer._tooltip._container.classList.contains('tooltip-hidden')) {
                                            layer._tooltip._container.classList.add('tooltip-hidden');
                                        }
                                    }
                                }
                            }
                        }
                    }
                });
            }
        }
    }

    //machine chart
    //var machinechart = document.getElementById("machinePerformancechart").getContext("2d");
    //var machinechartdata = new Chart(machinechart, { type: 'bar'});

    /******Connection status start******/
    async function init_Connection() {
        fotfmanager.server.getUserProfile().done(function (User_profile) {
            User = User_profile;
    
            if (/^Admin/i.test(User.Role)) {
                sidebar.addPanel({
                    id: 'connections',
                    tab: '<i class="pi-iconDiagramOutline"></i>',
                    position: 'top',
                    pane: '<div class="btn-toolbar" role="toolbar" id="connection_div">' +
                        '<div id="div_agvnotification" class="container-fluid">' +
                        '<h4>API Settings</h4>' +
                        '<button type="button" class="btn btn-primary float-left mb-2" name="addconnection">Add</button>' +
                        '<div class="card w-100 bg-white mt-2 pb-1">' +
                        '<div class="card-body">' +
                        '<div class="table-responsive">' +
                        '<table class="table table-sm table-hover table-condensed mb-1" id="connectiontable" style="border-collapse:collapse;">' +
                        '<thead class="thead-dark">' +
                        '<tr>' +
                        '<th class="row-connection-name">Name</th><th class="row-connection-type">Message Type</th><th class="row-connection-status">Status</th><th class="row-connection-action">Action</th>' +
                        '</tr>' +
                        '</thead>' +
                        '<tbody></tbody>' +
                        '</table>' +
                        '</div>' +
                        '</div>' +
                        '</div >' +
                        '</div></div>'
                });
                init_connection();
               // init_geometry_editing();
                $('button[name=addconnection]').off().on('click', function () {
                    /* close the sidebar */
                    sidebar.close();
                    Add_Connection();
                });
                $('button[name=machineinfoedit]').css('display', 'block');
            
            }

            $('button[name=addnotificationsetup]').off().on('click', function () {
                /* close the sidebar */
                sidebar.close();
                Notificationsetup({},"notificationsetuptable");
            });
            if (/^Admin/i.test(User.Role)) {
                sidebar.addPanel({
                    id: 'setting',
                    tab: '<i class="pi-iconGearFill"></i>',
                    position: 'bottom',
                    pane: '<div class="btn-toolbar" role="toolbar" id="app_setting">' +
                        '<div id="div_app_settingtable" class="container-fluid">' +
                        '<div class="card w-100">' +
                        '<div class="card-body">' +
                        '<div class="table-responsive fixedHeader" style="max-height: calc(100vh - 100px); ">' +
                        '<table class="table table-sm table-hover table-condensed" id="app_settingtable" style="border-collapse:collapse;">' +
                        '<thead class="thead-dark">' +
                        '<tr>' +
                        '<th class="row-name">Name</th><th class="row-value">Value</th><th class="row-action">Action</th>' +
                        '</tr>' +
                        '</thead>' +
                        '<tbody></tbody>' +
                        '</table>' +
                        '</div>' +
                        '</div>' +
                        '</div >' +
                        '</div></div>'
                });
            }
            if (/(^PMCCUser$)/i.test(User.UserId)) {
                //add QRCode
                var QRCodedisplay = L.Control.extend({
                    options: {
                        position: 'topright'
                    },
                    onAdd: function () {
                        var Domcntainer = L.DomUtil.create('div');
                        Domcntainer.id = "qrcodeUrl";
                        return Domcntainer;
                    }
                });
                map.addControl(new QRCodedisplay());

                var qrcode = new QRCode("qrcodeUrl", {
                    text: window.location.href,
                    width: 128,
                    height: 128,
                    colorDark: "#000000",
                    colorLight: "#ffffff",
                    correctLevel: QRCode.CorrectLevel.H
                });
            }
        });
    }

    /******Connection status end******/
    /****add map items start */
    async function init_Map() {
        $.connection.FOTFManager.server.getMap().done(function (MapData) {
            try {
                if (MapData.length === 1) {
                    MapData = MapData[0];
                    if (!$.isEmptyObject(MapData)) {
                        if (MapData.hasOwnProperty("Facility_TimeZone")) {
                            if (checkValue(MapData.Facility_TimeZone)) {
                                timezone = { Facility_TimeZone: MapData.Facility_TimeZone }
                            }
                        }
                        if (MapData.hasOwnProperty("Software_Version")) {
                            if (checkValue(MapData.Software_Version)) {
                                map.attributionControl.setPrefix("USPS Factory of the Future (" + MapData.Software_Version + ")");
                            }
                        }
                        if (MapData.hasOwnProperty("Facility_Name")) {
                            if (checkValue(MapData.Facility_Name)) {
                                $('#fotf-site-facility-name').append(MapData.Facility_Name);
                                map.attributionControl.addAttribution(MapData.Facility_Name);
                                $(document).prop('title', MapData.Facility_Name + ' FOTF');
                            }
                        }

                        if (MapData.hasOwnProperty("Environment")) {
                            //Environment Status Controls
                            if (/(DEV|SIT|CAT)/i.test(MapData.Environment)) {
                                var Environment = L.Control.extend({
                                    options: {
                                        position: 'topright'
                                    },
                                    onAdd: function () {
                                        var Domcntainer = L.DomUtil.create('input');
                                        Domcntainer.type = "button";
                                        Domcntainer.id = "environment";
                                        Domcntainer.className = getEnv(MapData.Environment);
                                        Domcntainer.value = MapData.Environment;
                                        return Domcntainer;
                                    }
                                });
                                map.addControl(new Environment());
                                function getEnv(env) {
                                    if (/CAT/i.test(env)) {
                                        return "btn btn-outline-primary btn-sm";
                                    }
                                    else if (/SIT/i.test(env)) {
                                        return "btn btn-outline-warning btn-sm";
                                    }
                                    else if (/DEV/i.test(env)) {
                                        return "btn btn-outline-danger btn-sm";
                                    }
                                }
                            }
                        }
                        //set new image
                        var img = new Image();
                        //load Base64 image
                        img.src = MapData.Base64Img;
                        //create he bound of the image.
                        var bounds = [[MapData.YMeter, MapData.XMeter], [MapData.HeightMeter + MapData.YMeter, MapData.WidthMeter + MapData.XMeter]];
                        //add the image to the map
                        L.imageOverlay(img.src, bounds).addTo(map);
                        //center image
                        map.setView([MapData.HeightMeter / 2, MapData.WidthMeter / 2], 1.5);

                    }
                }
                if ($.isEmptyObject(map)) {
                    $('div[id=map]').css('display', 'none');
                    $('<div/>', { class: 'jumbotron text-center' })
                        .append($('<h1/>', { class: 'display-4', text: "Map has not been Configured " }))
                        .append($('<p/>', { class: 'lead', text: 'Please Configure Map' }))
                        .append($('<hr/>', { class: 'my-4' }))
                        .append($('<div/>', { class: 'row' })
                            .append($('<div>', { class: 'col' })
                                .append($('<button/>', { class: 'btn btn-outline-success ', type: 'button', id: 'API_connection', text: 'Configure Map' })))

                        ).append($('<div/>', { class: 'row' })
                            .append($('<div>', { class: 'col text-center' })
                                .append($('<span/>', { class: 'text-info ', id: 'error_remove_server_connection' })))
                        ).appendTo('.body-content');
                }
            } catch (e) {
                $('div[id=map]').css('display', 'none');
                $('<div/>', { class: 'jumbotron text-center' })
                    .append($('<h1/>', { class: 'display-4', text: "Error loading Map" }))
                    .append($('<p/>', { class: 'lead', text: 'Erro:' + e }))
                    .append($('<hr/>', { class: 'my-4' }))
                    .appendTo('.body-content');
            }
        })
            .then(init_viewports())
            .then(init_machine())
            .then(init_dockdoor())
            .then(init_agvlocation())
            .then(init_zones())
            .then(init_arrive_depart_trips())
            .then(init_locators())

            .catch(
            function (err) {
                console.log(err.toString());
            });
    }
    // SelectizeJs Init for searching select boxes.
    var options = {
        create: false,
        sortField: "text",
    }
    $('.selectize-dropdown').click(function (e) {
        e.stopPropagation();        // To fix zone select scroll bug. May need to be revisited
    })
    $zoneSelect = $("#zoneselect").selectize(options);
    /**User  Modal***/
    function EditUserInfo(layerindex) {
        $('#modaluserHeader_ID').text('Edit User Info');

        if ($.isNumeric(layerindex)) {
            layerindex = parseInt(layerindex);
        }
        sidebar.close();
        try {
            if (map._layers[layerindex].hasOwnProperty("feature")) {
                if (map._layers[layerindex].feature.hasOwnProperty("properties")) {
                    $('input[type=text][name=employee_ein]').val(map._layers[layerindex].feature.properties.Employee_EIN);
                    $('input[type=text][name=employee_name]').val(map._layers[layerindex].feature.properties.Employee_Name);
                    $('input[type=text][name=tag_name]').val(map._layers[layerindex].feature.properties.name);
                    $('input[type=text][name=tag_id]').val(map._layers[layerindex].feature.properties.id);
                    $('input[type=text][name=employee_group]').val(map._layers[layerindex].feature.properties.Employee_Group_type);
                    $('input[type=text][name=employee_pl]').val(map._layers[layerindex].feature.properties.Employee_PL);
                    $('input[type=text][name=employee_role]').val(map._layers[layerindex].feature.properties.Employee_Role);
                    $('button[id=usertagsubmitBtn]').off().on('click', function () {
                        try {
                            $('button[id=usertagsubmitBtn]').prop('disabled', true);
                            var jsonObject = {};
                            $('input[type=text][name=employee_ein]').val() !== map._layers[layerindex].feature.properties.Employee_EIN ? jsonObject.Employee_EIN = $('input[type=text][name=employee_ein]').val() : "";
                            $('input[type=text][name=employee_name]').val() !== map._layers[layerindex].feature.properties.Employee_Name ? jsonObject.Employee_Name = $('input[type=text][name=employee_name]').val() : "";
                            $('input[type=text][name=employee_group]').val() !== map._layers[layerindex].feature.properties.Employee_Group_type ? jsonObject.Employee_Group_type = $('input[type=text][name=employee_group]').val() : "";
                            $('input[type=text][name=employee_pl]').val() !== map._layers[layerindex].feature.properties.Employee_PL ? jsonObject.Employee_PL = $('input[type=text][name=employee_pl]').val() : "";
                            $('input[type=text][name=employee_role]').text() !== map._layers[layerindex].feature.properties.Employee_Role ? jsonObject.Employee_Role = $('input[type=text][name=employee_role]').text() : "";
                            if (!$.isEmptyObject(jsonObject)) {
                                jsonObject.id = map._layers[layerindex].feature.properties.id;
                                $.connection.FOTFManager.server.editTagInfo(JSON.stringify(jsonObject)).done(function (Data) {
                                    if (Data.hasOwnProperty(ERROR_MESSAGE)) {
                                        $('span[id=error_usertagsubmitBtn]').text(Data.ERROR_MESSAGE);
                                    }
                                    if (Data[0].hasOwnProperty("MESSAGE_TYPE")) {
                                        $('span[id=error_usertagsubmitBtn]').text(Data.MESSAGE_TYPE);
                                        setTimeout(function () { $("#UserTag_Modal").modal('hide'); }, 3000);
                                    }
                                });
                            }
                            else {
                                $('span[id=error_usertagsubmitBtn]').text("No Tag Data has been Updated");
                                setTimeout(function () { $("#UserTag_Modal").modal('hide'); }, 3000);
                            }
                        } catch (e) {
                            $('span[id=error_usertagsubmitBtn]').text(e);
                        }
                    });

                    $('#UserTag_Modal').modal();
                }
            }
        } catch (e) {
            console.log(e);
        }
    }

    // Start the connection
    $.connection.hub.qs = { 'page_type': "FOTF".toUpperCase() };
    $.connection.hub.start()
        .then(init_Connection)
        .then(init_Map)
        .done(function () {
            conntoggle.state('conn-on');
        }).catch(
            function (err) {
                console.log(err.toString());
            });
    // Raised when the connection state changes. Provides the old state and the new state (Connecting, Connected, Reconnecting, or Disconnected).
    $.connection.hub.stateChanged(function (state) {
        switch (state.newState) {
            case 1:
                conntoggle.state('conn-on');
                break;
            case 2:
                conntoggle.state('conn-low');
                break;
            case 0:
                conntoggle.state('conn-low');
                break;
            case 4:
                conntoggle.state('conn-off');
                break;
            default:
        }
    });
    //handling Disconnect
    $.connection.hub.disconnected(function () {
        connecttimer = setTimeout(function () {
            if (connectattp > 10) {
                clearTimeout(connecttimer);
            }
            connectattp + 1;
            conntoggle.state('conn-off');
            $.connection.hub.start().done(function () {
                console.log("Connected time" + new Date($.now()));
            }).catch(function (err) {
                console.log(err.toString());
            });
        }, 10000); // Restart connection after 10 seconds.
    });
    //Raised when the underlying transport has reconnected.
    $.connection.hub.reconnecting(function () {
        conntoggle.state('conn-low');
        clearTimeout(connecttimer);
    });
    //Raised when the client detects a slow or frequently dropping connection
    $.connection.hub.connectionSlow(function () {
        conntoggle.state('conn-low');
    });
    // current zone staff
    //TODO: look in to this more.
    async function zonecurrentStaff() {
        try {
            //clear Old tags 
            if (circleMarker.hasOwnProperty("_layers")) {
                $.map(circleMarker._layers, (layer) => {
                    if (layer.hasOwnProperty("feature")) {
                        if (layer.feature.properties.hasOwnProperty("Tag_Type")) {
                            if (/person/i.test(layer.feature.properties.Tag_Type)) {
                                if (layer.feature.properties.tagVisible === true) {
                                    if (layer.feature.properties.hasOwnProperty("positionTS")) {
                                        var startTime = moment(layer.feature.properties.positionTS);
                                        var endTime = moment();
                                        var diffmillsec = endTime.diff(startTime, "milliseconds");

                                        if (diffmillsec > layer.feature.properties.tagVisibleMils) {
                                            layer.feature.properties.tagVisibleMils = diffmillsec;
                                        }
                                    }
                                    if (layer.feature.properties.tagVisibleMils > 80000) {
                                        layer.feature.properties.tagVisible = false;
                                        //this to hide tooltip
                                        if (layer.hasOwnProperty("_tooltip")) {
                                            if (layer._tooltip.hasOwnProperty("_container")) {
                                                if (!layer._tooltip._container.classList.contains('tooltip-hidden')) {
                                                    layer._tooltip._container.classList.add('tooltip-hidden');
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                })
            };
            // Machine zone
            if (polygonMachine.hasOwnProperty("_layers")) {
                $.map(polygonMachine._layers, function (Machinelayer, i) {
                    var MachineCurrentStaff = [];
                    if (circleMarker.hasOwnProperty("_layers")) {
                        $.map(circleMarker._layers, function (layer, i) {
                            if (layer.hasOwnProperty("feature")) {
                                if (/person/i.test(layer.feature.properties.Tag_Type)) {
                                    if (layer.feature.properties.hasOwnProperty("zones")) {
                                        $.map(layer.feature.properties.zones, function (p_zone) {
                                            if (p_zone.id == Machinelayer.feature.properties.id) {
                                                if (layer.hasOwnProperty("_tooltip")) {
                                                    if (layer._tooltip.hasOwnProperty("_container")) {
                                                        if (!layer._tooltip._container.classList.contains('tooltip-hidden')) {
                                                            MachineCurrentStaff.push({
                                                                name: checkValue(layer.feature.properties.craftName) ? layer.feature.properties.craftName : layer.feature.properties.id,
                                                                nameId: checkValue(layer.feature.properties.id) ? layer.feature.properties.id : layer.feature.properties.id,
                                                                id: layer.feature.properties.id
                                                            })
                                                        }
                                                    }
                                                }
                                            }
                                        });
                                    }
                                }
                            }
                        });
                    }

                    if (Machinelayer.hasOwnProperty("_tooltip")) {
                        if (MachineCurrentStaff.length !== Machinelayer.feature.properties.CurrentStaff) {
                            Machinelayer.setTooltipContent(Machinelayer.feature.properties.name + "<br/>" + "Staffing: " + MachineCurrentStaff.length);
                            Machinelayer.feature.properties.CurrentStaff = MachineCurrentStaff.length;
                            if ($('select[id=zoneselect] option:selected').val() === Machinelayer.feature.properties.id) {
                                var p2pdata = Machinelayer.feature.properties.hasOwnProperty("P2PData") ? Machinelayer.feature.properties.P2PData : "";
                                GetPeopleInZone(Machinelayer.feature.properties.id, p2pdata, MachineCurrentStaff);
                            }
                        }
                    }
                });
            }
            // other zone
            if (stagingAreas.hasOwnProperty("_layers")) {
                $.map(stagingAreas._layers, function (stagelayer, i) {
                    var CurrentStaff = [];
                    if (circleMarker.hasOwnProperty("_layers")) {
                        $.map(circleMarker._layers, function (layer, i) {
                            if (layer.hasOwnProperty("feature")) {
                                if (/person/i.test(layer.feature.properties.Tag_Type)) {
                                    if (layer.feature.properties.hasOwnProperty("zones")) {
                                        $.map(layer.feature.properties.zones, function (p_zone) {
                                            if (p_zone.id == stagelayer.feature.properties.id) {
                                                if (layer.hasOwnProperty("_tooltip")) {
                                                    if (layer._tooltip.hasOwnProperty("_container")) {
                                                        if (!layer._tooltip._container.classList.contains('tooltip-hidden')) {
                                                            CurrentStaff.push({
                                                                name: checkValue(layer.feature.properties.craftName) ? layer.feature.properties.craftName : layer.feature.properties.id,
                                                                nameId: checkValue(layer.feature.properties.id) ? layer.feature.properties.id : layer.feature.properties.id,
                                                                id: layer.feature.properties.id
                                                            })
                                                            //CurrentStaff.push({
                                                            //    id: layer.feature.properties.id
                                                            //})
                                                        }
                                                    }
                                                }
                                            }
                                        });
                                    }
                                }
                            }
                        });
                    }

                    if (stagelayer.hasOwnProperty("_tooltip")) {
                        if (CurrentStaff.length !== stagelayer.feature.properties.CurrentStaff) {
                            stagelayer.setTooltipContent(stagelayer.feature.properties.name + "<br/>" + "Staffing: " + CurrentStaff.length);
                            stagelayer.feature.properties.CurrentStaff = CurrentStaff.length;
                            if (stagingAreas.hasOwnProperty("feature")) {
                                if ($('select[id=zoneselect] option:selected').val() === stagingAreas.feature.properties.id) {
                                    var p2pdata = stagelayer.feature.properties.hasOwnProperty("P2PData") ? stagelayer.feature.properties.P2PData : "";
                                    GetPeopleInZone(stagelayer.feature.properties.idp2pdata, CurrentStaff);
                                }
                            }
                        }
                    }
                });
            }
        } catch (e) {
            console.log(e);
        }
    }
    async function GetUserProfile() {
        $.connection.FOTFManager.server.getADUserProfile().done(function (User) {
            $('#userfullname').text(User.FirstName + ' ' + User.SurName);
            $('#useremail').text(User.EmailAddress);
            $('#userphone').text(User.Phone);
            $('#usertitel').text(User.Role);

        });
    }
    async function LoadtagDetails(tagid) {
        try {
            if (map.hasOwnProperty("_layers")) {
                $.map(map._layers, function (layer, i) {
                    if (layer.hasOwnProperty("feature")) {
                        if (layer.feature.properties.id === tagid) {
                            map.setView(layer.getLatLng(), 3);
                            sidebar.open('home');
                            LoadVehicleTable(layer.feature.properties, 'vehicletable');
                            return false;
                        }
                    }
                });
            }
        } catch (e) {
            console.log(e);
        }
    }
    async function sortTable(table, order) {
        var asc = order === 'asc',
            tbody = table.find('tbody');

        tbody.find('tr').sort(function (a, b) {
            if (asc) {
                return $('td:first', a).text().localeCompare($('td:first', b).text());
            } else {
                return $('td:first', b).text().localeCompare($('td:first', a).text());
            }
        }).appendTo(tbody);
    }
    function Load_btn_details(properties) {
        if (properties.Closed > 0) {
            return '<button class="btn btn-outline-info btn-sm btn-block px-1 ctsdetails">' + properties.LoadPercent + '%</button>';
        }
        else {
            return properties.LoadPercent + '%';
        }
    }
    function Load_btn_door(properties) {
        if (properties.hasOwnProperty("doorNumber")) {
            if (checkValue(properties.doorNumber)) {
                return '<button class="btn btn-outline-info btn-sm btn-block px-1 doordetails" data-door="' + properties.doorNumber + '">' + properties.doorNumber + '</button>';
            } else {
                return '';
            }
        }
        else if (properties.hasOwnProperty("Door")) {
            if (checkValue(properties.Door)) {
                return '<button class="btn btn-outline-info btn-sm btn-block px-1 doordetails" data-door="' + properties.Door + '">' + properties.Door + '</button>';
            }
            else {
                return '';
            }
        }
        else {
            return '';
        }
    }
    $(document).on('click', '.ctsdetails', function () {
        var td = $(this);
        var tr = $(td).closest('tr'),
            route = tr.attr('data-route'),
            trip = tr.attr('data-trip');
        $('div[id=CTS_Details_Modal]').modal();
        $('#ctsdetailsmodalHeader').text('Retrieving Data from CTS Please Wait, The data shall be loaded upon retrieval');
        LoadCTSDetails(tr, route, trip, "ctsdetailstable");
    });
    $(document).on('click', '.containerdetails', function () {
        var td = $(this);
        var doorid = td.attr('data-doorid');
        var placardid = td.attr('data-placardid');
        if (checkValue(doorid)) {
            LoadContainerDetails(doorid, placardid);
        }
    });
  
    $(document).on('click', '.tagdetails', function (e) {
        var td = $(this);
        var tagid = td.attr('data-tag');
        if (checkValue(tagid)) {
            LoadtagDetails(tagid);
        }
    });
    $(document).on('click', '.viewportszones', function () {
        try {
            $('input[type=checkbox][name=followvehicle]').prop('checked', false).change();
            var selcValue = this.id;
            if (viewPortsAreas.hasOwnProperty("_layers")) {
                $.map(viewPortsAreas._layers, function (layer, i) {
                    if (layer.hasOwnProperty("feature")) {
                        if (layer.feature.properties.id === selcValue) {
                            var Center = new L.latLng(
                                (layer._bounds._southWest.lat + layer._bounds._northEast.lat) / 2,
                                (layer._bounds._southWest.lng + layer._bounds._northEast.lng) / 2);
                            map.setView(Center, 3);
                            Loadtable(layer.feature.properties.id, 'viewportstable');
                            return false;
                        }
                    }
                });
            }
        } catch (e) {
            console.log(e);
        }
    });
    $('#fotf-sidebar-close').on('click', function () {
        /* close the sidebar */
        sidebar.close();
    });
    $(document).on('click', '.connectionedit', function () {
        var td = $(this);
        var tr = $(td).closest('tr'),
            id = tr.attr('data-id');
        Edit_Connection(id);
    });
    $(document).on('click', '.connectiondelete', function () {
        var td = $(this);
        var tr = $(td).closest('tr'),
            id = tr.attr('data-id');
        Remove_Connection(id);
    });
    /*****CTS End******/
   
    function GetChartData(machine) {
        machinechartdata.destroy();
        machinechartdata = new Chart(machinechart, {
            type: 'bar',
            data: {
                labels: new function () {
                    var headers = [];
                    for (var i = 0; i <= 23; i++) {
                        headers.push(pad(i, 2) + ':00');
                    }
                    return headers;
                },
                datasets: [
                    {
                        data: [478, 267, 734, 784, 433, 259]
                    }
                ]
            },
            options: {
                plugins: {
                    legend: {
                        display: false,
                    }
                },
                scales: {
                    x: {
                        grid: {
                            display: false,
                        }
                    },
                    y: {
                        grid: {
                            display: false,
                        }
                    }
                },
                responsive: true,
                maintainAspectRatio: true,
                title: {
                    display: false
                },
                legend: {
                    labels: {
                        fontSize: 10
                    }
                },
                tooltips: {
                    callbacks: {
                        label: function (tooltipItem, data) {
                            return tooltipItem.yLabel.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
                        },
                    },
                    backgroundColor: 'rgba(64,64,64,0.9)',
                    position: 'nearest',
                    mode: 'label',
                    bodyFontStyle: 'normal'
                },
            }
        });
    }


  
    function Gettimediff(Scheduled) {
        try {
            var currenttime = moment().tz(Facility_TimeZone);  // 5am PDT
            console.debug("time = " + currenttime.format());
            var scheduledtime = moment(ScheduledTZ)
            // calculate total duration
            var timeduration = moment.duration(currenttime.diff(scheduledtime));
            var timerange = moment.preciseDiff(scheduledtime, currenttime, true);
            // duration in minutes
            var minutes = parseInt(timeduration.asMinutes()) % 60;
            if (minutes < 15) {
                return "redBg";
            }
            if (minutes > 15 && minutes < 30) {
                return "yellowBg";
            }
        } catch (e) {
            console.error(e.toString());
        }
    }
    function GettimediffforInbound(Scheduled, Actual) {
        var result = "";
        var actual = moment(Actual);  // 5am PDT
        var scheduled = moment(Scheduled);
        if (actual._isValid) {
            // calculate total duration
            var duration = moment.duration(actual.diff(scheduled));
            // duration in minutes
            var minutes = parseInt(duration.asMinutes()) % 60;
            if (minutes < 1) {
                result = "greenBg";
            }
        }
        var currenttime = moment().tz(Facility_TimeZone);
        if (!actual._isValid) {
            //  calculate current to scheduled duration
            var lateduration = moment.duration(currenttime.diff(scheduled));
            var lateminutes = parseInt(lateduration.asMinutes()) % 60;

            if (lateminutes <= 15) {
                result = "redBg";
            }
            if (lateminutes > 15 && lateminutes < 30) {
                result = "yellowBg";
            }
        }
        return result;
    }
    async function GetUserInfo() {
        try {


            // get people tags
            $.connection.FOTFManager.server.getPersonTagsList().done(function (Data) {
                try {
                    //staffing
                    var inBuilding = [];
                    var outBuilding = [];
                    if (Data.length > 0) {
                        new L.GeoJSON(Data, {
                            onEachFeature: function (feature, layer) {
                                var craftName_id = "";
                                var nameId_id = "";

                                if (checkValue(feature.properties.name)) {
                                    if (!/^n.a$/.test(feature.properties.name)) {
                                        var splitname = feature.properties.name.split('_');
                                        if (splitname.length > 1) {
                                            craftName_id = splitname[0];
                                            nameId_id = splitname[1];
                                        }
                                        else {
                                            craftName_id = feature.properties.name;
                                            nameId_id = "";
                                        }

                                        var currenttime = moment(feature.properties.Tag_TS);  // 5am PDT
                                        var lastpositiontime = moment(feature.properties.positionTS);
                                        if (currenttime._isValid && lastpositiontime._isValid) {
                                            // calculate total duration
                                            var duration = moment.duration(currenttime.diff(lastpositiontime));
                                            // duration
                                            if (duration._milliseconds > 1800000) {
                                                outBuilding.push({
                                                    craftName: craftName_id,
                                                    nameId: nameId_id,
                                                    id: feature.properties.id
                                                })
                                            }
                                            else {
                                                inBuilding.push({
                                                    craftName: craftName_id,
                                                    nameId: nameId_id,
                                                    id: feature.properties.id
                                                })
                                            }
                                        }
                                    }
                                }
                            }
                        });
                        var instaffCounts = [];
                        var suminBuildingCounts = {};
                        $.each(inBuilding, function (i, e) {
                            suminBuildingCounts[this.craftName] = (suminBuildingCounts[this.craftName] || 0) + 1;
                        });
                        $.each(suminBuildingCounts, function (index, value) {
                            instaffCounts.push({
                                name: index,
                                instaff: value
                            });
                        });
                        var outstaffCounts = [];
                        var outsumstaffCounts = {};
                        $.each(outBuilding, function (i, e) {
                            outsumstaffCounts[this.craftName] = (outsumstaffCounts[this.craftName] || 0) + 1;
                        });
                        $.each(outsumstaffCounts, function (index, value) {
                            outstaffCounts.push({
                                name: index,
                                outstaff: value
                            });
                        });
                        var staffing = $.extend(true, instaffCounts, outstaffCounts);
                        staffing.sort(SortByTagName);
                        staffing.push({
                            name: 'Total',
                            instaff: inBuilding.length,
                            outstaff: outBuilding.length
                        });

                        $userstop_Table = $('table[id=userstoptable]');
                        $userstop_Table_Body = $userstop_Table.find('tbody');
                        $userstop_Table_Body.empty();
                        $userstop_row_template = '<tr data-id={staffName}>' +
                            '<td>{staffName}</td>' +
                            '<td class="text-center">{instaff}</td>' +
                            '</tr>"';

                        function formatstafftoprow(properties) {
                            return $.extend(properties, {
                                staffName: properties.name,
                                staffId: properties.id,
                                instaff: properties.hasOwnProperty("instaff") ? properties.instaff : 0,
                                planstaff: properties.hasOwnProperty("planstaff") ? properties.planstaff : 0,
                                outstaff: properties.hasOwnProperty("outstaff") ? properties.outstaff : 0,
                            });
                        }
                        $.each(staffing, function () {
                            $userstop_Table_Body.append($userstop_row_template.supplant(formatstafftoprow(this)));
                        })
                    }
                } catch (e) {
                    console.log(e);
                }
            });

            //get undetected tags Data
            $.connection.FOTFManager.server.getUndetectedTagsList().done(function (undetectedtagsData) {
                try {
                    if (undetectedtagsData.length > 0) {
                        $compliance_Table = $('table[id=compliancetable]');
                        $compliance_Table_Body = $compliance_Table.find('tbody');
                        $('span[name=undetected_count]').text(undetectedtagsData.length);
                        $compliance_row_template = '<tr data-tag="{tag_id}">' +
                            '<td data-input="badge">{tag_name}</td>' +
                            '<td data-input="ldc" class="text-center">{ldc}</td>' +
                            '<td data-input="opcode" class="text-center">{op_code}</td>' +
                            '<td data-input="paylocation" class="text-center">{paylocation}</td>' +
                            '</tr>'
                            ;
                        function formatcompliancerow(properties) {
                            return $.extend(properties, {
                                tag_id: properties.id,
                                tag_name: !/^n.a$/.test(properties.name) ? properties.name : /^n.a$/.test(properties.craftName) ? properties.craftName : properties.id,
                                ldc: properties.hasOwnProperty("Tacs") ? properties.Tacs.hasOwnProperty("ldc") ? properties.Tacs.ldc : "No LDC" : "No Tacs",
                                op_code: properties.hasOwnProperty("Tacs") ? properties.Tacs.hasOwnProperty("operationId") ? properties.Tacs.operationId : "No Op Code" : "No Tacs",
                                paylocation: properties.hasOwnProperty("Tacs") ? properties.Tacs.hasOwnProperty("payLocation") ? properties.Tacs.payLocation : "No payLocation" : "No Tacs"
                            });
                        }
                        $.map(undetectedtagsData, async function (properties, i) {
                            var findtrdataid = $compliance_Table_Body.find('tr[data-tag=' + properties.properties.id + ']');
                            if (findtrdataid.length === 0) {
                                $compliance_Table_Body.append($compliance_row_template.supplant(formatcompliancerow(properties.properties)));
                                sortTable($compliance_Table, 'asc');
                            }
                        })
                    }
                } catch (e) {
                    console.log(e);
                }
            });
        } catch (e) {
            console.log(e)
        }
    }

    function formatURL(NASS_Code) {
        return $.extend(NASS_Code, {
            nasscode: NASS_Code
        });
    }

    function enableNotificationSubmit() {
        //AGV connections
        if ($('input[type=text][name=condition_name]').hasClass('is-valid') &&
            $('input[type=text][name=condition]').hasClass('is-valid') &&
            $('input[id=warning_condition]').val() >= 0 &&
            $('input[id=critical_condition]').val() >= 0) {
            $('button[id=notificationsubmitBtn]').prop('disabled', false);
            $('button[id=editnotificationsubmitBtn]').prop('disabled', false);
        }
        else {
            $('button[id=notificationsubmitBtn]').prop('disabled', true);
            $('button[id=editnotificationsubmitBtn]').prop('disabled', true);
        }
    }
    function validateNum(input, min, max) {
        if (input >= min && input <= max) {
            return true;
        }
        else {
            return false;
        }
    }

 
   
    //sort table header
    $('th').click(function () {
        var $th = $(this).closest('th');
        $th.toggleClass('selected');
        var isSelected = $th.hasClass('selected');
        var isInput = $th.hasClass('input');
        var column = $th.index();
        var $table = $th.closest('table');
        var isNum = $table.find('tbody > tr').children('td').eq(column).hasClass('num');
        var rows = $table.find('tbody > tr').get();
        rows.sort(function (rowA, rowB) {
            if (isInput) {
                var keyA = $(rowA).children('td').eq(column).children('input').val().toUpperCase();
                var keyB = $(rowB).children('td').eq(column).children('input').val().toUpperCase();
            } else {
                var keyA = $(rowA).children('td').eq(column).text().toUpperCase();
                var keyB = $(rowB).children('td').eq(column).text().toUpperCase();
            }
            if (isSelected) return OrderBy(keyA, keyB, isNum);
            return OrderBy(keyB, keyA, isNum);
        });
        $.each(rows, function (index, row) {
            $table.children('tbody').append(row);
        });
        return false;
    });
});
function setHeight() {
    var height = (this.window.innerHeight > 0 ? this.window.innerHeight : this.screen.height) - 1;
    $('div[id=map]').css("min-height", height + "px");
};
function SortByVehicleName(a, b) {
    return a.VEHICLENAME < b.VEHICLENAME ? -1 : a.VEHICLENAME > b.VEHICLENAME ? 1 : 0;
}
function formatTime(value_time) {
    try {
        if (!$.isEmptyObject(timezone)) {
            if (timezone.hasOwnProperty("Facility_TimeZone")) {
                var time = moment(value_time);
                if (time._isValid) {
                    return time.format("HH:mm");
                }
            }
        }
    } catch (e) {
        console.log(e);
    }
}
function formatDateTime(value_time) {
    try {

        var time = moment(value_time);
        if (time._isValid) {
            return time.format("M/D/YYYY h:mm a");
        }

    } catch (e) {
        console.log(e);
    }
}
function capitalize_Words(str) {
    return str.replace(/\w\S*/g, function (txt) {
        return txt.charAt(0).toUpperCase() + txt.substr(1).toLowerCase();
    });
}
function IPAddress_validator(value) {
    var ipPattern = /^(\d{1,3})\.(\d{1,3})\.(\d{1,3})\.(\d{1,3})$/;
    var ipArray = value.match(ipPattern);
    if (value === "0.0.0.0" || value === "255.255.255.255" || ipArray === null)
        return "Invalid IP Address";
    else {
        for (i = 1; i < ipArray.length; i++) {
            thisSegment = ipArray[i];
            if (thisSegment > 255) {
                return "Invalid IP Address";
            }
            if (i === 0 && thisSegment > 255) {
                return "Invalid IP Address";
            }
        }
    }
    return value;
}
//order
function OrderBy(a, b, n) {
    if (n) return a - b;
    if (a < b) return -1;
    if (a > b) return 1;
    return 0;
}
function SortByTagName(a, b) {
    return a.name < b.name ? -1 : a.name > b.name ? 1 : 0;
}

function GetPeopleInZone(zone, P2Pdata, staffarray) {
    //staffing
    var planstaffarray = [];
    var planstaffCounts = [];
    var plansumstaffCounts = {};
    var staffCounts = [];
    var sumstaffCounts = {};
    try {
        if (staffarray.length === 0) {
            if (circleMarker.hasOwnProperty("_layers")) {
                $.map(circleMarker._layers, function (layer, i) {
                    if (layer.hasOwnProperty("feature")) {
                        if (/person/i.test(layer.feature.properties.Tag_Type)) {
                            if (layer.feature.properties.hasOwnProperty("zones")) {
                                $.map(layer.feature.properties.zones, function (p_zone) {
                                    if (p_zone.id == zone) {
                                        if (layer.hasOwnProperty("_tooltip")) {
                                            if (layer._tooltip.hasOwnProperty("_container")) {
                                                if (!layer._tooltip._container.classList.contains('tooltip-hidden')) {
                                                    staffarray.push({
                                                        name: checkValue(layer.feature.properties.craftName) ? layer.feature.properties.craftName : layer.feature.properties.id,
                                                        nameId: checkValue(layer.feature.properties.id) ? layer.feature.properties.id : layer.feature.properties.id,
                                                        id: layer.feature.properties.id
                                                    })
                                                }
                                            }
                                        }
                                    }
                                });
                            }
                        }
                    }
                });
            }
        }

        if (checkValue(P2Pdata)) {
            if (P2Pdata.hasOwnProperty("clerk")) {
                if (P2Pdata.clerk >= 1) {
                    for (var i = 0; i < P2Pdata.clerk; i++) {
                        planstaffarray.push({
                            name: "Clerk",
                            value: i
                        })
                    }
                    //planstaffarray.push({
                    //    name: "Clerk",
                    //    value: P2Pdata.clerk
                    //})
                }
            }
            if (P2Pdata.hasOwnProperty("mh")) {
                if (P2Pdata.mh >= 1) {
                    for (var r = 0; r < P2Pdata.mh; r++) {
                        planstaffarray.push({
                            name: "MailHandler",
                            value: r
                        })
                    }
                    //planstaffarray.push({
                    //    name: "MailHandler",
                    //    value: P2Pdata.mh
                    //})
                }
            }
        }

        $.each(staffarray, function (i, e) {
            sumstaffCounts[this.name] = (sumstaffCounts[this.name] || 0) + 1;
        });
        $.each(sumstaffCounts, function (index, value) {
            staffCounts.push({
                name: index,
                currentstaff: value
            });
        });

        $.each(planstaffarray, function (i, e) {
            plansumstaffCounts[this.name] = (plansumstaffCounts[this.name] || 0) + 1;
        });
        $.each(plansumstaffCounts, function (index, value) {
            planstaffCounts.push({
                name: index,
                planstaff: value
            });
        });
        var staffing = $.extend(true, staffCounts, planstaffCounts);
        staffing.sort(SortByTagName);

        $('div[id=staff_div]').css('display', 'block');
        $stafftop_Table = $('table[id=stafftable]');
        $stafftop_Table_thead = $stafftop_Table.find('thead');
        $stafftop_Table_thead.empty();
        $stafftop_Table_thead.append('<tr><th>F1 Craft</th><th>Current</th><th>Planned</th></tr>');
        $stafftop_Table_Body = $stafftop_Table.find('tbody');
        $stafftop_Table_Body.empty();
        $stafftop_row_template = '<tr data-id={staffName}>' +
            '<td class="text-center">{staffName}</td>' +
            '<td class="text-center">{staffNameId}</td>' +
            '<td class="text-center">{planstaff}</td>' +
            '</tr>"';
        function formatstafftoprow(properties) {
            return $.extend(properties, {
                staffName: properties.name,
                staffId: properties.id,
                staffNameId: properties.hasOwnProperty("currentstaff") ? properties.currentstaff : 0,
                planstaff: properties.hasOwnProperty("planstaff") ? properties.planstaff : 0,
            });
        }
        staffCounts.push({
            name: 'Total',
            currentstaff: staffarray.length,
            planstaff: planstaffarray.length
        });
        $.each(staffCounts, function () {
            $stafftop_Table_Body.append($stafftop_row_template.supplant(formatstafftoprow(this)));
        })
    } catch (e) {
        console.log(e);
    }
}

function digits(num) {
    return num.replace(/(\d)(?=(\d\d\d)+(?!\d))/g, "$1,");
}
function checkValue(value) {
    switch (value) {
        case "": return false;
        case null: return false;
        case "undefined": return false;
        case undefined: return false;
        default: return true;
    }
}
function pad(str, max) {
    str = str.toString();
    return str.length < max ? pad("0" + str, max) : str;
};
